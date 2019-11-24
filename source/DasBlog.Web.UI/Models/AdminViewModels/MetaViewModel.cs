using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DasBlog.Web.Models.AdminViewModels
{
	public class MetaViewModel
	{
		public string MetaDescription { get; set; }
		public string MetaKeywords { get; set; }
		public string TwitterCard { get; set; }
		public string TwitterSite { get; set; }
		public string TwitterCreator { get; set; }
		public string TwitterImage { get; set; }
		public string FaceBookAdmins { get; set; }
		public string FaceBookAppID { get; set; }
		public string GoogleAnalyticsID {get; set;}
	}
}
