using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using DasBlog.Services.FileManagement;
using DasBlog.Web.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace DasBlog.Test.Integration
{
	public class ImageFolderTests
	{
		[Fact]
		public void CreateImageFolder_ExistingParent_CreatesAndListsFolder()
		{
			using var fixture = new ImageFolderFixture();
			Directory.CreateDirectory(Path.Combine(fixture.Root, "Open-Live-Writer"));

			var created = fixture.Controller.CreateImageFolder("/Open-Live-Writer/", "New folder");

			Assert.IsType<JsonResult>(created);
			Assert.True(Directory.Exists(Path.Combine(fixture.Root, "Open-Live-Writer", "New folder")));
			var root = JsonSerializer.SerializeToElement(Assert.IsType<JsonResult>(fixture.Controller.ListImages()).Value);
			var rootSource = root.GetProperty("data").GetProperty("sources")[0];
			Assert.Contains(rootSource.GetProperty("folders").EnumerateArray(), item => item.GetString() == "Open-Live-Writer");

			var nested = JsonSerializer.SerializeToElement(Assert.IsType<JsonResult>(fixture.Controller.ListImages("/Open-Live-Writer/")).Value);
			var nestedSource = nested.GetProperty("data").GetProperty("sources")[0];
			Assert.Equal("/Open-Live-Writer/", nestedSource.GetProperty("path").GetString());
			Assert.Contains(nestedSource.GetProperty("folders").EnumerateArray(), item => item.GetString() == "..");
			Assert.Contains(nestedSource.GetProperty("folders").EnumerateArray(), item => item.GetString() == "New folder");
		}

		[Fact]
		public void CreateImageFolder_ExistingFolder_ReturnsConflict()
		{
			using var fixture = new ImageFolderFixture();
			Directory.CreateDirectory(Path.Combine(fixture.Root, "existing"));

			Assert.IsType<ConflictObjectResult>(fixture.Controller.CreateImageFolder("/", "existing"));
		}

		[Fact]
		public void DeleteImageFolder_EmptyFolder_RemovesAndRefreshesListing()
		{
			using var fixture = new ImageFolderFixture();
			Directory.CreateDirectory(Path.Combine(fixture.Root, "empty"));

			Assert.IsType<JsonResult>(fixture.Controller.DeleteImageFolder("/", "empty"));

			Assert.False(Directory.Exists(Path.Combine(fixture.Root, "empty")));
			var listed = JsonSerializer.SerializeToElement(Assert.IsType<JsonResult>(fixture.Controller.ListImages()).Value);
			Assert.Empty(listed.GetProperty("data").GetProperty("sources")[0].GetProperty("folders").EnumerateArray());
		}

		[Fact]
		public void DeleteImageFolder_NonEmptyFolder_ReturnsConflictAndPreservesContents()
		{
			using var fixture = new ImageFolderFixture();
			var folder = Path.Combine(fixture.Root, "gallery");
			Directory.CreateDirectory(folder);
			var image = Path.Combine(folder, "photo.png");
			File.WriteAllText(image, "image content");

			Assert.IsType<ConflictObjectResult>(fixture.Controller.DeleteImageFolder("/", "gallery"));
			Assert.Equal("image content", File.ReadAllText(image));
		}

		[Theory]
		[InlineData("/", "..")]
		[InlineData("/", ".. ")]
		[InlineData("/", ".")]
		[InlineData("/", "")]
		[InlineData("/", "../outside")]
		[InlineData("../outside", "folder")]
		[InlineData("/.. /outside", "folder")]
		public void DeleteImageFolder_InvalidPathOrName_PreservesBinaryRoot(string path, string name)
		{
			using var fixture = new ImageFolderFixture();

			Assert.IsType<BadRequestObjectResult>(fixture.Controller.DeleteImageFolder(path, name));
			Assert.True(Directory.Exists(fixture.Root));
		}

		[Fact]
		public void DeleteImage_ExistingImage_RemovesOnlySelectedFile()
		{
			using var fixture = new ImageFolderFixture();
			var folder = Path.Combine(fixture.Root, "gallery");
			Directory.CreateDirectory(folder);
			File.WriteAllText(Path.Combine(folder, "photo.png"), "image");
			File.WriteAllText(Path.Combine(folder, "other.png"), "other");

			Assert.IsType<JsonResult>(fixture.Controller.DeleteImage("/gallery/", "photo.png"));
			Assert.False(File.Exists(Path.Combine(folder, "photo.png")));
			Assert.Equal("other", File.ReadAllText(Path.Combine(folder, "other.png")));
			Assert.True(Directory.Exists(folder));
		}

		[Theory]
		[InlineData("/", "../photo.png")]
		[InlineData("/", "site.config")]
		[InlineData("../outside", "photo.png")]
		[InlineData("/", "")]
		public void DeleteImage_InvalidPathOrName_PreservesFiles(string path, string name)
		{
			using var fixture = new ImageFolderFixture();
			var image = Path.Combine(fixture.Root, "photo.png");
			File.WriteAllText(image, "image");

			Assert.IsType<BadRequestObjectResult>(fixture.Controller.DeleteImage(path, name));
			Assert.Equal("image", File.ReadAllText(image));
		}

		[Fact]
		public void DeleteImage_MissingImage_ReturnsNotFound()
		{
			using var fixture = new ImageFolderFixture();
			Assert.IsType<NotFoundObjectResult>(fixture.Controller.DeleteImage("/", "missing.png"));
		}

		private sealed class ImageFolderFixture : IDisposable
		{
			private readonly string temporaryRoot = Path.Combine(Path.GetTempPath(), "dasblog-image-folder-" + Guid.NewGuid().ToString("N"));
			public string Root => Path.Combine(temporaryRoot, "binary");
			public BlogPostController Controller { get; }

			public ImageFolderFixture()
			{
				var configuration = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string>
				{
					["BinariesDir"] = Root
				}).Build();
				var resolver = new DasBlogPathResolver(temporaryRoot, "Test", configuration);
				Controller = new BlogPostController(null, null, null, null, null, null, null, null,
					NullLogger<BlogPostController>.Instance, null, null, null, resolver);
				var context = new ActionContext(new DefaultHttpContext(), new RouteData(), new ControllerActionDescriptor());
				Controller.ControllerContext = new ControllerContext(context);
				Controller.Url = new UrlHelper(context);
				Directory.CreateDirectory(Root);
			}

			public void Dispose()
			{
				Directory.Delete(temporaryRoot, true);
			}
		}
	}
}
