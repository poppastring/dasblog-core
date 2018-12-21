using AutoMapper;
using DasBlog.Core;
using DasBlog.Web.Models.BlogViewModels;
using DasBlog.Core.Common;
using newtelligence.DasBlog.Runtime;
using System.Collections.Generic;
using System.Linq;

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
				.ForMember(dest => dest.Content, opt => opt.MapFrom(src => src.Content))
				.ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
				.ForMember(dest => dest.Categories, opt => opt.MapFrom(src => ConvertCategory(src.Categories)))
				.ForMember(dest => dest.EntryId, opt => opt.MapFrom(src => src.EntryId))
				.ForMember(dest => dest.AllowComments, opt => opt.MapFrom(src => src.AllowComments))
				.ForMember(dest => dest.IsPublic, opt => opt.MapFrom(src => src.IsPublic))
				.ForMember(dest => dest.Syndicated, opt => opt.MapFrom(src => src.Syndicated))
				.ForMember(dest => dest.PermaLink, opt => opt.MapFrom(src => MakePermaLink(src)))
				.ForMember(dest => dest.CreatedDateTime, opt => opt.MapFrom(src => src.CreatedLocalTime))
				.ForMember(dest => dest.ModifiedDateTime, opt => opt.MapFrom(src => src.ModifiedLocalTime));

			CreateMap<PostViewModel, Entry>()
				.ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
				.ForMember(dest => dest.Content, opt => opt.MapFrom(src => src.Content))
				.ForMember(dest => dest.Description, opt => opt.MapFrom(src => (src.Description == null) ? string.Empty : src.Description))
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
				.ForMember(dest => dest.Date, opt => opt.MapFrom(src => src.CreatedLocalTime))
				.ForMember(dest => dest.HomePageUrl, opt => opt.MapFrom(src => src.AuthorHomepage))
				.ForMember(dest => dest.BlogPostId, opt => opt.MapFrom(src => src.TargetEntryId))
				.ForMember(dest => dest.CommentId, opt => opt.MapFrom(src => src.EntryId));

			CreateMap<AddCommentViewModel, Comment>()
				.ForMember(dest => dest.Author, opt => opt.MapFrom(src => src.Name))
				.ForMember(dest => dest.Content, opt => opt.MapFrom(src => src.Content))
				.ForMember(dest => dest.AuthorEmail, opt => opt.MapFrom(src => src.Email))
				.ForMember(dest => dest.TargetEntryId, opt => opt.MapFrom(src => src.TargetEntryId))
				.ForMember(dest => dest.AuthorHomepage, opt => opt.MapFrom(src => src.HomePage));

			CreateMap<Entry, CategoryPostItem>()
				.ForMember(dest => dest.BlogTitle, opt => opt.MapFrom(src => src.Title))
				.ForMember(dest => dest.BlogId, opt => opt.MapFrom(src => src.EntryId));
		}

		private IList<CategoryViewModel> ConvertCategory(string category)
		{
			return category.Split(";").ToList().Select(c => new CategoryViewModel {
													Category = c,
													CategoryUrl = Entry.InternalCompressTitle(c, _dasBlogSettings.SiteConfiguration.TitlePermalinkSpaceReplacement).ToLower() })
													.ToList();
		}

		private string MakePermaLink(Entry entry)
		{
			string link;

			if (_dasBlogSettings.SiteConfiguration.EnableTitlePermaLinkUnique)
			{
				link = entry.CreatedUtc.ToString("yyyyMMdd") + "/" 
				  + _dasBlogSettings.GetPermaTitle(entry.CompressedTitle);
			}
			else
			{
				link = _dasBlogSettings.GetPermaTitle(entry.CompressedTitle);
			}

			return link;
		}
	}
}
