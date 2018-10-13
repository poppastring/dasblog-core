namespace DasBlog.Managers
{
/*
dasBlogSettings.WebRootDirectory
dasBlogSettings.SiteConfiguration.LogDir
dasBlogSettings.WebRootDirectory
dasBlogSettings.SiteConfiguration.ContentDir
dasBlogSettings.GetPermaTitle(e.CompressedTitle)
dasBlogSettings.SiteConfiguration.TitlePermalinkSpaceReplacement
dasBlogSettings.GetContentLookAhead()
dasBlogSettings.GetConfiguredTimeZone()
dasBlogSettings.SiteConfiguration.FrontPageDayCount
dasBlogSettings.SiteConfiguration.FrontPageEntryCount
dasBlogSettings.SiteConfiguration.EntriesPerPage
dasBlogSettings.GetConfiguredTimeZone()
dasBlogSettings.SiteConfiguration.EnableTitlePermaLinkUnique
dasBlogSettings.SiteConfiguration.Root
dasBlogSettings.RelativeToRoot(
dasBlogSettings.GetPermaTitle()
dasBlogSettings.SiteConfiguration.Root
dasBlogSettings.RelativeToRoot(
dasBlogSettings.GetPermaTitle()
 */
	public class PingServices
	{
		
	}
	/// <summary>
	/// loaded from site.config file
	/// </summary>
	public class BlogManagerOptions
	{
//		public bool AdjustDisplayTimeZone { get; set; }
		public string ContentDir { get; set; }
//		public int ContentLookaheadDays { get; set; }
//		public int DaysCommentsAllowed { get; set; }
//		public decimal DisplayTimeZoneIndex { get; set; }
		public bool EnableAutoPingback { get; set; }
//		public bool EnableCommentDays { get; set; }
//		public bool EnableCrossPostFooter { get; set; }
		public bool EnableTitlePermaLinkUnique { get; set; }
//		public int FrontPageEntryCount { get; set; }
		public string LogDir { get; set; }
		//public object PingServices { get; set; }
				// currently hardcoded in BlogManager - will be sorted when strategy is clear
		public string Root { get; set; }
		public string Title { get; set; }
		public string TitlePermalinkSpaceReplacement { get; set; }
	}
	public class BlogManagerModifiableOptions
	{
		public bool AdjustDisplayTimeZone { get; set; }
//		public string ContentDir { get; set; }
		public int ContentLookaheadDays { get; set; }
		public int DaysCommentsAllowed { get; set; }
		public decimal DisplayTimeZoneIndex { get; set; }
//		public bool EnableAutoPingback { get; set; }
		public bool EnableCommentDays { get; set; }
		public bool EnableCrossPostFooter { get; set; }
//		public bool EnableTitlePermaLinkUnique { get; set; }
		public int FrontPageEntryCount { get; set; }
//		public string LogDir { get; set; }

		//public object PingServices { get; set; }
				// currently hardcoded in BlogManager - will be sorted when strategy is clear

//		public string Root { get; set; }
//		public string Title { get; set; }
//		public string TitlePermalinkSpaceReplacement { get; set; }
	}
	/// <summary>
	/// options not loaded from site.config
	/// </summary>
	public class BlogManagerExtraOptions
	{
		/// <summary>
		/// avoiding passing IHostingEnvironment (whose ContentRootPath is operative)
		/// so that we don't create an unnecessary dependency between DasBlog.Managers and ASP.NET
		/// </summary>
		public string ContentRootPath { get; set; }
	}
}
