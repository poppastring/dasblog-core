using System;
using System.Collections.Generic;
using System.Globalization;
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
		IReadOnlyList<ThemeBackupInfo> ListBackups(string themeName, string relativePath);
		void RevertFile(string themeName, string relativePath, string backupId);
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
		public int BackupCount { get; init; }
	}

	public sealed class ThemeBackupInfo
	{
		// Backup file name (no path). Acts as the opaque id used to revert a specific version.
		public string Id { get; init; }
		public DateTime SavedUtc { get; init; }
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

		// Sidecar suffix used to mark previous versions of an edited theme file.
		// Backup file naming: <sourceFileName>.<utcTimestamp>.bak (e.g. _Layout.cshtml.20251112T143055123Z.bak).
		// Files matching this suffix are excluded from the editor's file list.
		private const string BackupSuffix = ".bak";
		private const string BackupTimestampFormat = "yyyyMMdd'T'HHmmssfff'Z'";
		private const int MaxBackupsPerFile = 3;

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
				.Where(full => !full.EndsWith(BackupSuffix, StringComparison.OrdinalIgnoreCase))
				.Select(full =>
				{
					var rel = Path.GetRelativePath(themeDir, full).Replace('\\', '/');
					var info = new FileInfo(full);
					return new ThemeFileInfo
					{
						RelativePath = rel,
						Name = info.Name,
						IsEditable = IsEditableExtension(rel),
						Size = info.Length,
						BackupCount = EnumerateBackupFiles(full).Count()
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

			// Snapshot the existing file as a timestamped sidecar before overwriting it.
			// Keep at most MaxBackupsPerFile copies; oldest are pruned.
			if (File.Exists(fullPath))
			{
				var timestamp = DateTime.UtcNow.ToString(BackupTimestampFormat, CultureInfo.InvariantCulture);
				var backupPath = $"{fullPath}.{timestamp}{BackupSuffix}";

				// Extremely unlikely collision (sub-millisecond); add a counter just in case.
				var suffix = 1;
				while (File.Exists(backupPath))
				{
					backupPath = $"{fullPath}.{timestamp}_{suffix}{BackupSuffix}";
					suffix++;
				}

				File.Copy(fullPath, backupPath, overwrite: false);

				PruneOldBackups(fullPath, MaxBackupsPerFile);
			}

			File.WriteAllText(fullPath, content ?? string.Empty);
		}

		public IReadOnlyList<ThemeBackupInfo> ListBackups(string themeName, string relativePath)
		{
			try
			{
				var fullPath = ResolveAndValidateFile(themeName, relativePath);
				return EnumerateBackupFiles(fullPath)
					.OrderByDescending(b => b.SavedUtc)
					.ToList();
			}
			catch
			{
				return Array.Empty<ThemeBackupInfo>();
			}
		}

		public void RevertFile(string themeName, string relativePath, string backupId)
		{
			if (IsDefaultTheme(themeName))
			{
				throw new InvalidOperationException($"Theme '{themeName}' is a default theme and cannot be reverted.");
			}

			if (string.IsNullOrWhiteSpace(backupId))
			{
				throw new ArgumentException("A backup id is required.", nameof(backupId));
			}

			var fullPath = ResolveAndValidateFile(themeName, relativePath);
			var dir = Path.GetDirectoryName(fullPath) ?? string.Empty;
			var sourceName = Path.GetFileName(fullPath);

			// Backup ids are sidecar file names only — never paths.
			if (backupId.IndexOfAny(new[] { '/', '\\' }) >= 0
				|| !backupId.StartsWith(sourceName + ".", StringComparison.OrdinalIgnoreCase)
				|| !backupId.EndsWith(BackupSuffix, StringComparison.OrdinalIgnoreCase))
			{
				throw new InvalidOperationException("Invalid backup identifier.");
			}

			var backups = EnumerateBackupFiles(fullPath).ToList();
			var selected = backups.FirstOrDefault(b =>
				string.Equals(b.Id, backupId, StringComparison.OrdinalIgnoreCase));

			if (selected == null)
			{
				throw new InvalidOperationException($"Backup '{backupId}' was not found.");
			}

			var selectedPath = Path.Combine(dir, selected.Id);

			// Restore the chosen backup, then drop it plus any newer backups (the user is
			// effectively rewinding past them, so we never offer roll-forward).
			File.Copy(selectedPath, fullPath, overwrite: true);

			foreach (var b in backups.Where(b => b.SavedUtc >= selected.SavedUtc))
			{
				var p = Path.Combine(dir, b.Id);
				if (File.Exists(p))
				{
					File.Delete(p);
				}
			}
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

		private static IEnumerable<ThemeBackupInfo> EnumerateBackupFiles(string sourceFullPath)
		{
			var dir = Path.GetDirectoryName(sourceFullPath);
			if (string.IsNullOrEmpty(dir) || !Directory.Exists(dir))
			{
				yield break;
			}

			var sourceName = Path.GetFileName(sourceFullPath);
			var prefix = sourceName + ".";

			foreach (var path in Directory.EnumerateFiles(dir, sourceName + ".*" + BackupSuffix))
			{
				var name = Path.GetFileName(path);
				if (!name.StartsWith(prefix, StringComparison.OrdinalIgnoreCase) ||
					!name.EndsWith(BackupSuffix, StringComparison.OrdinalIgnoreCase))
				{
					continue;
				}

				var tsPart = name.Substring(prefix.Length, name.Length - prefix.Length - BackupSuffix.Length);
				// Tolerate the rare collision suffix (e.g. "...Z_2").
				var underscore = tsPart.IndexOf('_');
				var tsToParse = underscore > 0 ? tsPart.Substring(0, underscore) : tsPart;

				if (!DateTime.TryParseExact(tsToParse, BackupTimestampFormat, CultureInfo.InvariantCulture,
					DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal, out var savedUtc))
				{
					continue;
				}

				yield return new ThemeBackupInfo
				{
					Id = name,
					SavedUtc = savedUtc
				};
			}
		}

		private static void PruneOldBackups(string sourceFullPath, int keep)
		{
			var dir = Path.GetDirectoryName(sourceFullPath) ?? string.Empty;
			var all = EnumerateBackupFiles(sourceFullPath)
				.OrderByDescending(b => b.SavedUtc)
				.ToList();

			foreach (var b in all.Skip(keep))
			{
				var p = Path.Combine(dir, b.Id);
				if (File.Exists(p))
				{
					File.Delete(p);
				}
			}
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
