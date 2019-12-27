using AutoMapper;
using DasBlog.Services.ConfigFile;
using DasBlog.Web.Models.AdminViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DasBlog.Web.Mappers
{
	public class ProfileSettings : Profile
	{
		public ProfileSettings()
		{
			CreateMap<MetaTags, MetaViewModel>();
			CreateMap<MetaViewModel, MetaTags>();

			CreateMap<SiteConfig, SiteViewModel>();
			CreateMap<SiteViewModel, SiteConfig>();

		}
	}
}
