using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Xml.Serialization;
using DasBlog.Services;
using DasBlog.Services.ConfigFile;
using DasBlog.Services.ConfigFile.Interfaces;
using DasBlog.Web;
using DasBlog.Web.TagHelpers.RichEdit;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace DasBlog.Tests.UnitTests.Web
{
	public class RichEditorUpgradeTests
	{
		[Theory]
		[InlineData("TinyMce")]
		[InlineData("Froala")]
		[InlineData("NicEdit")]
		[InlineData("TextArea")]
		public void AddDasBlogWebServices_LegacyEditorSelection_UsesJodit(string legacySelection)
		{
			var services = new ServiceCollection();
			services.AddDasBlogWebServices(new ConfigurationBuilder().Build());
			services.AddSingleton<ISiteConfig>(new SiteConfig { EntryEditControl = legacySelection });
			services.AddSingleton<IUrlResolver>(Mock.Of<IUrlResolver>());

			using var provider = services.BuildServiceProvider();

			Assert.IsType<JoditBuilder>(provider.GetRequiredService<IRichEditBuilder>());
		}

		[Fact]
		public void SiteConfig_DeserializesLegacyEditorSettingsWithoutTinyMceApiKeyProperty()
		{
			const string xml = "<SiteConfig><EntryEditControl>TinyMce</EntryEditControl><TinyMCEApiKey>old-key</TinyMCEApiKey></SiteConfig>";
			var serializer = new XmlSerializer(typeof(SiteConfig));
			using var reader = new StringReader(xml);

			var siteConfig = Assert.IsType<SiteConfig>(serializer.Deserialize(reader));

			Assert.Equal("TinyMce", siteConfig.EntryEditControl);
			Assert.Null(typeof(SiteConfig).GetProperty("TinyMCEApiKey"));
		}

		[Fact]
		public void JoditBuilder_UsesPinnedReleaseAndKeepsImageApiEndpoints()
		{
			var urlResolver = new Mock<IUrlResolver>();
			urlResolver.Setup(resolver => resolver.GetBaseUrl()).Returns("https://blog.example/");
			var builder = new JoditBuilder(urlResolver.Object);
			var helper = new RichEditScriptsTagHelper(builder) { ControlId = "editor" };
			var context = new TagHelperContext(new TagHelperAttributeList(), new Dictionary<object, object>(), "test");
			var output = new TagHelperOutput("script", new TagHelperAttributeList(),
				(_, _) => Task.FromResult<TagHelperContent>(new DefaultTagHelperContent()));

			builder.ProcessScripts(helper, context, output);

			Assert.Equal("https://cdn.jsdelivr.net/npm/jodit@4.19.10/es2021/jodit.min.js", output.Attributes["src"].Value);
			var markup = output.PostElement.GetContent();
			Assert.Contains("https://cdn.jsdelivr.net/npm/jodit@4.19.10/es2021/jodit.min.css", markup);
			Assert.Contains("imageApiPath + '/list'", markup);
			Assert.Contains("imageApiPath + '/upload'", markup);
		}
	}
}
