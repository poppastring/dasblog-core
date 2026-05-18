using AutoMapper;
using DasBlog.Services;
using DasBlog.Web.Mappers;
using DasBlog.Web.Models.BlogViewModels;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using newtelligence.DasBlog.Runtime;
using System.Collections.Generic;
using Xunit;

namespace DasBlog.Tests.UnitTests.Mappers
{
	public class ProfilePostHeroImageTests
	{
		private static IMapper CreateMapper()
		{
			var settings = new Mock<IDasBlogSettings>();
			settings.Setup(s => s.GeneratePostUrl(It.IsAny<Entry>())).Returns("https://example.com/post");
			settings.Setup(s => s.GetDisplayTime(It.IsAny<System.DateTime>()))
				.Returns<System.DateTime>(dt => dt);
			settings.Setup(s => s.CompressTitle(It.IsAny<string>()))
				.Returns<string>(c => c);

			var config = new MapperConfiguration(cfg => cfg.AddProfile(new ProfilePost(settings.Object)), NullLoggerFactory.Instance);
			return config.CreateMapper();
		}

		[Fact]
		[Trait("Category", "UnitTest")]
		public void Entry_To_PostViewModel_PopulatesHeroImageFromEntry()
		{
			var mapper = CreateMapper();
			var entry = new Entry
			{
				Title = "T",
				Content = "<p>hi</p>",
				ImageUrl = "https://cdn.example.com/hero.jpg",
				ImageAlt = "Hero alt"
			};

			var vm = mapper.Map<PostViewModel>(entry);

			Assert.Equal("https://cdn.example.com/hero.jpg", vm.HeroImageUrl);
			Assert.Equal("Hero alt", vm.HeroImageAlt);
			// Resolved display image should also use the explicit hero value.
			Assert.Equal("https://cdn.example.com/hero.jpg", vm.ImageUrl);
			Assert.Equal("Hero alt", vm.ImageAlt);
		}

		[Fact]
		[Trait("Category", "UnitTest")]
		public void Entry_To_PostViewModel_EmptyHeroFieldsAreEmptyStrings()
		{
			var mapper = CreateMapper();
			var entry = new Entry
			{
				Title = "T",
				Content = "<p>no images</p>",
				ImageUrl = null,
				ImageAlt = null
			};

			var vm = mapper.Map<PostViewModel>(entry);

			Assert.Equal(string.Empty, vm.HeroImageUrl);
			Assert.Equal(string.Empty, vm.HeroImageAlt);
		}

		[Fact]
		[Trait("Category", "UnitTest")]
		public void PostViewModel_To_Entry_WritesHeroFieldsTrimmed()
		{
			var mapper = CreateMapper();
			var vm = new PostViewModel
			{
				Title = "T",
				Content = "<p>hi</p>",
				HeroImageUrl = "  https://cdn.example.com/hero.jpg  ",
				HeroImageAlt = "  Hero alt  ",
				AllCategories = new List<CategoryViewModel>()
			};

			var entry = mapper.Map<Entry>(vm);

			Assert.Equal("https://cdn.example.com/hero.jpg", entry.ImageUrl);
			Assert.Equal("Hero alt", entry.ImageAlt);
		}

		[Fact]
		[Trait("Category", "UnitTest")]
		public void PostViewModel_To_Entry_EmptyHeroFieldsBecomeNull()
		{
			var mapper = CreateMapper();
			var vm = new PostViewModel
			{
				Title = "T",
				Content = "<p>hi</p>",
				HeroImageUrl = "   ",
				HeroImageAlt = string.Empty,
				AllCategories = new List<CategoryViewModel>()
			};

			var entry = mapper.Map<Entry>(vm);

			Assert.Null(entry.ImageUrl);
			Assert.Null(entry.ImageAlt);
		}
	}
}
