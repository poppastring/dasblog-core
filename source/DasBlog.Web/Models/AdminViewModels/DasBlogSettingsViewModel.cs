using System;
using System.Collections.Generic;
using DasBlog.Web.Models.BlogViewModels;

namespace DasBlog.Web.Models.AdminViewModels
{
	public class DasBlogSettingsViewModel
	{
		public MetaViewModel MetaConfig { get; set; }
		public SiteViewModel SiteConfig { get; set; }
		public List<PostViewModel> Posts { get; set; }
		public IReadOnlyList<string> Categories { get; set; } = Array.Empty<string>();
	}
}
