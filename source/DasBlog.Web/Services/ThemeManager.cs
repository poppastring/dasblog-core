using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using DasBlog.Services;

namespace DasBlog.Web.Services
{
	public interface IThemeManager
	{
		IReadOnlyList<ThemeInfo> ListThemes();
		ThemeInfo GetTheme(string themeName);
		IReadOnlyList<ThemeFileInfo> ListThemeFiles(string themeName);
		string ReadFile(string themeName, string relativePath);
		void WriteFile(string themeName, string relativePath, string content);
		void CreateTheme(string newName, string sourceTheme);
		void DeleteTheme(string themeName);
		bool IsDefaultTheme(string themeName);
		bool IsActiveTheme(string themeName);
		bool IsEditableExtension(string relativePath);
	}

	public sealed class ThemeInfo
	{
		public string Name { get; init; }
		public bool IsDefault { get; init; }
		public bool IsActive { get; init; }
		public bool IsLocked => IsDefault || IsActive;
	}

	public sealed class ThemeFileInfo
	{
		public string RelativePath { get; init; }
		public string Name { get; init; }
		public bool IsEditable { get; init; }
		public long Size { get; init; }
	}

	public sealed class ThemeManager : IThemeManager
	{
		// Built-in themes shipped with DasBlog. These are protected from edit/delete
		// but remain selectable. Update this list when new themes ship in the repo.
		public static readonly IReadOnlyList<string> DefaultThemes = new[]
		{
			"darkly",
			"dasblog",
			"flamingo",
			"fulcrum",
			"kindofblue",
			"median"
		};

		// Allowlist of file extensions the theme editor can read/write.
		private static readonly HashSet<string> EditableExtensions = new(StringComparer.OrdinalIgnoreCase)
		{
			".cshtml", ".css", ".js", ".json", ".txt", ".map", ".html", ".htm", ".xml", ".svg"
		};

		private static readonly Regex ThemeNameRegex = new("^[A-Za-z0-9_-]+$", RegexOptions.Compiled);

		private readonly IDasBlogSettings dasBlogSettings;

		public ThemeManager(IDasBlogSettings dasBlogSettings)
		{
			this.dasBlogSettings = dasBlogSettings;
		}

		private string ThemesRoot => Path.Combine(dasBlogSettings.WebRootDirectory, "Themes");

		public bool IsDefaultTheme(string themeName)
		{
			if (string.IsNullOrWhiteSpace(themeName)) return false;
			return DefaultThemes.Contains(themeName, StringComparer.OrdinalIgnoreCase);
		}

		public bool IsActiveTheme(string themeName)
		{
			if (string.IsNullOrWhiteSpace(themeName)) return false;
			return string.Equals(themeName, dasBlogSettings.SiteConfiguration.Theme, StringComparison.OrdinalIgnoreCase);
		}

		public bool IsEditableExtension(string relativePath)
		{
			if (string.IsNullOrWhiteSpace(relativePath)) return false;
			return EditableExtensions.Contains(Path.GetExtension(relativePath));
		}

		public IReadOnlyList<ThemeInfo> ListThemes()
		{
			if (!Directory.Exists(ThemesRoot))
			{
				return Array.Empty<ThemeInfo>();
			}

			var folderNames = Directory.GetDirectories(ThemesRoot)
				.Select(Path.GetFileName)
				.Where(n => !string.IsNullOrWhiteSpace(n));

			return folderNames
				.Distinct(StringComparer.OrdinalIgnoreCase)
				.OrderBy(n => n, StringComparer.OrdinalIgnoreCase)
				.Select(n => new ThemeInfo
				{
					Name = n,
					IsDefault = IsDefaultTheme(n),
					IsActive = IsActiveTheme(n)
				})
				.ToList();
		}

		public ThemeInfo GetTheme(string themeName)
		{
			ValidateThemeName(themeName);
			var path = ResolveThemeDirectory(themeName);
			if (!Directory.Exists(path))
			{
				throw new DirectoryNotFoundException($"Theme '{themeName}' was not found.");
			}

			return new ThemeInfo
			{
				Name = Path.GetFileName(path),
				IsDefault = IsDefaultTheme(themeName),
				IsActive = IsActiveTheme(themeName)
			};
		}

		public IReadOnlyList<ThemeFileInfo> ListThemeFiles(string themeName)
		{
			ValidateThemeName(themeName);
			var themeDir = ResolveThemeDirectory(themeName);
			if (!Directory.Exists(themeDir))
			{
				throw new DirectoryNotFoundException($"Theme '{themeName}' was not found.");
			}

			var files = Directory.GetFiles(themeDir, "*", SearchOption.AllDirectories);

			return files
				.Select(full =>
				{
					var rel = Path.GetRelativePath(themeDir, full).Replace('\\', '/');
					var info = new FileInfo(full);
					return new ThemeFileInfo
					{
						RelativePath = rel,
						Name = info.Name,
						IsEditable = IsEditableExtension(rel),
						Size = info.Length
					};
				})
				.OrderBy(f => f.RelativePath, StringComparer.OrdinalIgnoreCase)
				.ToList();
		}

		public string ReadFile(string themeName, string relativePath)
		{
			var fullPath = ResolveAndValidateFile(themeName, relativePath);
			return File.ReadAllText(fullPath);
		}

		public void WriteFile(string themeName, string relativePath, string content)
		{
			if (IsDefaultTheme(themeName))
			{
				throw new InvalidOperationException($"Theme '{themeName}' is a default theme and cannot be edited.");
			}

			var fullPath = ResolveAndValidateFile(themeName, relativePath);
			File.WriteAllText(fullPath, content ?? string.Empty);
		}

		public void CreateTheme(string newName, string sourceTheme)
		{
			ValidateThemeName(newName);

			if (string.IsNullOrWhiteSpace(sourceTheme))
			{
				sourceTheme = "dasblog";
			}

			var sourceDir = ResolveThemeDirectory(sourceTheme);
			if (!Directory.Exists(sourceDir))
			{
				throw new DirectoryNotFoundException($"Source theme '{sourceTheme}' was not found.");
			}

			var targetDir = ResolveThemeDirectory(newName);
			if (Directory.Exists(targetDir))
			{
				throw new InvalidOperationException($"A theme named '{newName}' already exists.");
			}

			CopyDirectory(sourceDir, targetDir);
		}

		public void DeleteTheme(string themeName)
		{
			ValidateThemeName(themeName);

			if (IsDefaultTheme(themeName))
			{
				throw new InvalidOperationException($"Theme '{themeName}' is a default theme and cannot be deleted.");
			}

			if (IsActiveTheme(themeName))
			{
				throw new InvalidOperationException($"Theme '{themeName}' is currently selected and cannot be deleted.");
			}

			var dir = ResolveThemeDirectory(themeName);
			if (Directory.Exists(dir))
			{
				Directory.Delete(dir, recursive: true);
			}
		}

		private static void ValidateThemeName(string themeName)
		{
			if (string.IsNullOrWhiteSpace(themeName) || !ThemeNameRegex.IsMatch(themeName))
			{
				throw new ArgumentException(
					"Theme names may only contain letters, numbers, hyphens, and underscores.",
					nameof(themeName));
			}
		}

		private string ResolveThemeDirectory(string themeName)
		{
			ValidateThemeName(themeName);
			var root = Path.GetFullPath(ThemesRoot);
			var combined = Path.GetFullPath(Path.Combine(root, themeName));

			if (!combined.StartsWith(root + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase) &&
				!combined.Equals(root, StringComparison.OrdinalIgnoreCase))
			{
				throw new InvalidOperationException("Invalid theme path.");
			}

			return combined;
		}

		private string ResolveAndValidateFile(string themeName, string relativePath)
		{
			if (string.IsNullOrWhiteSpace(relativePath))
			{
				throw new ArgumentException("File path is required.", nameof(relativePath));
			}

			if (!IsEditableExtension(relativePath))
			{
				throw new InvalidOperationException(
					$"Files of type '{Path.GetExtension(relativePath)}' are not editable in this view.");
			}

			var themeDir = ResolveThemeDirectory(themeName);
			var normalized = relativePath.Replace('\\', '/').TrimStart('/');
			var combined = Path.GetFullPath(Path.Combine(themeDir, normalized));
			var themeRoot = themeDir + Path.DirectorySeparatorChar;

			if (!combined.StartsWith(themeRoot, StringComparison.OrdinalIgnoreCase))
			{
				throw new InvalidOperationException("File path escapes the theme directory.");
			}

			return combined;
		}

		private static void CopyDirectory(string sourceDir, string destinationDir)
		{
			Directory.CreateDirectory(destinationDir);

			foreach (var file in Directory.GetFiles(sourceDir))
			{
				var dest = Path.Combine(destinationDir, Path.GetFileName(file));
				File.Copy(file, dest, overwrite: false);
			}

			foreach (var subDir in Directory.GetDirectories(sourceDir))
			{
				var destSub = Path.Combine(destinationDir, Path.GetFileName(subDir));
				CopyDirectory(subDir, destSub);
			}
		}
	}
}
