using CookComputing.XmlRpc;
using DasBlog.Core;
using DasBlog.Managers.Interfaces;
using DasBlog.Core.Exceptions;
using DasBlog.Core.Security;
using newtelligence.DasBlog.Runtime;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Security;
using System.Text;
using Blogger = DasBlog.Core.XmlRpc.Blogger;
using MoveableType = DasBlog.Core.XmlRpc.MoveableType;
using MetaWeblog = DasBlog.Core.XmlRpc.MetaWeblog;

namespace DasBlog.Managers
{
	[XmlRpcService(Name = "DasBlog Blogger Access Point", Description = "Implementation of Blogger XML-RPC Api")]
	public class XmlRpcManager : IXmlRpcManager, MoveableType.IMovableType, Blogger.IBlogger, MetaWeblog.IMetaWeblog
	{
		private IBlogDataService dataService;
		private ISiteSecurityManager siteSecurityManager;
		private readonly ILoggingDataService loggingDataService;
		private readonly IDasBlogSettings dasBlogSettings;
		private readonly IFileSystemBinaryManager binaryManager;

		public XmlRpcManager(IDasBlogSettings dasBlogSettings, ISiteSecurityManager siteSecurityManager, IFileSystemBinaryManager binaryManager)
		{
			this.dasBlogSettings = dasBlogSettings;
			this.siteSecurityManager = siteSecurityManager;
			this.binaryManager = binaryManager;
			loggingDataService = LoggingDataServiceFactory.GetService(dasBlogSettings.WebRootDirectory + dasBlogSettings.SiteConfiguration.LogDir);
			dataService = BlogDataServiceFactory.GetService(dasBlogSettings.WebRootDirectory + dasBlogSettings.SiteConfiguration.ContentDir, loggingDataService);
		}

		public string Invoke(Stream requestStream)
		{
			try
			{
				XmlRpcSerializer xmlRpcSerializer = new XmlRpcSerializer();

				XmlRpcServiceAttribute xmlRpcServiceAttribute = (XmlRpcServiceAttribute)Attribute.GetCustomAttribute(this.GetType(), typeof(XmlRpcServiceAttribute));
				if (xmlRpcServiceAttribute != null)
				{
					if (xmlRpcServiceAttribute.XmlEncoding != null)
					{
						xmlRpcSerializer.XmlEncoding = Encoding.GetEncoding(xmlRpcServiceAttribute.XmlEncoding);
					}
					xmlRpcSerializer.UseIntTag = xmlRpcServiceAttribute.UseIntTag;
					xmlRpcSerializer.UseStringTag = xmlRpcServiceAttribute.UseStringTag;
					xmlRpcSerializer.UseIndentation = xmlRpcServiceAttribute.UseIndentation;
					xmlRpcSerializer.Indentation = xmlRpcServiceAttribute.Indentation;
				}

				var bodyStream = new StreamReader(requestStream);
				bodyStream.BaseStream.Seek(0, SeekOrigin.Begin);

				XmlRpcRequest request = xmlRpcSerializer.DeserializeRequest(bodyStream, this.GetType());
				XmlRpcResponse response = this.Invoke(request);
				Stream stream = new MemoryStream();
				xmlRpcSerializer.SerializeResponse(stream, response);
				stream.Seek(0L, SeekOrigin.Begin);

				StreamReader reader = new StreamReader(stream);
				return reader.ReadToEnd();
			}
			catch (Exception ex)
			{
				XmlRpcFaultException faultEx = (!(ex is XmlRpcException)) ? ((!(ex is XmlRpcFaultException)) ? new XmlRpcFaultException(0, ex.Message) : ((XmlRpcFaultException)ex)) : new XmlRpcFaultException(0, ((XmlRpcException)ex).Message);
				XmlRpcSerializer xmlRpcSerializer2 = new XmlRpcSerializer();
				Stream stream2 = new MemoryStream();
				xmlRpcSerializer2.SerializeFaultResponse(stream2, faultEx);
				stream2.Seek(0L, SeekOrigin.Begin);

				StreamReader reader2 = new StreamReader(stream2);
				return reader2.ReadToEnd();
			}
		}

		private XmlRpcResponse Invoke(XmlRpcRequest request)
		{
			MethodInfo methodInfo = null;
			methodInfo = (((object)request.mi == null) ? base.GetType().GetMethod(request.method) : request.mi);
			object retValue;
			try
			{
				retValue = methodInfo.Invoke((object)this, request.args);
			}
			catch (Exception ex)
			{
				if (ex.InnerException != null)
				{
					throw ex.InnerException;
				}
				throw ex;
			}
			return new XmlRpcResponse(retValue);
		}

		public MoveableType.Category[] mt_getCategoryList(string blogid, string username, string password)
		{
			if (!dasBlogSettings.SiteConfiguration.EnableBloggerApi)
			{
				throw new ServiceDisabledException();
			}

			if (!VerifyLogin(username, password))
			{
				throw new SecurityException();
			}

			return InternalGetCategoryList();
		}

		public MoveableType.Category[] mt_getPostCategories(string postid, string username, string password)
		{
			if (!dasBlogSettings.SiteConfiguration.EnableBloggerApi)
			{
				throw new ServiceDisabledException();
			}

			if (!VerifyLogin(username, password))
			{
				throw new SecurityException();
			}

			MoveableType.Category[] mcats = InternalGetCategoryList();
			Entry entry = dataService.GetEntry(postid);
			if (entry != null)
			{
				List<MoveableType.Category> acats = new List<MoveableType.Category>();
				string[] cats = entry.GetSplitCategories();
				if (cats.Length > 0)
				{
					foreach (string cat in cats)
					{
						foreach (MoveableType.Category mcat in mcats)
						{
							if (cat == mcat.categoryId)
							{
								MoveableType.Category cpcat = mcat;
								cpcat.isPrimary = (acats.Count == 0);
								acats.Add(cpcat);
								break;
							}
						}
					}
				}
				if (acats.Count == 0)
				{
					acats.Add(InternalGetFrontPageCategory());
				}
				return acats.ToArray();
			}
			else
			{
				return null;
			}
		}

		public MoveableType.PostTitle[] mt_getRecentPostTitles(string blogid, string username, string password, int numberOfPosts)
		{
			if (!dasBlogSettings.SiteConfiguration.EnableBloggerApi)
			{
				throw new ServiceDisabledException();
			}

			if (!VerifyLogin(username, password))
			{
				throw new SecurityException();
			}

			EntryCollection entries = dataService.GetEntriesForDay(DateTime.Now.ToUniversalTime(), dasBlogSettings.GetConfiguredTimeZone(), null,
				dasBlogSettings.SiteConfiguration.RssDayCount, numberOfPosts, null);
			List<MoveableType.PostTitle> arrayList = new List<MoveableType.PostTitle>();
			foreach (Entry entry in entries)
			{
				MoveableType.PostTitle post = new MoveableType.PostTitle();
				post.title = NoNull(entry.Title);
				post.dateCreated = entry.CreatedUtc;
				post.postid = NoNull(entry.EntryId);
				post.userid = username;
				arrayList.Add(post);
			}
			return arrayList.ToArray();
		}

		public MoveableType.TrackbackPing[] mt_getTrackbackPings(string postid)
		{
			if (!dasBlogSettings.SiteConfiguration.EnableBloggerApi)
			{
				throw new ServiceDisabledException();
			}
			List<MoveableType.TrackbackPing> arrayList = new List<MoveableType.TrackbackPing>();
			foreach (Tracking trk in dataService.GetTrackingsFor(postid))
			{
				if (trk.TrackingType == TrackingType.Trackback)
				{
					MoveableType.TrackbackPing tp = new MoveableType.TrackbackPing();
					tp.pingIP = "";
					tp.pingTitle = NoNull(trk.RefererTitle);
					tp.pingURL = NoNull(trk.PermaLink);
					arrayList.Add(tp);
				}
			}
			return arrayList.ToArray();
		}

		public bool mt_publishPost(string postid, string username, string password)
		{
			if (!dasBlogSettings.SiteConfiguration.EnableBloggerApi)
			{
				throw new ServiceDisabledException();
			}

			if (!VerifyLogin(username, password))
			{
				throw new SecurityException();
			}

			return true;
		}

		public bool mt_setPostCategories(string postid, string username, string password, MoveableType.Category[] categories)
		{
			if (!dasBlogSettings.SiteConfiguration.EnableBloggerApi)
			{
				throw new ServiceDisabledException();
			}

			if (!VerifyLogin(username, password))
			{
				throw new SecurityException();
			}

			Entry entry = dataService.GetEntryForEdit(postid);
			if (entry != null)
			{
				string cats = "";
				foreach (MoveableType.Category mcat in categories)
				{
					if (cats.Length > 0)
						cats += ";";
					cats += mcat.categoryId;
				}
				entry.Categories = cats;
				dataService.SaveEntry(entry);

				return true;
			}
			else
			{
				return false;
			}
		}

		public string[] mt_supportedMethods()
		{
			if (!dasBlogSettings.SiteConfiguration.EnableBloggerApi)
			{
				throw new ServiceDisabledException();
			}
			List<string> arrayList = new List<string>();
			arrayList.Add("mt.getCategoryList");
			arrayList.Add("mt.getPostCategories");
			arrayList.Add("mt.getRecentPostTitles");
			arrayList.Add("mt.getTrackbackPings");
			arrayList.Add("mt.publishPost");
			arrayList.Add("mt.setPostCategories");
			arrayList.Add("mt.supportedMethods");
			arrayList.Add("mt.supportedTextFilters");

			return arrayList.ToArray();
		}

		public MoveableType.TextFilter[] mt_supportedTextFilters()
		{
			if (!dasBlogSettings.SiteConfiguration.EnableBloggerApi)
			{
				throw new ServiceDisabledException();
			}
			MoveableType.TextFilter tf = new MoveableType.TextFilter();
			tf.key = "default";
			tf.@value = "default";
			return new MoveableType.TextFilter[] { tf };
		}

		public MoveableType.Category[] InternalGetCategoryList()
		{
			List<MoveableType.Category> arrayList = new List<MoveableType.Category>();
			CategoryCacheEntryCollection categories = dataService.GetCategories();
			if (categories.Count == 0)
			{
				arrayList.Add(InternalGetFrontPageCategory());
			}
			else
			{
				foreach (CategoryCacheEntry catEntry in categories)
				{
					MoveableType.Category category = new MoveableType.Category();
					category.categoryId = NoNull(catEntry.Name);
					category.categoryName = NoNull(catEntry.Name);
					//category.isPrimary=false;
					arrayList.Add(category);
				}
			}
			return arrayList.ToArray();
		}

		private MoveableType.Category InternalGetFrontPageCategory()
		{
			MoveableType.Category mcat = new MoveableType.Category();
			mcat.categoryId = "Front Page";
			mcat.categoryName = "Front Page";
			//mcat.isPrimary = true;
			return mcat;
		}

		private string NoNull(string s)
		{
			if (s == null)
				return string.Empty;
			else
				return s;
		}

		public bool blogger_deletePost(string appKey, string postid, string username, string password, [XmlRpcParameter(Description = "Where applicable, this specifies whether the blog should be republished after the post has been deleted.")] bool publish)
		{
			if (!dasBlogSettings.SiteConfiguration.EnableBloggerApi)
			{
				throw new ServiceDisabledException();
			}

			if (!VerifyLogin(username, password))
			{
				throw new SecurityException();
			}

			dataService.DeleteEntry(postid, null);

			return true;
		}

		public bool blogger_editPost(string appKey, string postid, string username, string password, string content, bool publish)
		{
			if (!dasBlogSettings.SiteConfiguration.EnableBloggerApi)
			{
				throw new ServiceDisabledException();
			}

			if (!VerifyLogin(username, password))
			{
				throw new SecurityException();
			}

			Entry entry = dataService.GetEntryForEdit(postid);
			if (entry != null)
			{
				FillEntryFromBloggerPost(entry, content, username);
				entry.IsPublic = publish;
				entry.Syndicated = publish;

				dataService.SaveEntry(entry);
			}
			return true;
		}

		public Blogger.Category[] blogger_getCategories(string blogid, string username, string password)
		{
			if (!dasBlogSettings.SiteConfiguration.EnableBloggerApi)
			{
				throw new ServiceDisabledException();
			}

			if (!VerifyLogin(username, password))
			{
				throw new SecurityException();
			}

			List<Blogger.Category> arrayList = new List<Blogger.Category>();
			CategoryCacheEntryCollection categories = dataService.GetCategories();
			if (categories.Count == 0)
			{
				Blogger.Category bcat = new Blogger.Category();
				bcat.categoryid = "Front Page";
				bcat.description = "Front Page";
				bcat.htmlUrl = dasBlogSettings.GetCategoryViewUrl(bcat.categoryid);
				bcat.rssUrl = dasBlogSettings.GetCategoryViewUrl(bcat.categoryid);
				bcat.title = NoNull(bcat.description);
				arrayList.Add(bcat);
			}
			else foreach (CategoryCacheEntry cat in categories)
				{
					Blogger.Category bcat = new Blogger.Category();
					bcat.categoryid = NoNull(cat.Name);
					bcat.description = NoNull(cat.Name);
					bcat.htmlUrl = dasBlogSettings.GetCategoryViewUrl(cat.Name);
					bcat.rssUrl = dasBlogSettings.GetCategoryViewUrl(cat.Name);  //Should this be GetRssCategoryUrl()
					bcat.title = NoNull(cat.Name);
					arrayList.Add(bcat);
				}
			return arrayList.ToArray();
		}

		public Blogger.Post blogger_getPost(string appKey, string postid, string username, string password)
		{
			if (!dasBlogSettings.SiteConfiguration.EnableBloggerApi)
			{
				throw new ServiceDisabledException();
			}

			if (!VerifyLogin(username, password))
			{
				throw new SecurityException();
			}

			Entry entry = dataService.GetEntry(postid);
			if (entry != null)
			{
				Blogger.Post post = new Blogger.Post();
				FillBloggerPostFromEntry(entry, ref post);
				return post;
			}
			else
			{
				return new Blogger.Post();
			}
		}

		public Blogger.Post[] blogger_getRecentPosts(string appKey, string blogid, string username, string password, int numberOfPosts)
		{
			if (!dasBlogSettings.SiteConfiguration.EnableBloggerApi)
			{
				throw new ServiceDisabledException();
			}

			if (!VerifyLogin(username, password))
			{
				throw new SecurityException();
			}

			EntryCollection entries = dataService.GetEntriesForDay(DateTime.Now.ToUniversalTime(), dasBlogSettings.GetConfiguredTimeZone(),
											null, dasBlogSettings.SiteConfiguration.RssDayCount, numberOfPosts, null);
			List<Blogger.Post> arrayList = new List<Blogger.Post>();
			foreach (Entry entry in entries)
			{
				Blogger.Post post = new Blogger.Post();
				FillBloggerPostFromEntry(entry, ref post);
				arrayList.Add(post);
			}
			return arrayList.ToArray();
		}

		public string blogger_getTemplate(string appKey, string blogid, string username, string password, string templateType)
		{
			if (!dasBlogSettings.SiteConfiguration.EnableBloggerApi)
			{
				throw new ServiceDisabledException();
			}

			if (!VerifyLogin(username, password))
			{
				throw new SecurityException();
			}

			return "";
		}

		public Blogger.UserInfo blogger_getUserInfo(string appKey, string username, string password)
		{
			if (!dasBlogSettings.SiteConfiguration.EnableBloggerApi)
			{
				throw new ServiceDisabledException();
			}

			if (!VerifyLogin(username, password))
			{
				throw new SecurityException();
			}

			User user = siteSecurityManager.GetUser(username);
			Blogger.UserInfo userInfo = new Blogger.UserInfo();

			userInfo.email = NoNull(user.EmailAddress);
			userInfo.url = NoNull(dasBlogSettings.SiteConfiguration.Root);
			userInfo.firstname = "";
			userInfo.lastname = "";
			userInfo.nickname = NoNull(user.DisplayName);
			return userInfo;
		}

		public Blogger.BlogInfo[] blogger_getUsersBlogs(string appKey, string username, string password)
		{
			if (!dasBlogSettings.SiteConfiguration.EnableBloggerApi)
			{
				throw new ServiceDisabledException();
			}

			if (!VerifyLogin(username, password))
			{
				throw new SecurityException();
			}

			Blogger.BlogInfo[] blogs = new Blogger.BlogInfo[1];
			Blogger.BlogInfo blog = new Blogger.BlogInfo();
			blog.blogid = "0";
			blog.blogName = NoNull(dasBlogSettings.SiteConfiguration.Title);
			blog.url = NoNull(dasBlogSettings.SiteConfiguration.Root);
			blogs[0] = blog;
			return blogs;
		}

		public string blogger_newPost(string appKey, string blogid, string username, string password, string content, bool publish)
		{
			if (!dasBlogSettings.SiteConfiguration.EnableBloggerApi)
			{
				throw new ServiceDisabledException();
			}

			if (!VerifyLogin(username, password))
			{
				throw new SecurityException();
			}

			Entry newPost = new Entry();
			newPost.Initialize();
			FillEntryFromBloggerPost(newPost, content, username);
			newPost.IsPublic = publish;
			newPost.Syndicated = publish;

			dataService.SaveEntry(newPost);

			return newPost.EntryId;
		}

		public bool blogger_setTemplate(string appKey, string blogid, string username, string password, string template, string templateType)
		{
			if (!dasBlogSettings.SiteConfiguration.EnableBloggerApi)
			{
				throw new ServiceDisabledException();
			}

			if (!VerifyLogin(username, password))
			{
				throw new SecurityException();
			}

			return false;
		}

		private void FillEntryFromBloggerPost(Entry entry, string content, string username)
		{
			const string TitleStart = "<title>";
			const string TitleStop = "</title>";

			// The title can optionally be sent inline with the content as an XML fragment.  Make sure to parse it out correctly
			// Can't use normal XML processing tools because it's not well formed.  Have to do this the old fashioned way.
			string title = "";
			string lowerContent = content.ToLower();

			int nTitleStart = lowerContent.IndexOf(TitleStart);
			if (nTitleStart >= 0)
			{
				nTitleStart += TitleStart.Length;
				int nTitleStop = lowerContent.IndexOf(TitleStop, nTitleStart);

				title = content.Substring(nTitleStart, nTitleStop - nTitleStart);
				content = content.Substring(nTitleStop + TitleStop.Length);
			}

			//newPost.CreatedUtc = DateTime.Now.ToUniversalTime();

			entry.Title = title;
			entry.Description = "";
			entry.Content = content;
			entry.Author = username;
		}

		private void FillBloggerPostFromEntry(Entry entry, ref Blogger.Post post)
		{
			post.content = NoNull(string.Format("<title>{0}</title>{1}", entry.Title, entry.Content));
			post.dateCreated = entry.CreatedUtc;
			post.postid = NoNull(entry.EntryId);
			post.userid = NoNull(entry.Author);
		}

		public bool metaweblog_editPost(string postid, string username, string password, MetaWeblog.Post post, bool publish)
		{
			if (!dasBlogSettings.SiteConfiguration.EnableBloggerApi)
			{
				throw new ServiceDisabledException();
			}

			if (!VerifyLogin(username, password))
			{
				throw new SecurityException();
			}

			Entry entry = dataService.GetEntryForEdit(postid);
			if (entry != null)
			{
				entry.Author = username;
				TrackbackInfoCollection trackbackList = FillEntryFromMetaWeblogPost(entry, post);

				entry.IsPublic = publish;
				entry.Syndicated = publish;

				dataService.SaveEntry(entry);
			}
			return true;
		}

		public MetaWeblog.CategoryInfo[] metaweblog_getCategories(string blogid, string username, string password)
		{
			if (!dasBlogSettings.SiteConfiguration.EnableBloggerApi)
			{
				throw new ServiceDisabledException();
			}

			if (!VerifyLogin(username, password))
			{
				throw new SecurityException();
			}

			List<MetaWeblog.CategoryInfo> arrayList = new List<MetaWeblog.CategoryInfo>();
			CategoryCacheEntryCollection categories = dataService.GetCategories();
			if (categories.Count == 0)
			{
				MetaWeblog.CategoryInfo bcat = new MetaWeblog.CategoryInfo();
				bcat.categoryid = "Front Page";
				bcat.description = "Front Page";
				bcat.htmlUrl = dasBlogSettings.GetCategoryViewUrl(bcat.categoryid);
				bcat.rssUrl = dasBlogSettings.GetRssCategoryUrl(bcat.categoryid);
				bcat.title = NoNull(bcat.description);
				arrayList.Add(bcat);
			}
			else
			{
				foreach (CategoryCacheEntry cat in categories)
				{
					MetaWeblog.CategoryInfo bcat = new MetaWeblog.CategoryInfo();
					bcat.categoryid = NoNull(cat.Name);
					bcat.description = NoNull(cat.Name);
					bcat.htmlUrl = dasBlogSettings.GetCategoryViewUrl(cat.Name);
					bcat.rssUrl = dasBlogSettings.GetRssCategoryUrl(cat.Name);
					bcat.title = NoNull(cat.Name);
					arrayList.Add(bcat);
				}
			}
			return arrayList.ToArray();
		}

		public MetaWeblog.Post metaweblog_getPost(string postid, string username, string password)
		{
			if (!dasBlogSettings.SiteConfiguration.EnableBloggerApi)
			{
				throw new ServiceDisabledException();
			}

			if (!VerifyLogin(username, password))
			{
				throw new SecurityException();
			}

			Entry entry = dataService.GetEntry(postid);
			if (entry != null)
			{
				return this.Create(entry);
			}
			else
			{
				return new MetaWeblog.Post();
			}
		}

		public MetaWeblog.Post[] metaweblog_getRecentPosts(string blogid, string username, string password, int numberOfPosts)
		{
			if (!dasBlogSettings.SiteConfiguration.EnableBloggerApi)
			{
				throw new ServiceDisabledException();
			}

			if (!VerifyLogin(username, password))
			{
				throw new SecurityException();
			}

			EntryCollection entries = dataService.GetEntriesForDay(DateTime.Now.ToUniversalTime(), dasBlogSettings.GetConfiguredTimeZone(), null,
														dasBlogSettings.SiteConfiguration.RssDayCount, numberOfPosts, null);
			List<MetaWeblog.Post> arrayList = new List<MetaWeblog.Post>();
			foreach (Entry entry in entries)
			{
				arrayList.Add(this.Create(entry));
			}
			return arrayList.ToArray();
		}

		public string metaweblog_newPost(string blogid, string username, string password, MetaWeblog.Post post, bool publish)
		{
			if (!dasBlogSettings.SiteConfiguration.EnableBloggerApi)
			{
				throw new ServiceDisabledException();
			}

			if (!VerifyLogin(username, password))
			{
				throw new SecurityException();
			}

			Entry newPost = new Entry();
			newPost.Initialize();
			newPost.Author = username;

			TrackbackInfoCollection trackbackList = FillEntryFromMetaWeblogPost(newPost, post);

			newPost.IsPublic = publish;
			newPost.Syndicated = publish;

			dataService.SaveEntry(newPost);

			return newPost.EntryId;
		}

		public MetaWeblog.UrlInfo metaweblog_newMediaObject(object blogid, string username, string password, MetaWeblog.MediaType enc)
		{
			if (!dasBlogSettings.SiteConfiguration.EnableBloggerApi)
			{
				throw new ServiceDisabledException();
			}

			if (!VerifyLogin(username, password))
			{
				throw new SecurityException();
			}

			Stream stream = new MemoryStream(enc.bits);

			var filePath = binaryManager.SaveFile(stream, enc.name);

			var urlInfo = new MetaWeblog.UrlInfo
			{
				url = Path.Combine(dasBlogSettings.RelativeToRoot(dasBlogSettings.SiteConfiguration.BinariesDir.TrimStart('~')), enc.name)
			};

			return urlInfo;
		}

		/// <summary>Fills a DasBlog entry from a MetaWeblog Post structure.</summary>
		/// <param name="entry">DasBlog entry to fill.</param>
		/// <param name="post">MetaWeblog post structure to fill from.</param>
		/// <returns>TrackbackInfoCollection of posts to send trackback pings.</returns>
		private TrackbackInfoCollection FillEntryFromMetaWeblogPost(Entry entry, MetaWeblog.Post post)
		{
			// W.Bloggar doesn't pass in the DataCreated, 
			// so we have to check for that
			if (post.dateCreated != DateTime.MinValue)
			{
				entry.CreatedUtc = post.dateCreated.ToUniversalTime();
			}

			//Patched to avoid html entities in title
			entry.Title = post.title; // TODO: Find out how to decode this...  HttpUtility.HtmlDecode(post.title);
			entry.Content = post.description;
			entry.Description = NoNull(post.mt_excerpt);

			// If mt_allow_comments is null, then the sender did not specify.  Use default dasBlog behavior in that case
			if (post.mt_allow_comments != null)
			{
				int nAllowComments = Convert.ToInt32(post.mt_allow_comments);
				if (nAllowComments == 0 || nAllowComments == 2)
					entry.AllowComments = false;
				else
					entry.AllowComments = true;
			}

			if (post.categories != null && post.categories.Length > 0)
			{
				// handle categories
				string categories = "";

				StringBuilder sb = new StringBuilder();
				bool needSemi = false;
				post.categories = RemoveDups(post.categories, true);
				foreach (string category in post.categories)
				{
					//watch for "" as a category
					if (category.Length > 0)
					{
						if (needSemi) sb.Append(";");
						sb.Append(category);
						needSemi = true;
					}
				}
				categories = sb.ToString();

				if (categories.Length > 0)
					entry.Categories = categories;
			}

			// We'll always return at least an empty collection
			TrackbackInfoCollection trackbackList = new TrackbackInfoCollection();

			// Only MT supports trackbacks in the post
			if (post.mt_tb_ping_urls != null)
			{
				foreach (string trackbackUrl in post.mt_tb_ping_urls)
				{
					trackbackList.Add(new TrackbackInfo(
						trackbackUrl,
						dasBlogSettings.GetPermaLinkUrl(entry.EntryId),
						entry.Title,
						entry.Description,
						dasBlogSettings.SiteConfiguration.Title));
				}
			}
			return trackbackList;
		}

		private string[] RemoveDups(string[] items, bool sort)
		{
			List<string> noDups = new List<string>();
			for (int i = 0; i < items.Length; i++)
			{
				if (!noDups.Contains(items[i].Trim()))
				{
					noDups.Add(items[i].Trim());
				}
			}
			if (sort) noDups.Sort();  //sorts list alphabetically
			string[] uniqueItems = new string[noDups.Count];
			noDups.CopyTo(uniqueItems);
			return uniqueItems;
		}

		private MetaWeblog.Post Create(Entry entry)
		{
			if (entry == null) throw new ArgumentNullException("entry");

			MetaWeblog.Post post = new MetaWeblog.Post();
			post.description = entry.Content ?? "";
			post.mt_excerpt = entry.Description ?? "";
			post.dateCreated = entry.CreatedUtc;
			post.title = entry.Title ?? "";
			post.link = post.permalink = dasBlogSettings.GetPermaLinkUrl(entry.EntryId);
			post.postid = entry.EntryId ?? "";
			post.categories = entry.GetSplitCategories();
			return post;
		}

		private bool VerifyLogin(string username, string password)
		{
			var user =siteSecurityManager.GetUser(username);
			return siteSecurityManager.VerifyHashedPassword(user.Password, password);
		}
	}
}
