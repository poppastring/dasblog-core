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


using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net.Mail;
using System.Reflection;
using System.Resources;
using System.Runtime.Serialization;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Caching;
using System.Web.UI;
using System.Web.UI.WebControls;
using newtelligence.DasBlog.Runtime;
using NodaTime;

namespace newtelligence.DasBlog.Web.Core
{
    class DaySorter : IComparer<DayEntry>
    {
        public int Compare(DayEntry left, DayEntry right)
        {
            return right.DateUtc.CompareTo(left.DateUtc);
        }
    }

    class EntrySorter : IComparer<Entry>
    {
        public int Compare(Entry left, Entry right)
        {
            return right.CreatedUtc.CompareTo(left.CreatedUtc);
        }
    }

    /// <summary>
	/// Use this attribute to mark all protected or public
	/// fields that your want to auto-serialize into 
	/// session state and which are scoped to the current
	/// conversation only.
	/// </summary>
	[AttributeUsage(AttributeTargets.Field)]
	public class TransientPageStateAttribute : Attribute
	{
		/// <summary>
		/// Enables auto-serialization of this field into
		/// conversation (page scoped session) state. The 
		/// data type must be serializable.
		/// </summary>
		public TransientPageStateAttribute()
		{
		}
	}

	/// <summary>
	/// Use this attribute to mark all protected or public
	/// fields that your want to auto-serialize into 
	/// session state and which are scoped to the session.
	/// </summary>
	[AttributeUsage(AttributeTargets.Field)]
	public class SessionPageStateAttribute : Attribute
	{
		internal string keyName = null;

		/// <summary>
		/// Enables auto-serialization of this field into
		/// session state (session scope). The data type 
		/// must be serializable.
		/// </summary>
		public SessionPageStateAttribute()
		{
		}

		/// <summary>
		/// Enables auto-serialization of this field into
		/// session state (session scope). The data type 
		/// must be serializable.
		/// </summary>
		/// <param name="KeyName">A site-unique key that enables 
		/// sharing of this state element value across pages</param>
		/// <remarks>
		/// This variant of this attribute enables declarative 
		/// sharing of session state between pages. Works with
		/// Redirects and Transfers.
		/// </remarks>
		public SessionPageStateAttribute(string KeyName)
		{
			keyName = KeyName;
		}
	}

	/// <summary>
	/// Use this attribute to mark all protected or public
	/// fields that your want to auto-serialize into 
	/// a permanent cookie and which are scope across
	/// sessions.
	/// </summary>
	[AttributeUsage(AttributeTargets.Field)]
	public class PersistentPageStateAttribute : Attribute
	{
		internal string keyName = null;

		/// <summary>
		/// Enables auto-serialization of this field into
		/// a persistent state cookie (user, page scope). 
		/// The data type must be serializable.
		/// </summary>
		public PersistentPageStateAttribute()
		{
		}

		/// <summary>
		/// Enables auto-serialization of this field into
		/// a persistent state cookie (user scope). The data type 
		/// must be serializable.
		/// </summary>
		/// <param name="KeyName">A site-unique key that enables 
		/// sharing of this state element value across pages
		/// and sessions</param>
		/// <remarks>
		/// This variant of this attribute enables declarative 
		/// sharing of session state between pages and across visits. 
		/// Works with Redirects and Transfers.
		/// </remarks>
		public PersistentPageStateAttribute(string KeyName)
		{
			keyName = KeyName;
		}

	}

	/// <summary>
	/// This is the base class for all ASP.NET pages in dasBlog. It implements
	/// common services and context information that most controls and macros access. 
	/// </summary>
	public class SharedBasePage : System.Web.UI.Page
    {
		private const string NOTMODIFIEDITEMKEY = "NotModifiedSent";
		private IBlogDataService dataService;
        private ILoggingDataService loggingService;
        private DataCache dataCache;
        private EventHandler PreRenderHandler = null;
        private const string keyPrefix = "__$stateManagingPages";
        protected string categoryName="Frontpage";
        protected string weblogEntryId="";
        protected DateTime _dayUtc;
		protected DateTime _month = DateTime.MinValue;
        internal BlogTheme blogTheme = null;
        [SessionPageState("userTheme")]
        public string userTheme="";
        protected Macros macros;
        private SiteConfig siteConfig;
        protected newtelligence.DasBlog.Runtime.EntryCollection entries;
        protected bool showTrackingDetail=false;
        protected Uri urlReferrer;
        protected bool errorHandlingOff = false;
        protected bool isAggregatedView = true;
        protected bool hideAdminTools = false;
        protected CultureInfo userCulture;
        protected static ResourceManager coreStringTables = new ResourceManager("newtelligence.DasBlog.Web.Core.StringTables.StringTables", typeof(SharedBasePage).Assembly);
        protected string titleOverride;
	    private string __weblogCalender = "weblogCalendar";
		private int _pageIndex;
		
		public enum TextDirection
		{
			LeftToRight,
			RightToLeft
		}

		private TextDirection readingDirection = TextDirection.LeftToRight;
		/// <summary>
		/// The Reading Direction based on the current language
		/// http://www.microsoft.com/globaldev/DrIntl/columns/017/default.mspx
		/// </summary>
		public TextDirection ReadingDirection
		{
			get { return this.readingDirection; }
		}

		/// <summary>
		/// TitleOverride will append a page specific
		/// TITLE depending on context.
		/// <seealso cref="Macros.SiteName"/>
		/// </summary>
		/// 
		public string TitleOverride
		{
			set
			{
				titleOverride = value;
			}
			get
			{
				return titleOverride;
			}
		}

        /// <summary>
        /// This expression is shared by all instances and precompiled
        /// </summary>
        private static readonly Regex findBodyTag = new Regex("<body(?:\\s*(\\w|:)+\\s*=\\s*(?:\"(?:(?!>)[^\"]*)\"|(?:(?!>)\\S+)))*\\s*>", RegexOptions.Compiled|RegexOptions.IgnoreCase);

        /// <summary>
        /// Public constructor. Sets up events.
        /// </summary>
        public SharedBasePage()
        {
            Init += new EventHandler(this.SessionLoad);
            Init += new EventHandler(this.SetupPage);
        }


        /// <summary>
        /// Page setup handler. Called by the Init event. 
        /// </summary>
        /// <param name="o">ignored</param>
        /// <param name="e">ignored</param>
        private void SetupPage(object o, EventArgs e)
        {
            Load += new EventHandler(this.Page_Load);
            PreRender += PreRenderHandler = new EventHandler(this.SessionStore);
            if ( !errorHandlingOff )
            {
                Error += new EventHandler(SharedBasePageErrorHandler);
            }

            // Obsolete
            //SmartNavigation = false;
            MaintainScrollPositionOnPostBack = false;

            siteConfig = SiteConfig.GetSiteConfig();
            loggingService = LoggingDataServiceFactory.GetService(SiteUtilities.MapPath(siteConfig.LogDir));
            dataService = BlogDataServiceFactory.GetService(SiteUtilities.MapPath(siteConfig.ContentDir), loggingService);

            Uri binaryRootUrl = new Uri(new Uri(SiteUtilities.GetBaseUrl(siteConfig)), siteConfig.BinariesDirRelative);

            BinaryDataService = BinaryDataServiceFactory.GetService(SiteUtilities.MapPath(siteConfig.BinariesDir), binaryRootUrl ,loggingService);
            dataCache = CacheFactory.GetCache();
            
			DayUtc = DateTime.UtcNow.AddDays(siteConfig.ContentLookaheadDays);

            // if the user sends an Accept-Language header, we grab the most
            // preferred language (culture) and make that the default culture
            // for the page. 
            if ( siteConfig.UseUserCulture && Request.UserLanguages != null && Request.UserLanguages.Length > 0)
            {
                try
                {
                    // The Accept-Language header's elements are defined as 
                    // <language-code>[;q=<quality>]. We're not interested in the 
                    // quality part and go by order and hence we cut off the part
                    // after and including the semicolon.
                    userCulture = System.Globalization.CultureInfo.CreateSpecificCulture(Request.UserLanguages[0].Split(';')[0]);
                }
                catch
                {
                    // if the culture isn't installed, we fall back to the invariant culture.
                    userCulture = System.Globalization.CultureInfo.InvariantCulture;
                }
            }
            else
            {
                userCulture = System.Globalization.CultureInfo.InvariantCulture;
            }
            System.Threading.Thread.CurrentThread.CurrentCulture = 
                System.Threading.Thread.CurrentThread.CurrentUICulture = userCulture;
                                       
                        
            macros = InitializeMacros();

            if ( Request.QueryString["category"] != null )
            {
                CategoryName = Request.QueryString["category"];
				TitleOverride = CategoryName;
            }
            if (Request.QueryString["date"] != null)
            {
                try
                {
                    DayUtc = DateTime.ParseExact(Request.QueryString["date"], "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
                    TitleOverride = DayUtc.ToLongDateString();
                }
                catch
                {
                }
            }
            else if (Request.QueryString["month"] != null)
            {
                try
                {
                    Month = DateTime.ParseExact(Request.QueryString["month"], "yyyy-MM", System.Globalization.CultureInfo.InvariantCulture);
                    TitleOverride = Month.ToString("MMMM, yyyy");
                }
                catch
                {
                }
            }
            else if (Request.QueryString["page"] != null)
            {
                try
                {
                    PageIndex = Int32.Parse(Request.QueryString["page"], System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture);
                }
                catch
                {
                }
            }

            // TSC: we are looking for an submit from the calendar control, but we will only
            // do something if the default page was submited
            else if (Request.Path.ToLower().IndexOf("default") != -1 &&
                Request.Params["__EVENTTARGET"] != null &&
                Request.Params["__EVENTTARGET"] != string.Empty &&
                Request.Params["__EVENTTARGET"].IndexOf(__weblogCalender) != -1 &&
                Request.Params["__EVENTARGUMENT"] != null &&
                Request.Params["__EVENTARGUMENT"] != string.Empty)
            {
                string _mDate = Request.Params["__EVENTARGUMENT"].Replace("V", "");
                // the initial time for the calendar control counting
                DateTime d1 = new System.DateTime(2000, 01, 01, 0, 0, 0, 0);
                // build an time span to the end of month (mostly)
                System.TimeSpan duration = new System.TimeSpan(Convert.ToInt32(_mDate) + 30, 0, 0, 0);
                d1 = d1.Add(duration);

                _mDate = new StringBuilder(d1.Year.ToString())
                    .Append("-")
                    .Append(d1.Month < 10 ? "0" + d1.Month.ToString() : d1.Month.ToString())
                    .Append("-")
                    .Append(d1.Day < 10 ? "0" + d1.Day.ToString() : d1.Day.ToString()).ToString();

                try
                {
                    DayUtc = DateTime.ParseExact(_mDate, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
                    TitleOverride = DayUtc.ToLongDateString();
                }
                catch
                {
                    // supress
                }
            }

			if ( Request.QueryString["title"] != null )
			{
				Entry entry = null;
				// check to see if we have the date
				if (Request.QueryString["date"] != null)
				{
					DateTime date = DateTime.ParseExact(Request.QueryString["date"],"yyyy-MM-dd",System.Globalization.CultureInfo.InvariantCulture);
					DayEntry dayEntry = dataService.GetDayEntry(date);
					entry = dayEntry.GetEntryByTitle(HttpUtility.UrlEncode(Request.QueryString["title"]));
				}
				else
				{
					entry = dataService.GetEntry(Request.QueryString["title"] );
				}
				
				if ( entry != null )
				{
					WeblogEntryId = entry.EntryId;
					// set page title
					TitleOverride = entry.Title;
				}
				else
				{
					// is very obviously invalid 
					WeblogEntryId = "";
				}
			}
			if ( Request.QueryString["guid"] != null )
            {
				WeblogEntryId = Request.QueryString["guid"];
				Entry entry = DataService.GetEntry(WeblogEntryId);
                if ( entry != null )
                {
                    // set page title
                    TitleOverride = entry.Title;
                }
                else
                {
                    // is very obviously invalid 
                    WeblogEntryId = "";
                }
            }
            if ( Request.QueryString["external_referrer"] != null )
            {
                try
                {
                    urlReferrer = new Uri(Request.QueryString["external_referrer"]);
                }
                catch
                {
                    // absorb
                }
            }

            // OmarS: handle BlogX style Permalink and Category
            // BlogX expects a permalink such as PermaLink.aspx/GUID
            // where dasBlog wants PermaLink.aspx?guid=GUID
            if (Request.QueryString.Count == 0 & Request.PathInfo.Length > 0)
            {
                string filePath = Request.FilePath.ToLower();

				if (filePath.EndsWith("permalink.aspx"))
				{
					WeblogEntryId = Request.PathInfo.Substring(1).ToUpper();
					this.Redirect(SiteUtilities.GetPermaLinkUrl(WeblogEntryId));
				}
				else if (filePath.EndsWith("categoryview.aspx"))
				{
					CategoryName = Request.PathInfo.Substring(1).ToUpper();
					this.Redirect(SiteUtilities.GetCategoryViewUrl(CategoryName));
				}
				else if (filePath.EndsWith("commentview.aspx"))
				{
					WeblogEntryId = Request.PathInfo.Substring(1).ToUpper();
					this.Redirect(SiteUtilities.GetCommentViewUrl(WeblogEntryId));
				}
            }
        }

        /// <summary>
        /// The error handler logs the current exception to the event log and 
        /// redirects to the error page.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SharedBasePageErrorHandler(object sender, EventArgs e )
        {
			Exception pageException = Server.GetLastError();
            try
            {
                loggingService.AddEvent(
                    new EventDataItem(EventCodes.Error,
                    pageException.ToString().Replace("\n","<br />"),
                    this.Page.Request.Url.ToString()));
                //SDH: Per the ASP.NET team, Trace.Fail is dangerous and the default implementation has been 
				// known to throw a DIALOG BOX and block threads in IIS/ASP.NET.
				//System.Diagnostics.Trace.Fail(this.Context.Error.Message);
                //SDH: Requires FullTrust
				//System.Diagnostics.Debug.WriteLine(this.Context.Error.Message);
            }
            catch
            {
            }

			// we need to handle this exception seperatley because if there are r/w errors
			// the exception will be System.ArgumentNullException and FormatPage.aspx won't work
			// we we redirect to a static page
			System.ArgumentNullException argumentNullException = new ArgumentNullException();
			System.UnauthorizedAccessException unauthorizedAccessException = new UnauthorizedAccessException();
			if (pageException.GetType() == argumentNullException.GetType() |
				pageException.GetType() == unauthorizedAccessException.GetType())
			{
				Response.Redirect("SiteConfig/setuperror.html",true);
			}
			else
			{
				Response.Redirect("FormatPage.aspx?path=SiteConfig/pageerror.format.html",true);
			}
        }

        
        /// <summary>
        /// This method initializes the default macro set.
        /// </summary>
        /// <returns></returns>
        protected virtual Macros InitializeMacros()
        {
            return MacrosFactory.CreateMacrosInstance(this);
        }

        /// <summary>
        /// Returns the URL for a "themed" image based on its image name.
        /// The resolution is done in the following order:
        /// <list type="number">
        ///    <item>
        ///       <description>
        ///         Lookup in the current theme's configured image list. 
        ///         The returned URL references the image whose name attribute 
        ///         matches the image name
        ///       </description>
        ///    </item>
        ///    <item>
        ///       <description>
        ///         Lookup in the current theme directory using the image name
        ///         with an appended extension ".gif"
        ///       </description>
        ///    </item>
        ///    <item>
        ///       <description>
        ///         Lookup in the site's "image" directory using the image name
        ///         with an appended extension ".gif"
        ///       </description>
        ///    </item>
        /// </list>
        /// Currently, the following image names are used:
        /// <list type="bullet">
        ///    <item>
        ///      <term>addbutton-list</term>
        ///      <description>An image symbolizing adding an item to a list. The default image is a plus-sign.</description>
        ///    </item>
        ///    <item>
        ///      <term>editbutton-list</term>
        ///      <description>An image symbolizing editing an item in a list. The default image is a pen.</description>
        ///    </item>
        ///    <item>
        ///      <term>deletebutton-list</term>
        ///      <description>An image symbolizing deleting an item in a list. The default image is an "x" shape.</description>
        ///    </item>
        ///    <item>
        ///      <term>okbutton-list</term>
        ///      <description>An image symbolizing saving an item in a list. The default image is a checkmark "hook"</description>
        ///    </item>
        ///    <item>
        ///      <term>undobutton-list</term>
        ///      <description>An image symbolizing undoing editing item in a list. The default image is a circular arrow</description>
        ///    </item>
        ///    <item>
        ///      <term>outlinearrow</term>
        ///      <description>An image serving as symbol for a collapsed hierachy in an outline view. The default image is an arrowhead pointing to the right.</description>
        ///    </item>
        ///    <item>
        ///      <term>outlinedown</term>
        ///      <description>An image serving as symbol for a expanded hierachy in an outline view. The default image is an arrowhead pointing down.</description>
        ///    </item>
        ///    <item>
        ///      <term>editbutton</term>
        ///      <description>An image symbolizing editing an item. The default image is a pen.</description>
        ///    </item>
        ///    <item>
        ///      <term>deletebutton</term>
        ///      <description>An image symbolizing deleting an item. The default image is an "x" shape</description>
        ///    </item>
        ///    <item>
        ///      <term>itemLink</term>
        ///      <description>An image symbolizing the permanent link for an item. The default image is a pound sign '#'</description>
        ///    </item>
        ///    <item>
        ///      <term>dayLink</term>
        ///      <description>An image symbolizing the link for a calendar day. The default image is a stylized calendar page.</description>
        ///    </item>
        ///    <item>
        ///      <term>mailTo</term>
        ///      <description>An image symbolizing a mail-to action. The default image is an mail envelope.</description>
        ///    </item>
        ///    <item>
        ///      <term>xmlButton</term>
        ///      <description>An image symbolizing a link to an XML file.</description>
        ///    </item>
        ///    <item>
        ///      <term>opmlButton</term>
        ///      <description>An image symbolizing a link to an OPML file.</description>
        ///    </item>
        ///    <item>
        ///      <term>atomButton</term>
        ///      <description>An image symbolizing a link to an Atom file.</description>
        ///    </item>
        ///    <item>
        ///      <term>rssButton</term>
        ///      <description>An image symbolizing a link to an RSS file.</description>
        ///    </item>
        /// </list>
        /// </summary>
        /// <param name="imageName"></param>
        /// <returns></returns>
        public string GetThemedImageUrl( string imageName )
        {
            string imageFileName = BlogTheme.ImageList[imageName] as string;
            if ( imageFileName == null )
            {
                imageFileName = imageName + ".gif";
            }

            string themedImagePath = Path.Combine(BlogTheme.ImageDirectory,imageFileName);
            if ( File.Exists( SiteUtilities.MapPath(themedImagePath ) ))
            {
				// OmarS: macintosh browsers cannot deal woth a \ as a path seperator, so replace with a /
				themedImagePath = themedImagePath.Replace("\\", "/");
				themedImagePath = new Uri(new Uri(SiteUtilities.GetBaseUrl(this.siteConfig)), themedImagePath).ToString();
                return themedImagePath;
            }
            else
            {
                // if that wasn't found, we fall back to the default location
				themedImagePath = "images/"+imageFileName;
				themedImagePath = new Uri(new Uri(SiteUtilities.GetBaseUrl(this.siteConfig)), themedImagePath).ToString();
                return themedImagePath;
            }
        }


        public virtual string GetPageTemplate(string path)
        {
            return GetHomeTemplate(path);
        }

        public virtual string GetHomeTemplate(string path)
        {
            string templateString="";
            using ( TextReader sr = BlogTheme.OpenHomeTemplate(path, CategoryName ) )
            {
                templateString = sr.ReadToEnd();
            }
            return templateString;
        }

        public virtual string GetDayTemplate()
        {
            string templateString;
            string path = Path.Combine(Path.GetPathRoot(Request.PhysicalPath),Path.GetDirectoryName(Request.PhysicalPath));
            using ( TextReader sr = BlogTheme.OpenDayTemplate(path, CategoryName ) )
            {
                templateString = sr.ReadToEnd();
            }
            return templateString;
        }

        public virtual string GetItemTemplate()
        {
            string templateString;
            string path = Path.Combine(Path.GetPathRoot(Request.PhysicalPath),Path.GetDirectoryName(Request.PhysicalPath));
            using ( TextReader sr = BlogTheme.OpenItemTemplate(path, CategoryName ) )
            {
                templateString = sr.ReadToEnd();
            }
            return templateString;
        }

        private static readonly Regex hrefRegEx = new Regex("href=\"themes", RegexOptions.IgnoreCase|RegexOptions.Compiled);

        
        /// <summary>
        /// This method invokes template processing and, when done, injects the required ASP.NET form
        /// into the resulting HTML stream.
        /// </summary>
        public virtual void ProcessTemplate()
        {
            TemplateProcessor templateProcessor = new TemplateProcessor();
            string path = Request.PhysicalApplicationPath;
            string templateString = GetPageTemplate(path);
			
            Match match = findBodyTag.Match(templateString);
            if ( match.Success )
            {
                // this section splits the template into a header, body and footer section
                // (all above and including <body>, everything between <body></body> and all below and including </body>
                int indexBody = templateString.IndexOf("</body>");
                if ( indexBody == -1 )
                {
                    indexBody = templateString.IndexOf("</BODY>");
                }

				// the header template contains everything above and including the body tag
				string headerTemplate=templateString.Substring(0,match.Index+match.Length);
				
				// insert necessary headtags and fix stylesheet relative links
				// fix any relative css link tags
				headerTemplate = hrefRegEx.Replace(headerTemplate, String.Format("href=\"{0}themes", SiteUtilities.GetBaseUrl()));

				string baseTag = String.Format("<base href=\"{0}\"></base>\r\n", SiteUtilities.GetBaseUrl());
				string linkTag = String.Format("<link rel=\"alternate\" type=\"application/rss+xml\" title=\"{2}\" href=\"{0}\" />\r\n<link rel=\"alternate\" type=\"application/atom+xml\" title=\"{2}\" href=\"{1}\" />\r\n", SiteUtilities.GetRssUrl(),SiteUtilities.GetAtomUrl(),HttpUtility.HtmlEncode(siteConfig.Title));
				string rsdTag = String.Format("<link rel=\"EditURI\" type=\"application/rsd+xml\" title=\"RSD\" href=\"{0}\" />", SiteUtilities.GetRsdUrl());
				string microsummaryTag = String.Format("<link rel=\"microsummary\" type=\"application/x.microsummary+xml\" href=\"{0}\" />",SiteUtilities.GetMicrosummaryUrl());

				int indexHead = headerTemplate.IndexOf("</head>");
				if ( indexHead == -1 )
				{
					indexHead = headerTemplate.IndexOf("</HEAD>");
				}

                if (!SiteUtilities.IsAMPage())
                {
                    headerTemplate = headerTemplate.Insert(indexHead, baseTag + linkTag + rsdTag + microsummaryTag + Seo.CreateSeoMetaInformation(this.WeblogEntries, this.dataService));
                }
                else
                {
                    headerTemplate = headerTemplate.Insert(indexHead, Seo.CreateAMPSeoMetaInformation(this.WeblogEntries, this.dataService));
                }

				// therefore it must close with a closing angle bracket, but it's better to check 
				if ( headerTemplate[headerTemplate.Length-1] == '>' )
				{
					// if that's so, we want to inject the reading order designator if we're right-to-left
					// or it's explicitly specified
					string pageReadingDirection = coreStringTables.GetString("page_reading_direction");
					if ( pageReadingDirection != null && pageReadingDirection.Length > 0 )
					{
						if (pageReadingDirection == "RTL") this.readingDirection = TextDirection.RightToLeft; 
						headerTemplate = headerTemplate.Substring(0, headerTemplate.Length-1) + " dir=\"" + pageReadingDirection + "\">";
					}
				}
                
                string bodyTemplate,footerTemplate;
                if( indexBody != -1 )
                {
                    bodyTemplate=templateString.Substring(match.Index+match.Length,indexBody-(match.Index+match.Length));
                    footerTemplate=templateString.Substring(indexBody);
                }
                else
                {
                    bodyTemplate=templateString.Substring(match.Index+match.Length);
                    footerTemplate="";
                }
						
                // now we process the header and attach the results to the content place holder
                templateProcessor.ProcessTemplate( this, headerTemplate, ContentPlaceHolder, macros );

                // once that's done, we create a form to wrap the body content and append that
                // to the place holder as well,
                // and we add an id to the form, so we are able to referencing to this form.
                if (!SiteUtilities.IsAMPage())
                {
                    BaseHtmlForm mainForm = new BaseHtmlForm();
                    mainForm.ID = "mainForm";
                    ContentPlaceHolder.Controls.Add(mainForm);
                    templateProcessor.ProcessTemplate(this, bodyTemplate, mainForm, macros);
                }
                else
                {
                    templateProcessor.ProcessTemplate(this, bodyTemplate, ContentPlaceHolder, macros);
                }

                // and finally the footer
                if ( footerTemplate.Length > 0 )
                {
                    templateProcessor.ProcessTemplate( this, footerTemplate, ContentPlaceHolder, macros );
                }
            }
            else
            {
                // if the page is just an unrecognizable mess of tags, process in one shot.
                templateProcessor.ProcessTemplate( this, templateString, ContentPlaceHolder, macros );
            }
			
        }

        /// <summary>
        /// This method is used by controls to insert xhtml tags into the page head tag.
        /// </summary>
        /// <param name="htmlHeadContent">String containing valid xhtml tags.</param>
        public void InsertInPageHeader(string htmlHeadContent)
        {
            if (ContentPlaceHolder.HasControls())
            {
                foreach (Control c in ContentPlaceHolder.Controls)
                {
                    if (c is LiteralControl)
                    {
                        LiteralControl lc = c as LiteralControl;
                        int indexHead = lc.Text.IndexOf("</head>");
                        if (indexHead == -1)
                        {
                            indexHead = lc.Text.IndexOf("</HEAD>");
                        }
                        if (indexHead > -1)
                        {
                            lc.Text = lc.Text.Insert(indexHead, "\r\n" + htmlHeadContent + "\r\n");
                            break;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// This method processes the day template
        /// </summary>
        /// <param name="day"></param>
        /// <param name="ContentPlaceHolder"></param>
        public virtual void ProcessDayTemplate( DateTime day, Control ContentPlaceHolder )
        {
            TemplateProcessor templateProcessor = new TemplateProcessor();
            templateProcessor.ProcessTemplate(this, GetDayTemplate(), ContentPlaceHolder, new DayMacros(this, day));
        }

        /// <summary>
        /// This method processes the item template.
        /// </summary>
        /// <param name="item"></param>
        /// <param name="ContentPlaceHolder"></param>
        public virtual void ProcessItemTemplate( newtelligence.DasBlog.Runtime.Entry item, Control ContentPlaceHolder )
        {
			TemplateProcessor templateProcessor = new TemplateProcessor();
            // the tenplate string is prefixed with an invisible bookmark anchor tag that can be used
            // for cross-references on the same page. All bookmarks take the form "a"+entryId

            // Check if this is a regular template or an AMP template...
            string templateString = String.Empty;
            templateString = String.Format("<a name=\"a{0}\"></a>{1}", item.EntryId, GetItemTemplate());
			templateProcessor.ProcessTemplate( this, item, templateString, ContentPlaceHolder, new ItemMacros( this, item ) );
        }


        /// <summary>
        /// Loads the entries for the page. This is the default behavior for the
        /// default (start) page and is to be overridden in other pages.
        /// </summary>
        /// <returns></returns>
        protected virtual EntryCollection LoadEntries()
        {
            string languageFilter = Request.Headers["Accept-Language"];

            if ( SiteSecurity.IsInRole("admin") )
            {
                languageFilter = "";
            }

			if(PageIndex > 0)
			{
                // Incorrect
				return GetEntriesForPage(PageIndex, languageFilter);
			}

			if (Month == DateTime.MinValue) 
			{
				// if we adjust for time zones (we don't show everything in UTC), we get the entries
				// using the configured time zone.
				if ( SiteConfig.AdjustDisplayTimeZone )
				{
					return GetEntriesForDay( DayUtc, DateTimeZone.Utc, languageFilter, siteConfig.FrontPageDayCount, siteConfig.FrontPageEntryCount );
				}
				else
				{
					return GetEntriesForDay( DayUtc, DateTimeZone.Utc, languageFilter, siteConfig.FrontPageDayCount, siteConfig.FrontPageEntryCount );
				}
			}
			else 
			{
				// if we adjust for time zones (we don't show everything in UTC), we get the entries
				// using the configured time zone.
				if ( SiteConfig.AdjustDisplayTimeZone )
				{
					return GetEntriesForMonth( Month, DateTimeZone.Utc, languageFilter);
				}
				else
				{
					return GetEntriesForMonth( Month, DateTimeZone.Utc, languageFilter);
				}
			}
              
        }

        public virtual EntryCollection GetFrontPageEntries()
        {
            DateTime fpDayUtc;
            string languageFilter = Request.Headers["Accept-Language"];

            if ( !HideAdminTools && SiteSecurity.IsInRole("admin") )
            {
                languageFilter = "";
            }

            // the front page contains all the newest entries
			fpDayUtc = DateTime.UtcNow.AddDays(SiteConfig.ContentLookaheadDays);

            if ( siteConfig.AdjustDisplayTimeZone )
            {
                return GetEntriesForDay( fpDayUtc, DateTimeZone.Utc, languageFilter , siteConfig.FrontPageDayCount, siteConfig.FrontPageEntryCount );
            }
            else
            {
                return GetEntriesForDay( fpDayUtc, DateTimeZone.Utc, languageFilter, siteConfig.FrontPageDayCount, siteConfig.FrontPageEntryCount );
            }
            
        }

        private EntryCollection GetEntriesForDay(DateTime startUtc, DateTimeZone tz, string langCode, int maxDays, int maxEntries)
        {
            string categoryFilter;

            categoryFilter = siteConfig.FrontPageCategory;
            if ( !HideAdminTools && SiteSecurity.IsInRole("admin") )
            {
                categoryFilter = "";
            }

            return dataService.GetEntriesForDay( startUtc, tz, langCode, maxDays, maxEntries, categoryFilter );
        }

		private EntryCollection GetEntriesForMonth(DateTime month, DateTimeZone tz, string langCode) 
		{
			return DataService.GetEntriesForMonth(month, tz, langCode);
		}

		private EntryCollection GetEntriesForPage(int pageIndex, string langCode)
        {
            string category = this.SiteConfig.FrontPageCategory;

            // prepare the predicate
            Predicate<Entry> pred = null;
            if (!string.IsNullOrEmpty(langCode))
            {
                pred = (e) => string.IsNullOrEmpty(e.Language) || String.Compare(e.Language, langCode, StringComparison.OrdinalIgnoreCase) == 0;
            }
            if (!string.IsNullOrEmpty(category))
            {
                if (pred == null)
                {
                    pred = (e) => e.Categories.Contains(category);
                }
                else
                {
                    pred += (e) => e.Categories.Contains(category);
                }
            }

            //Shallow copy as we're going to modify it...and we don't want to modify THE cache.
            EntryCollection cache = this.DataService.GetEntries(null, pred, Int32.MaxValue, Int32.MaxValue);

            // remove the posts from the front page
            EntryCollection fp = this.GetFrontPageEntries();
            
            cache.RemoveRange(0, fp.Count);

            int entriesPerPage = this.siteConfig.EntriesPerPage;

            // compensate for frontpage
			if (( pageIndex -1) * entriesPerPage < cache.Count)
			{
				// Remove all entries before the current page's first entry.
                int end = (pageIndex - 1) * entriesPerPage;
                cache.RemoveRange(0, end);

				// Remove all entries after the page's last entry.
				if (cache.Count - entriesPerPage > 0)
				{
					cache.RemoveRange(entriesPerPage, cache.Count - entriesPerPage);
                    // should match
                    bool postCount = cache.Count <= entriesPerPage;
				}

				return DataService.GetEntries(null, EntryCollectionFilter.DefaultFilters.IsInEntryIdCacheEntryCollection(cache),
					Int32.MaxValue,
					Int32.MaxValue);
			}

            // The page index is out of range (i.e. too large).
			return new EntryCollection();
        }

        /// <summary>
        /// This property exposes the loaded entry collection.
        /// </summary>
        public newtelligence.DasBlog.Runtime.EntryCollection WeblogEntries
        {
            get
            {
                return entries;
            }
        }

        
		private void ValidatePageAndAddAdminJavascript()
		{
			if (!this.HideAdminTools && SiteSecurity.IsValidContributor())
			{
				// add the javascript to allow deletion of the entry
				string scriptString;

				if(!Page.ClientScript.IsClientScriptBlockRegistered(this.GetType(),"deleteEntryScript"))
				{
                    scriptString = "<script type=\"text/javascript\" language=\"JavaScript\">\n";
                    scriptString += "//<![CDATA[\n";
                    scriptString += "function deleteEntry(entryId, entryTitle)\n";
                    scriptString += "{\n";
                    scriptString += String.Format("	if(confirm(\"{0} \\n\\n\" + entryTitle))\n", this.CoreStringTables.GetString("text_delete_confirm"));
                    scriptString += "	{\n";
                    scriptString += "		location.href=\"deleteItem.ashx?entryid=\" +  entryId \n";
                    scriptString += "	}\n";
                    scriptString += "}\n";
                    scriptString += "//]]>\n";
                    scriptString += "</script>";


					Page.ClientScript.RegisterClientScriptBlock(this.GetType(),"deleteEntryScript", scriptString);
				}

				if(!Page.ClientScript.IsClientScriptBlockRegistered(this.GetType(),"deleteReferralScript"))
				{
				    // add the javascript to allow deletion of trackback/referral items
                    scriptString = "<script type=\"text/javascript\" language=\"JavaScript\">\n";
                    scriptString += "//<![CDATA[\n";
				    scriptString += "function deleteReferral(entryId, referralPermalink, type)\n";
				    scriptString += "{\n";
				    scriptString += String.Format("	if(confirm(\"{0} \\n\\n\" + referralPermalink))\n", this.CoreStringTables.GetString("text_delete_confirm"));
				    scriptString += "	{\n";
                    scriptString += "		location.href=\"deleteItem.ashx?entryid=\" +  entryId + \"&referralPermalink=\" + escape(referralPermalink) + \"&type=\" + type\n";
				    scriptString += "	}\n";
                    scriptString += "}\n";
                    scriptString += "//]]>\n";
				    scriptString += "</script>";

					Page.ClientScript.RegisterClientScriptBlock(this.GetType(),"deleteReferralScript", scriptString);
				}
			}

			// add the javascript for showing a filtered referral list if we are on a page showing trackings
			if ((this.showTrackingDetail) && !Page.ClientScript.IsClientScriptBlockRegistered(this.GetType(), "showReferralScript"))
			{
				string referralScript = "<script type=\"text/javascript\" language=\"JavaScript\">\n<!--\n";
				referralScript += "function showReferral()\n";
				referralScript += "{\n";
				referralScript += "var elems = document.getElementsByTagName('*');\n";
				referralScript += "var count = 0;\n";
				referralScript += "for (var i=0;i<elems.length;i++) {\n";
				referralScript += "    if ( elems[i].id.indexOf('referralSpanHidden') != -1 ) {\n";
				referralScript += "        elems[i].style.display='inline';\n";
				referralScript += "        count++;\n";
				referralScript += "    }\n";
				referralScript += "    else if ( elems[i].id.indexOf('referralMore') != -1 ) {\n";
				referralScript += "        elems[i].style.display='none';\n";
				referralScript += "        count++;\n";
				referralScript += "    }\n";
				referralScript += "    if (count == 2) {break;}\n";
				referralScript += "}\n";
				referralScript += "}\n// -->";
				referralScript += "</script>";
                               
				Page.ClientScript.RegisterClientScriptBlock(this.GetType(),"showReferralScript", referralScript);
				
			}
		}
		private bool BlacklistedReferrer()
		{
			bool isBlacklisted = false;
			// bail if we are getting a local referral from our website
			if (UrlReferrer!= null && SiteUtilities.ReferralFromSelf(SiteConfig, UrlReferrer.ToString()) == false)
			{
				string referrer = UrlReferrer.AbsoluteUri;

				bool permalinkPageRequested = (WeblogEntryId.Length != 0);
				if (permalinkPageRequested)
				{
					Entry entry = dataService.GetEntry(WeblogEntryId);

					if (entry != null)
					{
						if (ReferralBlackList.IsBlockedReferrer(referrer, entry.Title))
						{
							isBlacklisted = true;
						}
						else
						{
							// Log an ItemReferralReceived event.
							loggingService.AddEvent(new EventDataItem(EventCodes.ItemReferralReceived,
								Context.Request.UserHostAddress,
								SiteUtilities.GetPermaLinkUrl(entry),
								referrer,
								entry.Title));

							// Add tracking for entry.
							AddTracking(entry);
						}
					}
				}
				else // This was a request for a non-permalink page
				{
					if (ReferralBlackList.IsBlockedReferrer(referrer))
					{
						isBlacklisted = true;
					}
					else
					{
						// Log an ReferralReceived event.
						loggingService.AddEvent(new EventDataItem(EventCodes.ReferralReceived,
							Context.Request.UserHostAddress,
							null,
							referrer));
					}
				}

				// Log if the referrer was not blocked.
				if (!isBlacklisted)
				{
					// Log referrer.
					loggingService.AddReferral(
						new LogDataItem(
						Context.Request.RawUrl,
						referrer,
						Context.Request.UserAgent,
						Context.Request.UserHostName));
				}
			}

			return isBlacklisted;
		}

		public void AddTracking(Entry entry)
		{
			// Do not log referrals from online aggregators such as bloglines and newsgator.
			if (Request.UserAgent != null && Request.UserAgent.IndexOf("subscriber") == -1)
			{
				Tracking t = new Tracking();
				t.PermaLink = Request.UrlReferrer.AbsoluteUri;
				t.TrackingType = TrackingType.Referral;
				t.TargetEntryId = entry.EntryId;
				t.TargetTitle = entry.Title;
				
				if ( siteConfig.SendReferralsByEmail &&
					siteConfig.SmtpServer != null && siteConfig.SmtpServer.Length > 0 )
				{
					MailMessage emailMessage = new MailMessage();
					if ( siteConfig.NotificationEMailAddress != null && 
						siteConfig.NotificationEMailAddress.Length > 0 )
					{
						emailMessage.To.Add(siteConfig.NotificationEMailAddress);
					}
					else
					{
						emailMessage.To.Add(siteConfig.Contact);
					}
					emailMessage.Subject = String.Format("Weblog referral by '{0}' on '{1}'", t.PermaLink, t.TargetTitle);
					emailMessage.Body = String.Format("You got a referral from\n{0}\r\non your weblog entry '{1}'\n({2})", t.PermaLink, t.TargetTitle, SiteUtilities.GetPermaLinkUrl(entry));
					emailMessage.IsBodyHtml = false; // .BodyFormat = MailFormat.Text;
					emailMessage.BodyEncoding = System.Text.Encoding.UTF8;
					emailMessage.From = new MailAddress(siteConfig.Contact);
					SendMailInfo sendMailInfo = new SendMailInfo(emailMessage, siteConfig.SmtpServer,
						siteConfig.EnableSmtpAuthentication, siteConfig.UseSSLForSMTP, siteConfig.SmtpUserName, siteConfig.SmtpPassword, siteConfig.SmtpPort);
						
					if (siteConfig.EnableEntryReferrals)
					{
						dataService.AddTracking(t, sendMailInfo);
					}
				}
				else
				{
					if (siteConfig.EnableEntryReferrals)
					{
						dataService.AddTracking(t);
					}
				}
			}
		}

		public bool NotModified(EntryCollection entryCollection)
		{
			//Never cache Admin
			if(SiteSecurity.IsInRole("Admin") )
			{
				return false;
			}

            // never cache commentview or login
            string localpath = HttpContext.Current.Request.Url.LocalPath.ToUpper();

            if (localpath.EndsWith("COMMENTVIEW.ASPX") == true || localpath.EndsWith("LOGIN.ASPX") == true)
            {
                return false;
            }

			bool shouldReturn = false;
			if ( !IsPostBack && !HttpContext.Current.Items.Contains(NOTMODIFIEDITEMKEY))
			{
				//If Not Modified Check
				if(SiteSecurity.IsValidContributor() == false)
				{
					Response.Cache.SetCacheability(System.Web.HttpCacheability.Public);
					//Can we get away with an "if-not-modified" header?
					if (SiteUtilities.GetStatusNotModified(SiteUtilities.GetLatestModifedEntryDateTime(dataService, entryCollection)))
					{
						shouldReturn = true;
					}
					else //put a hint for other folks who may call this method later
					{
						HttpContext.Current.Items.Add(NOTMODIFIEDITEMKEY,true);
					}
				}
			}
			return shouldReturn;
		}

		private void Page_Load(object sender, System.EventArgs e)
		{
			//Force a XSS check...
			if (HttpContext.Current != null)
			{
				ValidatePageAndAddAdminJavascript();
			}

			// if the page gets referenced through a "GET" (no postback), we
			// log a referral
			if ( !IsPostBack )
			{
				// Check referrer blacklists.
				BlacklistedReferrer();
			}

			entries = LoadEntries();

			if (NotModified(entries))
			{
				Response.End();
				return;
			}

            ProcessTemplate();

			// The X-pingback header that is injected into every page serve tells 
			// pingback clients where to find the endpoint for pingback.
			if (siteConfig.EnablePingbackService)
			{
				Response.AppendHeader("X-Pingback",new Uri(new Uri(siteConfig.Root),"pingback.aspx").ToString());
			}

			// Add JavaScript and CS for highlighting search words from both our search enginge and Yahoo/Google
			if (siteConfig.EnableSearchHighlight)
			{
				Page.Controls.Add(new SearchHighlight());
			}
		}

        private void Page_PreRender(object sender, System.EventArgs e)
        {
            DataBind();
        }


        /// <summary>
        /// Event handler attached to "Page.Init" that recovers
        /// marked fields from session state
        /// </summary>
        /// <param name="o">Object firing the event</param>
        /// <param name="e">Event arguments</param>
        private void SessionLoad(object o, EventArgs e)
        {

            FieldInfo[] fields = GetType().GetFields(BindingFlags.Public|
                BindingFlags.NonPublic|
                BindingFlags.Instance);

            // Persistent, page scope values
            HttpCookie pageCookie = Request.Cookies[GetType().FullName];
            if ( pageCookie != null )
            {
                foreach( FieldInfo field in fields )
                {
                    if ( field.IsDefined(typeof(PersistentPageStateAttribute),
                        true ) )
                    {
                        PersistentPageStateAttribute ppsa = 
                            (PersistentPageStateAttribute)
                            field.GetCustomAttributes(
                            typeof(PersistentPageStateAttribute),true)[0];
                        if ( ppsa.keyName == null && pageCookie[field.Name] != null )
                        {
                            field.SetValue(this,
                                Convert.ChangeType(pageCookie[field.Name],
                                field.FieldType,
                                CultureInfo.InvariantCulture));
                        }
                    }
                }
            }

            // Persistent, user scope values
            HttpCookie siteCookie = Request.Cookies[keyPrefix+Request.Path.Substring(0,Request.Path.LastIndexOf('/'))];
            if ( siteCookie != null)
            {
                if ( !Request.Path.Substring(0,Request.Path.LastIndexOf('/')).StartsWith(siteCookie.Path))
                {
                    Request.Cookies.Remove(keyPrefix+Request.Path.Substring(0,Request.Path.LastIndexOf('/')));
                }
                else
                {
                    foreach( FieldInfo field in fields )
                    {
                        if ( field.IsDefined(typeof(PersistentPageStateAttribute),
                            true ) )
                        {
                            PersistentPageStateAttribute ppsa = 
                                (PersistentPageStateAttribute)
                                field.GetCustomAttributes(
                                typeof(PersistentPageStateAttribute),true)[0];

                            if ( ppsa.keyName != null && siteCookie[ppsa.keyName] != null )
                            {
                                field.SetValue(this,
                                    Convert.ChangeType(siteCookie[ppsa.keyName],
                                    field.FieldType,
                                    CultureInfo.InvariantCulture));
                            }
                        }
                    }
                }
            }

            // Session scope values
            foreach( FieldInfo field in fields )
            {
                if ( field.IsDefined(typeof(SessionPageStateAttribute),true ) &&
                    field.FieldType.IsSerializable )
                {
                    SessionPageStateAttribute spsa = 
                        (SessionPageStateAttribute)
                        field.GetCustomAttributes(
                        typeof(SessionPageStateAttribute),true)[0];

                    if ( spsa.keyName == null )
                    {
                        field.SetValue(this,
                            Session[field.DeclaringType.FullName+"."+field.Name]);
                    }
                    else
                    {
                        field.SetValue(this,
                            Session[keyPrefix+spsa.keyName]);
                    }
                }
            }

            if ( IsPostBack )
            {
                // Conversation scope values
                foreach( FieldInfo field in fields)
                {
                    if ( field.IsDefined(typeof(TransientPageStateAttribute),
                        true ) &&
                        field.FieldType.IsSerializable )
                    {
                        field.SetValue(this,
                            Session[field.DeclaringType.FullName+"."+field.Name]);
                    }
                }
            }
        }

        
        /// <summary>
        /// This property contains the user's preferred theme for this session.
        /// </summary>
        public string UserTheme
        {
            get
            {
                return userTheme;
            }
            set
            {
                userTheme = value;
            }
        }

        /// <summary>
        /// Gets the current theme.
        /// </summary>
        public BlogTheme BlogTheme
        {
            get
            {
                if ( blogTheme == null )
                {
					// build the list of themes
					ThemeDictionary themes = (ThemeDictionary) dataCache["Themes"];

					if (themes == null)
					{  
                        string themesDirPath = SiteUtilities.MapPath("themes");
                        themes = BlogTheme.Load(themesDirPath);
						
						// no themes were found
						if (themes == null || themes.Count == 0 || themes.ContainsKey(SiteConfig.Theme) == false)
						{
							// user must have upgraded, lets be nice and create the manifest for their default theme.
							string themeName = SiteConfig.Theme;
							string path = Path.Combine(themesDirPath, themeName);

                            // only try to create the manifest if we can find the directory for the default theme 
                            if (Directory.Exists(path))
                            {

                                using (StreamWriter sw = new StreamWriter(Path.Combine(path, "theme.manifest"), false))
                                {
                                    sw.WriteLine("<?xml version=\"1.0\" encoding=\"utf-8\" ?> ");
                                    sw.WriteLine("<theme name=\"{0}\" title=\"{0}\" templateDirectory=\"themes/{0}\" imageDirectory=\"themes/{0}\" />", themeName);
                                }
                            }

                            // reload themes and cache if succes
							themes = BlogTheme.Load(themesDirPath);
                            if (themes != null && themes.Count > 0)
                            {
                                dataCache.Insert("Themes", themes, new CacheDependency(themesDirPath));
                            }
						}
                        else // cache the results so we don't do a lot of IO
						{
                            dataCache.Insert("Themes", themes, new CacheDependency(themesDirPath));
						}
					}
                    //themes = ConfigurationSettings.GetConfig("newtelligence.DasBlog.Themes") as ThemeDictionary;
                    
					//Are we on a Mobile Device? See if we have a mobile theme and use it instead.
					//System.Web.Mobile.MobileCapabilities mobile = (System.Web.Mobile.MobileCapabilities)Request.Browser;
					//if(mobile.IsMobileDevice == true)
					//{
					//	if(themes.TryGetValue("mobile", out blogTheme) == false )
					//	{
					//		loggingService.AddEvent(new EventDataItem(EventCodes.Error,
					//			String.Format("If you have a theme called 'mobile' in your themes folder, readers who visit your site via a Mobile Device will automatically get that theme. User-Agent: {0}",Request.UserAgent),
					//			String.Empty));
					//	}
					//	else 
					//	{
					//		return blogTheme;
					//	}
					//}


                    //TODO: Test this
                    if (SiteUtilities.IsAMPage())
                    {
                        if (themes.TryGetValue("amp", out blogTheme) == false)
                        {
                            loggingService.AddEvent(new EventDataItem(EventCodes.Error,
                                String.Format("If you have a theme called 'amp' in your themes folder, readers who visit your site via a an appropriate reader will automatically get that theme. User-Agent: {0}", Request.UserAgent),
                                String.Empty));
                        }
                        else
                        {
                            return blogTheme;
                        }
                    }


                    BlogTheme theme;

                    if (!String.IsNullOrEmpty(UserTheme) && themes.TryGetValue(UserTheme, out theme))
                    {
                        blogTheme = theme;
                    }

                    else if (!String.IsNullOrEmpty(SiteConfig.Theme) && themes.TryGetValue(SiteConfig.Theme, out theme))
                    {
                        UserTheme = SiteConfig.Theme;
                        blogTheme = theme;
                    }

                    else
                    {
                        string errorMessage = String.Format("Failed to load theme selected by user {0} and theme selected in config {1}", UserTheme, siteConfig.Theme);
                        loggingService.AddEvent(new EventDataItem(EventCodes.Error, errorMessage, "."));
                        UserTheme = "dasBlog";
                        blogTheme = themes["dasBlog"];
                    }
                }
                return blogTheme;
            }
        }

        /// <summary>
        /// Gets or sets the current category that is displayed
        /// </summary>
        public string CategoryName
        {
            get
            {
                return categoryName;
            }
            set
            {
                categoryName = value;
            }
        }

        /// <summary>
        /// The start date for rendering, expressed in UTC
        /// </summary>
        public DateTime DayUtc
        {
            get
            {
                return _dayUtc;
            }
            set
            {
                _dayUtc = value;
            }
        }

		public DateTime Month
		{
			get
			{
				return _month;
			}
			set 
			{
				_month = value;
			}
		}

		public int PageIndex
		{
			get
			{
				return _pageIndex;
			}
			set 
			{
				if(value < 0)
				{
					_pageIndex = 0;
					return;
				}

				_pageIndex = value;
			}
		}

		public bool IsLastPage
		{
			get
			{
                // incorrect! the first page can have a different number of 
                // entries
				return (PageIndex + 1) * SiteConfig.FrontPageEntryCount >=
					DataService.GetEntries(false).Count;
			}
		}

        /// <summary>
        /// The start date for rendering, expressed in "Display Time". "Display Time"
        /// is the time that's in the configured timezone.
        /// </summary>
        public DateTime DayDisplayTime
        {
            get
            {
                return siteConfig.GetConfiguredTimeZone().ToLocalTime(DayUtc);
            }
            set
            {
                DayUtc = siteConfig.GetConfiguredTimeZone().ToUniversalTime(value);
            }
        }

        /// <summary>
        /// The current weblog entry being processed.
        /// </summary>
        public string WeblogEntryId
        {
            get
            {
                return weblogEntryId;
            }
            set
            {
                weblogEntryId = value;
            }
        }

        /// <summary>
        /// Must be overridden in all descandant pages and return a place holder into
        /// which content is rendered by the rendering engine. This should be abstract,
        /// but that would kill the designers.
        /// </summary>
        protected virtual PlaceHolder ContentPlaceHolder
        {
            get { return null; }
        }

        /// <summary>
        /// Gets a reference to the data service
        /// </summary>
        public IBlogDataService DataService
        {
            get
            {
                return dataService;
            }
        }


        /// <summary>
        /// Gets a reference to the binary data service.
        /// </summary>
        public IBinaryDataService BinaryDataService
        {
            get;
            private set;
        }



        /// <summary>
        /// Gets a reference to the logging service
        /// </summary>
        public ILoggingDataService LoggingService
        {
            get
            {
                return loggingService;
            }
        }


        /// <summary>
        /// Gets a reference to the data cache.
        /// </summary>
        public DataCache DataCache {
            get {
                return this.dataCache;
            }
        }


        /// <summary>
        /// Gets the current site configuration
        /// </summary>
        public virtual SiteConfig SiteConfig
        {
            get
            {
                return siteConfig;
            }
        }

        /// <summary>
        /// Gets a boolean value on whether to show the tracking detail (referrers, trackbacks, pingbacks) 
        /// on the page. Off by default, on for the Permalink page.
        /// </summary>
        public bool ShowTrackingDetail
        {
            get
            {
                return showTrackingDetail;
            }
        }

        /// <summary>
        /// This property exposes the ViewState property of the page 
        /// for access by controls. This is used by editors that use nested
        /// Datagrids, because nested Datagrids lose their view state if
        /// they rely on their container.
        /// </summary>
        public StateBag PageViewState
        {
            get
            {
                return ViewState;
            }
        }

        /// <summary>
        /// Gets the page referrer
        /// </summary>
        public Uri UrlReferrer
        {
            get
            {
                if ( urlReferrer != null )
                {
                    return urlReferrer;
                }
                else
                {
                    return Request.UrlReferrer;
                }
            }
        }

        /// <summary>
        /// This property indicates whether the page is an aggregated view. 
        /// Aggregated views show more than one item. Examples are the default page and
        /// the category pages.
        /// </summary>
        public bool IsAggregatedView
        {
            get
            {
                return isAggregatedView;
            }
        }

        /// <summary>
        /// Returns a boolean indicator for whethet to hide the administrator tools.
        /// This will cause the login box, administrator toolbar and edit buttons to
        /// be hidden, independent of the login state of the user. This is used to make
        /// the start page (default.aspx) compatible with caching.
        /// </summary>
        public bool HideAdminTools
        {
            get
            {
                return hideAdminTools;
            }
        }
        

        /// <summary>
        /// Gets the the resource manager for the resource string table of the
        /// Core assembly.
        /// </summary>
        internal ResourceManager CoreStringTables
        {
            get
            {
                return coreStringTables;
            }
        }

        
        /// <summary>
        /// Event handler attached to "Page.Unload" that sticks marked fields
        /// into session state
        /// </summary>
        /// <param name="o">Object firing the event</param>
        /// <param name="e">Event arguments</param>
        private void SessionStore(object o, EventArgs e)
        {

            FieldInfo[] fields = GetType().GetFields(BindingFlags.Public|
                BindingFlags.NonPublic|
                BindingFlags.Instance);

            // Persistent state
            HttpCookie pageCookie = new HttpCookie(GetType().FullName);
            HttpCookie siteCookie = Request.Cookies[keyPrefix+Request.Path.Substring(0,Request.Path.LastIndexOf('/'))];

            if ( siteCookie == null )
            {
                siteCookie = new HttpCookie(keyPrefix+Request.Path.Substring(0,Request.Path.LastIndexOf('/')));
            }
            siteCookie.Expires = pageCookie.Expires = DateTime.Now.ToUniversalTime().AddYears(2);
            siteCookie.Path = Request.Path.Substring(0,Request.Path.LastIndexOf('/'));
            
            
            foreach( FieldInfo field in fields )
            {
                if ( field.IsDefined(typeof(PersistentPageStateAttribute),true ))
                {
                    PersistentPageStateAttribute ppsa = 
                        (PersistentPageStateAttribute)
                        field.GetCustomAttributes(
                        typeof(PersistentPageStateAttribute),true)[0];
                    if ( ppsa.keyName == null )
                    {
                        pageCookie[field.Name] = 
                            (string)Convert.ChangeType(field.GetValue(this),
                            typeof(string),
                            CultureInfo.InvariantCulture);
                    }
                    else
                    {
                        siteCookie[ppsa.keyName] = 
                            (string)Convert.ChangeType(field.GetValue(this),
                            typeof(string),
                            CultureInfo.InvariantCulture);
                    }
					
                }
            }

            if ( pageCookie.Values.Count > 0 )
            {
                Response.AppendCookie(pageCookie);
            }
            if ( siteCookie.Values.Count > 0 )
            {
                Response.AppendCookie(siteCookie);
            }

            // Transient & Session scope state
            foreach( FieldInfo field in fields)
            {
                if ( field.IsDefined(typeof(TransientPageStateAttribute),true ) ||
                    field.IsDefined(typeof(SessionPageStateAttribute),true ) )
                {
                    if (  field.FieldType.IsSerializable )
                    {
                        string fieldName;

                        fieldName = field.DeclaringType.FullName+"."+field.Name;
                        if ( field.IsDefined(typeof(SessionPageStateAttribute),true) )
                        {
                            SessionPageStateAttribute spsa = 
                                (SessionPageStateAttribute)
                                field.GetCustomAttributes(
                                typeof(SessionPageStateAttribute),true)[0];
                            if ( spsa.keyName != null )
                            {
                                fieldName = keyPrefix+spsa.keyName;
                            }
                        }

                        Session[fieldName] = field.GetValue(this);
                    }
                    else
                    {
                        throw new SerializationException(
                            String.Format("Type {0} of field '{0}' is not serializable",
                            field.FieldType.FullName,field.Name));
                    }
                }
            }
        }

        /// <summary>
        /// Private utility function invoked by Transfer and Redirect. 
        /// Clears out all transient state of the current page.
        /// </summary>
        private void SessionDiscard()
        {
            // Discard transient page state only
            foreach( FieldInfo field in GetType().GetFields(BindingFlags.Public|BindingFlags.NonPublic|BindingFlags.Instance))
            {
                if ( field.IsDefined(typeof(TransientPageStateAttribute),true ) &&
                    field.FieldType.IsSerializable )
                {
                    Session.Remove(field.DeclaringType.FullName+"."+field.Name);
                }
            }
        }

        /// <summary>
        /// Use this as a replacement for Server.Transfer. This
        /// method will discard the transient page state and 
        /// call Server.Transfer.
        /// </summary>
        /// <param name="target"></param>  
        public virtual void Transfer( string target )
        {
            // ;store persistent session aspects, since PreRender isn't fired now
            SessionStore(null, null); 
            // ;and then discard the transient state
            SessionDiscard();
            Server.Transfer(target);
        }

        /// <summary>
        /// Use this as a replacement for Response.Redirect. This
        /// method will discard the transient page state and 
        /// call Response.Redirect.
        /// </summary>
        /// <param name="target"></param>
        public virtual void Redirect( string target )
        {
            // ;store persistent session aspects, since PreRender isn't fired now
            SessionStore(null, null); 
            // ;and then discard the transient state
            SessionDiscard();
            Response.Redirect(target,true);
        }
    }
}



