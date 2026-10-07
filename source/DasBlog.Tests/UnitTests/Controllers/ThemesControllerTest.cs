using System.Collections.Generic;
using System.IO;
using AutoMapper;
using DasBlog.Managers.Interfaces;
using DasBlog.Services;
using DasBlog.Services.ConfigFile;
using DasBlog.Web.Controllers;
using DasBlog.Web.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

namespace DasBlog.Tests.UnitTests.Controllers
{
	public class ThemesControllerTest
	{
		[Fact]
		public void SetActive_WhenSaveReturnsFalse_PreservesLiveThemeAndReportsFailure()
		{
			var fixture = new Fixture();
			fixture.Files.Setup(files => files.SaveSiteConfig(fixture.Candidate)).Callback(() => fixture.AssertDuringSave()).Returns(false);
			fixture.AssertRedirect(fixture.Controller.SetActive("selected-alias"));
			fixture.AssertLive("dasblog");
			Assert.Equal("Unable to save site configuration.", fixture.Controller.TempData["ErrorMessage"]);
			Assert.False(fixture.Controller.TempData.ContainsKey("SuccessMessage"));
			var entry = Assert.Single(fixture.Logger.Entries);
			Assert.Equal(LogLevel.Error, entry.Level);
			Assert.Equal(500, entry.EventId.Id);
			Assert.Null(entry.Exception);
			Assert.Equal("Error", entry.State["0"]);
			Assert.Equal("(null)", entry.State["1"]);
			Assert.Equal("{0} :: Unable to save site config when setting active theme :: {1}", entry.State["{OriginalFormat}"]);
			fixture.Verify();
		}

		[Fact]
		public void SetActive_WhenSaveThrows_PreservesLiveThemeAndLogsOriginalException()
		{
			var fixture = new Fixture();
			var exception = new IOException("Destination is locked");
			fixture.Files.Setup(files => files.SaveSiteConfig(fixture.Candidate)).Callback(() => fixture.AssertDuringSave()).Throws(exception);
			fixture.AssertRedirect(fixture.Controller.SetActive("selected-alias"));
			fixture.AssertLive("dasblog");
			Assert.Equal(exception.Message, fixture.Controller.TempData["ErrorMessage"]);
			Assert.False(fixture.Controller.TempData.ContainsKey("SuccessMessage"));
			var entry = Assert.Single(fixture.Logger.Entries);
			Assert.Equal(LogLevel.Error, entry.Level);
			Assert.Same(exception, entry.Exception);
			Assert.Equal("selected-alias", entry.State["Theme"]);
			Assert.Equal("Unable to activate theme {Theme}", entry.State["{OriginalFormat}"]);
			fixture.Verify();
		}

		[Fact]
		public void SetActive_WhenSaveSucceeds_UpdatesLiveThemeOnlyAfterSaveReturns()
		{
			var fixture = new Fixture();
			fixture.Files.Setup(files => files.SaveSiteConfig(fixture.Candidate)).Callback(() => fixture.AssertDuringSave()).Returns(true);
			fixture.AssertRedirect(fixture.Controller.SetActive("selected-alias"));
			fixture.AssertLive("darkly");
			Assert.Equal("Active theme set to 'darkly'.", fixture.Controller.TempData["SuccessMessage"]);
			Assert.False(fixture.Controller.TempData.ContainsKey("ErrorMessage"));
			var entry = Assert.Single(fixture.Logger.Entries);
			Assert.Equal(LogLevel.Information, entry.Level);
			Assert.Null(entry.Exception);
			Assert.Equal("Site", entry.State["0"]);
			Assert.Equal("Site :: Active theme set to 'darkly' :: (null)", entry.Message);
			fixture.Verify();
		}

		private sealed class Fixture
		{
			public SiteConfig Live { get; } = new SiteConfig { Theme = "dasblog", Description = "Keep original description" };
			public SiteConfig Candidate { get; } = new SiteConfig { Theme = "dasblog", Description = "Keep original description" };
			public Mock<IFileSystemBinaryManager> Files { get; } = new(MockBehavior.Strict);
			public RegressionLogger<ThemesController> Logger { get; } = new();
			public ThemesController Controller { get; }
			private readonly Mock<IMapper> mapper = new(MockBehavior.Strict);
			private readonly Mock<IThemeManager> themes = new(MockBehavior.Strict);
			private readonly Mock<IDasBlogSettings> settings = new(MockBehavior.Strict);
			private readonly Mock<IOptionsMonitor<SiteConfig>> monitor = new(MockBehavior.Strict);
			public Fixture()
			{
				settings.SetupGet(value => value.SiteConfiguration).Returns(Live);
				monitor.SetupGet(value => value.CurrentValue).Returns(Live);
				themes.Setup(value => value.GetTheme("selected-alias")).Returns(new ThemeInfo { Name = "darkly" });
				mapper.Setup(value => value.Map<SiteConfig, SiteConfig>(Live)).Returns(Candidate);
				var context = new DefaultHttpContext();
				Controller = new ThemesController(settings.Object, Files.Object, themes.Object, Mock.Of<IThemeContentValidator>(),
					mapper.Object, monitor.Object, Logger)
				{
					ControllerContext = new ControllerContext { HttpContext = context },
					TempData = new TempDataDictionary(context, new InMemoryTempDataProvider())
				};
			}
			public void AssertDuringSave()
			{
				Assert.NotSame(Live, Candidate);
				Assert.Equal("darkly", Candidate.Theme);
				Assert.Equal("Keep original description", Candidate.Description);
				AssertLive("dasblog");
			}
			public void AssertLive(string theme)
			{
				Assert.Same(Live, monitor.Object.CurrentValue);
				Assert.Same(Live, settings.Object.SiteConfiguration);
				Assert.Equal(theme, Live.Theme);
				Assert.Equal("Keep original description", Live.Description);
			}
			public void AssertRedirect(IActionResult result) => Assert.Equal("Index", Assert.IsType<RedirectToActionResult>(result).ActionName);
			public void Verify()
			{
				mapper.Verify(value => value.Map<SiteConfig, SiteConfig>(Live), Times.Once);
				themes.Verify(value => value.GetTheme("selected-alias"), Times.Once);
				Files.Verify(value => value.SaveSiteConfig(Candidate), Times.Once);
				mapper.VerifyNoOtherCalls();
				themes.VerifyNoOtherCalls();
				Files.VerifyNoOtherCalls();
			}
		}

		private sealed class InMemoryTempDataProvider : ITempDataProvider
		{
			public IDictionary<string, object> LoadTempData(HttpContext context) => new Dictionary<string, object>();
			public void SaveTempData(HttpContext context, IDictionary<string, object> values) { }
		}
	}
}

