using System.IO;
using Moq;
using Xunit;
using DasBlog.Services.FileManagement;

namespace DasBlog.Tests.UnitTests.Services
{
	public class SiteRepairerTest
	{
		private Mock<IDasBlogPathResolver> pathResolverMock;

		public SiteRepairerTest()
		{
			pathResolverMock = new Mock<IDasBlogPathResolver>();
		}

		[Fact]
		public void Constructor_WithValidDependencies_ConstructsInstance()
		{
			var tempDir = Path.Combine(Path.GetTempPath(), "dasblog-test-" + Path.GetRandomFileName());
			pathResolverMock.Setup(p => p.BinariesPath).Returns(Path.Combine(tempDir, "binaries"));
			pathResolverMock.Setup(p => p.ThemeFolderPath).Returns(Path.Combine(tempDir, "themes"));
			pathResolverMock.Setup(p => p.RadioStoriesFolderPath).Returns(Path.Combine(tempDir, "radio"));

			var repairer = new DasBlog.Services.Site.SiteRepairer(pathResolverMock.Object);
			Assert.NotNull(repairer);
		}

		[Fact]
		public void RepairSite_CreatesDirectories_WhenTheyDontExist()
		{
			var tempDir = Path.Combine(Path.GetTempPath(), "dasblog-test-" + Path.GetRandomFileName());
			var binariesPath = Path.Combine(tempDir, "binaries");
			var themePath = Path.Combine(tempDir, "themes", "default");
			var radioPath = Path.Combine(tempDir, "content", "radioStories");

			pathResolverMock.Setup(p => p.BinariesPath).Returns(binariesPath);
			pathResolverMock.Setup(p => p.ThemeFolderPath).Returns(themePath);
			pathResolverMock.Setup(p => p.RadioStoriesFolderPath).Returns(radioPath);

			try
			{
				var repairer = new DasBlog.Services.Site.SiteRepairer(pathResolverMock.Object);
				var (result, errorMessage) = repairer.RepairSite();

				Assert.True(result);
				Assert.Equal(string.Empty, errorMessage);
				Assert.True(Directory.Exists(binariesPath));
				Assert.True(Directory.Exists(themePath));
				Assert.True(Directory.Exists(radioPath));
			}
			finally
			{
				if (Directory.Exists(tempDir))
					Directory.Delete(tempDir, recursive: true);
			}
		}

		[Fact]
		public void RepairSite_Succeeds_WhenDirectoriesAlreadyExist()
		{
			var tempDir = Path.Combine(Path.GetTempPath(), "dasblog-test-" + Path.GetRandomFileName());
			var binariesPath = Path.Combine(tempDir, "binaries");
			var themePath = Path.Combine(tempDir, "themes");
			var radioPath = Path.Combine(tempDir, "radio");

			Directory.CreateDirectory(binariesPath);
			Directory.CreateDirectory(themePath);
			Directory.CreateDirectory(radioPath);

			pathResolverMock.Setup(p => p.BinariesPath).Returns(binariesPath);
			pathResolverMock.Setup(p => p.ThemeFolderPath).Returns(themePath);
			pathResolverMock.Setup(p => p.RadioStoriesFolderPath).Returns(radioPath);

			try
			{
				var repairer = new DasBlog.Services.Site.SiteRepairer(pathResolverMock.Object);
				var (result, errorMessage) = repairer.RepairSite();

				Assert.True(result);
				Assert.Equal(string.Empty, errorMessage);
			}
			finally
			{
				if (Directory.Exists(tempDir))
					Directory.Delete(tempDir, recursive: true);
			}
		}
	}
}
