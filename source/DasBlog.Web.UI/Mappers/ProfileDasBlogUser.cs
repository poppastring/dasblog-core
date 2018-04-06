using AutoMapper;
using DasBlog.Web.Core.Security;
using DasBlog.Web.UI.Models.AccountViewModels;
using DasBlog.Web.UI.Models.Identity;

namespace DasBlog.Web.UI.Mappers
{
	public class ProfileDasBlogUser : Profile
	{
		public ProfileDasBlogUser()
		{
			CreateMap<RegisterViewModel, DasBlogUser>()
				.ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
				.ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Email));

			CreateMap<DasBlogUser, User>()
				.ForMember(dest => dest.Password, opt => opt.MapFrom(src => src.PasswordHash))
				.ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.UserName))
				.ForMember(dest => dest.XmlPassword, opt => opt.MapFrom(src => src.SecurityStamp))
				.ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Role))
				.ForMember(dest => dest.EmailAddress, opt => opt.MapFrom(src => src.Email));

			CreateMap<User, DasBlogUser>()
				.ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.EmailAddress))
				.ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Name))
				.ForMember(dest => dest.PasswordHash, opt => opt.MapFrom(src => src.Password));

		}
	}
}
