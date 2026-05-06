using System;
using System.IO;
using System.Linq;
using System.Threading;
using DasBlog.Services.FileManagement;
using DasBlog.Web.Services;
using Xunit;

namespace DasBlog.Tests.UnitTests.Services
{
	public class StaticPageManagerTest : IDisposable
	{
		private readonly string contentRoot;
		private readonly StaticPageManager manager;

		public StaticPageManagerTest()
		{
			contentRoot = Path.Combine(Path.GetTempPath(),
				"dasblog-staticpages-tests-" + Guid.NewGuid().ToString("N"));
			Directory.CreateDirectory(contentRoot);
			manager = new StaticPageManager(new FakePathResolver(contentRoot));
		}

		public void Dispose()
		{
			try
			{
				if (Directory.Exists(contentRoot))
				{
					Directory.Delete(contentRoot, recursive: true);
				}
			}
			catch
			{
				// best-effort cleanup
			}
		}

		// ---------- Allowlist ----------

		[Fact]
		public void AllowedPageNames_ContainsAboutOnly()
		{
			Assert.Equal(new[] { "about" }, manager.AllowedPageNames.ToArray());
		}

		[Fact]
		public void IsAllowed_TrueForAbout_FalseForOthers()
		{
			Assert.True(manager.IsAllowed("about"));
			Assert.True(manager.IsAllowed("About")); // case-insensitive
			Assert.False(manager.IsAllowed("contact"));
			Assert.False(manager.IsAllowed(""));
			Assert.False(manager.IsAllowed(null));
		}

		[Fact]
		public void Write_RejectsPageOutsideAllowlist()
		{
			Assert.Throws<InvalidOperationException>(() => manager.Write("contact", "<p>x</p>"));
		}

		[Fact]
		public void Write_RejectsInvalidNameCharacters()
		{
			Assert.Throws<ArgumentException>(() => manager.Write("../etc/passwd", "x"));
			Assert.Throws<ArgumentException>(() => manager.Write("about/sub", "x"));
			Assert.Throws<ArgumentException>(() => manager.Write("about page", "x"));
		}

		// ---------- Listing & GetPage ----------

		[Fact]
		public void ListPages_AlwaysIncludesAbout_WhenFileMissing()
		{
			var pages = manager.ListPages();
			var about = Assert.Single(pages);
			Assert.Equal("about", about.Name);
			Assert.False(about.Exists);
			Assert.Null(about.LastModifiedUtc);
			Assert.Equal(0, about.BackupCount);
			Assert.Equal("/about", about.PublicUrlPath);
		}

		[Fact]
		public void GetPage_ReportsExistsAndMetadata_AfterWrite()
		{
			manager.Write("about", "<p>hello</p>");

			var info = manager.GetPage("about");

			Assert.True(info.Exists);
			Assert.NotNull(info.LastModifiedUtc);
			Assert.True(info.Size > 0);
			Assert.Equal("About", info.DisplayTitle);
		}

		// ---------- Read / Write ----------

		[Fact]
		public void Read_ReturnsEmpty_WhenFileMissing()
		{
			Assert.Equal(string.Empty, manager.Read("about"));
		}

		[Fact]
		public void Write_CreatesFile_AndRoundTripsContent()
		{
			manager.Write("about", "<h1>About</h1>");

			Assert.Equal("<h1>About</h1>", manager.Read("about"));
			Assert.True(File.Exists(Path.Combine(contentRoot, "static", "about.html")));
		}

		[Fact]
		public void Write_NullContent_StoredAsEmpty()
		{
			manager.Write("about", null);
			Assert.Equal(string.Empty, manager.Read("about"));
		}

		// ---------- Backups ----------

		[Fact]
		public void Write_FirstSave_DoesNotCreateBackup()
		{
			manager.Write("about", "v1");
			Assert.Empty(manager.ListBackups("about"));
		}

		[Fact]
		public void Write_OverwritesProduceBackups_OldestPrunedAfterMax()
		{
			manager.Write("about", "v1");
			SleepForUniqueTimestamp();
			manager.Write("about", "v2");
			SleepForUniqueTimestamp();
			manager.Write("about", "v3");
			SleepForUniqueTimestamp();
			manager.Write("about", "v4");
			SleepForUniqueTimestamp();
			manager.Write("about", "v5");

			var backups = manager.ListBackups("about");
			Assert.Equal(3, backups.Count);
			// Newest first
			Assert.True(backups[0].SavedUtc >= backups[1].SavedUtc);
			Assert.True(backups[1].SavedUtc >= backups[2].SavedUtc);

			// Live file is the latest write
			Assert.Equal("v5", manager.Read("about"));
		}

		[Fact]
		public void Revert_RestoresChosenBackup_AndDiscardsNewerBackups()
		{
			manager.Write("about", "v1");
			SleepForUniqueTimestamp();
			manager.Write("about", "v2"); // backup of v1 created
			SleepForUniqueTimestamp();
			manager.Write("about", "v3"); // backup of v2 created
			SleepForUniqueTimestamp();
			manager.Write("about", "v4"); // backup of v3 created

			var backups = manager.ListBackups("about");
			Assert.Equal(3, backups.Count);

			// Pick the oldest backup -> should restore v1
			var oldest = backups.OrderBy(b => b.SavedUtc).First();
			manager.Revert("about", oldest.Id);

			Assert.Equal("v1", manager.Read("about"));
			// All newer (or equal-time) backups discarded; no roll-forward.
			Assert.Empty(manager.ListBackups("about"));
		}

		[Fact]
		public void Revert_ToMostRecentBackup_KeepsOlderBackups()
		{
			manager.Write("about", "v1");
			SleepForUniqueTimestamp();
			manager.Write("about", "v2");
			SleepForUniqueTimestamp();
			manager.Write("about", "v3");

			var backups = manager.ListBackups("about");
			Assert.Equal(2, backups.Count);

			// Most recent backup is v2
			var newest = backups.OrderByDescending(b => b.SavedUtc).First();
			manager.Revert("about", newest.Id);

			Assert.Equal("v2", manager.Read("about"));
			// The newest backup was discarded; the older v1 backup remains.
			var remaining = manager.ListBackups("about");
			Assert.Single(remaining);
		}

		[Fact]
		public void Revert_RejectsUnknownBackupId()
		{
			manager.Write("about", "v1");
			SleepForUniqueTimestamp();
			manager.Write("about", "v2");

			Assert.Throws<InvalidOperationException>(
				() => manager.Revert("about", "about.html.20990101T000000000Z.bak"));
		}

		[Fact]
		public void Revert_RejectsBackupIdWithPathSeparator()
		{
			manager.Write("about", "v1");
			SleepForUniqueTimestamp();
			manager.Write("about", "v2");
			var any = manager.ListBackups("about").First();

			Assert.Throws<InvalidOperationException>(
				() => manager.Revert("about", "../" + any.Id));
		}

		[Fact]
		public void Revert_RejectsBackupIdNotMatchingPagePrefix()
		{
			manager.Write("about", "v1");
			SleepForUniqueTimestamp();
			manager.Write("about", "v2");

			Assert.Throws<InvalidOperationException>(
				() => manager.Revert("about", "other.html.20990101T000000000Z.bak"));
		}

		[Fact]
		public void Revert_RejectsEmptyBackupId()
		{
			Assert.Throws<ArgumentException>(() => manager.Revert("about", ""));
		}

		// ---------- Delete ----------

		[Fact]
		public void Delete_RemovesFileAndAllBackups()
		{
			manager.Write("about", "v1");
			SleepForUniqueTimestamp();
			manager.Write("about", "v2");
			SleepForUniqueTimestamp();
			manager.Write("about", "v3");

			Assert.NotEmpty(manager.ListBackups("about"));

			manager.Delete("about");

			Assert.False(manager.GetPage("about").Exists);
			Assert.Empty(manager.ListBackups("about"));
			Assert.False(File.Exists(Path.Combine(contentRoot, "static", "about.html")));
		}

		[Fact]
		public void Delete_NoOp_WhenFileMissing()
		{
			// Should not throw even if nothing to delete.
			manager.Delete("about");
			Assert.False(manager.GetPage("about").Exists);
		}

		// ---------- Helpers ----------

		// Backup file names embed UTC timestamps with millisecond precision.
		// On fast machines two writes can land in the same millisecond, which
		// would make the writer add a `_N` discriminator. That keeps the test
		// correct, but it also makes ordering by SavedUtc identical for both
		// records and harder to reason about. A small sleep keeps timestamps
		// strictly increasing.
		private static void SleepForUniqueTimestamp() => Thread.Sleep(5);

		private sealed class FakePathResolver : IDasBlogPathResolver
		{
			public FakePathResolver(string contentRoot)
			{
				ContentRootPath = contentRoot;
				ContentFolderPath = contentRoot;
			}

			public string ContentRootPath { get; }
			public string ContentFolderPath { get; }

			// Unused members for these tests.
			public string SiteConfigFilePath => string.Empty;
			public string MetaConfigFilePath => string.Empty;
			public string SecurityConfigFilePath => string.Empty;
			public string IISUrlRewriteFilePath => string.Empty;
			public string OEmbedProvidersFilePath => string.Empty;
			public string DefaultSiteConfigRelativePath => string.Empty;
			public string SiteConfigRelativePath => string.Empty;
			public string DefaultMetaConfigRelativePath => string.Empty;
			public string MetaConfigRelativePath => string.Empty;
			public string IISUrlRewriteRelativePath => string.Empty;
			public string DefaultIISUrlRewriteRelativePath => string.Empty;
			public string DefaultSecurityConfigRelativePath => string.Empty;
			public string SecurityConfigRelativePath => string.Empty;
			public string OEmbedProvidersRelativePath => string.Empty;
			public string ThemeFolderPath => string.Empty;
			public string BinariesPath => string.Empty;
			public string BinariesUrlRelativePath => string.Empty;
			public string LogFolderPath => string.Empty;
			public string RadioStoriesFolderPath => string.Empty;
		}
	}
}
