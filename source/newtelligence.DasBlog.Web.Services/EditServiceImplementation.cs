using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Web;
using System.Net.Mail;
using System.Web.Services;
using System.Collections;
using System.Web.Services.Description;
using System.Web.Services.Protocols;
using System.Xml.Serialization;
using newtelligence.DasBlog.Runtime;
using newtelligence.DasBlog.Web.Core;
using newtelligence.DasBlog.Web.Services.Rsd;
using System.Security;

namespace newtelligence.DasBlog.Web.Services
{
	/// <summary>
	/// Summary description for DasBlogEditting.
	/// </summary>
	public class EditServiceImplementation : WebService
	{
		public EditServiceImplementation()
		{
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
		protected override void Dispose(bool disposing)
		{
			if (disposing && components != null)
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#endregion

		[WebMethod]
		public bool CanEdit(string username, string password)
		{
			if (!SiteConfig.GetSiteConfig().EnableEditService)
			{
				throw new ServiceDisabledException();
			}

			try
			{
				if (Context.Request.IsAuthenticated)
				{
                    return SiteSecurity.IsValidContributor();
				}

                Authenticate(username, password);
                return true; // no exception we're good

			}
			catch (Exception e)
			{
				ErrorTrace.Trace(TraceLevel.Error, e);
				return false;
			}
		}

		[WebMethod]
		[SoapDocumentMethod(ParameterStyle=SoapParameterStyle.Wrapped,Use=SoapBindingUse.Literal)]
		public RsdRoot GetRsd()
		{
            // TODO: NLS - Make the default API configurable through SiteConfig
			SiteConfig siteConfig = SiteConfig.GetSiteConfig();
			RsdApiCollection apiCollection = new RsdApiCollection();

			RsdRoot rsd = new RsdRoot();
			RsdService dasBlogService = new RsdService();
			dasBlogService.HomePageLink = SiteUtilities.GetBaseUrl(siteConfig);

			RsdApi metaWeblog    = new RsdApi();
			metaWeblog.Name      = "MetaWeblog";
			metaWeblog.Preferred = ( siteConfig.PreferredBloggingAPI == metaWeblog.Name );
			metaWeblog.ApiLink   = SiteUtilities.GetBloggerUrl(siteConfig);
			metaWeblog.BlogID    = dasBlogService.HomePageLink;
			apiCollection.Add(metaWeblog);

			RsdApi blogger    = new RsdApi();
			blogger.Name      = "Blogger";
			blogger.Preferred = ( siteConfig.PreferredBloggingAPI == blogger.Name );
			blogger.ApiLink   = SiteUtilities.GetBloggerUrl(siteConfig);
			blogger.BlogID    = dasBlogService.HomePageLink;
			apiCollection.Add(blogger);

            RsdApi moveableType    = new RsdApi();
            moveableType.Name      = "Moveable Type";
            moveableType.Preferred = ( siteConfig.PreferredBloggingAPI == moveableType.Name );
            moveableType.ApiLink   = SiteUtilities.GetBloggerUrl( siteConfig );
            moveableType.BlogID    = dasBlogService.HomePageLink;
            apiCollection.Add( moveableType );

			dasBlogService.RsdApiCollection = apiCollection;
			rsd.Services.Add(dasBlogService);

			return rsd;
		}

		[WebMethod]
		public void DeleteEntry(string entryId, string username, string password)
		{
			SiteConfig siteConfig = SiteConfig.GetSiteConfig();
			if (!siteConfig.EnableEditService)
			{
				throw new ServiceDisabledException();
			}

            Authenticate(username, password);

            ILoggingDataService logService = LoggingDataServiceFactory.GetService(SiteConfig.GetLogPathFromCurrentContext());
            IBlogDataService dataService = BlogDataServiceFactory.GetService(SiteConfig.GetContentPathFromCurrentContext(), logService);

			//dataService.DeleteEntry(entryId, siteConfig.CrosspostSites);
			SiteUtilities.DeleteEntry(entryId, siteConfig, logService, dataService);
		}

        [WebMethod]
		public string UpdateEntry(Entry entry, string username, string password)
		{
			SiteConfig siteConfig = SiteConfig.GetSiteConfig();
			if (!siteConfig.EnableEditService)
			{
				throw new ServiceDisabledException();
			}

            Authenticate(username, password);

            ILoggingDataService logService = LoggingDataServiceFactory.GetService(SiteConfig.GetLogPathFromCurrentContext());
            IBlogDataService dataService = BlogDataServiceFactory.GetService(SiteConfig.GetContentPathFromCurrentContext(), logService);

			EntrySaveState val = SiteUtilities.UpdateEntry(entry, null, null, siteConfig, logService, dataService);

            string rtn = string.Empty;
            if (val.Equals(EntrySaveState.Updated))
                rtn = entry.EntryId;
            else
                rtn = val.ToString();

            return rtn;
		}

		[WebMethod]
		public string CreateEntry(Entry entry, string username, string password)
		{
			SiteConfig siteConfig = SiteConfig.GetSiteConfig();
			if (!siteConfig.EnableEditService)
			{
				throw new ServiceDisabledException();
			}
            
            Authenticate(username, password);

			// ensure that the entryId was filled in
			//
			if (entry.EntryId == null || entry.EntryId.Length == 0)
			{
				entry.EntryId = Guid.NewGuid().ToString();
			}

			// ensure the dates were filled in, otherwise use NOW
			if (entry.CreatedUtc == DateTime.MinValue)
			{
				entry.CreatedUtc = DateTime.UtcNow;
			}
			if (entry.ModifiedUtc == DateTime.MinValue)
			{
				entry.ModifiedUtc = DateTime.UtcNow;
			}

            ILoggingDataService logService = LoggingDataServiceFactory.GetService(SiteConfig.GetLogPathFromCurrentContext());
            IBlogDataService dataService = BlogDataServiceFactory.GetService(SiteConfig.GetContentPathFromCurrentContext(), logService);

			SiteUtilities.SaveEntry(entry, string.Empty, null, siteConfig, logService, dataService);

			return entry.EntryId;
		}
        /// <summary>
        /// 12 Oct 2006 / MOT :
        /// Get entry by date and title.  If there is more than one match, the most recent entry will be returned.
        /// </summary>
        /// <param name="entryDate">Date the entry was posted.</param>
        /// <param name="entryTitle">Title of the post.</param>
        /// <returns>DasBlog.Runtime.Entry</returns>
        [WebMethod (MessageName="GetEntryByDateAndTitle",Description="Get entry by date and title.  If there is more than one match, the most recent entry will be returned.")]
        public Entry GetEntry(DateTime entryDate, string entryTitle)
        {
            SiteConfig siteConfig = SiteConfig.GetSiteConfig();
            ILoggingDataService logService = LoggingDataServiceFactory.GetService(SiteConfig.GetLogPathFromCurrentContext());
            IBlogDataService dataService = BlogDataServiceFactory.GetService(SiteConfig.GetContentPathFromCurrentContext(), logService);

            //Only pass in the date portion of the entry date, do not pass in the time portion.
            DayEntry dayEntry = dataService.GetDayEntry(entryDate.Date);

            //this replacement of characters in the title was lifted directly from
            //  DasBlog.Web.Core::TitleMapperModule.HandleBeginRequest()
            entryTitle = entryTitle.Replace(".aspx", "");
            entryTitle = entryTitle.Replace("+", "");
            entryTitle = entryTitle.Replace(" ", "");
            entryTitle = entryTitle.Replace("%2b", "");
            entryTitle = entryTitle.Replace("%20", "");

            //now that we have a properly formatted title, use it to get a specific
            //  entry from the DayEntry object.  If there is more than one match to the
            //  title, the most recent entry will be matched.
            Entry entry = dayEntry.GetEntryByTitle(entryTitle);

            return entry;
        }
        /// <summary>
        /// 12 Oct 2006 / MOT :
        /// Get entry by unique guid string.  This will return, at most, one entry.
        /// </summary>
        /// <param name="entryId">guid string</param>
        /// <returns>DasBlog.Runtime.Entry</returns>
        [WebMethod(MessageName = "GetEntryByEntryID", Description="Get entry by guid string.  There will only ever be at most one match.")]
        public Entry GetEntry(string entryId)
        {
            SiteConfig siteConfig = SiteConfig.GetSiteConfig();
            ILoggingDataService logService = LoggingDataServiceFactory.GetService(SiteConfig.GetLogPathFromCurrentContext());
            IBlogDataService dataService = BlogDataServiceFactory.GetService(SiteConfig.GetContentPathFromCurrentContext(), logService);

            Entry entry = dataService.GetEntry(entryId);
            return entry;
        }

        /// <summary>
        /// Tries to login the user with the supplied username and password, 
        /// throws when the user cannot be authenticated or is not a valid contributor.
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <exception cref="System.Security.SecurityException">Thrown when the user can not be authenticated.</exception>
        private static void Authenticate(string username, string password)
        {
            // parameter validationis handled in the SiteSecurity.Login method.
            
            UserToken user = SiteSecurity.Login(username, password);
            if (user == null || !SiteSecurity.IsValidContributor())
            {
                throw new SecurityException("Invalid username or password.");
            }
        }
	}
}
