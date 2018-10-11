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
	public class BlogManagerOptions
	{
		public bool AdjustDisplayTimeZone { get; set; }
		public string ContentDir { get; set; }
		public int ContentLookaheadDays { get; set; }
		public string DaysCommentsAllowed { get; set; }
		public decimal DisplayTimeZoneIndex { get; set; }
		public bool EnableAutoPingback { get; set; }
		public bool EnableCommentDays { get; set; }
		public bool EnableCrossPostFooter { get; set; }
		public bool EnableTitlePermaLinkUnique { get; set; }
		public int FrontPageEntryCount { get; set; }
		public string LogDir { get; set; }
		public string PingServices { get; set; }
		public string Root { get; set; }
		public string Title { get; set; }
		public string TitlePermalinkSpaceReplacement { get; set; }
	}
}
