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
				.ForMember(dest => dest.Email, opt => opt.MapFrom(m => m.Email))
				.ForMember(dest => dest.UserName, opt => opt.MapFrom(m => m.Email));

			CreateMap<DasBlogUser, User>();
		}
	}
}
