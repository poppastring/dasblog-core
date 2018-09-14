using System;
using System.Linq;
using AutoMapper;
using DasBlog.Managers;
using DasBlog.Managers.Interfaces;
using DasBlog.Web.Models.BlogViewModels;
using DasBlog.Web.Services;
using DasBlog.Web.Services.Interfaces;
using newtelligence.DasBlog.Runtime;
using Remotion.Linq.Parsing.Structure.IntermediateModel;
using Xunit;

namespace DasBlog.Tests.UnitTests.UIServices
{
	public class BlogPostViewModelCreateorTest
	{
		private readonly IBlogManager blogManager;
		private readonly IMapper mapper;

		public BlogPostViewModelCreateorTest()
		{
			this.blogManager = new FakeBlogManager();
			this.mapper = new FakeMapper();
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
		[Fact(Skip = "true")]
		[Trait("Category", "UnitTest")]
		public void WhenCreated_DefaultBlogPost_IncludesExistingCategories()
		{
			IBlogPostViewModelCreator bpvmc = new BlogPostViewModelCreator(blogManager, mapper);
			var postViewModel = bpvmc.CreateBlogPostVN();
			Assert.Equal(5, postViewModel.AllCategories.Count);
		}
	}

	internal class FakeBlogManager : IBlogManager
	{
		public Entry GetBlogPost(string postid, DateTime? postDate)
		{
			throw new NotImplementedException();
		}

		public Entry GetEntryForEdit(string postid)
		{
			throw new NotImplementedException();
		}

		public EntryCollection GetFrontPagePosts(string acceptLanguageHeader)
		{
			throw new NotImplementedException();
		}

		public EntryCollection GetEntriesForPage(int pageIndex, string acceptLanguageHeader)
		{
			throw new NotImplementedException();
		}

		public EntrySaveState CreateEntry(Entry entry)
		{
			throw new NotImplementedException();
		}

		public EntrySaveState UpdateEntry(Entry entry)
		{
			throw new NotImplementedException();
		}

		public void DeleteEntry(string postid)
		{
			throw new NotImplementedException();
		}

		public CategoryCacheEntryCollection GetCategories()
		{
			throw new NotImplementedException();
		}

		public CommentSaveState AddComment(string postid, Comment comment)
		{
			throw new NotImplementedException();
		}

		public CommentSaveState DeleteComment(string postid, string commentid)
		{
			throw new NotImplementedException();
		}

		public CommentSaveState ApproveComment(string postid, string commentid)
		{
			throw new NotImplementedException();
		}

		public CommentCollection GetComments(string postid, bool allComments)
		{
			throw new NotImplementedException();
		}

		public EntryCollection SearchEntries(string searchString, string acceptLanguageHeader)
		{
			throw new NotImplementedException();
		}
	}

	internal class FakeMapper : IMapper
	{
		public TDestination Map<TDestination>(object source)
		{
			throw new NotImplementedException();
		}

		public TDestination Map<TDestination>(object source, Action<IMappingOperationOptions> opts)
		{
			throw new NotImplementedException();
		}

		public TDestination Map<TSource, TDestination>(TSource source)
		{
			throw new NotImplementedException();
		}

		public TDestination Map<TSource, TDestination>(TSource source, Action<IMappingOperationOptions<TSource, TDestination>> opts)
		{
			throw new NotImplementedException();
		}

		public TDestination Map<TSource, TDestination>(TSource source, TDestination destination)
		{
			throw new NotImplementedException();
		}

		public TDestination Map<TSource, TDestination>(TSource source, TDestination destination, Action<IMappingOperationOptions<TSource, TDestination>> opts)
		{
			throw new NotImplementedException();
		}

		public object Map(object source, Type sourceType, Type destinationType)
		{
			throw new NotImplementedException();
		}

		public object Map(object source, Type sourceType, Type destinationType, Action<IMappingOperationOptions> opts)
		{
			throw new NotImplementedException();
		}

		public object Map(object source, object destination, Type sourceType, Type destinationType)
		{
			throw new NotImplementedException();
		}

		public object Map(object source, object destination, Type sourceType, Type destinationType, Action<IMappingOperationOptions> opts)
		{
			throw new NotImplementedException();
		}

		public IConfigurationProvider ConfigurationProvider { get; }
		public Func<Type, object> ServiceCtor { get; }
	}
}
