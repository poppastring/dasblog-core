﻿using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using DasBlog.Web.TagHelpers.Site;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Xunit;

namespace DasBlog.Tests.UnitTests.UI
{
	public class SeoTagHelperTests
	{
		private static (TagHelperContext ctx, TagHelperOutput output) MakeContext(string tagName)
		{
			var ctx = new TagHelperContext(new TagHelperAttributeList(), new Dictionary<object, object>(), Guid.NewGuid().ToString("N"));
			var output = new TagHelperOutput(tagName, new TagHelperAttributeList(), (_, _) =>
				Task.FromResult<TagHelperContent>(new DefaultTagHelperContent()));
			return (ctx, output);
		}

		private static ViewContext MakeViewContext()
		{
			return new ViewContext();
		}

		// ----- OpenGraphTagHelper -----

		[Fact]
		[Trait("Category", "UnitTest")]
		public void OpenGraphTagHelper_NoOgTypeSet_DefaultsToWebsite()
		{
			var sut = new OpenGraphTagHelper { ViewContext = MakeViewContext() };
			var (ctx, output) = MakeContext("open-graph");

			sut.Process(ctx, output);

			var html = output.Content.GetContent();
			Assert.Contains("<meta property=\"og:type\" content=\"website\"", html);
			Assert.DoesNotContain("content=\"article\"", html);
		}

		[Fact]
		[Trait("Category", "UnitTest")]
		public void OpenGraphTagHelper_OgTypeArticle_EmitsArticle()
		{
			var viewContext = MakeViewContext();
			viewContext.ViewData["OgType"] = "article";
			var sut = new OpenGraphTagHelper { ViewContext = viewContext };
			var (ctx, output) = MakeContext("open-graph");

			sut.Process(ctx, output);

			Assert.Contains("<meta property=\"og:type\" content=\"article\"", output.Content.GetContent());
		}

		[Fact]
		[Trait("Category", "UnitTest")]
		public void OpenGraphTagHelper_OgTypeEmpty_DefaultsToWebsite()
		{
			var viewContext = MakeViewContext();
			viewContext.ViewData["OgType"] = string.Empty;
			var sut = new OpenGraphTagHelper { ViewContext = viewContext };
			var (ctx, output) = MakeContext("open-graph");

			sut.Process(ctx, output);

			Assert.Contains("content=\"website\"", output.Content.GetContent());
		}

		[Fact]
		[Trait("Category", "UnitTest")]
		public void OpenGraphTagHelper_VideoUrlMissing_OmitsVideoMeta()
		{
			var sut = new OpenGraphTagHelper { ViewContext = MakeViewContext() };
			var (ctx, output) = MakeContext("open-graph");

			sut.Process(ctx, output);

			Assert.DoesNotContain("og:video", output.Content.GetContent());
		}

		[Fact]
		[Trait("Category", "UnitTest")]
		public void OpenGraphTagHelper_VideoUrlWhitespace_OmitsVideoMeta()
		{
			var viewContext = MakeViewContext();
			viewContext.ViewData["PageVideoUrl"] = "   ";
			var sut = new OpenGraphTagHelper { ViewContext = viewContext };
			var (ctx, output) = MakeContext("open-graph");

			sut.Process(ctx, output);

			Assert.DoesNotContain("og:video", output.Content.GetContent());
		}

		[Fact]
		[Trait("Category", "UnitTest")]
		public void OpenGraphTagHelper_VideoUrlPresent_EmitsVideoMeta()
		{
			var viewContext = MakeViewContext();
			viewContext.ViewData["PageVideoUrl"] = "https://example.com/clip.mp4";
			var sut = new OpenGraphTagHelper { ViewContext = viewContext };
			var (ctx, output) = MakeContext("open-graph");

			sut.Process(ctx, output);

			Assert.Contains("<meta property=\"og:video\" content=\"https://example.com/clip.mp4\"", output.Content.GetContent());
		}

		// ----- BlogPostingSchemaTagHelper -----

		private static JsonElement ProcessSchema(ViewContext viewContext)
		{
			var sut = new BlogPostingSchemaTagHelper { ViewContext = viewContext };
			var (ctx, output) = MakeContext("blog-posting-schema");
			sut.Process(ctx, output);
			using var doc = JsonDocument.Parse(output.Content.GetContent());
			return doc.RootElement.Clone();
		}

		[Fact]
		[Trait("Category", "UnitTest")]
		public void BlogPostingSchema_MinimalData_OmitsEmptyFieldsAndAuthorAndMainEntity()
		{
			var json = ProcessSchema(MakeViewContext());

			Assert.Equal("http://schema.org", json.GetProperty("@context").GetString());
			Assert.Equal("BlogPosting", json.GetProperty("@type").GetString());
			Assert.False(json.TryGetProperty("headline", out _));
			Assert.False(json.TryGetProperty("description", out _));
			Assert.False(json.TryGetProperty("url", out _));
			Assert.False(json.TryGetProperty("image", out _));
			Assert.False(json.TryGetProperty("datePublished", out _));
			Assert.False(json.TryGetProperty("dateModified", out _));
			Assert.False(json.TryGetProperty("author", out _));
			Assert.False(json.TryGetProperty("mainEntityOfPage", out _));
		}

		[Fact]
		[Trait("Category", "UnitTest")]
		public void BlogPostingSchema_DateModifiedMissing_FallsBackToDatePublished()
		{
			var viewContext = MakeViewContext();
			viewContext.ViewData["DatePublished"] = "2026-06-12T10:00:00Z";
			var json = ProcessSchema(viewContext);

			Assert.Equal("2026-06-12T10:00:00Z", json.GetProperty("datePublished").GetString());
			Assert.Equal("2026-06-12T10:00:00Z", json.GetProperty("dateModified").GetString());
		}

		[Fact]
		[Trait("Category", "UnitTest")]
		public void BlogPostingSchema_ExplicitDateModified_OverridesFallback()
		{
			var viewContext = MakeViewContext();
			viewContext.ViewData["DatePublished"] = "2026-06-12T10:00:00Z";
			viewContext.ViewData["DateModified"] = "2026-06-13T12:00:00Z";
			var json = ProcessSchema(viewContext);

			Assert.Equal("2026-06-13T12:00:00Z", json.GetProperty("dateModified").GetString());
		}

		[Fact]
		[Trait("Category", "UnitTest")]
		public void BlogPostingSchema_AuthorNameOnly_EmitsPersonWithoutUrl()
		{
			var viewContext = MakeViewContext();
			viewContext.ViewData["Author"] = "Test Author";
			var json = ProcessSchema(viewContext);

			var author = json.GetProperty("author");
			Assert.Equal("Person", author.GetProperty("@type").GetString());
			Assert.Equal("Test Author", author.GetProperty("name").GetString());
			Assert.False(author.TryGetProperty("url", out _));
		}

		[Fact]
		[Trait("Category", "UnitTest")]
		public void BlogPostingSchema_AuthorNameAndUrl_EmitsBoth()
		{
			var viewContext = MakeViewContext();
			viewContext.ViewData["Author"] = "Test Author";
			viewContext.ViewData["AuthorUrl"] = "https://example.com/";
			var json = ProcessSchema(viewContext);

			var author = json.GetProperty("author");
			Assert.Equal("Test Author", author.GetProperty("name").GetString());
			Assert.Equal("https://example.com/", author.GetProperty("url").GetString());
		}

		[Fact]
		[Trait("Category", "UnitTest")]
		public void BlogPostingSchema_AuthorAbsent_OmitsAuthorField()
		{
			var viewContext = MakeViewContext();
			viewContext.ViewData["Author"] = string.Empty;
			viewContext.ViewData["AuthorUrl"] = string.Empty;
			var json = ProcessSchema(viewContext);

			Assert.False(json.TryGetProperty("author", out _));
		}

		[Fact]
		[Trait("Category", "UnitTest")]
		public void BlogPostingSchema_CanonicalPresent_EmitsMainEntityOfPage()
		{
			var viewContext = MakeViewContext();
			viewContext.ViewData["Canonical"] = "https://example.com/post/hello";
			var json = ProcessSchema(viewContext);

			var main = json.GetProperty("mainEntityOfPage");
			Assert.Equal("WebPage", main.GetProperty("@type").GetString());
			Assert.Equal("https://example.com/post/hello", main.GetProperty("@id").GetString());
			Assert.Equal("https://example.com/post/hello", json.GetProperty("url").GetString());
		}

		[Fact]
		[Trait("Category", "UnitTest")]
		public void BlogPostingSchema_FullPayload_ProducesValidJson()
		{
			var viewContext = MakeViewContext();
			viewContext.ViewData["PageTitle"] = "Hello \"world\"";
			viewContext.ViewData["Description"] = "A post";
			viewContext.ViewData["Canonical"] = "https://example.com/post/hello";
			viewContext.ViewData["PageImageUrl"] = "https://example.com/img.png";
			viewContext.ViewData["DatePublished"] = "2026-06-12T10:00:00Z";
			viewContext.ViewData["Author"] = "Test Author";
			viewContext.ViewData["AuthorUrl"] = "https://example.com/";

			var json = ProcessSchema(viewContext);

			Assert.Equal("Hello \"world\"", json.GetProperty("headline").GetString());
			Assert.Equal("https://example.com/img.png", json.GetProperty("image").GetString());
			Assert.Equal("Test Author", json.GetProperty("author").GetProperty("name").GetString());
		}
	}
}
