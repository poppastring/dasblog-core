using AutoMapper;
using DasBlog.Services.ConfigFile.Interfaces;
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
			CreateMap<IMetaTags, MetaViewModel>();
			CreateMap<MetaViewModel, IMetaTags>();

			CreateMap<ISiteConfig, SiteViewModel>();
			CreateMap<SiteViewModel, ISiteConfig>();

		}
	}
}
