using System;
using System.IO;
using System.Linq;
using DasBlog.Services;
using DasBlog.Services.ConfigFile.Interfaces;
using DasBlog.Web.Services;
using Moq;
using Xunit;

namespace DasBlog.Tests.UnitTests.Services
{
	public class ThemeManagerTest : IDisposable
	{
		private readonly string tempRoot;
		private readonly Mock<IDasBlogSettings> settingsMock;

		public ThemeManagerTest()
		{
			tempRoot = Path.Combine(Path.GetTempPath(), "dasblog-test-" + Guid.NewGuid().ToString("N"));
			Directory.CreateDirectory(tempRoot);

			// Create theme directories matching the built-in defaults
			foreach (var theme in new[] { "darkly", "dasblog", "flamingo", "flatly", "kindofblue", "median" })
			{
				var themeDir = Path.Combine(tempRoot, "Themes", theme);
				Directory.CreateDirectory(themeDir);
				File.WriteAllText(Path.Combine(themeDir, "_Layout.cshtml"), "<html></html>");
				File.WriteAllText(Path.Combine(themeDir, "custom.css"), "body {}");
			}

			var siteConfigMock = new Mock<ISiteConfig>();
			siteConfigMock.SetupGet(s => s.Theme).Returns("darkly");

			settingsMock = new Mock<IDasBlogSettings>();
			settingsMock.SetupGet(s => s.WebRootDirectory).Returns(tempRoot);
			settingsMock.SetupGet(s => s.SiteConfiguration).Returns(siteConfigMock.Object);
		}

		public void Dispose()
		{
			try { Directory.Delete(tempRoot, true); } catch { }
		}

		private ThemeManager CreateManager() => new ThemeManager(settingsMock.Object);

		[Theory]
		[Trait("Category", "UnitTest")]
		[InlineData("darkly", true)]
		[InlineData("dasblog", true)]
		[InlineData("flamingo", true)]
		[InlineData("flatly", true)]
		[InlineData("kindofblue", true)]
		[InlineData("median", true)]
		[InlineData("mytheme", false)]
		[InlineData("custom-blog", false)]
		[InlineData("", false)]
		[InlineData(null, false)]
		public void IsDefaultTheme_ReturnsExpectedResult(string themeName, bool expected)
		{
			var manager = CreateManager();
			Assert.Equal(expected, manager.IsDefaultTheme(themeName));
		}

		[Theory]
		[Trait("Category", "UnitTest")]
		[InlineData(".cshtml", true)]
		[InlineData(".css", true)]
		[InlineData(".js", true)]
		[InlineData(".json", true)]
		[InlineData(".svg", true)]
		[InlineData(".html", true)]
		[InlineData(".xml", true)]
		[InlineData(".dll", false)]
		[InlineData(".png", false)]
		[InlineData(".exe", false)]
		[InlineData(".jpg", false)]
		[InlineData("", false)]
		[InlineData(null, false)]
		public void IsEditableExtension_ReturnsExpectedResult(string extension, bool expected)
		{
			var manager = CreateManager();
			var path = string.IsNullOrEmpty(extension) ? extension : $"file{extension}";
			Assert.Equal(expected, manager.IsEditableExtension(path));
		}

		[Theory]
		[Trait("Category", "UnitTest")]
		[InlineData("Archive.cshtml", true)]
		[InlineData("ArchiveAll.cshtml", true)]
		[InlineData("Category.cshtml", true)]
		[InlineData("_Layout.cshtml", false)]
		[InlineData("custom.css", false)]
		[InlineData("", false)]
		[InlineData(null, false)]
		public void IsMaterializableCoreFile_ReturnsExpectedResult(string path, bool expected)
		{
			var manager = CreateManager();
			Assert.Equal(expected, manager.IsMaterializableCoreFile(path));
		}

		[Fact]
		[Trait("Category", "UnitTest")]
		public void ListThemes_ReturnsAllThemeDirectories()
		{
			var manager = CreateManager();
			var themes = manager.ListThemes();
			Assert.Equal(6, themes.Count);
			Assert.Contains(themes, t => t.Name == "darkly" && t.IsActive && t.IsDefault);
			Assert.Contains(themes, t => t.Name == "dasblog" && !t.IsActive && t.IsDefault);
		}

		[Fact]
		[Trait("Category", "UnitTest")]
		public void ListThemeFiles_ReturnsFilesInTheme()
		{
			var manager = CreateManager();
			var files = manager.ListThemeFiles("darkly");

			Assert.Contains(files, f => f.Name == "_Layout.cshtml" && f.IsEditable);
			Assert.Contains(files, f => f.Name == "custom.css" && f.IsEditable);
			// Materializable core files should appear as missing
			Assert.Contains(files, f => f.RelativePath == "Archive.cshtml" && f.IsMissing);
		}

		[Fact]
		[Trait("Category", "UnitTest")]
		public void WriteFile_BlocksDefaultTheme()
		{
			var manager = CreateManager();
			Assert.Throws<InvalidOperationException>(() =>
				manager.WriteFile("darkly", "_Layout.cshtml", "<html>modified</html>"));
		}
	}
}
