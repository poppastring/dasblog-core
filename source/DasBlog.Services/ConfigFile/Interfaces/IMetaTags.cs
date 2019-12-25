using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace DasBlog.Services.ConfigFile.Interfaces
{
	public interface IMetaTags
    {
        string MetaDescription { get; set; }
        string MetaKeywords { get; set; }
        string TwitterCard { get; set; }
        string TwitterSite { get; set; }
        string TwitterCreator { get; set; }
        string TwitterImage { get; set; }
        string FaceBookAdmins { get; set; }
        string FaceBookAppID { get; set; }
		string GoogleAnalyticsID { get; set; }
	}
}
