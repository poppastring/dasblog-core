using System;
using NUnit.Framework;
using newtelligence.DasBlog.Web.Core;

namespace newtelligence.DasBlog.Web.Core.Test
{
	[TestFixture]
	public class UtilitiesTests
	{
		[Test]
		public void ApplyContentFilters_NoReplacements()
		{
			string content = "Smile";
			string expectedContent = "Smile";

			SiteConfig siteConfig = createSiteConfigWithFilter(@":-)", @"<img src='happy.gif'>", false);
			string modifiedContent = SiteUtilities.ApplyContentFilters(siteConfig, content);

			Assert.AreEqual(expectedContent, modifiedContent, "Should not have changed anything.");
		}

		[Test]
		public void ApplyContentFilters_UrlReplaceOnly()
		{
			string content = "$url(multiple words)";
			string expectedContent = "multiple+words";

			SiteConfig siteConfig = createSiteConfigWithFilter(@":-)", @"<img src='happy.gif'>", false);
			string modifiedContent = SiteUtilities.ApplyContentFilters(siteConfig, content);

			Assert.AreEqual(expectedContent, modifiedContent, "Should have URL encoded the output.");
		}

		[Test]
		public void ApplyContentFilters_NonRegexReplace()
		{
			string content = "Smile :-) if you are happy.";
			string expectedContent = "Smile <img src='happy.gif'> if you are happy.";

			SiteConfig siteConfig = createSiteConfigWithFilter(@":-)", @"<img src='happy.gif'>", false);
			string modifiedContent = SiteUtilities.ApplyContentFilters(siteConfig, content);

			Assert.AreEqual(expectedContent, modifiedContent, "Should have replaced emoticon with img tag.");
		}

		[Test]
		public void ApplyContentFilters_SingleWordRegexReplace()
		{
			string content = "This is a $g(query)";
			string expectedContent = "This is a search?q=query";

			SiteConfig siteConfig = createSiteConfigWithFilter(@"\$g\((?<expr>[\w\s\d]+)\)", "search?q=${expr}", true);
			string modifiedContent = SiteUtilities.ApplyContentFilters(siteConfig, content);

			Assert.AreEqual(expectedContent, modifiedContent, "Should have replaced $g with a search URL.");
		}

		[Test]
		public void ApplyContentFilters_MultipleWordRegexReplace()
		{
			string content = "This is a $g(compound query)";
			string expectedContent = "This is a search?q=compound query";
			
			SiteConfig siteConfig = createSiteConfigWithFilter(@"\$g\((?<expr>[\w\s\d]+)\)", "search?q=${expr}", true);
			string modifiedContent = SiteUtilities.ApplyContentFilters(siteConfig, content);

			Assert.AreEqual(expectedContent, modifiedContent, "Should have replaced $g with a search URL.");
		}

		[Test]
		public void ApplyContentFilters_MultipleWordRegexReplaceWithUrlEncode()
		{
			string content = "This is a $g(compound query)";
			string expectedContent = "This is a search?q=compound+query";

			SiteConfig siteConfig = createSiteConfigWithFilter(@"\$g\((?<expr>[\w\s\d]+)\)", "search?q=$url(${expr})", true);
			string modifiedContent = SiteUtilities.ApplyContentFilters(siteConfig, content);

			Assert.AreEqual(expectedContent, modifiedContent, "Should have replaced $g with a URL encoded search URL.");
		}

		[Test]
		public void ApplyContentFilters_MultipleReplaceWithUrlEncode()
		{
			string content = "This is a $g(compound query) and $g(another one)";
			string expectedContent = "This is a search?q=compound+query and search?q=another+one";

			SiteConfig siteConfig = createSiteConfigWithFilter(@"\$g\((?<expr>[\w\s\d]+)\)", "search?q=$url(${expr})", true);
			string modifiedContent = SiteUtilities.ApplyContentFilters(siteConfig, content);

			Assert.AreEqual(expectedContent, modifiedContent, "Should have replaced both expressions");
		}

		[Test]
		public void ApplyContentFilters_MultipleFilters()
		{
			string content = "This is a $g(query)";
			string expectedContent = "This was a search?q=query";

			SiteConfig siteConfig = createSiteConfigWithFilter(@"\$g\((?<expr>[\w\s\d]+)\)", "search?q=${expr}", true);
			siteConfig.ContentFilters.Add(createContentFilter(" is ", " was ", false));
			string modifiedContent = SiteUtilities.ApplyContentFilters(siteConfig, content);

			Assert.AreEqual(expectedContent, modifiedContent, "Should performed both filters");
		}

		[Test]
		public void ApplyContentFilters_NonSpaceUrlEncode()
		{
			string content = "Encode: $url(<tag> & ﷲ)";
			string expectedContent = "Encode: %3ctag%3e+%26+%ef%b7%b2";

			SiteConfig siteConfig = createSiteConfigWithFilter(@"\$g\((?<expr>[\w\s\d]+)\)", "search?q=${expr}", true);
			string modifiedContent = SiteUtilities.ApplyContentFilters(siteConfig, content);

			Assert.AreEqual(expectedContent, modifiedContent, "Should have replaced $g with a search URL.");
		}

		private SiteConfig createSiteConfigWithFilter(string expression, string mapTo, bool isRegularExpression)
		{
			SiteConfig siteConfig = new SiteConfig();
			siteConfig.ContentFilters.Add(createContentFilter(expression, mapTo, isRegularExpression));
			return siteConfig;
		}

		private ContentFilter createContentFilter(string expression, string mapTo, bool isRegularExpression)
		{
			ContentFilter contentFilter = new ContentFilter(expression, mapTo);
			contentFilter.IsRegEx = isRegularExpression;
			return contentFilter;
		}

		[Test]
		public void HtmlFilterTests(){

			string one = "a<b>aa</b>a";
			string one_result = "a&lt;b&gt;aa&lt;/b&gt;a";
			Assert.AreEqual( one_result, SiteUtilities.FilterHtml( one, null ), "Test one failed.");

			string two = "aaa";
			string two_result = two; // must remain unchanged
			Assert.AreEqual( two_result, SiteUtilities.FilterHtml( two, new ValidTagCollection("a") ), "Test two failed." ); 
			
			string three = "aa<i>aa";
			string three_result = "aa&lt;i&gt;aa";
			Assert.AreEqual( three_result, SiteUtilities.FilterHtml( three, new ValidTagCollection("a") ), "Test three failed."); 

			string four = "aa<i />aa";
			string four_result = four; //must remain unchanged
			Assert.AreEqual( four_result, SiteUtilities.FilterHtml( four, new ValidTagCollection("i") ), "Test four failed." ); 

			string five = "aa<b>aa<i />";
			string five_result = "aa&lt;b&gt;aa<i />";
			Assert.AreEqual( five_result, SiteUtilities.FilterHtml( five, new ValidTagCollection("i") ), "Test five failed.");

			string six = "aa<a>aa";
			string six_result = "aa<a />aa";
			Assert.AreEqual( six_result, SiteUtilities.FilterHtml( six, new ValidTagCollection("a") ), "Test six failed.");

			string seven = "aa<a>aa</a>aa";
			string seven_result = seven; // must remain unchanged
			Assert.AreEqual( seven_result, SiteUtilities.FilterHtml( seven, new ValidTagCollection("a") ), "Test seven failed.");

			string eight = "aa<a>aa<b>bb</b>aa</a>aa";
			string eight_result = eight; // must remain unchanged
			Assert.AreEqual( eight_result, SiteUtilities.FilterHtml( eight, new ValidTagCollection("a,b")), "Test eight failed.");

			string nine = "aa<a>aa<b>b<img>b</b>aa</a>aa";
			string nine_result = "aa<a>aa<b>b<img />b</b>aa</a>aa";
			Assert.AreEqual( nine_result, SiteUtilities.FilterHtml( nine, new ValidTagCollection("a,b,img")), "Test nine failed.");

			string ten = "aa</a>aa";
			string ten_result = "aa&lt;/a&gt;aa";
			Assert.AreEqual( ten_result, SiteUtilities.FilterHtml( ten, new ValidTagCollection("a")), "Test ten failed." );
		}

		[Test]
		public void HtmlAttributeFilterTest(){
			
			ValidTagCollection tags = new ValidTagCollection("a@href@title,img@src");

			string one = "aa<a href=\"attValue\" title=\"attTitle\">bb</a>";
			string one_result = one;

			Assert.AreEqual( one_result, SiteUtilities.FilterHtml( one, tags), "Test one failed.");

			string two = "aa<a onclick=\"evil javascript\" href=\"link\">bb</a>";
			string two_result = "aa<a href=\"link\">bb</a>";

			Assert.AreEqual( two_result, SiteUtilities.FilterHtml( two, tags), "Test two failed." );

			string three = "aa<a href=attValue title=attTitle>bb</a>";
			string three_result = "aa<a href=\"attValue\" title=\"attTitle\">bb</a>";

			Assert.AreEqual( three_result, SiteUtilities.FilterHtml( three, tags), "Test three failed." );


			string four = "aa<a href title=\"title\">bb</a>";
			string four_result = "aa<a title=\"title\">bb</a>";

			Assert.AreEqual( four_result, SiteUtilities.FilterHtml( four, tags), "Test four failed." );

			string five = "aa<a title=\"title\" href>bb</a>";
			string five_result = "aa<a title=\"title\">bb</a>";

			Assert.AreEqual( five_result, SiteUtilities.FilterHtml( five, tags), "Test five failed." );


		}
	}
}
