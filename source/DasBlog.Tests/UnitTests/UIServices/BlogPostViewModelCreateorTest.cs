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
	public class BlogPostViewModelCreateorTest
	{
		private readonly IBlogManager blogManager;
		private readonly IMapper mapper;

		public BlogPostViewModelCreateorTest()
		{
			IServiceProvider serviceProvider = InjectDependencies();
			var blogManagerMock = new Mock<IBlogManager>();
			blogManagerMock.Setup(bm => bm.GetCategories())
			  .Returns(new CategoryCacheEntryCollection());
			this.blogManager = blogManagerMock.Object;
			var mapperMock = new Mock<IMapper>();
			mapperMock.Setup(m => m.Map<IBlogManager, IList<CategoryViewModel>>(this.blogManager))
			  .Returns(new List<CategoryViewModel>{new CategoryViewModel()});
			this.mapper = mapperMock.Object;
		}

		private IServiceProvider InjectDependencies()
		{
			var services = new ServiceCollection();
			
			return services.BuildServiceProvider();
		}

		[Fact]
		[Trait("Category", "UnitTest")]
		public void WhenCreated_DefaultBlogPost_IsPublic()
		{
			IBlogPostViewModelCreator bpvmc = new BlogPostViewModelCreator(blogManager, mapper);
			var postViewModel = bpvmc.CreateBlogPostVN();
			Assert.True(postViewModel.IsPublic);
		}
		[Fact]
		[Trait("Category", "UnitTest")]
		public void WhenCreated_DefaultBlogPost_IncludesLotsOfLanguages()
		{
			IBlogPostViewModelCreator bpvmc = new BlogPostViewModelCreator(blogManager, mapper);
			var postViewModel = bpvmc.CreateBlogPostVN();
			Assert.True(postViewModel.Languages.Count() > 50);
				// 841 entries on Windows 10 UK english Imac Parallels Windows Version 10.0.17134.285]
		}
		[Fact(Skip="true")]
		[Trait("Category", "UnitTest")]
		public void WhenCreated_DefaultBlogPost_IncludesExistingCategories()
		{
			IBlogPostViewModelCreator bpvmc = new BlogPostViewModelCreator(blogManager, mapper);
			var postViewModel = bpvmc.CreateBlogPostVN();
			Assert.Equal(1, postViewModel.AllCategories.Count);
		}
	}
}
