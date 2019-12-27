using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DasBlog.Web.Models.AdminViewModels
{
	public class DasBlogSettingsViewModel
	{
		public MetaViewModel MetaConfig { get; set; }
		public SiteViewModel SiteConfig { get; set; }
	}
}
