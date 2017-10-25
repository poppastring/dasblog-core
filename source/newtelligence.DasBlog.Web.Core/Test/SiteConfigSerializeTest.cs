using System;
using System.Diagnostics;
using System.IO;
using System.Xml.Serialization;

using NUnit.Framework;
using newtelligence.DasBlog.Web.Core;

namespace newtelligence.DasBlog.Web.Core.Test
{
	[TestFixture]
	public class SiteConfigSerializeTest
	{

		public SiteConfigSerializeTest()
		{
			// ...
		}

		[Test]
		public void SerializeTest(){

			SiteConfig config = new SiteConfig();

			XmlSerializer ser = new XmlSerializer(typeof(SiteConfig));

			using( StringWriter writer = new StringWriter() ){

				ser.Serialize( writer, config );

				//Debug.Write( writer.ToString() );
			}
			


		}
	}
}
