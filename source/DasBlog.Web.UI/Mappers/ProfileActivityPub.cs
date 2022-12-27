using AutoMapper;
using DasBlog.Services.ActivityPub;
using DasBlog.Web.Models.ActivityPubModels;

namespace DasBlog.Web.Mappers
{
	public class ProfileActivityPub : Profile
	{
		public ProfileActivityPub()
		{
			CreateMap<WebFinger, WebFingerViewModel>()
				.ForMember(dest => dest.subject, opt => opt.MapFrom(src => src.Subject))
				.ForMember(dest => dest.aliases, opt => opt.MapFrom(src => src.Aliases.ToArray()));

			CreateMap<WebFingerLink, WebFingerLinkViewModel>()
				.ForMember(dest => dest.type, opt => opt.MapFrom(src => src.Type))
				.ForMember(dest => dest.href, opt => opt.MapFrom(src => src.HRef))
				.ForMember(dest => dest.template, opt => opt.MapFrom(src => src.Template))
				.ForMember(dest => dest.rel, opt => opt.MapFrom(src => src.Relationship));

			CreateMap<UserPage, UserPageViewModel>()
				.ForMember(dest => dest.prev, opt => opt.MapFrom(src => src.Previous))
				.ForMember(dest => dest.next, opt => opt.MapFrom(src => src.Next))
				.ForMember(dest => dest.partOf, opt => opt.MapFrom(src => src.PartOf))
				.ForMember(dest => dest.type, opt => opt.MapFrom(src => src.Type))
				.ForMember(dest => dest.id, opt => opt.MapFrom(src => src.Id));

			CreateMap<UserPage, UserPageViewModel>()
				.ForMember(dest => dest.prev, opt => opt.MapFrom(src => src.Previous))
				.ForMember(dest => dest.next, opt => opt.MapFrom(src => src.Next))
				.ForMember(dest => dest.partOf, opt => opt.MapFrom(src => src.PartOf))
				.ForMember(dest => dest.type, opt => opt.MapFrom(src => src.Type))
				.ForMember(dest => dest.id, opt => opt.MapFrom(src => src.Id));

			CreateMap<OrderedItem, OrderedItemViewModel>()
				.ForMember(dest => dest.id, opt => opt.MapFrom(src => src.Id))
				.ForMember(dest => dest.published, opt => opt.MapFrom(src => src.Published))
				.ForMember(dest => dest.actor, opt => opt.MapFrom(src => src.Actor))
				.ForMember(dest => dest.sensitive, opt => opt.MapFrom(src => src.Sensitive))
				.ForMember(dest => dest.content, opt => opt.MapFrom(src => src.Content))
				.ForMember(dest => dest.to, opt => opt.MapFrom(src => src.To.ToArray()))
				.ForMember(dest => dest.cc, opt => opt.MapFrom(src => src.Cc.ToArray()));

			CreateMap<User, UserViewModel>()
				.ForMember(dest => dest.first, opt => opt.MapFrom(src => src.First))
				.ForMember(dest => dest.id, opt => opt.MapFrom(src => src.Id))
				.ForMember(dest => dest.context, opt => opt.MapFrom(src => src.Context))
				.ForMember(dest => dest.type, opt => opt.MapFrom(src => src.Type));
		}
	}
}
