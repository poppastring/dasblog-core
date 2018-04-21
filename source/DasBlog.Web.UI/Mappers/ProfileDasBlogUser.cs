using AutoMapper;
using DasBlog.Core.Security;
using DasBlog.Web.Models.AccountViewModels;
using DasBlog.Web.Identity;
using System.Text.RegularExpressions;

namespace DasBlog.Web.Mappers
{
	public class ProfileDasBlogUser : Profile
	{
		public ProfileDasBlogUser()
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
		}
	}
}
