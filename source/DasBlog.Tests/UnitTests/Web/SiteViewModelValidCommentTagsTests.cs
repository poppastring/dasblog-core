using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using DasBlog.Core.Common.Comments;
using DasBlog.Services;
using DasBlog.Services.ConfigFile;
using DasBlog.Web.Mappers;
using DasBlog.Web.Models.AdminViewModels;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Xunit;

namespace DasBlog.Tests.UnitTests.Web
{
	public class SiteViewModelValidCommentTagsTests
	{
		[Theory]
		[InlineData(true)]
		[InlineData(false)]
		[Trait("Category", "UnitTest")]
		public void NormalizeValidCommentTags_RoundTripRemovesMalformedEntriesAndPreservesValues(bool enableComments)
		{
			var mapper = CreateMapper();
			var siteConfig = new SiteConfig
			{
				EnableComments = enableComments,
				ValidCommentTags = new[]
				{
					new ValidCommentTags
					{
						Tag = new List<Tag>
						{
							null,
							new Tag { Name = null, Allowed = true },
							new Tag { Name = " ", Allowed = true },
							new Tag { Name = "a", Attributes = "href,title", Allowed = true },
							new Tag { Name = "A", Attributes = "duplicate", Allowed = false },
							new Tag { Name = " strong ", Attributes = string.Empty, Allowed = false }
						}
					}
				}
			};

			var viewModel = mapper.Map<SiteViewModel>(siteConfig);
			viewModel.NormalizeValidCommentTags();
			var savedConfig = mapper.Map<SiteConfig>(viewModel);
			var tags = savedConfig.ValidCommentTags[0].Tag;

			Assert.Equal(enableComments, savedConfig.EnableComments);
			Assert.Equal(2, tags.Count);
			Assert.Equal(new[] { "a", "strong" }, tags.Select(tag => tag.Name));
			Assert.Equal("href,title", tags[0].Attributes);
			Assert.True(tags[0].Allowed);
			Assert.Equal(string.Empty, tags[1].Attributes);
			Assert.False(tags[1].Allowed);
		}

		[Fact]
		[Trait("Category", "UnitTest")]
		public void NormalizeValidCommentTags_NullTagListBecomesEmptyList()
		{
			var viewModel = new SiteViewModel
			{
				ValidCommentTags = new[] { new ValidCommentTagsViewModel() }
			};

			viewModel.NormalizeValidCommentTags();

			Assert.Empty(viewModel.ValidCommentTags[0].Tag);
		}

		private static IMapper CreateMapper()
		{
			var settings = new Mock<IDasBlogSettings>();
			var config = new MapperConfiguration(
				cfg =>
				{
					cfg.AddProfile(new ProfileSettings());
					cfg.AddProfile(new ProfilePost(settings.Object));
				},
				NullLoggerFactory.Instance);

			return config.CreateMapper();
		}
	}
}
