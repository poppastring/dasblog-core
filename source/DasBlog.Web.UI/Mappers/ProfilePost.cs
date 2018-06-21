using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using AutoMapper;
using DasBlog.Core;
using DasBlog.Web.Models.BlogViewModels;
using newtelligence.DasBlog.Runtime;

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

			CreateMap<Comment, CommentViewModel>()
				.ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Author))
				.ForMember(dest => dest.Text, opt => opt.MapFrom(src => src.Content))
				.ForMember(dest => dest.GravatarHashId, opt => opt.MapFrom(src => GetGravatarHash(src.AuthorEmail)))
				.ForMember(dest => dest.Date, opt => opt.MapFrom(src => src.CreatedLocalTime))
				.ForMember(dest => dest.HomePageUrl, opt => opt.MapFrom(src => src.AuthorHomepage));

			CreateMap<AddCommentViewModel, Comment>()
				.ForMember(dest => dest.Author, opt => opt.MapFrom(src => src.Name))
				.ForMember(dest => dest.Content, opt => opt.MapFrom(src => src.Comment))
				.ForMember(dest => dest.AuthorEmail, opt => opt.MapFrom(src => src.Email))
				.ForMember(dest => dest.TargetEntryId, opt => opt.MapFrom(src => src.TargetEntryId))
				.ForMember(dest => dest.AuthorHomepage, opt => opt.MapFrom(src => src.HomePage));

			CreateMap<Entry, CategoryPostItem>()
				.ForMember(dest => dest.BlogTitle, opt => opt.MapFrom(src => src.Title))
				.ForMember(dest => dest.BlogId, opt => opt.MapFrom(src => src.EntryId));

		}

		private IList<CategoryViewModel> ConvertCategory(string category)
		{
			return category
				.Split(";")
				.Select(c => new CategoryViewModel
				{
					Category = c,
					CategoryUrl = Regex.Replace(c.ToLower(), @"[^A-Za-z0-9_\.~]+", _dasBlogSettings.SiteConfiguration.TitlePermalinkSpaceReplacement)
				}).ToList();
		}

		private string GetGravatarHash(string email)
		{
			string hash = string.Empty;
			byte[] data, enc;

			data = Encoding.Default.GetBytes(email.ToLowerInvariant());

			using (MD5 md5 = new MD5CryptoServiceProvider())
			{
				enc = md5.TransformFinalBlock(data, 0, data.Length);
				foreach (byte b in md5.Hash)
				{
					hash += Convert.ToString(b, 16).ToLower().PadLeft(2, '0');
				}
				md5.Clear();
			}

			return hash;
		}
	}
}
