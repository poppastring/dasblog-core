using AutoMapper;
using DasBlog.Web.Core;
using DasBlog.Web.Core.Configuration;
using DasBlog.Web.UI.Models.BlogViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using newtelligence.DasBlog.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DasBlog.Web.UI.Mappers
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
				.ForMember(dest => dest.Content, opt => opt.MapFrom(src => src.Content))
				.ForMember(dest => dest.Categories, opt => opt.MapFrom(src => ConvertCategory(src.Categories)))
				.ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
				.ForMember(dest => dest.EntryId, opt => opt.MapFrom(src => src.EntryId))
				.ForMember(dest => dest.AllowComments, opt => opt.MapFrom(src => src.AllowComments))
				.ForMember(dest => dest.IsPublic, opt => opt.MapFrom(src => src.IsPublic))
				.ForMember(dest => dest.PermaLink, opt => opt.MapFrom(src => _dasBlogSettings.GetPermaTitle(src.CompressedTitle)))
				.ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
				.ForMember(dest => dest.CreatedDateTime, opt => opt.MapFrom(src => src.CreatedLocalTime));
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
