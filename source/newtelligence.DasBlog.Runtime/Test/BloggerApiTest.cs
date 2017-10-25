using System;
using System.Diagnostics;
using System.IO;
using CookComputing.XmlRpc;
using newtelligence.DasBlog.Runtime.Proxies;
using NUnit.Framework;

namespace newtelligence.DasBlog.Runtime.Test
{
	/// <summary>
	/// Summary description for BloggerApiTest.
	/// </summary>
	[TestFixture]
	public class BloggerApiTest : TestBaseServer
	{
		private BloggerAPIClientProxy proxy;
		private CrosspostInfo crosspostInfo;
		private CrosspostSite site = new CrosspostSite("localhost", "localhost", 80, "/dasblog/blogger.aspx");

		#region Tests
		[Test]
		public void CreateMetaWeblogPost()
		{
			site.ApiType = "metaweblog";
			site.BlogId = "BlogId";
			site.Username = BloggerApiTest.Username;
			site.Password = BloggerApiTest.Password;
			crosspostInfo = new CrosspostInfo(site);

			proxy = new BloggerAPIClientProxy();
			UriBuilder uriBuilder = new UriBuilder("http",crosspostInfo.Site.HostName,crosspostInfo.Site.Port,crosspostInfo.Site.Endpoint);
			proxy.Url = uriBuilder.ToString();
			proxy.UserAgent="newtelligence dasBlog/1.4";

			Entry testEntry = TestEntry.CreateEntry("MetaWeblog " + DateTime.Now.ToShortTimeString(), 5, 2);
			testEntry.Author = BloggerApiTest.Username;

			mwPost newPost = CreateMovableTypePost(testEntry);
			
			Entry newEntry = GetDataService().GetEntry(newPost.postid);
			Assert.IsNotNull(newEntry);
			Assert.IsTrue(testEntry.CompareTo(newEntry) == 0);

			// now delete the entry
			string entryId = newEntry.EntryId;
			GetDataService().DeleteEntry(entryId, null);
			Assert.IsNull(localhostBlogService.GetEntry(entryId));
		}

		[Test]
		public void EditMetaWeblogPost()
		{
			Entry testEntry = TestEntry.CreateEntry("MetaWeblog " + DateTime.Now.ToLongTimeString(), 5, 2);
			testEntry.Author = BloggerApiTest.Username;

			// create a new entry
			mwPost newPost = CreateMovableTypePost(testEntry);
			
			// now modify the entry
			newPost.title = "MetaWeblog Edit " + DateTime.Now.ToLongTimeString();
			newPost.categories = new string[] {"Blogging"};

			bool success = proxy.metaweblog_editPost(newPost.postid,
				crosspostInfo.Site.Username,
				crosspostInfo.Site.Password,
				newPost, true);
			Assert.IsTrue(success);

			Entry editedEntry = GetDataService().GetEntry(newPost.postid);
			Assert.IsNotNull(editedEntry);
			Console.WriteLine(editedEntry.Title);
			Assert.IsTrue(testEntry.CompareTo(editedEntry) == 1);

			// now delete the entry
			string entryId = editedEntry.EntryId;
			GetDataService().DeleteEntry(entryId, null);
			Assert.IsNull(localhostBlogService.GetEntry(entryId));
		}
		#endregion Tests

		private mwPost CreateMovableTypePost(Entry testEntry)
		{
			mwPost newPost = new mwPost();
			newPost.link ="";
			newPost.permalink="";
			newPost.postid="";
			newPost.categories = new string[0];
			newPost.dateCreated = testEntry.CreatedUtc;
			newPost.description = testEntry.Content;
			newPost.title = testEntry.Title;
			newPost.postid = proxy.metaweblog_newPost(crosspostInfo.Site.BlogId,
				crosspostInfo.Site.Username,
				crosspostInfo.Site.Password,
				newPost, true);
			Crosspost cp = new Crosspost();
			cp.TargetEntryId = newPost.postid;
			cp.ProfileName = crosspostInfo.Site.ProfileName;
			cp.Categories = crosspostInfo.Categories;
			testEntry.Crossposts.Add( cp );
			return newPost;
		}
	}
}
