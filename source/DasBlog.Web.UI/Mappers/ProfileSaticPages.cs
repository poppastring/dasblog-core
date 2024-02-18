using AutoMapper;
using DasBlog.Web.Models.BlogViewModels;
using DasBlog.Core.Common;
using DasBlog.Core.Extensions;
using newtelligence.DasBlog.Runtime;
using System.Collections.Generic;
using System.Linq;
using DasBlog.Services;
using DasBlog.Core.Common.Comments;
using DasBlog.Web.Models.AdminViewModels;

namespace DasBlog.Web.Mappers
{
	public class ProfileStaticPage : Profile
	{
		public ProfileStaticPage()
		{
			CreateMap<StaticPage, StaticPageViewModel>();
		}
    }
}
