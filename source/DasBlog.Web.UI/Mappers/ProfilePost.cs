using AutoMapper;
using DasBlog.Web.Models.BlogViewModels;
using DasBlog.Core.Common;
using DasBlog.Core.Extensions;
using newtelligence.DasBlog.Runtime;
using System.Collections.Generic;
using System.Linq;
using DasBlog.Services;
using DasBlog.Core.Common.Comments;
using DasBlog.Web.Models.AdminViewModels;

namespace DasBlog.Web.Mappers
{
	public class ProfilePost : Profile
	{
		private readonly IDasBlogSettings _dasBlogSettings;

		public ProfilePost()
		{
		}

		public ProfilePost(IDasBlogSettings dasBlogSettings)
		{
			_dasBlogSettings = dasBlogSettings;

			CreateMap<Entry, PostViewModel>()
				.ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
				.ForMember(dest => dest.Content, opt => opt.MapFrom(src => src.Content ?? string.Empty))
				.ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description ?? string.Empty))
				.ForMember(dest => dest.Categories, opt => opt.MapFrom(src => ConvertCategory(src.Categories)))
				.ForMember(dest => dest.EntryId, opt => opt.MapFrom(src => src.EntryId))
				.ForMember(dest => dest.AllowComments, opt => opt.MapFrom(src => src.AllowComments))
				.ForMember(dest => dest.IsPublic, opt => opt.MapFrom(src => src.IsPublic))
				.ForMember(dest => dest.Syndicated, opt => opt.MapFrom(src => src.Syndicated))
				.ForMember(dest => dest.PermaLink, opt => opt.MapFrom(src => _dasBlogSettings.GeneratePostUrl(src)))
				.ForMember(dest => dest.ImageUrl, opt => opt.MapFrom(src => src.Content.FindHeroImage()))
				.ForMember(dest => dest.VideoUrl, opt => opt.MapFrom(src => src.Content.FindFirstYouTubeVideo()))
				.ForMember(dest => dest.CreatedDateTime, opt => opt.MapFrom(src => _dasBlogSettings.GetDisplayTime(src.CreatedUtc)))
				.ForMember(dest => dest.ModifiedDateTime, opt => opt.MapFrom(src => _dasBlogSettings.GetDisplayTime(src.ModifiedUtc)));

			CreateMap<PostViewModel, Entry>()
				.ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
				.ForMember(dest => dest.Content, opt => opt.MapFrom(src => src.Content ?? string.Empty))
				.ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description ?? string.Empty))
				.ForMember(dest => dest.Categories, opt => opt.MapFrom(src => string.Join(";", src.AllCategories.Where(x => x.Checked).Select(x => x.Category))))
				.ForMember(dest => dest.EntryId, opt => opt.MapFrom(src => src.EntryId))
				.ForMember(dest => dest.AllowComments, opt => opt.MapFrom(src => src.AllowComments))
				.ForMember(dest => dest.IsPublic, opt => opt.MapFrom(src => src.IsPublic))
				.ForMember(dest => dest.Syndicated, opt => opt.MapFrom(src => src.Syndicated))
				.ForMember(dest => dest.CreatedUtc, opt => opt.MapFrom(src => src.CreatedDateTime))
				.ForMember(dest => dest.ModifiedLocalTime, opt => opt.MapFrom(src => src.ModifiedDateTime));

			CreateMap<CategoryCacheEntry, CategoryViewModel>()
				.ForMember(dest => dest.Category, opt => opt.MapFrom(src => src.DisplayName))
				.ForMember(dest => dest.CategoryUrl, opt => opt.MapFrom(src => ConvertCategory(src.DisplayName).FirstOrDefault().CategoryUrl));

			CreateMap<CategoryViewModel, CategoryCacheEntry>()
				.ForMember(dest => dest.DisplayName, opt => opt.MapFrom(src => src.Category));

			CreateMap<Comment, CommentViewModel>()
				.ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Author))
				.ForMember(dest => dest.Text, opt => opt.MapFrom(src => src.Content))
				.ForMember(dest => dest.GravatarHashId, opt => opt.MapFrom(src => Utils.GetGravatarHash(src.AuthorEmail)))
				.ForMember(dest => dest.Date, opt => opt.MapFrom(src => _dasBlogSettings.GetDisplayTime(src.CreatedUtc)))
				.ForMember(dest => dest.HomePageUrl, opt => opt.MapFrom(src => src.AuthorHomepage))
				.ForMember(dest => dest.BlogPostId, opt => opt.MapFrom(src => src.TargetEntryId))
				.ForMember(dest => dest.CommentId, opt => opt.MapFrom(src => src.EntryId))
				.ForMember(dest => dest.SpamState, opt => opt.MapFrom(src => src.SpamState))
				.ForMember(dest => dest.IsPublic, opt => opt.MapFrom(src => src.IsPublic));

			CreateMap<Comment, CommentAdminViewModel>()
				.ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Author))
				.ForMember(dest => dest.Text, opt => opt.MapFrom(src => src.Content))
				.ForMember(dest => dest.GravatarHashId, opt => opt.MapFrom(src => Utils.GetGravatarHash(src.AuthorEmail)))
				.ForMember(dest => dest.Date, opt => opt.MapFrom(src => _dasBlogSettings.GetDisplayTime(src.CreatedUtc)))
				.ForMember(dest => dest.HomePageUrl, opt => opt.MapFrom(src => src.AuthorHomepage))
				.ForMember(dest => dest.BlogPostId, opt => opt.MapFrom(src => src.TargetEntryId))
				.ForMember(dest => dest.CommentId, opt => opt.MapFrom(src => src.EntryId))
				.ForMember(dest => dest.SpamState, opt => opt.MapFrom(src => src.SpamState))
				.ForMember(dest => dest.IsPublic, opt => opt.MapFrom(src => src.IsPublic))
				.ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.AuthorEmail))
				.ForMember(dest => dest.AuthorIPAddress, opt => opt.MapFrom(src => src.AuthorIPAddress));

			CreateMap<AddCommentViewModel, Comment>()
				.ForMember(dest => dest.Author, opt => opt.MapFrom(src => src.Name))
				.ForMember(dest => dest.Content, opt => opt.MapFrom(src => src.Content))
				.ForMember(dest => dest.AuthorEmail, opt => opt.MapFrom(src => src.Email))
				.ForMember(dest => dest.TargetEntryId, opt => opt.MapFrom(src => src.TargetEntryId))
				.ForMember(dest => dest.AuthorHomepage, opt => opt.MapFrom(src => src.HomePage));

			CreateMap<Entry, CategoryPostItem>()
				.ForMember(dest => dest.BlogTitle, opt => opt.MapFrom(src => src.Title))
				.ForMember(dest => dest.BlogId, opt => opt.MapFrom(src => _dasBlogSettings.GeneratePostUrl(src)))
				.ForMember(dest => dest.Category, opt => opt.MapFrom(src => src.GetSplitCategories().FirstOrDefault()))
				.ForMember(dest => dest.Date, opt => opt.MapFrom(src => src.CreatedLocalTime));

			CreateMap<Tag, TagViewModel>()
				.ForMember(dest => dest.Allowed, opt => opt.MapFrom(src => src.Allowed))
				.ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
				.ForMember(dest => dest.Attributes, opt => opt.MapFrom(src => src.Attributes));

			CreateMap<TagViewModel, Tag>()
				.ForMember(dest => dest.Allowed, opt => opt.MapFrom(src => src.Allowed))
				.ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
				.ForMember(dest => dest.Attributes, opt => opt.MapFrom(src => src.Attributes));

			CreateMap<ValidCommentTags, ValidCommentTagsViewModel>()
				.ForMember(dest => dest.Tag, opt => opt.MapFrom(src => src.Tag));

			CreateMap<ValidCommentTagsViewModel, ValidCommentTags>()
				.ForMember(dest => dest.Tag, opt => opt.MapFrom(src => src.Tag));

		}

		private IList<CategoryViewModel> ConvertCategory(string category)
		{
			if (string.IsNullOrWhiteSpace(category))
				return new List<CategoryViewModel>();

			return category.Split(";").ToList().Select(c => new CategoryViewModel {
													Category = c,
													CategoryUrl = _dasBlogSettings.CompressTitle(c) })
													.ToList();
		}
	}
}
