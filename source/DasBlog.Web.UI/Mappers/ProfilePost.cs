using AutoMapper;
using DasBlog.Web;
using DasBlog.Core.Configuration;
using DasBlog.Web.Models.BlogViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using newtelligence.DasBlog.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

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

			CreateMap<Entry, PostViewModel >()
				.ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
				.ForMember(dest => dest.Content, opt => opt.MapFrom(src => src.Content))
				.ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
				.ForMember(dest => dest.Categories, opt => opt.MapFrom(src => ConvertCategory(src.Categories)))
				.ForMember(dest => dest.EntryId, opt => opt.MapFrom(src => src.EntryId))
				.ForMember(dest => dest.AllowComments, opt => opt.MapFrom(src => src.AllowComments))
				.ForMember(dest => dest.IsPublic, opt => opt.MapFrom(src => src.IsPublic))
				.ForMember(dest => dest.Syndicated, opt => opt.MapFrom(src => src.Syndicated))
				.ForMember(dest => dest.PermaLink, opt => opt.MapFrom(src => _dasBlogSettings.GetPermaTitle(src.CompressedTitle)))
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
		}

		private IList<CategoryViewModel> ConvertCategory(string category)
		{
			return category.Split(";").ToList().Select(c => new CategoryViewModel {
												Category = c,
												CategoryUrl = Regex.Replace(c.ToLower(), @"[^A-Za-z0-9_\.~]+", _dasBlogSettings.SiteConfiguration.TitlePermalinkSpaceReplacement) })
												.ToList();
		}
	}
}
