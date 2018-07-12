using AutoMapper;
using DasBlog.Core.Security;
using DasBlog.Web.Models.AccountViewModels;
using DasBlog.Web.Identity;
using System.Text.RegularExpressions;
using DasBlog.Core.Services.Interfaces;
using DasBlog.Managers.Interfaces;
using DasBlog.Web.Models;
using Microsoft.EntityFrameworkCore.Storage.Internal;

namespace DasBlog.Web.Mappers
{
	public class ProfileDasBlogUser : Profile
	{
		public ProfileDasBlogUser()
		{
			
		}
		public ProfileDasBlogUser(ISiteSecurityManager siteSecurityManager)
		{
			CreateMap<RegisterViewModel, DasBlogUser>()
				.ForMember(dest => dest.UserName, opt => opt.MapFrom(src => Regex.Replace(src.Name, @"[^A-Za-z0-9]+", string.Empty)))
				.ForMember(dest => dest.DisplayName, opt => opt.MapFrom(src => src.Name))
				.ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
				.ForMember(dest => dest.Role, opt => opt.MapFrom(src => "Contributor"));

			CreateMap<DasBlogUser, User>()
				.ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.UserName))
				.ForMember(dest => dest.EmailAddress, opt => opt.MapFrom(src => src.Email))
				.ForMember(dest => dest.DisplayName, opt => opt.MapFrom(src => src.DisplayName))
				.ForMember(dest => dest.Password, opt => opt.MapFrom(src => src.PasswordHash))
				.ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.DisplayName));

			CreateMap<User, DasBlogUser>()
				.ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Name))
				.ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.EmailAddress))
				.ForMember(dest => dest.DisplayName, opt => opt.MapFrom(src => src.DisplayName))
				.ForMember(dest => dest.PasswordHash, opt => opt.MapFrom(src => src.Password))
				.ForMember(dest => dest.SecurityStamp, opt => opt.MapFrom(src => src.Name));
			
			CreateMap<UsersViewModel, User>()
				.ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
				.ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Role))
				.ForMember(dest => dest.Ask, opt => opt.MapFrom(src => src.Ask == "on"))
				.ForMember(dest => dest.EmailAddress, opt => opt.MapFrom(src => src.EmailAddress))
				.ForMember(dest => dest.DisplayName, opt => opt.MapFrom(src => src.DisplayName))
				.ForMember(dest => dest.OpenIDUrl, opt => opt.MapFrom(src => src.OpenIDUrl))
				.ForMember(dest => dest.NotifyOnAllComment, opt => opt.MapFrom(src => src.NotifyOnAllComment == "on"))
				.ForMember(dest => dest.NotifyOnOwnComment, opt => opt.MapFrom(src => src.NotifyOnOwnComment == "on"))
				.ForMember(dest => dest.NotifyOnNewPost, opt => opt.MapFrom(src => src.NotifyOnNewPost == "on"))
				.ForMember(dest => dest.Active, opt => opt.MapFrom(src => src.Active == "on"))
				.ForMember(dest => dest.Password, opt => opt.MapFrom(src => siteSecurityManager.HashPassword((src.Password))))
				;

			CreateMap<User, UsersViewModel>()
				.ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
				.ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Role))
				.ForMember(dest => dest.Ask, opt => opt.MapFrom(src => src.Ask ? "on" : string.Empty))
				.ForMember(dest => dest.EmailAddress, opt => opt.MapFrom(src => src.EmailAddress))
				.ForMember(dest => dest.DisplayName, opt => opt.MapFrom(src => src.DisplayName))
				.ForMember(dest => dest.OpenIDUrl, opt => opt.MapFrom(src => src.OpenIDUrl))
				.ForMember(dest => dest.NotifyOnAllComment, opt => opt.MapFrom(src => src.NotifyOnAllComment ? "on" : string.Empty))
				.ForMember(dest => dest.NotifyOnOwnComment, opt => opt.MapFrom(src => src.NotifyOnOwnComment ? "on" : string.Empty))
				.ForMember(dest => dest.NotifyOnNewPost, opt => opt.MapFrom(src => src.NotifyOnNewPost ? "on" : string.Empty))
				.ForMember(dest => dest.Active, opt => opt.MapFrom(src => src.Active ? "on" : string.Empty))
				.ForMember(dest => dest.Password, opt => opt.MapFrom(src => string.Empty))
						// we don't know what the password is - so we can't tell the user
				;
		}
	}
}
