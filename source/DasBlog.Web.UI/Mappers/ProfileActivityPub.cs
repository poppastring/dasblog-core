using AutoMapper;
using DasBlog.Services.ActivityPub;
using DasBlog.Services.ConfigFile;
using DasBlog.Web.Models.ActivityPubModels;
using DasBlog.Web.Models.AdminViewModels;

namespace DasBlog.Web.Mappers
{
	public class ProfileActivityPub : Profile
	{
		public ProfileActivityPub()
		{
			CreateMap<WebFinger, WebFingerViewModel>()
				.ForMember(dest => dest.subject, opt => opt.MapFrom(src => src.Subject));

			CreateMap<WebFingerLink, WebFingerLinkViewModel>()
				.ForMember(dest => dest.type, opt => opt.MapFrom(src => src.Type))
				.ForMember(dest => dest.href, opt => opt.MapFrom(src => src.HRef))
				.ForMember(dest => dest.template, opt => opt.MapFrom(src => src.Template))
				.ForMember(dest => dest.rel, opt => opt.MapFrom(src => src.Relationship));

		}
	}
}
