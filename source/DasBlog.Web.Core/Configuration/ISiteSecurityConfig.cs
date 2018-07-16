using DasBlog.Core.Security;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace DasBlog.Core.Configuration
{
	[XmlRoot("SiteSecurityConfig")]
	public interface ISiteSecurityConfig
	{
		[XmlElement(ElementName = "Users" )]
		[XmlArrayItem(DataType = "string", ElementName = "User")]
		List<User> Users { get; set; }

		void Refresh();
	}
}
