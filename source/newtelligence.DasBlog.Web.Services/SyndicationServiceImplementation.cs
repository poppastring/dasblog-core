#region Copyright (c) 2003, newtelligence AG. All rights reserved.
/*
// Copyright (c) 2003, newtelligence AG. (http://www.newtelligence.com)
// Original BlogX Source Code: Copyright (c) 2003, Chris Anderson (http://simplegeek.com)
// All rights reserved.
//  
// Redistribution and use in source and binary forms, with or without modification, are permitted 
// provided that the following conditions are met: 
//  
// (1) Redistributions of source code must retain the above copyright notice, this list of 
// conditions and the following disclaimer. 
// (2) Redistributions in binary form must reproduce the above copyright notice, this list of 
// conditions and the following disclaimer in the documentation and/or other materials 
// provided with the distribution. 
// (3) Neither the name of the newtelligence AG nor the names of its contributors may be used 
// to endorse or promote products derived from this software without specific prior 
// written permission.
//      
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS 
// OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY 
// AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL 
// DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, 
// DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER 
// IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT 
// OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
// -------------------------------------------------------------------------
//
// Original BlogX source code (c) 2003 by Chris Anderson (http://simplegeek.com)
// 
// newtelligence is a registered trademark of newtelligence Aktiengesellschaft.
// 
// For portions of this software, the some additional copyright notices may apply 
// which can either be found in the license.txt file included in the source distribution
// or following this notice. 
//
*/
#endregion

using System.Globalization;

namespace newtelligence.DasBlog.Web.Services
{
    using System;
    using System.ComponentModel;
    using System.Web;
    using System.Web.Services;
    using System.Web.Services.Protocols;
    using System.Web.Services.Description;
    using System.Xml;
    using System.Xml.Serialization;
    using newtelligence.DasBlog.Runtime;
    using newtelligence.DasBlog.Web.Services.Rss20;
    using newtelligence.DasBlog.Web.Services.Atom10;
    using newtelligence.DasBlog.Web.Core;
    using newtelligence.DasBlog.Util.Html;
    using System.Collections.Generic;
    using InstantArticle;
	using NodaTime;

	/// <summary>
	/// Summary description for DasBlogBrowsing.
	/// </summary>
	[WebService(Namespace="urn:schemas-newtelligence-com:dasblog:syndication-services")]
	public class SyndicationServiceImplementation : SyndicationServiceBase 
	{
		protected bool inASMX = false;

		public SyndicationServiceImplementation()
		{
			InitializeComponent();
		}

		public SyndicationServiceImplementation(SiteConfig siteConfig, IBlogDataService dataService, ILoggingDataService loggingService):
			base( siteConfig, dataService, loggingService )
		{
			//CODEGEN: This call is required by the ASP.NET Web Services Designer
			InitializeComponent();
		}


		#region Component Designer generated code
		
		//Required by the Web Services Designer 
		private IContainer components = null;
				
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if(disposing && components != null)
			{
				components.Dispose();
			}
			base.Dispose(disposing);		
		}
		
		#endregion

		#region RSS 2.0
		[WebMethod]
		[SoapDocumentMethod(ParameterStyle=SoapParameterStyle.Wrapped,Use=SoapBindingUse.Literal)]
		public string GetRssUrl()
		{
			return SiteUtilities.GetRssUrl(siteConfig);
		}

		[WebMethod]
		[SoapDocumentMethod(ParameterStyle=SoapParameterStyle.Wrapped,Use=SoapBindingUse.Literal)]
		public string GetCommentsRssUrl() 
		{
			return SiteUtilities.GetCommentsRssUrl(siteConfig);
		}
		
		// OmarS: no longer need CacheDuration since we are sending Last Modified
		// and ETag
		//[WebMethod(CacheDuration=60)]
		[WebMethod]
		[SoapDocumentMethod(ParameterStyle=SoapParameterStyle.Wrapped,Use=SoapBindingUse.Literal)]
		[return:XmlAnyElement]
		public RssRoot GetRss()
		{
			int maxDays = siteConfig.RssDayCount;
			int maxEntries = siteConfig.RssMainEntryCount;
			return GetRssWithCounts(maxDays, maxEntries);
		}

		[WebMethod(CacheDuration=180)]
		[SoapDocumentMethod(ParameterStyle=SoapParameterStyle.Wrapped,Use=SoapBindingUse.Literal)]
		[return:XmlAnyElement]
		public RssRoot GetCommentsRss() 
		{
			CommentCollection _com = dataService.GetAllComments();
			_com = ReorderComments(_com);
			return GetCommentsRssCore(_com);
		}

		//[WebMethod(CacheDuration=180)]
		[WebMethod]
		[SoapDocumentMethod(ParameterStyle=SoapParameterStyle.Wrapped,Use=SoapBindingUse.Literal)]
		[return:XmlAnyElement]
		public RssRoot GetEntryCommentsRss(string guid) 
		{
			CommentCollection _com = dataService.GetPublicCommentsFor(guid);
			_com = ReorderComments(_com);
			return GetCommentsRssCore(_com,guid);
		}

		[WebMethod(CacheDuration=60)]
		[SoapDocumentMethod(ParameterStyle=SoapParameterStyle.Wrapped,Use=SoapBindingUse.Literal)]
		[return:XmlAnyElement]
		public RssRoot GetRssCategory(string categoryName)
		{
			return GetRssCore(categoryName, 0, 0);
		}

		[WebMethod(CacheDuration=60)]
		[SoapDocumentMethod(ParameterStyle=SoapParameterStyle.Wrapped,Use=SoapBindingUse.Literal)]
		[return:XmlAnyElement]
		public RssRoot GetRssWithCounts(int maxDayCount, int maxEntryCount)
		{
			return GetRssCore(null, maxDayCount, maxEntryCount);
		}

		private bool RedirectToFeedBurnerIfNeeded(string category)
		{
			if( inASMX )
			{
				//If we are using FeedBurner, only allow them to get our feed...
				if(siteConfig.FeedBurnerName != null && siteConfig.FeedBurnerName.Length > 0)
				{
					HttpContext current = HttpContext.Current;
					string userAgent = current.Request.UserAgent;
					if(userAgent != null && userAgent.Length >0)
					{
						// If they aren't FeedBurner and they aren't asking for a category, redirect them!
						if (userAgent.StartsWith("FeedBurner") == false && (category == null || category.Length == 0))
						{
							current.Response.StatusCode = 301;
							current.Response.Status = "301 Moved Permanently";
							current.Response.RedirectLocation = SiteUtilities.GetRssUrl(siteConfig);
							return true;
						}
					}
				}
			}
			return false;				
		}
		private RssRoot GetRssCore(string category, int maxDayCount, int maxEntryCount)
		{
			if (RedirectToFeedBurnerIfNeeded(category) == true)
			{
				return null;
			}
			EntryCollection entries = null;
			//We only build the entries if blogcore doesn't exist and we'll need them later...
			if (dataService.GetLastEntryUpdate() == DateTime.MinValue)
			{
				entries = BuildEntries(category, maxDayCount, maxEntryCount);
			}
			
			//Try to get out as soon as possible with as little CPU as possible
			if ( inASMX )
			{
				DateTime lastModified = SiteUtilities.GetLatestModifedEntryDateTime(dataService, entries);
				if (SiteUtilities.GetStatusNotModified(lastModified))
					return null;
			}

			if ( inASMX )
			{
				string referrer = Context.Request.UrlReferrer!=null?Context.Request.UrlReferrer.AbsoluteUri:"";
				if (ReferralBlackList.IsBlockedReferrer(referrer))
				{
					if (siteConfig.EnableReferralUrlBlackList404s)
					{
						return null;
					}
				}
				else
				{
					loggingService.AddReferral(
						new LogDataItem(
						Context.Request.RawUrl,
						referrer,
						Context.Request.UserAgent,
						Context.Request.UserHostName));
				}
			}

			//not-modified didn't work, do we have this in cache?
			string CacheKey = "Rss:" + category + ":" + maxDayCount.ToString() + ":" + maxEntryCount.ToString();
			RssRoot documentRoot = cache[CacheKey] as RssRoot;
			if (documentRoot == null) //we'll have to build it...
			{
				//However, if we made it this far, the not-modified check didn't work, and we may not have entries...
				if(entries == null)
				{
					entries = BuildEntries(category, maxDayCount, maxEntryCount);			
				}

				documentRoot = new RssRoot();
				documentRoot.Namespaces.Add("dc","http://purl.org/dc/elements/1.1/");
				documentRoot.Namespaces.Add("trackback","http://madskills.com/public/xml/rss/module/trackback/");
				documentRoot.Namespaces.Add("pingback","http://madskills.com/public/xml/rss/module/pingback/");
				if (siteConfig.EnableComments) 
				{
					documentRoot.Namespaces.Add("wfw","http://wellformedweb.org/CommentAPI/");
					documentRoot.Namespaces.Add("slash","http://purl.org/rss/1.0/modules/slash/");
				}
                if (siteConfig.EnableGeoRss)
                {
                    documentRoot.Namespaces.Add("georss", "http://www.georss.org/georss");
                }

				RssChannel ch = new RssChannel();
				
				if(category == null)
				{
					ch.Title = siteConfig.Title;
				}
				else
				{
					ch.Title = siteConfig.Title + " - " + category;
				}				
				
				if ( siteConfig.Description == null || siteConfig.Description.Trim().Length == 0 )
				{
					ch.Description = siteConfig.Subtitle;
				}
				else
				{
					ch.Description=siteConfig.Description;
				}
            
				ch.Link = SiteUtilities.GetBaseUrl(siteConfig);
				ch.Copyright = siteConfig.Copyright;
				if (siteConfig.RssLanguage != null && siteConfig.RssLanguage.Length >0)
				{
					ch.Language = siteConfig.RssLanguage;
				}
				ch.ManagingEditor = siteConfig.Contact;
				ch.WebMaster = siteConfig.Contact;
				ch.Image = null;
				if (siteConfig.ChannelImageUrl != null && siteConfig.ChannelImageUrl.Trim().Length > 0)
				{
					ChannelImage channelImage = new ChannelImage();
					channelImage.Title = ch.Title;
					channelImage.Link = ch.Link;
					if (siteConfig.ChannelImageUrl.StartsWith("http"))
					{
						channelImage.Url = siteConfig.ChannelImageUrl;
					}
					else
					{
						channelImage.Url = SiteUtilities.RelativeToRoot(siteConfig,siteConfig.ChannelImageUrl);
					}
					ch.Image = channelImage;
				}

				documentRoot.Channels.Add(ch);

				foreach (Entry entry in entries)
				{
					if (entry.IsPublic == false || entry.Syndicated == false)
					{
						continue;
					}
					XmlDocument doc2 = new XmlDocument();
                    List<XmlElement> anyElements = new List<XmlElement>();
					RssItem item = new RssItem();
					item.Title = entry.Title;
					item.Guid = new Rss20.Guid();
					item.Guid.IsPermaLink = false;
					item.Guid.Text = SiteUtilities.GetPermaLinkUrl(siteConfig, entry.EntryId);
                    item.Link = SiteUtilities.GetPermaLinkUrl(siteConfig, (ITitledEntry)entry);
					User user = SiteSecurity.GetUser(entry.Author);
					//Scott Hanselman: According to the RSS 2.0 spec and FeedValidator.org, 
					// we can have EITHER an Author tag OR the preferred dc:creator tag, but NOT BOTH.
					//					if (user != null && user.EmailAddress != null && user.EmailAddress.Length > 0)
					//					{
					//						if (user.DisplayName != null && user.DisplayName.Length > 0)
					//						{
					//							item.Author = String.Format("{0} ({1})", user.EmailAddress, user.DisplayName);
					//						}
					//						else 
					//						{
					//							item.Author = user.EmailAddress;
					//						}
					//					}
					XmlElement trackbackPing = doc2.CreateElement("trackback","ping","http://madskills.com/public/xml/rss/module/trackback/");
					trackbackPing.InnerText= SiteUtilities.GetTrackbackUrl(siteConfig,entry.EntryId);
					anyElements.Add(trackbackPing);
					
					XmlElement pingbackServer = doc2.CreateElement("pingback","server","http://madskills.com/public/xml/rss/module/pingback/");
					pingbackServer.InnerText= new Uri(new Uri(SiteUtilities.GetBaseUrl(siteConfig)), "pingback.aspx").ToString();
					anyElements.Add(pingbackServer);
					
					XmlElement pingbackTarget = doc2.CreateElement("pingback","target","http://madskills.com/public/xml/rss/module/pingback/");
					pingbackTarget.InnerText=SiteUtilities.GetPermaLinkUrl(siteConfig, entry.EntryId);
					anyElements.Add(pingbackTarget);
				
					XmlElement dcCreator = doc2.CreateElement("dc","creator","http://purl.org/dc/elements/1.1/");
					if (user != null)
					{
						// HACK AG No author e-mail address in feed items.
						//						if (user.DisplayName != null && user.DisplayName.Length > 0)
						//						{
						//							if(user.EmailAddress != null && user.EmailAddress.Length > 0)
						//							{
						//								dcCreator.InnerText = String.Format("{0} ({1})", user.EmailAddress, user.DisplayName);
						//							}
						//							else
						//							{
						dcCreator.InnerText = user.DisplayName;
						//							}
						//						}
						//						else 
						//						{
						//							dcCreator.InnerText = user.EmailAddress;
						//						}
					}
					anyElements.Add(dcCreator);

                    // Add GeoRSS if it exists.
                    if (siteConfig.EnableGeoRss)
                    {
						Nullable<double> latitude = new Nullable<double>();
						Nullable<double> longitude = new Nullable<double>();

						if (entry.Latitude.HasValue)
						{
							latitude = entry.Latitude;
						}
						else
						{
							if (siteConfig.EnableDefaultLatLongForNonGeoCodedPosts)
							{
								latitude = siteConfig.DefaultLatitude;
							}
						}

						if (entry.Longitude.HasValue)
						{
							longitude = entry.Longitude;
						}
						else
						{
							if (siteConfig.EnableDefaultLatLongForNonGeoCodedPosts)
							{
								longitude = siteConfig.DefaultLongitude;
							}
						}

						if (latitude.HasValue && longitude.HasValue)
                        {
                            XmlElement geoLoc = doc2.CreateElement("georss", "point", "http://www.georss.org/georss");
							geoLoc.InnerText = String.Format(CultureInfo.InvariantCulture, "{0:R} {1:R}", latitude, longitude);
                            anyElements.Add(geoLoc);
                        }
                    }

					if (siteConfig.EnableComments) 
					{
						if (entry.AllowComments)
						{
							XmlElement commentApi = doc2.CreateElement("wfw","comment","http://wellformedweb.org/CommentAPI/");
							commentApi.InnerText=SiteUtilities.GetCommentViewUrl(siteConfig,entry.EntryId);
							anyElements.Add(commentApi);
						}

						XmlElement commentRss = doc2.CreateElement("wfw","commentRss","http://wellformedweb.org/CommentAPI/");
						commentRss.InnerText=SiteUtilities.GetEntryCommentsRssUrl(siteConfig,entry.EntryId);
						anyElements.Add(commentRss);

						//for RSS conformance per FeedValidator.org
						int commentsCount = dataService.GetPublicCommentsFor(entry.EntryId).Count;
						if (commentsCount > 0)
						{
							XmlElement slashComments = doc2.CreateElement("slash","comments","http://purl.org/rss/1.0/modules/slash/");
							slashComments.InnerText=commentsCount.ToString();
							anyElements.Add(slashComments);
						}
						item.Comments = SiteUtilities.GetCommentViewUrl(siteConfig,entry.EntryId);
					}
					item.Language = entry.Language;
					
					if (entry.Categories != null && entry.Categories.Length > 0)
					{
						if(item.Categories == null) item.Categories = new RssCategoryCollection();

						string[] cats = entry.Categories.Split(';');
						foreach(string c in cats)
						{
							RssCategory cat = new RssCategory();
							string cleanCat = c.Replace('|', '/');
							cat.Text = cleanCat;
							item.Categories.Add(cat);
						}
					}
					if (entry.Attachments.Count > 0)
					{
						// RSS currently supports only a single enclsoure so we return the first one	
						item.Enclosure =  new Enclosure();
						item.Enclosure.Url = SiteUtilities.GetEnclosureLinkUrl(entry.EntryId, entry.Attachments[0]);
						item.Enclosure.Type = entry.Attachments[0].Type;
						item.Enclosure.Length = entry.Attachments[0].Length.ToString();
					}
					item.PubDate = entry.CreatedUtc.ToString("R");
					if (ch.LastBuildDate == null || ch.LastBuildDate.Length == 0)
					{
						ch.LastBuildDate = item.PubDate;
					}


					if (!siteConfig.AlwaysIncludeContentInRSS && 
						entry.Description != null && 
						entry.Description.Trim().Length > 0)
					{
						item.Description = PreprocessItemContent(entry.EntryId, entry.Description);
                    
					}
					else
					{
					   if (siteConfig.HtmlTidyContent == false)
					   {
					      item.Description = "<div>" + PreprocessItemContent(entry.EntryId, entry.Content) + "</div>";
					   }
					   else
					   {
                     item.Description = ContentFormatter.FormatContentAsHTML(PreprocessItemContent(entry.EntryId, entry.Content));


                     try
                     {
                        string xhtml = ContentFormatter.FormatContentAsXHTML(PreprocessItemContent(entry.EntryId, entry.Content));
                        doc2.LoadXml(xhtml);
                        anyElements.Add((XmlElement)doc2.SelectSingleNode("//*[local-name() = 'body'][namespace-uri()='http://www.w3.org/1999/xhtml']"));
                     }
                     catch //(Exception ex)
                     {
                        //Debug.Write(ex.ToString());
                        // absorb
                     }
					   }
					}

					item.anyElements = anyElements.ToArray();
					ch.Items.Add(item);
				}
				cache.Insert(CacheKey,documentRoot,DateTime.Now.AddMinutes(5));
			}
			return documentRoot;
		}

		
		private CommentCollection ReorderComments(CommentCollection _comments) 
		{
            CommentCollection tempCol = new CommentCollection( _comments );
            tempCol.Sort( new CommentSorter() );
            return tempCol;
		}

		private RssRoot GetCommentsRssCore(CommentCollection _com)
		{
			return GetCommentsRssCore(_com,String.Empty);
		}

		private RssRoot GetCommentsRssCore(CommentCollection _com, string guid) 
		{
			//Try to get out as soon as possible with as little CPU as possible
			if ( inASMX )
			{
				DateTime lastModified = SiteUtilities.GetLatestModifedCommentDateTime(dataService, _com);
				if (SiteUtilities.GetStatusNotModified(lastModified))
					return null;
			}

			if ( inASMX )
			{
				string referrer = Context.Request.UrlReferrer!=null?Context.Request.UrlReferrer.AbsoluteUri:"";
				if (ReferralBlackList.IsBlockedReferrer(referrer) == false)
				{
					loggingService.AddReferral(
						new LogDataItem(
						Context.Request.RawUrl,
						referrer,
						Context.Request.UserAgent,
						Context.Request.UserHostName));
				}            
			}
			
			// TODO: Figure out why this code is copied and pasted from above rather than using a function (shame!)
			RssRoot documentRoot = new RssRoot();
			RssChannel ch = new RssChannel();
			if (guid != null && guid != String.Empty)
			{
				//Set the title for this RSS Comments feed
				ch.Title = siteConfig.Title + " - Comments on " + dataService.GetEntry(guid).Title;
			}
			else
			{
				ch.Title = siteConfig.Title + " - Comments";
			}
			ch.Link = SiteUtilities.GetBaseUrl(siteConfig);
			ch.Copyright = siteConfig.Copyright;
			ch.ManagingEditor = siteConfig.Contact;
			ch.WebMaster = siteConfig.Contact;
			documentRoot.Channels.Add(ch);
			

			int i = 0;
			
			foreach (Comment c in _com) 
			{
                List<XmlElement> anyElements = new List<XmlElement>();
				if (i == siteConfig.RssEntryCount) { break; }
				i++;
				string tempTitle = "";
				RssItem item = new RssItem();

				if(c.TargetTitle != null && c.TargetTitle.Length > 0 ) 
				{
					tempTitle = " on \"" + c.TargetTitle + "\"";
				}

				if(c.Author == null || c.Author == "") 
				{
					item.Title = "Comment" + tempTitle;
				}
				else 
				{
					item.Title = "Comment by " + c.Author + tempTitle;
				}

				//Per the RSS Comments Spec it makes more sense for guid and link to be the same.
				// http://blogs.law.harvard.edu/tech/rss#comments
				// 11/11/05 - SDH - Now, I'm thinking not so much...
				item.Guid = new Rss20.Guid();
				item.Guid.Text = c.EntryId;
				item.Guid.IsPermaLink = false;
				item.Link = SiteUtilities.GetCommentViewUrl(siteConfig, c.TargetEntryId, c.EntryId);

				item.PubDate = c.CreatedUtc.ToString("R");
				
				item.Description = c.Content.Replace(Environment.NewLine, "<br />");
				if (c.AuthorHomepage == null || c.AuthorHomepage == "") 
				{
					if (c.AuthorEmail == null || c.AuthorEmail == "") 
					{
						if(!(c.Author == null || c.Author == "") )
						{
							item.Description = c.Content.Replace(Environment.NewLine, "<br />") + "<br /><br />" + "Posted by: " + c.Author;
						}
					}
					else 
					{
						string content = c.Content.Replace(Environment.NewLine, "<br />");
						if (!siteConfig.SupressEmailAddressDisplay) 
						{
							item.Description = content + "<br /><br />" + "Posted by: " + "<a href=\"mailto:" + SiteUtilities.SpamBlocker(c.AuthorEmail) + "\">" + c.Author + "</a>";
						}
						else 
						{
							item.Description = content + "<br /><br />" + "Posted by: " + c.Author;
						}
							
					}
				}
				else 
				{
					if (c.AuthorHomepage.IndexOf("http://") < 0) 
					{
						c.AuthorHomepage = "http://" + c.AuthorHomepage; 
					}
					item.Description += "<br /><br />" + "Posted by: " + "<a href=\"" + c.AuthorHomepage +
						"\">" + c.Author + "</a>";
				}

				if (c.Author != null && c.Author.Length > 0 ) 
				{

					 	// the rss spec requires an email address in the author tag
					// and it can not be obfuscated
						// according to the feedvalidator
					string email;

					if ( !siteConfig.SupressEmailAddressDisplay ) 
					{
						email = (c.AuthorEmail != null && c.AuthorEmail.Length > 0 ? c.AuthorEmail : "unknown@unknown.org");
					} 
					else 
					{
						email = "suppressed@unknown.org";
					}

					item.Author = String.Format( "{0} ({1})", email, c.Author);
				}

				item.Comments = SiteUtilities.GetCommentViewUrl(siteConfig,c.TargetEntryId,c.EntryId);
				
				if (ch.LastBuildDate == null || ch.LastBuildDate.Length == 0) 
				{
					ch.LastBuildDate = item.PubDate;
				}
				
				XmlDocument doc2 = new XmlDocument();
				try 
				{
					doc2.LoadXml(c.Content);
					anyElements.Add( (XmlElement)doc2.SelectSingleNode("//*[local-name() = 'body'][namespace-uri()='http://www.w3.org/1999/xhtml']") );
				}
				catch 
				{
					// absorb
				}
				item.anyElements = anyElements.ToArray();
				ch.Items.Add(item);
			}

			return documentRoot;
		}

		[WebMethod]
		[SoapDocumentMethod(ParameterStyle=SoapParameterStyle.Wrapped,Use=SoapBindingUse.Literal)]
		public LogDataItemCollection GetReferrerLog(DateTime date)
		{
			return loggingService.GetReferralsForDay(date);
		}

		[WebMethod]
		[SoapDocumentMethod(ParameterStyle=SoapParameterStyle.Wrapped,Use=SoapBindingUse.Literal)]
		public string[] GetCategoryList()
		{
			CategoryCacheEntryCollection categories = dataService.GetCategories();
			string[] cats = new string[categories.Count];
			for (int i=0; i<cats.Length; i++)
			{
				cats[i] = categories[i].Name;
			}

			return cats;
		}

		[WebMethod]
		[SoapDocumentMethod(ParameterStyle=SoapParameterStyle.Wrapped,Use=SoapBindingUse.Literal)]
		public EntryCollection GetCategoryEntries(string categoryName)
		{
			return dataService.GetEntriesForCategory( categoryName, null );
		}

		[WebMethod]
		[SoapDocumentMethod(ParameterStyle=SoapParameterStyle.Wrapped,Use=SoapBindingUse.Literal)]
		public DateTime[] GetDaysWithEntries()
		{
			return dataService.GetDaysWithEntries(DateTimeZone.Utc);
		}

		[WebMethod]
		[SoapDocumentMethod(ParameterStyle=SoapParameterStyle.Wrapped,Use=SoapBindingUse.Literal)]
		public DayEntry GetDayEntry(DateTime date)
		{
			return dataService.GetDayEntry(date);
		}

		[WebMethod]
		[SoapDocumentMethod(ParameterStyle=SoapParameterStyle.Wrapped,Use=SoapBindingUse.Literal)]
		public DayExtra GetDayExtra(DateTime date)
		{
			return dataService.GetDayExtra(date);
		}
        #endregion

        #region Atom 1.0
        [WebMethod]
		[SoapDocumentMethod(ParameterStyle=SoapParameterStyle.Wrapped,Use=SoapBindingUse.Literal)]
		public AtomRoot GetAtom()
		{
			int maxDays = siteConfig.RssDayCount;
			int maxEntries = siteConfig.RssMainEntryCount;
			return GetAtomWithCounts(maxDays, maxEntries);
		}

		[WebMethod]
		[SoapDocumentMethod(ParameterStyle=SoapParameterStyle.Wrapped,Use=SoapBindingUse.Literal)]
		public AtomRoot GetAtomCategory(string categoryName)
		{
			return GetAtomCore(categoryName, 0, 0);
		}

		[WebMethod]
		[SoapDocumentMethod(
			 ParameterStyle=SoapParameterStyle.Wrapped,
			 Use=SoapBindingUse.Literal)]
		public AtomRoot GetAtomWithCounts(int maxDayCount, int maxEntryCount)
		{
			return GetAtomCore(null, maxDayCount, maxEntryCount);
		}

		private AtomRoot GetAtomCore(string category, int maxDayCount, int maxEntryCount)
		{
			if (RedirectToFeedBurnerIfNeeded(category) == true)
			{
				return null;
			}
			
			EntryCollection entries = null;
			//We only build the entries if blogcore doesn't exist and we'll need them later...
			if (dataService.GetLastEntryUpdate() == DateTime.MinValue)
			{
				entries = BuildEntries(category, maxDayCount, maxEntryCount);			
			}

			//Try to get out as soon as possible with as little CPU as possible
			if ( inASMX )
			{
				if (SiteUtilities.GetStatusNotModified(SiteUtilities.GetLatestModifedEntryDateTime(dataService, entries)))
					return null;
			}

			if ( inASMX )
			{
				string referrer = Context.Request.UrlReferrer!=null?Context.Request.UrlReferrer.AbsoluteUri:"";
				if (ReferralBlackList.IsBlockedReferrer(referrer))
				{
					if (siteConfig.EnableReferralUrlBlackList404s)
					{
						return null;
					}
				}
				else
				{
					loggingService.AddReferral(
						new LogDataItem(
						Context.Request.RawUrl,
						referrer,
						Context.Request.UserAgent,
						Context.Request.UserHostName));
				}
			}

			
			//not-modified didn't work, do we have this in cache?
			string CacheKey = "Atom:" + category + ":" + maxDayCount.ToString() + ":" + maxEntryCount.ToString();
			AtomRoot atomFeed = cache[CacheKey] as AtomRoot;
			if (atomFeed == null) //we'll have to build it...
			{
				atomFeed = new AtomRoot();

				//However, if we made it this far, the not-modified check didn't work, and we may not have entries...
				if(entries == null)
				{
					entries = BuildEntries(category, maxDayCount, maxEntryCount);			
				}
			
				if (siteConfig.RssLanguage != null && siteConfig.RssLanguage.Length >0)
				{
					atomFeed.Lang = siteConfig.RssLanguage;
				}
				else //original default behavior of dasBlog
				{
					atomFeed.Lang = "en-us";
				}
				
				if(category == null)
				{
					atomFeed.Title = siteConfig.Title;
				}
				else
				{
					atomFeed.Title = siteConfig.Title + " - " + category; 
				}


				atomFeed.Tagline = siteConfig.Subtitle;
				atomFeed.Links.Add(new AtomLink(SiteUtilities.GetBaseUrl(siteConfig)));
				atomFeed.Links.Add(new AtomLink(SiteUtilities.GetAtomUrl(siteConfig),"self",null));
				atomFeed.Author = new AtomParticipant();
                // shouldn't we use the display name of an owner??
                atomFeed.Author.Name = siteConfig.Copyright;
				atomFeed.Generator = new AtomGenerator();
				atomFeed.Generator.Version = GetType().Assembly.GetName().Version.ToString();
				atomFeed.Icon = "favicon.ico";

				atomFeed.Id = SiteUtilities.GetBaseUrl(siteConfig);

				atomFeed.Logo = null;
				if (siteConfig.ChannelImageUrl != null && siteConfig.ChannelImageUrl.Trim().Length > 0)
				{
					if (siteConfig.ChannelImageUrl.StartsWith("http"))
					{
						atomFeed.Logo = siteConfig.ChannelImageUrl;
					}
					else
					{
						atomFeed.Logo= SiteUtilities.RelativeToRoot(siteConfig,siteConfig.ChannelImageUrl);
					}
				}


				DateTime feedModified = DateTime.MinValue;

				foreach (Entry entry in entries)
				{
					if (entry.IsPublic == false || entry.Syndicated == false)
					{
						continue;
					}

                    string entryLink = SiteUtilities.GetPermaLinkUrl(siteConfig, (ITitledEntry)entry);
					string entryGuid = SiteUtilities.GetPermaLinkUrl(siteConfig, entry.EntryId);
					
					AtomEntry atomEntry = new AtomEntry(entry.Title, new AtomLink(entryLink), entryGuid, entry.CreatedUtc, entry.ModifiedUtc);
					//atomEntry.Created = entry.CreatedUtc; //not needed in 1.0
					atomEntry.Summary = PreprocessItemContent(entry.EntryId, entry.Description);
                    
                    // for multiuser blogs we want to be able to 
                    // show the author per entry
                    User user = SiteSecurity.GetUser(entry.Author);
                    if (user != null)
                    {
                        // only the displayname
                        atomEntry.Author = new AtomParticipant() { Name = user.DisplayName };
                    }

					if (entry.Categories != null && entry.Categories.Length > 0)
					{
						if(atomEntry.Categories == null) atomEntry.Categories = new AtomCategoryCollection();

						string[] cats = entry.Categories.Split(';');
						foreach(string c in cats)
						{
							AtomCategory cat = new AtomCategory();

							//paulb: scheme should be a valid IRI, acsording to the spec 
							// (http://www.atomenabled.org/developers/syndication/atom-format-spec.php#element.category)
							// so I changed this to the Category view for specified category. Now the feed validator likes us again ;-)
							cat.Scheme = SiteUtilities.GetCategoryViewUrl( siteConfig, c ); // "hierarchical";
							// sub categories should be delimited using the path delimiter
							string cleanCat = c.Replace('|', '/');
							//Grab the first category, atom doesn't let us do otherwise!
							cat.Term = HttpUtility.HtmlEncode(cleanCat);
							cat.Label = HttpUtility.HtmlEncode(cleanCat);
							atomEntry.Categories.Add(cat);
						}
					}


					// only if we don't have a summary we emit the content
					if ( siteConfig.AlwaysIncludeContentInRSS || entry.Description == null || entry.Description.Length == 0 )
					{
						atomEntry.Summary = null; // remove empty summary tag
						
						try
						{
							AtomContent atomContent = new AtomContent();
							atomContent.Type = "xhtml";

							XmlDocument xmlDoc2 = new XmlDocument();
							xmlDoc2.LoadXml(ContentFormatter.FormatContentAsXHTML(PreprocessItemContent(entry.EntryId, entry.Content),"div"));
							atomContent.anyElements = new XmlElement[]{xmlDoc2.DocumentElement}; 

							// set the langauge for the content item
							atomContent.Lang = entry.Language;

							atomEntry.Content = atomContent;
						}
						catch(Exception) //XHTML isn't happening today
						{
							//Try again as HTML
							AtomContent atomContent = new AtomContent();
							atomContent.Type = "html";
							atomContent.TextContent = ContentFormatter.FormatContentAsHTML(PreprocessItemContent(entry.EntryId, entry.Content));
							// set the langauge for the content item
							atomContent.Lang = entry.Language;

							atomEntry.Content = atomContent;
						}
					}

					if (atomEntry.ModifiedUtc > feedModified)
						feedModified = atomEntry.ModifiedUtc;

					atomFeed.Entries.Add(atomEntry);
				}

				// set feed modified date to the most recent entry date
				atomFeed.ModifiedUtc = feedModified;
				cache.Insert(CacheKey,atomFeed,DateTime.Now.AddMinutes(5));
			}
			return atomFeed;
		}

        #endregion

        #region Facebook Instant Articles
        [WebMethod]
        [SoapDocumentMethod(ParameterStyle = SoapParameterStyle.Wrapped, Use = SoapBindingUse.Literal)]
        public string GetInstantArticleUrl()
        {
            return SiteUtilities.GetRssUrl(siteConfig);
        }

        [WebMethod]
        [SoapDocumentMethod(ParameterStyle = SoapParameterStyle.Wrapped, Use = SoapBindingUse.Literal)]
        [return: XmlAnyElement]
        public IARoot GetInstantArticle()
        {
            int maxDays = siteConfig.RssDayCount;
            int maxEntries = siteConfig.RssMainEntryCount;
            return GetInstantArticleCount(maxDays, maxEntries);
        }

        private IARoot GetInstantArticleCount(int maxDays, int maxEntries)
        {
            EntryCollection entries = null;
            //We only build the entries if blogcore doesn't exist and we'll need them later...
            if (dataService.GetLastEntryUpdate() == DateTime.MinValue)
            {
                entries = BuildEntries(null, maxDays, maxEntries);
            }

            //Try to get out as soon as possible with as little CPU as possible
            if (inASMX)
            {
                DateTime lastModified = SiteUtilities.GetLatestModifedEntryDateTime(dataService, entries);
                if (SiteUtilities.GetStatusNotModified(lastModified))
                    return null;
            }

            if (inASMX)
            {
                string referrer = Context.Request.UrlReferrer != null ? Context.Request.UrlReferrer.AbsoluteUri : "";
                if (ReferralBlackList.IsBlockedReferrer(referrer))
                {
                    if (siteConfig.EnableReferralUrlBlackList404s)
                    {
                        return null;
                    }
                }
                else
                {
                    loggingService.AddReferral(
                        new LogDataItem(
                        Context.Request.RawUrl,
                        referrer,
                        Context.Request.UserAgent,
                        Context.Request.UserHostName));
                }
            }

            //not-modified didn't work, do we have this in cache?
            string CacheKey = string.Format("InstantArticle:{0}:{1}",maxDays,maxEntries);
            IARoot documentRoot = cache[CacheKey] as IARoot;

            if (documentRoot == null) //we'll have to build it...
            {
                //However, if we made it this far, the not-modified check didn't work, and we may not have entries...
                if (entries == null)
                {
                    entries = BuildEntries(null, maxDays, maxEntries);
                }

                documentRoot = new IARoot();
                documentRoot.Namespaces.Add("content", "http://purl.org/rss/1.0/modules/content/");

                IAChannel ch = new IAChannel();

                ch.Title = siteConfig.Title;
                ch.Link = SiteUtilities.GetBaseUrl(siteConfig);

                if (siteConfig.Description == null || siteConfig.Description.Trim().Length == 0)
                {
                    ch.Description = siteConfig.Subtitle;
                }
                else
                {
                    ch.Description = siteConfig.Description;
                }

                ch.PubDate = DateTime.UtcNow.ToString();
                ch.LastBuildDate = DateTime.UtcNow.ToString(); 

                if (siteConfig.RssLanguage != null && siteConfig.RssLanguage.Length > 0)
                {
                    ch.Language = siteConfig.RssLanguage;
                }

                //generator
                ch.Docs = string.Empty;

                documentRoot.Channels.Add(ch);

                foreach (Entry entry in entries)
                {
                    if (entry.IsPublic == false || entry.Syndicated == false)
                    {
                        continue;
                    }

                    IAItem item = new IAItem();
                    List<XmlElement> anyElements = new List<XmlElement>();
                    XmlDocument xmlDoc = new XmlDocument();

                    item.Title = entry.Title;
                    item.PubDate = entry.CreatedUtc.ToString("R");

                    if (ch.LastBuildDate == null || ch.LastBuildDate.Length == 0)
                    {
                        ch.LastBuildDate = item.PubDate;
                    }

                    item.Link = SiteUtilities.GetPermaLinkUrl(siteConfig, (ITitledEntry)entry);
                    item.Guid = entry.EntryId;

                    if (!siteConfig.AlwaysIncludeContentInRSS &&
                        entry.Description != null &&
                        entry.Description.Trim().Length > 0)
                    {
                        item.Description = PreprocessItemContent(entry.EntryId, entry.Description);
                    }
                    else
                    {
                        item.Description = ContentFormatter.FormatContentAsHTML(PreprocessItemContent(entry.EntryId, entry.Content));
                    }

                    XmlElement contentEncoded = xmlDoc.CreateElement("content", "encoded", "http://purl.org/rss/1.0/modules/content/");
                    string encData = string.Format("<!doctype html>" +
                        "<html lang=\"en\" prefix=\"op: http://media.facebook.com/op#\">" +
                        "<head>" +
                        "<meta charset=\"utf-8\">" +
                        "<link rel=\"canonical\" href=\"{3}\">" +
                        "<meta property=\"op:markup_version\" content=\"v1.0\">" +
                        "</head>" +
                        "<body><article>" +
                        "<header>{0}</header>" +
                        "{1}" +
                        "<footer>{2}</footer>" +
                        "</article></body></html>",
                        entry.Title,
                        ContentFormatter.FormatContentAsHTML(PreprocessItemContent(entry.EntryId, entry.Content)),
                        string.Empty, item.Link);
                    XmlCDataSection cdata = xmlDoc.CreateCDataSection(encData);
                    contentEncoded.AppendChild(cdata);

                    anyElements.Add(contentEncoded);

                    item.anyElements = anyElements.ToArray();

                    ch.Items.Add(item);
                }
                cache.Insert(CacheKey, documentRoot, DateTime.Now.AddMinutes(5));
            }
            return documentRoot;
        }

        #endregion
    }
}
