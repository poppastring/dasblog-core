using System;
using System.Xml;
using NUnit;
using NUnit.Framework;

namespace newtelligence.DasBlog.Web.Services.Test
{
	/// <summary>
	/// Summary description for Class1.
	/// </summary>
	[TestFixture]
	public class ConfigServiceTester
	{
		ConfigService.ConfigEditingService service;
		[SetUp]
		public void SetUpForTests()
		{
			this.service = new ConfigService.ConfigEditingService();
			service.authenticationHeaderValue = new ConfigService.authenticationHeader();
			service.authenticationHeaderValue.userName="admin";
			service.authenticationHeaderValue.password="admin";
		}

		[TearDown]
		public void TearDownTests()
		{
			// This method will be run each and every time a test method is run.
			// Place common cleanup steps here that should be run after
			// every test.
		}

		[Test]
		public void GetBlogRolls()
		{
			string[] blogRolls = service.EnumBlogrolls();

			Assert.IsNotNull(blogRolls);

			foreach( string blogRoll in blogRolls )
			{
				Console.WriteLine("Blogroll: {0}", blogRoll );
				XmlElement opmlRoot = service.GetBlogroll(blogRoll);
				opmlRoot.WriteTo( new XmlTextWriter(Console.Out) );
				service.PostBlogroll( blogRoll, opmlRoot );
			}
		}

		[Test]
		public void GetSiteConfig()
		{
			ConfigService.SiteConfig siteConfig = service.GetSiteConfig();
			Assert.IsNotNull(siteConfig);
			service.UpdateSiteConfig( siteConfig ); 
		}
	}
}
