using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using DasBlog.Services.FileManagement;

namespace DasBlog.Web.Services
{
	public interface IStaticPageManager
	{
		IReadOnlyList<string> AllowedPageNames { get; }
		IReadOnlyList<StaticPageItemInfo> ListPages();
		StaticPageItemInfo GetPage(string name);
		string Read(string name);
		void Write(string name, string content);
		void Delete(string name);
		bool IsAllowed(string name);
		IReadOnlyList<StaticPageBackupInfo> ListBackups(string name);
		void Revert(string name, string backupId);
	}

	public sealed class StaticPageItemInfo
	{
		public string Name { get; init; }
		public string DisplayTitle { get; init; }
		public string PublicUrlPath { get; init; }
		public bool Exists { get; init; }
		public DateTime? LastModifiedUtc { get; init; }
		public long Size { get; init; }
		public int BackupCount { get; init; }
	}

	public sealed class StaticPageBackupInfo
	{
		public string Id { get; init; }
		public DateTime SavedUtc { get; init; }
	}

	public sealed class StaticPageManager : IStaticPageManager
	{
		// Allowlist of page names (file stems) the admin UI may edit.
		// Add more entries here when the feature expands beyond About.
		// Keys are case-insensitive; the canonical casing is what's shown in the UI.
		private static readonly IReadOnlyDictionary<string, string> AllowedPagesCatalog =
			new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
			{
				["about"] = "About"
			};

		// Sidecar suffix used to mark previous versions of an edited page.
		// Backup naming: <name>.html.<utcTimestamp>.bak
		private const string PageExtension = ".html";
		private const string BackupSuffix = ".bak";
		private const string BackupTimestampFormat = "yyyyMMdd'T'HHmmssfff'Z'";
		private const int MaxBackupsPerFile = 3;
		private const string StaticFolderName = "static";

		// Page names use the same character class as theme names for safety
		// (no spaces, no slashes, no traversal). Keep this strict.
		private static readonly Regex PageNameRegex = new("^[A-Za-z0-9_-]+$", RegexOptions.Compiled);

		private readonly IDasBlogPathResolver pathResolver;

		public StaticPageManager(IDasBlogPathResolver pathResolver)
		{
			this.pathResolver = pathResolver;
		}

		public IReadOnlyList<string> AllowedPageNames =>
			AllowedPagesCatalog.Keys.OrderBy(k => k, StringComparer.OrdinalIgnoreCase).ToList();

		public bool IsAllowed(string name)
		{
			if (string.IsNullOrWhiteSpace(name)) return false;
			return AllowedPagesCatalog.ContainsKey(name);
		}

		public IReadOnlyList<StaticPageItemInfo> ListPages()
		{
			return AllowedPagesCatalog
				.OrderBy(kv => kv.Key, StringComparer.OrdinalIgnoreCase)
				.Select(kv => GetPage(kv.Key))
				.ToList();
		}

		public StaticPageItemInfo GetPage(string name)
		{
			ValidateAllowed(name);

			var displayTitle = AllowedPagesCatalog[name];
			var fullPath = ResolveFile(name, ensureDirectory: false);

			if (!File.Exists(fullPath))
			{
				return new StaticPageItemInfo
				{
					Name = name,
					DisplayTitle = displayTitle,
					PublicUrlPath = "/" + name,
					Exists = false,
					LastModifiedUtc = null,
					Size = 0,
					BackupCount = 0
				};
			}

			var info = new FileInfo(fullPath);
			return new StaticPageItemInfo
			{
				Name = name,
				DisplayTitle = displayTitle,
				PublicUrlPath = "/" + name,
				Exists = true,
				LastModifiedUtc = info.LastWriteTimeUtc,
				Size = info.Length,
				BackupCount = EnumerateBackupFiles(fullPath).Count()
			};
		}

		public string Read(string name)
		{
			ValidateAllowed(name);
			var fullPath = ResolveFile(name, ensureDirectory: false);
			if (!File.Exists(fullPath))
			{
				return string.Empty;
			}
			return File.ReadAllText(fullPath);
		}

		public void Write(string name, string content)
		{
			ValidateAllowed(name);
			var fullPath = ResolveFile(name, ensureDirectory: true);

			// Snapshot the existing file as a timestamped sidecar before overwriting it.
			// Keep at most MaxBackupsPerFile copies; oldest are pruned.
			if (File.Exists(fullPath))
			{
				var timestamp = DateTime.UtcNow.ToString(BackupTimestampFormat, CultureInfo.InvariantCulture);
				var backupPath = $"{fullPath}.{timestamp}{BackupSuffix}";

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

		public void Delete(string name)
		{
			ValidateAllowed(name);
			var fullPath = ResolveFile(name, ensureDirectory: false);

			if (File.Exists(fullPath))
			{
				File.Delete(fullPath);
			}

			// Drop any sidecar backups too — there's nothing left to roll back to.
			foreach (var b in EnumerateBackupFiles(fullPath))
			{
				var dir = Path.GetDirectoryName(fullPath) ?? string.Empty;
				var p = Path.Combine(dir, b.Id);
				if (File.Exists(p))
				{
					File.Delete(p);
				}
			}
		}

		public IReadOnlyList<StaticPageBackupInfo> ListBackups(string name)
		{
			try
			{
				ValidateAllowed(name);
				var fullPath = ResolveFile(name, ensureDirectory: false);
				return EnumerateBackupFiles(fullPath)
					.OrderByDescending(b => b.SavedUtc)
					.ToList();
			}
			catch
			{
				return Array.Empty<StaticPageBackupInfo>();
			}
		}

		public void Revert(string name, string backupId)
		{
			ValidateAllowed(name);

			if (string.IsNullOrWhiteSpace(backupId))
			{
				throw new ArgumentException("A backup id is required.", nameof(backupId));
			}

			var fullPath = ResolveFile(name, ensureDirectory: true);
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

		private void ValidateAllowed(string name)
		{
			if (string.IsNullOrWhiteSpace(name) || !PageNameRegex.IsMatch(name))
			{
				throw new ArgumentException(
					"Page names may only contain letters, numbers, hyphens, and underscores.",
					nameof(name));
			}

			if (!AllowedPagesCatalog.ContainsKey(name))
			{
				throw new InvalidOperationException(
					$"Static page '{name}' is not enabled for editing.");
			}
		}

		private string ResolveFile(string name, bool ensureDirectory)
		{
			var staticRoot = Path.GetFullPath(
				Path.Combine(pathResolver.ContentFolderPath, StaticFolderName));

			if (ensureDirectory)
			{
				Directory.CreateDirectory(staticRoot);
			}

			var combined = Path.GetFullPath(Path.Combine(staticRoot, name + PageExtension));
			var rootWithSep = staticRoot + Path.DirectorySeparatorChar;

			if (!combined.StartsWith(rootWithSep, StringComparison.OrdinalIgnoreCase))
			{
				throw new InvalidOperationException("Static page path escapes the content directory.");
			}

			return combined;
		}

		private static IEnumerable<StaticPageBackupInfo> EnumerateBackupFiles(string sourceFullPath)
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
				var fileName = Path.GetFileName(path);
				if (!fileName.StartsWith(prefix, StringComparison.OrdinalIgnoreCase) ||
					!fileName.EndsWith(BackupSuffix, StringComparison.OrdinalIgnoreCase))
				{
					continue;
				}

				var tsPart = fileName.Substring(prefix.Length, fileName.Length - prefix.Length - BackupSuffix.Length);
				var underscore = tsPart.IndexOf('_');
				var tsToParse = underscore > 0 ? tsPart.Substring(0, underscore) : tsPart;

				if (!DateTime.TryParseExact(tsToParse, BackupTimestampFormat, CultureInfo.InvariantCulture,
					DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal, out var savedUtc))
				{
					continue;
				}

				yield return new StaticPageBackupInfo
				{
					Id = fileName,
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
	}
}
