using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using DasBlog.Managers.Interfaces;
using DasBlog.Web.Models.BlogViewModels;
using DasBlog.Web.Services;
using DasBlog.Web.Services.Interfaces;
using Microsoft.Extensions.Options;
using newtelligence.DasBlog.Runtime;
using Xunit;
using Moq;
using DasBlog.Services.Site;

namespace DasBlog.Tests.UnitTests.UIServices
{
	/**
	 * This would be better as an integration test.  There really isn't sufficient logic
	 * to justify a unit test
	 */
	public class BlogPostViewModelCreateorTest
	{
		private readonly IBlogManager blogManager;
		private readonly IMapper mapper;
		private ITimeZoneProvider timeZoneProvider;
		public BlogPostViewModelCreateorTest()
		{
			var blogManagerMock = new Mock<IBlogManager>();
			blogManagerMock.Setup(bm => bm.GetCategories())
			  .Returns(new CategoryCacheEntryCollection());
			this.blogManager = blogManagerMock.Object;
			var mapperMock = new Mock<IMapper>();
			mapperMock.Setup(m => m.Map<List<CategoryViewModel>>(new CategoryCacheEntryCollection()))
			  .Returns(new List<CategoryViewModel>{new CategoryViewModel()});
			this.mapper = mapperMock.Object;
			var opt = new TimeZoneProviderOptions
			{
				AdjustDisplayTimeZone = false,
				DisplayTimeZoneIndex = decimal.Zero
			};
			var optAccessor = new OptionsAccessor<TimeZoneProviderOptions>
			{
				Value = opt
			};
			timeZoneProvider = new TimeZoneProvider(optAccessor);
		}

		[Fact]
		[Trait("Category", "UnitTest")]
		public void CreatedBlogPost_ShouldShowActive_IsPublicSyndicatedAndAllowComments()
		{
			IBlogPostViewModelCreator bpvmc = new BlogPostViewModelCreator(blogManager, mapper, timeZoneProvider);
			var postViewModel = bpvmc.CreateBlogPostVM();
			Assert.True(postViewModel.IsPublic && postViewModel.Syndicated && postViewModel.AllowComments);
		}
		[Fact]
		[Trait("Category", "UnitTest")]
		public void WhenCreated_DefaultBlogPost_IncludesLotsOfLanguages()
		{
			IBlogPostViewModelCreator bpvmc = new BlogPostViewModelCreator(blogManager, mapper, timeZoneProvider);
			var postViewModel = bpvmc.CreateBlogPostVM();
			Assert.True(postViewModel.Languages.Count() > 50);
				// 841 entries on Windows 10 UK english Imac Parallels Windows Version 10.0.17134.285]
		}
		[Fact]
		[Trait("Category", "UnitTest")]
		public void WhenCreated_DefaultBlogPost_IncludesExistingCategories()
		{
			IBlogPostViewModelCreator bpvmc = new BlogPostViewModelCreator(blogManager, mapper, timeZoneProvider);
			var postViewModel = bpvmc.CreateBlogPostVM();
			Assert.Single(postViewModel.AllCategories);
		}
		[Fact]
		[Trait("Category", "UnitTest")]
		public void WhenCreated_DefaultBlogPost_UsesTimeZone()
		{
			IBlogPostViewModelCreator bpvmc = new BlogPostViewModelCreator(blogManager, mapper, timeZoneProvider);
			var postViewModel = bpvmc.CreateBlogPostVM();
			var ts = new TimeSpan(0, 0, 10);
			Assert.True(postViewModel.CreatedDateTime - ts < DateTime.UtcNow);
		}
	}
	public class OptionsAccessor<T> : IOptions<T> where T : class, new()
	{
		public T Value { get; set; }
	}
}
