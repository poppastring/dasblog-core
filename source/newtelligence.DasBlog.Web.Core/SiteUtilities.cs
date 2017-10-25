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

namespace newtelligence.DasBlog.Web.Core
{

    using System;
    using System.Collections;
    using System.Collections.Specialized;
    using System.Diagnostics;
    using System.IO;
    using System.Security;
    using System.Security.Principal;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Web;
    using System.Web.Hosting;
    using System.Web.UI.WebControls;
    using newtelligence.DasBlog.Runtime;
    using System.Collections.Generic;



    /// <summary>
    /// Summary description for Utils.
    /// </summary>
    public static class SiteUtilities
    {


        /// <summary>
        /// Maps a virtual path to a physical path, respects the mapping rules from Server.MapPath.
        /// </summary>
        /// <param name="virtualPath">The virtual path to map.</param>
        /// <returns>The physical Path.</returns>
        public static string MapPath(string virtualPath) {

            // HostingEnvironment.MapPath doesn't support mapping 
            // relative paths, null and empty strings, where Server.MapPath does.
            // So we special case for those.
            if (String.IsNullOrEmpty(virtualPath)) {

                return HostingEnvironment.ApplicationPhysicalPath;
            }

            if (virtualPath[0] != '/' && virtualPath[0] != '~') {
                virtualPath = "~/" + virtualPath;
            }

            return HostingEnvironment.MapPath(virtualPath);
        }


		public static string GetUserAgent()
		{
			string version = typeof(SiteUtilities).Assembly.GetName().Version.ToString();
			return "newtelligence dasBlog/" + version;
		}

        public static string RelativeToRoot(string relative)
        {
            return new Uri(new Uri(RootUrl()),relative).AbsoluteUri;
        }

        public static string RelativeToRoot(SiteConfig siteConfig, string relative)
        {
			Uri root = new Uri(RootUrl(siteConfig));
			string url = new Uri(root,relative).AbsoluteUri;
            return url;
        }

		public static bool ReferralFromSelf(SiteConfig siteConfig, string referral)
		{
			return ReferralFromSelf(referral, siteConfig.Root);
		}

		public static bool ReferralFromSelf(string referral, string baseUri)
		{
			baseUri = baseUri.Replace("www.", "");
			referral = referral.Replace("www.", "");

			if (referral.ToUpper().StartsWith(baseUri.ToUpper()))
			{
				return true;
			}
			
			return false;
		}

		public static string SpamBlocker(string email)
		{
			if (SiteConfig.GetSiteConfig().ObfuscateEmail)
			{
				email = Regex.Replace(email, "@", "AT NOSPAM");
				email = Regex.Replace(email, @"\.", " dot ");
			}
			return email;
		}
		
		public static string GetRsdUrl()
		{
            return GetRsdUrl(SiteConfig.GetSiteConfig());
		}
    	
		public static string GetRsdUrl(SiteConfig siteConfig)
		{
			return new Uri( new Uri(siteConfig.Root),"EditService.asmx/GetRsd").ToString();
		}

		public static string GetMicrosummaryUrl()
		{
			return GetMicrosummaryUrl(SiteConfig.GetSiteConfig());
		}

		public static string GetMicrosummaryUrl(SiteConfig siteConfig)
		{
			return new Uri( new Uri(siteConfig.Root),"Microsummary.ashx").ToString();
		}



        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static string GetRssUrl()
        {
            SiteConfig siteConfig = SiteConfig.GetSiteConfig();
            if (siteConfig.RSSEndPointRewrite != null && siteConfig.RSSEndPointRewrite.Trim().Length > 0)
            {
                Uri rss = new Uri(siteConfig.Root + siteConfig.RSSEndPointRewrite);
                return rss.AbsoluteUri;
            }

            return GetRssUrl(siteConfig);
        }

        public static string GetRssUrl(SiteConfig siteConfig)
        {
			if (siteConfig.FeedBurnerName != null && siteConfig.FeedBurnerName.Length >0)
			{
				return new Uri(new Uri("http://feeds.feedburner.com/"),siteConfig.FeedBurnerName).ToString();
			}
            return new Uri( new Uri(siteConfig.Root),"SyndicationService.asmx/GetRss").ToString();
        }

		/// <summary>
		/// Returns the Rss Url, where the default protocol "http" is replaced by the
		/// standard "feed" protocol
		/// </summary>
		/// <returns>a string like this: "feed://www.mysite.com/SyndicationService.asmx/GetRss"</returns>
		public static string GetFeedUrl() {
			return GetFeedUrl(SiteConfig.GetSiteConfig());
		}

		/// <summary>
		/// Returns the Rss Url, where the default protocol "http" is replaced by the
		/// standard "feed" protocol
		/// </summary>
		/// <param name="siteConfig">SiteConfig</param>
		/// <returns>a string like this: "feed://www.mysite.com/SyndicationService.asmx/GetRss"</returns>
		public static string GetFeedUrl(SiteConfig siteConfig) {
			if (siteConfig.FeedBurnerName != null && siteConfig.FeedBurnerName.Length >0)
			{
				return String.Concat("feed:", new Uri(new Uri("http://feeds.feedburner.com/"),siteConfig.FeedBurnerName));
			}

			string baseuri = siteConfig.Root;
			if (baseuri.StartsWith("https")) {
				return String.Concat("feed:", new Uri( new Uri(siteConfig.Root),"SyndicationService.asmx/GetRss").ToString());
			}	else {
				return String.Concat("feed:", new Uri( new Uri(siteConfig.Root),"SyndicationService.asmx/GetRss").ToString().Remove(0,5));
			}
		}

		public static string GetCdfUrl() {
			return GetCdfUrl(SiteConfig.GetSiteConfig());
		}
		
		public static string GetCdfUrl(SiteConfig siteConfig)
        {
            return new Uri( new Uri(siteConfig.Root),"cdf.ashx").ToString();
        }

        public static string GetCommentsRssUrl()
        {
            return GetCommentsRssUrl( SiteConfig.GetSiteConfig());
        }

        public static string GetEntryCommentsRssUrl(string id)
        {
            return GetEntryCommentsRssUrl( SiteConfig.GetSiteConfig(), id );
        }

        public static string GetCommentsRssUrl(SiteConfig siteConfig)
        {
            return new Uri( new Uri(siteConfig.Root),"SyndicationService.asmx/GetCommentsRss").ToString();
        }

        public static string GetEntryCommentsRssUrl(SiteConfig siteConfig, string id)
        {
            return new Uri( new Uri(siteConfig.Root),"SyndicationService.asmx/GetEntryCommentsRss?guid=" + id).ToString();
        }

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public static string GetBloggerUrl()
		{
            // TODO: Why is this calling GetAtomUrl() and not the overloaded GetBloggerUrl???
			return GetAtomUrl(SiteConfig.GetSiteConfig());
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public static string GetBloggerUrl(SiteConfig siteConfig)
		{
			return RelativeToRoot(siteConfig,"blogger.aspx");
		}
    	
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static string GetAtomUrl()
        {
            return GetAtomUrl(SiteConfig.GetSiteConfig());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static string GetAtomUrl(SiteConfig siteConfig)
        {
			if (siteConfig.FeedBurnerName != null && siteConfig.FeedBurnerName.Length >0)
			{
				return new Uri(new Uri("http://feeds.feedburner.com/"),siteConfig.FeedBurnerName).ToString();
			}
			return RelativeToRoot(siteConfig,"SyndicationService.asmx/GetAtom");
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="categoryName"></param>
        /// <returns></returns>
        public static string GetAtomCategoryUrl(string categoryName)
        {
            return GetAtomCategoryUrl(SiteConfig.GetSiteConfig(), categoryName );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="categoryName"></param>
        /// <returns></returns>
        public static string GetAtomCategoryUrl(SiteConfig siteConfig, string categoryName)
        {
            return RelativeToRoot(siteConfig, "SyndicationService.asmx/GetAtomCategory?categoryName="+HttpContext.Current.Server.UrlEncode(categoryName));
        }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="categoryName"></param>
		/// <returns></returns>
		public static string GetFeedCategoryUrl(string categoryName)
		{
			return GetRssCategoryUrl(SiteConfig.GetSiteConfig(), categoryName );
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="categoryName"></param>
		/// <returns></returns>
		public static string GetFeedCategoryUrl(SiteConfig siteConfig, string categoryName)
		{
			string baseuri = siteConfig.Root;
			if (baseuri.StartsWith("https")) 
			{
				return String.Concat("feed:", new Uri( new Uri(siteConfig.Root),"SyndicationService.asmx/GetRssCategory?categoryName="+HttpContext.Current.Server.UrlEncode(categoryName)).ToString());
			}	
			else 
			{
				return String.Concat("feed:", new Uri( new Uri(siteConfig.Root),"SyndicationService.asmx/GetRssCategory?categoryName="+HttpContext.Current.Server.UrlEncode(categoryName)).ToString().Remove(0,5));
			}
		}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="categoryName"></param>
        /// <returns></returns>
        public static string GetRssCategoryUrl(string categoryName)
        {
            return GetRssCategoryUrl(SiteConfig.GetSiteConfig(), categoryName );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="categoryName"></param>
        /// <returns></returns>
        public static string GetRssCategoryUrl(SiteConfig siteConfig, string categoryName)
        {
            return RelativeToRoot(siteConfig, "SyndicationService.asmx/GetRssCategory?categoryName="+HttpContext.Current.Server.UrlEncode(categoryName));
        }

        /// <summary>
        /// Gets the raw style permalink of a post. Does not return the <see cref="Entry.CompressedTitle"/> of an <see cref="Entry"/>.
        /// </summary>
        public static string GetPermaLinkUrl( string id )
        {
            return GetPermaLinkUrl(SiteConfig.GetSiteConfig(),id);
        }

		/// <summary>
		/// Gets the permalink of a post based on the <see cref="SiteConfig.EnableTitlePermaLink"/>. 
		/// Does not return the <see cref="Entry.CompressedTitle"/> of an <see cref="Entry"/>.
		/// </summary>
		public static string GetPermaLinkUrl(Entry entry)
		{	
			return GetPermaLinkUrl(SiteConfig.GetSiteConfig(), (ITitledEntry)entry);
		}

		/// <summary>
		/// Gets the compressed title URL.
		/// </summary>
		/// <param name="title">Title.</param>
		/// <returns></returns>
		public static string GetCompressedTitleUrl( string title )
		{
			return GetCompressedTitleUrl(SiteConfig.GetSiteConfig(),title);
		}


		public static string GetEnclosureLinkUrl( string id, Attachment attachment )
		{
			return GetEnclosureLinkUrl(SiteConfig.GetSiteConfig(),id, attachment);
		}

        public static string GetEditViewUrl( string id )
        {
            return GetEditViewUrl(SiteConfig.GetSiteConfig(),id);
        }

		public static string GetCommentViewUrl( string targetEntryId)
		{
			return GetCommentViewUrl(SiteConfig.GetSiteConfig(), targetEntryId, String.Empty );
		}
		
		public static string GetCommentViewUrl( string targetEntryId, string commentId )
        {
            return GetCommentViewUrl(SiteConfig.GetSiteConfig(), targetEntryId, commentId );
        }
		
		public static string GetCommentDeleteUrl( string targetEntryId, string commentId )
		{
			return GetCommentDeleteUrl(SiteConfig.GetSiteConfig(), targetEntryId, commentId );
		}

		public static string GetCommentApproveUrl( string targetEntryId, string commentId ){
			return GetCommentApproveUrl( SiteConfig.GetSiteConfig(), targetEntryId, commentId );
		}

        public static string GetTrackbackUrl( string id )
        {
            return GetTrackbackUrl( SiteConfig.GetSiteConfig(), id );
        }
    	
		public static string GetTrackbackDeleteUrl( string entryId, string referralPermalink, TrackingType type)
		{
			return GetTrackbackDeleteUrl( SiteConfig.GetSiteConfig(), entryId,  referralPermalink, type);
		}

        public static string GetBaseUrl()
        {
            return GetBaseUrl(SiteConfig.GetSiteConfig());
        }

        public static string GetStartPageUrl()
        {
            return GetStartPageUrl(SiteConfig.GetSiteConfig());
        }

		public static string GetAdminPageUrl()
		{
			return GetAdminPageUrl(SiteConfig.GetSiteConfig());
		}

        public static string GetDayViewUrl( DateTime day )
        {
            return GetDayViewUrl( SiteConfig.GetSiteConfig(), day );
        }

		public static string GetMonthViewUrl( DateTime month )
		{
			return GetMonthViewUrl( SiteConfig.GetSiteConfig(), month);
		}

		public static string GetPagedViewUrl(int page)
		{
			return GetPagedViewUrl(SiteConfig.GetSiteConfig(), page);
		}

		public static string GetCompressedTitleUrl( SiteConfig siteConfig, string title )
		{
			return siteConfig.Root + title + ".aspx";
		}

		/// <summary>
		/// Gets the raw style permalink of a post. Does not return the <see cref="Entry.CompressedTitle"/> of an <see cref="Entry"/>.
		/// </summary>
		public static string GetPermaLinkUrl( SiteConfig siteConfig, string id )
		{
			return LinkRewriter(siteConfig, RelativeToRoot(siteConfig, "PermaLink.aspx?guid="+id));
		}

		/// <summary>
		/// Gets the permalink of a post based on the <see cref="SiteConfig.EnableTitlePermaLink"/>. 
		/// Does not return the <see cref="Entry.CompressedTitle"/> of an <see cref="Entry"/>.
		/// </summary>
		[Obsolete("Please use the overload that takes a SiteConfig and an ITitledEntry")]
        public static string GetPermaLinkUrl( SiteConfig siteConfig, Entry entry )
        {
            return GetPermaLinkUrl(siteConfig, ((ITitledEntry)entry));
        }

        /// <summary>
        /// Gets the permalink of a post.
        /// </summary>
        /// <param name="siteConfig">The site config.</param>
        /// <param name="titledEntry">The titled entry.</param>
        /// <returns></returns>
        public static string GetPermaLinkUrl(SiteConfig siteConfig, ITitledEntry titledEntry)
        {
            if (siteConfig == null) throw new ArgumentNullException("siteConfig");
            if (titledEntry == null) throw new ArgumentNullException("titledEntry");

            string titlePermalink;

            if (siteConfig.EnableTitlePermaLink)
            {
                //replace spaces with nowt, or the configured space replacement string.
                string compressedTitle = (siteConfig.EnableTitlePermaLinkUnique) ? titledEntry.CompressedTitleUnique : titledEntry.CompressedTitle;
                string spaceReplacement = !siteConfig.EnableTitlePermaLinkSpaces ? "" : siteConfig.TitlePermalinkSpaceReplacement;
                titlePermalink = GetCompressedTitleUrl(siteConfig, compressedTitle.Replace("+", spaceReplacement));
            }
            else
            {
                titlePermalink = LinkRewriter(siteConfig, RelativeToRoot(siteConfig, "PermaLink.aspx?guid=" + titledEntry.EntryId));
            }

            if (siteConfig.ExtensionlessUrls == true)
            {
                titlePermalink = titlePermalink.Replace(".aspx", "");
            }

            return titlePermalink;
        }

        /// <summary>
		/// Gets the permalink of a post based on the <see cref="SiteConfig.EnableTitlePermaLink"/>. 
		/// Does not return the <see cref="Entry.CompressedTitle"/> of an <see cref="EntryIdCacheEntry"/>.
		/// </summary>
		[Obsolete("Please use the overload that takes a SiteConfig and an ITitledEntry")]
		public static string GetPermaLinkUrl( SiteConfig siteConfig, EntryIdCacheEntry e )
		{
            return GetPermaLinkUrl(siteConfig, ((ITitledEntry) e));
		}

		public static string GetEnclosureLinkUrl( SiteConfig siteConfig, string id, Attachment attachment )
		{
			if (attachment.Name.StartsWith("http"))
				return attachment.Name;
			else
				return LinkRewriter(siteConfig, RelativeToRoot(siteConfig, Path.Combine(Path.Combine(siteConfig.BinariesDirRelative, id), attachment.Name)));
		}

        public static string GetEditViewUrl( SiteConfig siteConfig, string id )
        {
            return RelativeToRoot(siteConfig, "EditEntry.aspx?guid="+id);
        }

		public static string GetCommentViewUrl( SiteConfig siteConfig, string entryId )
		{
			return LinkRewriter(siteConfig, RelativeToRoot(siteConfig, "CommentView.aspx?guid="+entryId));
		}
		
		public static string GetCommentViewUrl( SiteConfig siteConfig, string entryId, string commentId )
        {
            return LinkRewriter(siteConfig, RelativeToRoot(siteConfig, "CommentView.aspx?guid="+entryId),commentId);
        }
    	
		public static string GetTrackbackDeleteUrl( SiteConfig siteConfig, string entryId, string referralPermalink, TrackingType type)
		{
			return LinkRewriter(siteConfig, RelativeToRoot(siteConfig, "deleteItem.ashx?entryid=" +  entryId + "&referralPermalink=" + referralPermalink + "&type=" + type));
		}
    	
		public static string GetCommentDeleteUrl( SiteConfig siteConfig, string entryId, string commentId )
		{
			return LinkRewriter(siteConfig, RelativeToRoot(siteConfig, "deleteItem.ashx?entryid=" +  entryId + "&commentId=" + commentId));
		}

		public static string GetCommentApproveUrl( SiteConfig siteConfig, string entryId, string commentId )
		{
			return LinkRewriter( siteConfig, RelativeToRoot( siteConfig, "approveItem.ashx?entryid=" + entryId + "&commentId=" + commentId));
		}

		public static string GetCommentReportUrl( SiteConfig siteConfig, string entryId, string commentId )
		{
			return LinkRewriter(siteConfig, RelativeToRoot(siteConfig, "deleteItem.ashx?entryid=" +  entryId + "&commentId=" + commentId + "&report=true"));
		}

        public static string GetClickThroughUrl( SiteConfig siteConfig, string id )
        {
            return RelativeToRoot(siteConfig, "ct.ashx?id="+id);
        }

        public static string GetAggregatorBugUrl( SiteConfig siteConfig, string id )
        {
            return RelativeToRoot(siteConfig, "aggbug.ashx?id="+id);
        }

        public static string GetCrosspostTrackingUrlBase( SiteConfig siteConfig )
        {
            return RelativeToRoot(siteConfig, "cptrk.ashx?id=");
        }

        public static string GetTrackbackUrl( SiteConfig siteConfig, string id )
        {
			return RelativeToRoot(siteConfig, "Trackback.aspx?guid="+id);
        }

        public static string GetBaseUrl(SiteConfig siteConfig)
        {
            return RelativeToRoot(siteConfig, "");
        }

        public static string GetStartPageUrl(SiteConfig siteConfig)
        {
			if (SiteSecurity.IsInRole("admin"))
			{
				return GetAdminPageUrl(siteConfig);

			}
			return RelativeToRoot(siteConfig, "default.aspx");
        }

		public static string GetAdminPageUrl(SiteConfig siteConfig)
		{
			return RelativeToRoot(siteConfig, "default.aspx?page=admin");
		}

        public static string GetDayViewUrl( SiteConfig siteConfig, DateTime day )
        {
            return LinkRewriter(siteConfig, RelativeToRoot(siteConfig, "default.aspx?date=" + day.ToString("yyyy-MM-dd")));
        }

		public static string GetMonthViewUrl( SiteConfig siteConfig, DateTime month )
		{
			return LinkRewriter(siteConfig, RelativeToRoot(siteConfig, "default.aspx?month=" +  month.ToString("yyyy-MM")));
		}

		public static string GetPagedViewUrl(SiteConfig siteConfig, int page)
		{
			return LinkRewriter(siteConfig, RelativeToRoot(siteConfig, "default.aspx?page=" + page.ToString(System.Globalization.CultureInfo.InvariantCulture)));
		}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="categoryName"></param>
        /// <returns></returns>
        public static string GetCategoryViewUrl(string categoryName) {
            return GetCategoryViewUrl(SiteConfig.GetSiteConfig(), categoryName, 0);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="categoryName"></param>
        /// <param name="page">The page this link points to, or 0 (zero) when there is no paging.</param>
        /// <returns></returns>
        public static string GetCategoryViewUrl(string categoryName, int page) {
            return GetCategoryViewUrl(SiteConfig.GetSiteConfig(), categoryName, page);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="siteConfig"></param>
        /// <param name="categoryName"></param>
        /// <returns></returns>
        public static string GetCategoryViewUrl(SiteConfig siteConfig, string categoryName) {

            return GetCategoryViewUrl(siteConfig, categoryName, 0);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="siteConfig"></param>
        /// <param name="categoryName"></param>
        /// <param name="page">The page this link points to, or 0 (zero) when there is no paging.</param>
        /// <returns></returns>
        public static string GetCategoryViewUrl(SiteConfig siteConfig, string categoryName, int page) {

            // Moved the URLEncode after this, otherwise the pipe gets encoded to %7c, and
            // never gets converted to a comma!

            if (siteConfig.EnableUrlRewriting) {
                categoryName = categoryName.Replace("|", ",");
            }

            if (siteConfig.EnableUrlRewriting) {
                categoryName = Util.HttpHelper.GetURLSafeString(categoryName);
            }
            else {
                    categoryName = HttpContext.Current.Server.UrlEncode(categoryName);
            }

			string url = "";
			if (page > 0)
			{
				if (siteConfig.EnableUrlRewriting)
				{
					url = String.Format("CategoryView.aspx?category={0},{1}", categoryName, page);
				}
				else
				{
					url = String.Format("CategoryView.aspx?category={0}&page={1}", categoryName, page);
				}
			}
			else
			{
				url = String.Format("CategoryView.aspx?category={0}", categoryName);
			}

            return LinkRewriter(siteConfig, RelativeToRoot(siteConfig, url));
        }


		public static string GetUserViewUrl(string userName)
		{
			return GetUserViewUrl(SiteConfig.GetSiteConfig(), userName);
		}

		public static string GetUserViewUrl(SiteConfig siteConfig, string userName)
		{
			// escape + to %2B
			userName = userName.Replace("+", "%2B");

			if (siteConfig.EnableUrlRewriting)
			{
				userName = userName.Replace("|", ",");
			}
			else
			{
			}

			return LinkRewriter(siteConfig, RelativeToRoot(siteConfig, "UserView.aspx?user=" + userName));
		}

        public static string GetProfileViewUrl( SiteConfig siteConfig, string userName, bool profileMapping )
        {
            if ( profileMapping )
            {
                User   user    = SiteSecurity.GetUser( userName );
                string display = user.DisplayName.Replace( " ", "+" );
                return LinkRewriter( siteConfig, RelativeToRoot( siteConfig, string.Format( "Profiles/{0}.aspx", display ) ) );
            }
            else
            {
                return LinkRewriter( siteConfig, RelativeToRoot( siteConfig, "Profile.aspx?user=" + userName ));
            }

        }

        private static string RootUrl()
        {
            return RootUrl(SiteConfig.GetSiteConfig());

        }

        private static string RootUrl(SiteConfig siteConfig)
        {
            return siteConfig.Root;
        }
        

        public static string FilterContent( SiteConfig siteConfig, string entryId, string content )
        {
            content = ApplyContentFilters( siteConfig, content );
            content = ApplyClickThrough( siteConfig, entryId, content );
            return content;
        }

		public static HyperLink ParseSearchString(string referral)
		{
			if (referral == null || referral.Length == 0)
			{
				return null;
			}

			try
			{
				Uri url = new Uri(referral);

				string searchString = url.Query.TrimStart('?');

				string[] query = searchString.Split('&');
				Hashtable queryString = new Hashtable();
						
				foreach (string s in query)
				{
					string[] param = s.Split('=');
					if (param.Length == 2 && queryString.ContainsKey(param[0]) == false)
					{
						queryString.Add(param[0], param[1]);
					}
				}

				if (queryString.Keys.Count == 0)
				{
					return null;
				}

				string text = null;
				// Simple search.
				if (queryString.ContainsKey("q")) 
				{
					text = HttpUtility.UrlDecode(queryString["q"].ToString());
				}
				// Yahoo search.
				else if (queryString.ContainsKey("p"))
				{
					text = HttpUtility.UrlDecode(queryString["p"].ToString());
				}
				// Advanced Google search.
				else if (url.Host.IndexOf("google") != -1 && !queryString.ContainsKey("q"))
				{
					text = GetAdvancedGoogleQuery(queryString);
				}

				if (text != null)
				{
					HyperLink link = new HyperLink();
					link.Text = String.Format("{0} ({1})", System.Web.HttpUtility.HtmlEncode(text), url.Host);
					link.NavigateUrl = referral;
					return link;
				}

				return null;
			}
			catch (UriFormatException)
			{
				return null;
			}
		}

		private static string GetAdvancedGoogleQuery(Hashtable queryString)
		{
			StringBuilder advancedGoogleSearch = new StringBuilder();
				
			// Logical AND.
			if (queryString["as_q"] != null && queryString["as_q"].ToString().Length > 0)
			{
				advancedGoogleSearch.Append(HttpUtility.UrlDecode(String.Format("{0} ", queryString["as_q"].ToString())));
			}

			// Logical OR.
			if (queryString["as_oq"] != null && queryString["as_oq"].ToString().Length > 0)
			{
				bool insertOr = false;
				foreach(string fragment in HttpUtility.UrlDecode(queryString["as_oq"].ToString()).Split(' '))
				{
					if (fragment.Trim().Length > 0)
					{
						if (insertOr)
						{
							advancedGoogleSearch.Append("OR ");
						}

						advancedGoogleSearch.AppendFormat("{0} ", fragment);
						insertOr = true;
					}
				}
			}

			// Exact terms.
			if (queryString["as_epq"] != null && queryString["as_epq"].ToString().Length > 0)
			{
				advancedGoogleSearch.Append(HttpUtility.UrlDecode(String.Format("\"{0}\" ", queryString["as_epq"].ToString())));
			}

			// Exclude terms.
			if (queryString["as_eq"] != null && queryString["as_eq"].ToString().Length > 0)
			{
				foreach(string fragment in HttpUtility.UrlDecode(queryString["as_eq"].ToString()).Split(' '))
				{
					if (fragment.Trim().Length > 0)
					{
						advancedGoogleSearch.AppendFormat("-{0} ", fragment);
					}
				}
			}

			if (advancedGoogleSearch.Length == 0)
			{
				 return null;
			}

			return advancedGoogleSearch.ToString();
		}

		private static readonly Regex clickThroughHrefRegex = new Regex( "<a(?<prefix>[^>]*)href=\"?(?=http)(?<link>[^\"\\s>]*)(?<postfix>[^>]*)>",RegexOptions.IgnoreCase|RegexOptions.Singleline|RegexOptions.CultureInvariant|RegexOptions.Compiled);

		public static MatchCollection FindHyperLinks(string content)
		{
			return clickThroughHrefRegex.Matches(content);
		}

		public static string ApplyClickThrough( SiteConfig siteConfig, string entryId, string content )
        {
            if ( siteConfig.EnableClickThrough && content != null && !SiteSecurity.IsValidContributor())
            {
                StringBuilder contentSb = new StringBuilder();
                int currentIndex = 0;
                foreach( Match match in FindHyperLinks(content) )
                {
                    contentSb.Append(content.Substring(currentIndex,match.Index-currentIndex));
                    currentIndex = match.Index+match.Length;
                    if(!match.Groups["postfix"].Value.Equals("\""))
					{
                        // Patched with the {4} to avoid duplicated quotes at the end of the href
						contentSb.AppendFormat("<a{0} href=\"{1}&amp;url={2}{4}{3}>",
							match.Groups["prefix"].Value,GetClickThroughUrl(siteConfig, entryId),
							HttpUtility.UrlEncode(HttpUtility.HtmlDecode(match.Groups["link"].Value)),
                            match.Groups["postfix"].Value, 
                            (match.Groups["postfix"].Value.StartsWith("\"") ? "" : "\""));
					}
					else
					{

						contentSb.AppendFormat("<a{0} href=\"{1}&amp;url={2}\">",
							match.Groups["prefix"].Value,GetClickThroughUrl(siteConfig, entryId),
							HttpUtility.UrlEncode(HttpUtility.HtmlDecode(match.Groups["link"].Value)));
					}
                }
                contentSb.Append(content.Substring(currentIndex));
                content = contentSb.ToString();
            }
            return content;
        }

    	public static string ApplyContentFilters(SiteConfig siteConfig, string content)
    	{
    		if (content == null)
    		{
    			return String.Empty;
    		}

    		foreach (ContentFilter filter in siteConfig.ContentFilters)
    		{
    			string mapTo = filter.MapTo.Replace("~/", siteConfig.Root);

    			if (filter.IsRegEx)
    			{
    				try
    				{
    					content = Regex.Replace(content, filter.Expression, mapTo, RegexOptions.Singleline);
    				}
    				catch
    				{
    				}
    			}
    			else
    			{
    				content = content.Replace(filter.Expression, mapTo);
    			}
    		}
    		content = ApplyUrlEncoding(content);
    		return content;
    	}

		private static string ApplyUrlEncoding(string content)
		{
			const string urlEncodingGroupStart = "$url(";
			const string urlEncodingGroupEnd = ")";
			StringBuilder modifiedContent = new StringBuilder(content.Length);

			int endIndex = 0;
			int previousEndIndex = 0;
			int startIndex = content.IndexOf(urlEncodingGroupStart);
			while (startIndex >= 0)
			{
				endIndex = content.IndexOf(urlEncodingGroupEnd, startIndex);
				if (endIndex < 0) break;
				
				int beforeTextStart = previousEndIndex;
				int beforeTextLength = startIndex - beforeTextStart;
				modifiedContent.Append(content.Substring(beforeTextStart, beforeTextLength));
				
				int wrappedTextStart = startIndex + urlEncodingGroupStart.Length;
				int wrappedTextLength = endIndex - wrappedTextStart;
				
				modifiedContent.Append( HttpUtility.UrlEncode(content.Substring(wrappedTextStart, wrappedTextLength)) );

				previousEndIndex = endIndex + 1;
				startIndex = content.IndexOf(urlEncodingGroupStart, endIndex);
			}
			modifiedContent.Append(content.Substring(previousEndIndex));
			return modifiedContent.ToString();
		}

        public static string FilterContent( string entryId, string content )
        {
            return FilterContent( SiteConfig.GetSiteConfig(), entryId, content );
        }

		private static readonly Regex mapUrlRegEx = new Regex("\\{(?<expr>\\w+)\\}",RegexOptions.Compiled);


		/// <summary>
		/// Maps a re-written Url written using <see cref="LinkRewriter"/> to a normal PathAndQuery url.
		/// </summary>
		/// <param name="requestUrl">The re-written url</param>
		/// <returns>A normal url</returns>
		public static string MapUrl (string requestUrl)
		{
			NameValueCollection urlMaps = (NameValueCollection)System.Configuration.ConfigurationManager.GetSection(UrlMapperModuleSectionHandler.ConfigSectionName);
			if ( urlMaps != null )
			{
				for ( int loop=0;loop<urlMaps.Count;loop++)
				{
					string matchExpression = urlMaps.GetKey(loop);
					Regex regExpression = new Regex(matchExpression,RegexOptions.IgnoreCase|RegexOptions.Singleline|RegexOptions.CultureInvariant|RegexOptions.Compiled);
					Match matchUrl = regExpression.Match(requestUrl);
					if ( matchUrl != null && matchUrl.Success )
					{
						bool isCategoryQuery = false;

						string mapTo = urlMaps[matchExpression];
						foreach( Match matchExpr in mapUrlRegEx.Matches(mapTo) )
						{
							Group urlExpr;
							string expr = matchExpr.Groups["expr"].Value;
							urlExpr = matchUrl.Groups[expr];
							if ( urlExpr != null )
							{
								if (urlExpr.Value == "category")
								{
									isCategoryQuery = true;
								}
								if (expr == "value" & isCategoryQuery)
								{
									string categoryList = urlExpr.Value.Replace(",", "|");
									mapTo = mapTo.Replace("{"+expr+"}", categoryList);
								}
								else
								{
									mapTo = mapTo.Replace("{"+expr+"}", urlExpr.Value);
								}
									
							}
						}
						return mapTo;
					}
				}
			}

			return requestUrl;
		}

		private static string LinkRewriter(SiteConfig siteConfig, string link)
		{
			return LinkRewriter(siteConfig, link, string.Empty);
		}

		/// <summary>
		/// TSC: LinkRewriter rewrites an given link for search robots. 
		/// </summary>
		/// <param name="link"></param>
		/// <returns></returns>
		private static string LinkRewriter(SiteConfig siteConfig, string link, string bookmark)
		{
			string _urlRW = "LINKRW";

			if (siteConfig.EnableUrlRewriting )
			{
                NameValueCollection urlMaps = (NameValueCollection)System.Configuration.ConfigurationManager.GetSection(UrlMapperModuleSectionHandler.ConfigSectionName);
								
				if ( urlMaps != null )
				{
					for ( int loop=0;loop<urlMaps.Count;loop++)
					{
						string matchExpression = urlMaps.GetKey(loop);
						if (matchExpression.IndexOf(_urlRW)!=-1)
						{
							matchExpression = matchExpression.Replace(_urlRW, "");
							Regex regExpression = new Regex(matchExpression,RegexOptions.IgnoreCase|RegexOptions.Singleline|RegexOptions.CultureInvariant|RegexOptions.Compiled);
							Match matchUrl = regExpression.Match(link);
							if ( matchUrl != null && matchUrl.Success )
							{
								string mapTo = urlMaps[_urlRW+matchExpression];
								foreach( Match matchExpr in mapUrlRegEx.Matches(mapTo) )
								{
									Group urlExpr;
									string expr = matchExpr.Groups["expr"].Value;
									urlExpr = matchUrl.Groups[expr];
									if ( urlExpr != null )
									{
										mapTo = mapTo.Replace("{"+expr+"}",urlExpr.Value);
									}
								}
								link = mapTo;

								//Add any appended bookmarks/anchors
								if(bookmark != null && bookmark.Length > 0)
								{
									link += ("#" + bookmark);
								}
								break;
							}
						}
					}
				}
			}
			
			return link;
			
		}

		public static string ClipString( string text, int length )
		{
			return ClipString(text, length, "...");
		}
		
		public static string ClipString(string text, int length, string trailer)
        {
            if (text != null && text.Length > 0)
            {
                if (text.Length > length) 
                {
                    text = text.Substring(0, length) + trailer;
                }
            }
            else 
            {
                text = "";
            }
            return text;
        }

		public static string GetObfuscatedEmailUrl( string emailAddress ){

			  return GetObfuscatedEmailUrl( emailAddress, null );
		}

		public static string GetObfuscatedEmailUrl( string emailAddress, string subject ){

			string[] split = emailAddress.Split('@');

			string url;

			if (split.Length == 2) {
				string alias = split[0];
				string domain = split[1];

				if( subject == null || subject.Length == 0 ){
					url = String.Format("javascript:var e1='{2}',e2='{1}', e3='{0}';var e0=e2+e3+'%40'+e1;(window.location?window.location.replace(e0):document.write(e0)); ", 
						SiteUtilities.GetObfuscatedString(alias), 
						HttpUtility.HtmlEncode("mailto: "), 
						SiteUtilities.GetObfuscatedString(domain)
						);
				}else{
					url = String.Format("javascript:var e1='{2}',e2='{1}', e3='{0}';var e0=e2+e3+'%40'+e1+'?Subject={3}';(window.location?window.location.replace(e0):document.write(e0)); ", 
						SiteUtilities.GetObfuscatedString(alias), 
						HttpUtility.HtmlEncode("mailto: "), 
						SiteUtilities.GetObfuscatedString(domain),
						"Comments on: " + subject);
				}
			}
			else {
				if( subject == null || subject.Length == 0 ){
						url =String.Format("mailto:{0}", emailAddress);
				}else{
					url = String.Format("mailto:{0}?Subject={1}", emailAddress, subject);
				}
			}

			return url;
		}

		/// <summary>
		/// Obfuscates a string using HTML character references (hex) and character entities. This is usefull for hiding e-mail addresses from spammers.
		/// </summary>
		/// <param name="input">The string to obfuscate.</param>
		/// <returns>A string.</returns>
		/// <remarks>The resulant string has a mix of HTML character references, salted characters and character entities</remarks>
		public static string GetObfuscatedString(string input)
		{
			// Random randObj = new Random();
			System.Text.StringBuilder sb = new System.Text.StringBuilder();

			foreach (char c in input)
			{
				byte b = System.Convert.ToByte(c);
				sb.Append("%");
				sb.Append(b.ToString("x2"));

				/* This code (hex) causes problems in firefox
				int rand = randObj.Next(3);
					
				switch(rand)
				{
					case(0):
						sb.Append("&#");
						sb.Append(Convert.ToInt32(c));
						break;
					case(1):
						byte b = System.Convert.ToByte(c);
						sb.Append("%");
						sb.Append(b.ToString("x2"));
						break;
					default:
						sb.Append(c);
						break;
				}
				*/
			}
			return sb.ToString();
		}

		public static bool AreCommentsAllowed(Entry entry, SiteConfig siteConfig)
		{
			if (entry.AllowComments == false)
			{
				return false;
			}

			if (siteConfig.EnableCommentDays) 
			{

				int days = siteConfig.DaysCommentsAllowed;
				if (days > 0)
				{
					TimeSpan timeSpan = new TimeSpan(DateTime.Now.Ticks - entry.CreatedUtc.Ticks);

					if (timeSpan.Days > days)
					{
						return false;
					}
				}
			}

			return true;
		}

        public static bool IsAMPage()
        {
            return (SiteConfig.GetSiteConfig().AMPPagesEnabled && !String.IsNullOrWhiteSpace(HttpContext.Current.Request.QueryString["amp"]));
        }

		public static bool GetStatusNotModified(DateTime latest)
		{
			bool notModified = false;
			//SDH: This gets turned into UTC internally by the Framework when SetLastModified is called
			latest = (latest < DateTime.Now ? latest : DateTime.Now);

			// check the etag value first. if it matches then 
			// we send a 304. If not, check the if-modified-since
			string etag = HttpContext.Current.Request.Headers["If-None-Match"];
			if (etag != null)
			{
				notModified = (etag.Equals(latest.Ticks.ToString()));
			}
			else
			{
				string ifModifiedSince = HttpContext.Current.Request.Headers["if-modified-since"];
				
				if (ifModifiedSince != null)
				{					
					try 
					{
						// ifModifiedSince can have a legnth param in there
						// If-Modified-Since: Wed, 29 Dec 2004 18:34:27 GMT; length=126275
						if (ifModifiedSince.IndexOf(";")> -1)
						{
							ifModifiedSince = ifModifiedSince.Split(';').GetValue(0).ToString();	
						}

						DateTime ifModDate = DateTime.Parse(ifModifiedSince);
						
						notModified = (latest <= ifModDate);
					}
					catch(FormatException e)
					{
						ErrorTrace.Trace(TraceLevel.Error, e.ToString() );
					}
					catch(IndexOutOfRangeException e)
					{
						ErrorTrace.Trace(TraceLevel.Error, e.ToString() );
					}
					catch(Exception e)
					{
						ErrorTrace.Trace(TraceLevel.Error, e.ToString() );
					}
				}
			}

			// since nothing has changed since the last
			// request for this feed, we send an HTTP 304
			// to the client
			if (notModified)
			{
				HttpContext.Current.Response.StatusCode = 304;
				HttpContext.Current.Response.SuppressContent = true;
				return true;
			}

			// Either no one used if-modified-since or we have
			// new data.  Record the last modified time and
			// etag in the http header for next time.
			HttpContext.Current.Response.Cache.SetCacheability(HttpCacheability.Public);
			HttpContext.Current.Response.Cache.SetLastModified(latest);
			HttpContext.Current.Response.Cache.SetETag(latest.Ticks.ToString());

			return false;
		}

		public static DateTime GetLatestModifedEntryDateTime(IBlogDataService dataService, EntryCollection entries)
		{
			// Check to see if we should send a HTTP 304 letting the RSS client
			// know that they have the latest version of the feed
			DateTime latest = DateTime.MinValue;

			// need to check to see if the last entry value doesn't exist
			// if it doesn't loop through posts to get the latest date
			if (dataService.GetLastEntryUpdate() == DateTime.MinValue)
			{
				foreach (Entry entry in entries)
				{
					DateTime created = entry.ModifiedLocalTime;
					if (created > latest) latest = created;
				}
			}
			else
			{
				latest = dataService.GetLastEntryUpdate().ToLocalTime();
			}

			// we need to check to see if a comment entry has occured
			// after the last entry update. If it has don't return 304
            DateTime latestComment = dataService.GetLastCommentUpdate();
			if ( ( latestComment != DateTime.MinValue ) && ( latest < latestComment.ToLocalTime() ) )
				latest = latestComment.ToLocalTime();

			return new DateTime(latest.Year, latest.Month, latest.Day, latest.Hour, latest.Minute, latest.Second);
		}

		public static DateTime GetLatestModifedCommentDateTime(IBlogDataService dataService, CommentCollection comments)
		{
			// Check to see if we should send a HTTP 304 letting the RSS client
			// know that they have the latest version of the feed
			DateTime latest = DateTime.MinValue;

			// need to check to see if the last entry value doesn't exist
			// if it doesn't loop through posts to get the latest date
			if (dataService.GetLastCommentUpdate() == DateTime.MinValue)
			{
				foreach (Comment comment in comments)
				{
					DateTime created = comment.ModifiedLocalTime;
					if (created > latest) latest = created;
				}
			}
			else
			{
				latest = dataService.GetLastCommentUpdate().ToLocalTime();
			}

			// we need to check to see if an entry has occured
			// after the last comment update. If it has don't return 304
			if (latest < dataService.GetLastEntryUpdate().ToLocalTime())
				latest = dataService.GetLastEntryUpdate().ToLocalTime();

			return new DateTime(latest.Year, latest.Month, latest.Day, latest.Hour, latest.Minute, latest.Second);
		}

		public static string FilterHtml( string input, ValidTagCollection allowedTags ){

			// no tags allowed so just html encode
			if( allowedTags == null || allowedTags.Count == 0 ){
				return HttpUtility.HtmlEncode( input );
			}

			// check for matches
			MatchCollection matches = htmlFilterRegex.Matches( input );

			// no matches, normal encoding
			if( matches.Count == 0 ){
				return HttpUtility.HtmlEncode( input );
			}

			StringBuilder sb = new StringBuilder();

			MatchedTagCollection collection = new MatchedTagCollection( allowedTags );
			collection.Init( matches);
			
			int inputIndex = 0;

			foreach( MatchedTag tag in collection ){

				// add the normal text between the current index and the index of the current tag
				if( inputIndex < tag.Index ){
					sb.Append(HttpUtility.HtmlEncode(input.Substring(inputIndex, tag.Index - inputIndex)));
				}

				// add the filtered value
				sb.Append( tag.FilteredValue );

				// move the current index past the tag
				inputIndex = tag.Index + tag.Length;
			}

			// add remainder
			if( inputIndex < input.Length ){
				sb.Append( HttpUtility.HtmlEncode(input.Substring( inputIndex)) );
			}

			return sb.ToString();
		}

		// static so we have max benefit from the compiled option
		private static Regex htmlFilterRegex = new Regex("<(?<end>/)?(?<name>\\w+)((\\s+(?<attNameValue>(?<attName>\\w+)(\\s*=\\s*(?:\"(?<attVal>[^\"]*)\"|'(?<attVal>[^']*)'|(?<attVal>[^'\">\\s]+)))?))+\\s*|\\s*)(?<self>/)?>", 
			RegexOptions.CultureInvariant | RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Compiled );



        //TODO: The following methods should be moved

        private static bool CanEdit(IPrincipal user, Entry entry)
        {
           // 
            
            
            // admins can edit all
            if (user.IsInRole("admin"))
            {
                return true;
            }

            // contributors can only edit their own
            if (user.IsInRole("contributor") && String.Compare(user.Identity.Name, entry.Author,StringComparison.Ordinal) == 0)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Deletes an entry, including comments enclosures etc.
        /// </summary>
        /// <remarks>Admins can delete all, contributors only their own.</remarks>
        /// <param name="entryId">The entry to delete.</param>
        /// <param name="siteConfig"></param>
        /// <param name="logService"></param>
        /// <param name="dataService"></param>
        public static void DeleteEntry(string entryId, SiteConfig siteConfig, ILoggingDataService logService, IBlogDataService dataService)
        {
            try
            {
                IPrincipal user = HttpContext.Current.User;
                Entry entry = dataService.GetEntry(entryId);
                //fix: admins can delete all, contributors only their own
                if (!CanEdit(user, entry))
                {
                    throw new SecurityException("Current user is not allowed to delete this entry!");
                }

                string permalink = SiteUtilities.GetPermaLinkUrl(entry.EntryId);
                //string[] categories = entry.GetSplitCategories();

                dataService.DeleteEntry(entryId, siteConfig.CrosspostSites);

                BreakCache(siteConfig, entry.GetSplitCategories());

                // give the XSS upstreamer a hint that things have changed
                //FIX:    XSSUpstreamer.TriggerUpstreaming();

                // TODO: when we add support for more than just enclosures, we can't delete the entire folder
                DirectoryInfo enclosuresPath = new DirectoryInfo((Path.Combine(SiteConfig.GetBinariesPathFromCurrentContext(), entryId)));
                if (enclosuresPath.Exists) enclosuresPath.Delete(true);

                logService.AddEvent(
                    new EventDataItem(
                    EventCodes.EntryDeleted, entry.Title,
                    permalink));
            }
            catch (SecurityException)
            {
                throw;
            }
            catch (Exception ex)
            {
                StackTrace st = new StackTrace();
                logService.AddEvent(
                    new EventDataItem(EventCodes.Error, ex.ToString() + Environment.NewLine + st.ToString(), HttpContext.Current.Request.RawUrl));

                // DESIGN: We should rethrow the exception for the calling class, but this break too much for this point release.
            }
        }

        public static void SaveEntry(Entry entry, SiteConfig siteConfig, ILoggingDataService logService, IBlogDataService dataService)
        {
            SaveEntry(entry, string.Empty, null, siteConfig, logService, dataService);
        }

        public static void SaveEntry(Entry entry, CrosspostInfoCollection crosspostList, SiteConfig siteConfig, ILoggingDataService logService, IBlogDataService dataService)
        {
            SaveEntry(entry, string.Empty, crosspostList, siteConfig, logService, dataService);
        }

        public static void SaveEntry(Entry entry, TrackbackInfoCollection trackbackList, SiteConfig siteConfig, ILoggingDataService logService, IBlogDataService dataService)
        {
            InternalSaveEntry(entry, trackbackList, null, siteConfig, logService, dataService);

            logService.AddEvent(
                new EventDataItem(
                EventCodes.EntryAdded, entry.Title,
                SiteUtilities.GetPermaLinkUrl(siteConfig, entry.EntryId)));
        }

        public static void SaveEntry(Entry entry, string trackbackUrl, CrosspostInfoCollection crosspostList, SiteConfig siteConfig, ILoggingDataService logService, IBlogDataService dataService)
        {
            TrackbackInfoCollection trackbackList = null;
            if (trackbackUrl != null && trackbackUrl.Length > 0)
            {
                trackbackList = new TrackbackInfoCollection();
                trackbackList.Add(new TrackbackInfo(
                    trackbackUrl,
                    SiteUtilities.GetPermaLinkUrl(siteConfig, (ITitledEntry)entry),
                    entry.Title,
                    entry.Description,
                    siteConfig.Title));
            }
            InternalSaveEntry(entry, trackbackList, crosspostList, siteConfig, logService, dataService);

            logService.AddEvent(
                new EventDataItem(
                EventCodes.EntryAdded, entry.Title,
                SiteUtilities.GetPermaLinkUrl(siteConfig, entry.EntryId)));
        }

        public static void SaveEntry(Entry entry, TrackbackInfoCollection trackbackList, CrosspostInfoCollection crosspostList, SiteConfig siteConfig, ILoggingDataService logService, IBlogDataService dataService)
        {
            InternalSaveEntry(entry, trackbackList, crosspostList, siteConfig, logService, dataService);

            logService.AddEvent(
                new EventDataItem(
                    EventCodes.EntryAdded, entry.Title,
                    SiteUtilities.GetPermaLinkUrl(siteConfig, entry.EntryId)));
        }

        public static EntrySaveState UpdateEntry(Entry entry, string trackbackUrl, CrosspostInfoCollection crosspostList, SiteConfig siteConfig, ILoggingDataService logService, IBlogDataService dataService)
        {
            EntrySaveState rtn = EntrySaveState.Failed;

            entry.ModifiedLocalTime = DateTime.Now;

            TrackbackInfoCollection trackbackList = null;
            if (trackbackUrl != null && trackbackUrl.Length > 0)
            {
                trackbackList = new TrackbackInfoCollection();
                trackbackList.Add(new TrackbackInfo(
                    trackbackUrl,
                    SiteUtilities.GetPermaLinkUrl(siteConfig, (ITitledEntry)entry),
                    entry.Title,
                    entry.Description,
                    siteConfig.Title));
            }

            rtn = InternalSaveEntry(entry, trackbackList, crosspostList, siteConfig, logService, dataService);

            logService.AddEvent(
                new EventDataItem(
                    EventCodes.EntryChanged, entry.Title,
                    SiteUtilities.GetPermaLinkUrl(entry.EntryId)));

            return rtn;
        }

        private static EntrySaveState InternalSaveEntry(Entry entry, TrackbackInfoCollection trackbackList, CrosspostInfoCollection crosspostList, SiteConfig siteConfig, ILoggingDataService logService, IBlogDataService dataService)
        {

            EntrySaveState rtn = EntrySaveState.Failed;
            // we want to prepopulate the cross post collection with the crosspost footer
            if (siteConfig.EnableCrossPostFooter && siteConfig.CrossPostFooter != null && siteConfig.CrossPostFooter.Length > 0)
            {
                foreach (CrosspostInfo info in crosspostList)
                {
                    info.CrossPostFooter = siteConfig.CrossPostFooter;
                }
            }

            // now save the entry, passign in all the necessary Trackback and Pingback info.
            try
            {
                // if the post is missing a title don't publish it
                if (entry.Title == null || entry.Title.Length == 0)
                {
                    entry.IsPublic = false;
                }

                // if the post is missing categories, then set the categories to empty string.
                if (entry.Categories == null)
                    entry.Categories = "";

                rtn = dataService.SaveEntry(
                    entry,
                    (siteConfig.PingServices.Count > 0) ?
                        new WeblogUpdatePingInfo(siteConfig.Title, SiteUtilities.GetBaseUrl(siteConfig), SiteUtilities.GetBaseUrl(siteConfig), SiteUtilities.GetRssUrl(siteConfig), siteConfig.PingServices) : null,
                    (entry.IsPublic) ?
                        trackbackList : null,
                    siteConfig.EnableAutoPingback && entry.IsPublic ?
                        new PingbackInfo(
                            SiteUtilities.GetPermaLinkUrl(siteConfig, (ITitledEntry)entry),
                            entry.Title,
                            entry.Description,
                            siteConfig.Title) : null,
                    crosspostList);

                SendEmail(entry, siteConfig, logService);

            }
            catch (Exception ex)
            {
                StackTrace st = new StackTrace();
                logService.AddEvent(
                    new EventDataItem(EventCodes.Error, ex.ToString() + Environment.NewLine + st.ToString(), ""));
            }

            // we want to invalidate all the caches so users get the new post
            BreakCache(siteConfig, entry.GetSplitCategories());

            // give the XSS upstreamer a hint that things have changed
            //FIX:  XSSUpstreamer.TriggerUpstreaming();

            return rtn;
        }

        private static void BreakCache(SiteConfig siteConfig, string[] categories)
        {
            DataCache cache = CacheFactory.GetCache();

            // break the caching
            cache.Remove("BlogCoreData");
            cache.Remove("Rss::" + siteConfig.RssDayCount.ToString() + ":" + siteConfig.RssEntryCount.ToString());

            foreach (string category in categories)
            {
                string CacheKey = "Rss:" + category + ":" + siteConfig.RssDayCount.ToString() + ":" + siteConfig.RssEntryCount.ToString();
                cache.Remove(CacheKey);
            }
        }

        /// <summary>
        /// Send an email notification that an entry has been made.
        /// </summary>
        /// <param name="siteConfig">The page making the request.</param>
        /// <param name="entry">The entry being added.</param>
        internal static void SendEmail(Entry entry, SiteConfig siteConfig, ILoggingDataService logService)
        {
            if (siteConfig.SendPostsByEmail &&
                siteConfig.SmtpServer != null &&
                siteConfig.SmtpServer.Length > 0)
            {
                List<object> actions = new List<object>();
                actions.Add(ComposeMail(entry, siteConfig));

                foreach (User user in SiteSecurity.GetSecurity().Users)
                {
                    if (user.EmailAddress == null || user.EmailAddress.Length == 0)
                        continue;

                    if (user.NotifyOnNewPost)
                    {
                        SendMailInfo sendMailInfo = ComposeMail(entry, siteConfig);
                        sendMailInfo.Message.To.Add(user.EmailAddress);
                        actions.Add(sendMailInfo);
                    }
                }

                IBlogDataService dataService = BlogDataServiceFactory.GetService(HttpContext.Current.Server.MapPath(siteConfig.ContentDir), logService);
                dataService.RunActions(actions.ToArray());
            }
        }

        private static SendMailInfo ComposeMail(Entry entry, SiteConfig siteConfig)
        {
            System.Net.Mail.MailMessage emailMessage = new System.Net.Mail.MailMessage();

            if (siteConfig.NotificationEMailAddress != null &&
                siteConfig.NotificationEMailAddress.Length > 0)
            {
                emailMessage.To.Add(siteConfig.NotificationEMailAddress);
            }
            else
            {
                emailMessage.To.Add(siteConfig.Contact);
            }

            emailMessage.Subject = String.Format("Weblog post by '{0}' on '{1}'", entry.Author, entry.Title);
            emailMessage.Body = String.Format("{0}<p>Post page: <a href=\"{1}\">{1}</a></p>", entry.Content, SiteUtilities.GetPermaLinkUrl(siteConfig, (ITitledEntry)entry));
            emailMessage.IsBodyHtml = true;
            emailMessage.BodyEncoding = Encoding.UTF8;

            User entryAuthor = SiteSecurity.GetUser(entry.Author);

            //didn't work? try again with emailAddress...
            if (entryAuthor == null)
            {
                entryAuthor = SiteSecurity.GetUserByEmail(entry.Author);
            }

            if (entryAuthor != null && entryAuthor.EmailAddress != null && entryAuthor.EmailAddress.Length > 0)
            {
                emailMessage.From = new System.Net.Mail.MailAddress(entryAuthor.EmailAddress);
            }
            else
            {
                emailMessage.From = new System.Net.Mail.MailAddress(siteConfig.Contact);
            }

            SendMailInfo sendMailInfo = new SendMailInfo(emailMessage, siteConfig.SmtpServer,
                siteConfig.EnableSmtpAuthentication, siteConfig.UseSSLForSMTP, siteConfig.SmtpUserName, siteConfig.SmtpPassword, siteConfig.SmtpPort);

            // Christoph De Baene: Put in comments, because mail is sent through IBlogDataService where the EnableSmtpAuthentication is handled there.
            // RyanG: Added support for authentication on outgoing smtp
            /*
            if (sendMailInfo.EnableSmtpAuthentication)
            {
                sendMailInfo.Message.Fields.Add("http://schemas.microsoft.com/cdo/configuration/smtpauthenticate", "1");	// enable basic authentication
                sendMailInfo.Message.Fields.Add("http://schemas.microsoft.com/cdo/configuration/sendusername", sendMailInfo.SmtpUserName);	// set the username
                sendMailInfo.Message.Fields.Add("http://schemas.microsoft.com/cdo/configuration/sendpassword", sendMailInfo.SmtpPassword);  // set the password
            }
            */

            return sendMailInfo;
        }
	}
}
