using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DasBlog.Web;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace DasBlog.Tests.UnitTests.Web
{
	public class DasBlogSecurityHeadersTests
	{
		[Theory]
		[InlineData(null)]
		[InlineData("")]
		[InlineData(" ; ; ")]
		public async Task UseDasBlogSecurityHeaders_MissingOrBlankSources_EmitsRequiredPolicy(string sources)
		{
			var headers = await GetHeaders(new Dictionary<string, string>
			{
				["SecurityStyleSources"] = sources,
				["SecurityScriptSources"] = sources,
				["DefaultSources"] = sources
			});

			var directives = ParsePolicy(headers);
			Assert.Equal(new[] { "'self'" }, directives["default-src"]);
			foreach (var directive in new[] { "style-src", "script-src", "font-src" })
			{
				Assert.Contains("'self'", directives[directive]);
				Assert.Contains("https://cdnjs.cloudflare.com", directives[directive]);
			}
			Assert.Contains("https://ajax.aspnetcdn.com", directives["script-src"]);
			Assert.Contains("https://fonts.googleapis.com", directives["style-src"]);
			Assert.Contains("https://fonts.gstatic.com", directives["font-src"]);
			Assert.Contains("'unsafe-inline'", directives["style-src"]);
			Assert.Contains("'unsafe-inline'", directives["script-src"]);
			Assert.Contains("'unsafe-eval'", directives["script-src"]);
			foreach (var directive in new[] { "style-src", "script-src", "font-src", "img-src" })
				Assert.Contains("https://cdn.jsdelivr.net", directives[directive]);
			Assert.Equal("nosniff", headers["X-Content-Type-Options"].ToString());
			Assert.Equal("SameOrigin", headers["X-Frame-Options"].ToString());
			Assert.True(headers.ContainsKey("Permissions-Policy"));
		}

		[Theory]
		[InlineData("SecurityStyleSources")]
		[InlineData("SecurityScriptSources")]
		[InlineData("DefaultSources")]
		public async Task UseDasBlogSecurityHeaders_OneSourceSettingMissing_KeepsPolicyAndOtherSources(string missingKey)
		{
			var sources = new Dictionary<string, string>
			{
				["SecurityStyleSources"] = "https://styles.example.com",
				["SecurityScriptSources"] = "https://scripts.example.com",
				["DefaultSources"] = "https://media.example.com"
			};
			sources.Remove(missingKey);

			var directives = ParsePolicy(await GetHeaders(sources));

			Assert.Contains("https://cdnjs.cloudflare.com", directives["style-src"]);
			Assert.Contains("https://cdnjs.cloudflare.com", directives["script-src"]);
			Assert.Contains("https://cdnjs.cloudflare.com", directives["font-src"]);
			if (missingKey != "SecurityStyleSources")
				Assert.Contains("https://styles.example.com", directives["style-src"]);
			if (missingKey != "SecurityScriptSources")
				Assert.Contains("https://scripts.example.com", directives["script-src"]);
			if (missingKey != "DefaultSources")
			{
				Assert.Contains("https://media.example.com", directives["default-src"]);
				Assert.Contains("https://media.example.com", directives["font-src"]);
			}
		}

		[Fact]
		public async Task UseDasBlogSecurityHeaders_CustomSources_MergesAndTrimsWithoutBroadeningOtherDirectives()
		{
			var directives = ParsePolicy(await GetHeaders(new Dictionary<string, string>
			{
				["SecurityStyleSources"] = " https://styles.example.com ; ; https://cdnjs.cloudflare.com ; https://styles2.example.com ",
				["SecurityScriptSources"] = " https://scripts.example.com ; ; https://cdnjs.cloudflare.com ",
				["DefaultSources"] = " data: ; https://media.example.com ; "
			}));

			Assert.Contains("https://styles.example.com", directives["style-src"]);
			Assert.Contains("https://styles2.example.com", directives["style-src"]);
			Assert.Contains("https://scripts.example.com", directives["script-src"]);
			Assert.Contains("data:", directives["default-src"]);
			Assert.Contains("https://media.example.com", directives["default-src"]);
			Assert.Contains("data:", directives["font-src"]);
			Assert.Contains("https://media.example.com", directives["font-src"]);
			Assert.Contains("'self'", directives["font-src"]);
			Assert.Contains("data:", directives["img-src"]);
			Assert.Contains("https://media.example.com", directives["img-src"]);
			Assert.Contains("https://cdn.jsdelivr.net", directives["img-src"]);
			Assert.DoesNotContain("https://cdnjs.cloudflare.com", directives["default-src"]);
			Assert.DoesNotContain("https://cdn.jsdelivr.net", directives["default-src"]);
			Assert.DoesNotContain("https://ajax.aspnetcdn.com", directives["style-src"]);
			Assert.DoesNotContain("https://fonts.googleapis.com", directives["script-src"]);
			Assert.DoesNotContain("https://fonts.gstatic.com", directives["style-src"]);
			Assert.DoesNotContain("https://styles.example.com", directives["script-src"]);
			Assert.DoesNotContain("https://scripts.example.com", directives["style-src"]);
			foreach (var directive in new[] { "style-src", "script-src", "font-src" })
				Assert.Equal(1, directives[directive].Count(source => source == "https://cdnjs.cloudflare.com"));
		}

		private static async Task<IHeaderDictionary> GetHeaders(Dictionary<string, string> sources)
		{
			using var services = new ServiceCollection().AddLogging().AddOptions().BuildServiceProvider();
			var configuration = new ConfigurationBuilder().AddInMemoryCollection(sources).Build();
			var app = new ApplicationBuilder(services);
			app.UseDasBlogSecurityHeaders(configuration);
			app.Run(context => Task.CompletedTask);
			var context = new DefaultHttpContext { RequestServices = services };

			await app.Build()(context);

			return context.Response.Headers;
		}

		private static Dictionary<string, string[]> ParsePolicy(IHeaderDictionary headers)
		{
			Assert.True(headers.ContainsKey("Content-Security-Policy"));
			return headers["Content-Security-Policy"].ToString()
				.Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
				.Select(directive => directive.Split(' ', StringSplitOptions.RemoveEmptyEntries))
				.ToDictionary(tokens => tokens[0], tokens => tokens.Skip(1).ToArray());
		}
	}
}
