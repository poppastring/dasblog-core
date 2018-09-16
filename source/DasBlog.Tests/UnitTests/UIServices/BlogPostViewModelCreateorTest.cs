using System;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using DasBlog.Managers;
using DasBlog.Managers.Interfaces;
using DasBlog.Web.Models.BlogViewModels;
using DasBlog.Web.Services;
using DasBlog.Web.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using newtelligence.DasBlog.Runtime;
using Remotion.Linq.Parsing.Structure.IntermediateModel;
using Xunit;
using Moq;


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
		}

		[Fact]
		[Trait("Category", "UnitTest")]
		public void CreatedBlogPost_ShouldShowActive_IsPublicSyndicatedAndAllowComments()
		{
			IBlogPostViewModelCreator bpvmc = new BlogPostViewModelCreator(blogManager, mapper);
			var postViewModel = bpvmc.CreateBlogPostVM();
			Assert.True(postViewModel.IsPublic && postViewModel.Syndicated && postViewModel.AllowComments);
		}
		[Fact]
		[Trait("Category", "UnitTest")]
		public void WhenCreated_DefaultBlogPost_IncludesLotsOfLanguages()
		{
			IBlogPostViewModelCreator bpvmc = new BlogPostViewModelCreator(blogManager, mapper);
			var postViewModel = bpvmc.CreateBlogPostVM();
			Assert.True(postViewModel.Languages.Count() > 50);
				// 841 entries on Windows 10 UK english Imac Parallels Windows Version 10.0.17134.285]
		}
		[Fact]
		[Trait("Category", "UnitTest")]
		public void WhenCreated_DefaultBlogPost_IncludesExistingCategories()
		{
			IBlogPostViewModelCreator bpvmc = new BlogPostViewModelCreator(blogManager, mapper);
			var postViewModel = bpvmc.CreateBlogPostVM();
			Assert.Equal(1, postViewModel.AllCategories.Count);
		}
		[Fact]
		[Trait("Category", "UnitTest")]
		public void WhenCreated_DefaultBlogPost_UsesTimeZone()
		{
			IBlogPostViewModelCreator bpvmc = new BlogPostViewModelCreator(blogManager, mapper);
			var postViewModel = bpvmc.CreateBlogPostVM();
			var ts = new TimeSpan(0, 0, 10);
			Assert.True(postViewModel.CreatedDateTime - ts < DateTime.UtcNow);
		}
	}
}
