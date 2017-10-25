using System;
using System.Web;
using newtelligence.DasBlog.Runtime;
using newtelligence.DasBlog.Web.Core;


namespace newtelligence.DasBlog.Web.Services 
{
	
	/// <summary>
	/// Currently only used to approve comments, but can easily be modified 
	/// to approve other content types (entries, trackbacks etc.).
	/// </summary>
	public class ApproveItemHandler : IHttpHandler
	{
		
		public ApproveItemHandler()
		{
			// ...
		}

		public bool IsReusable {
			get 
			{
				return true;
			}
		}

		public void ProcessRequest( HttpContext context ) 
		{

			if( !SiteSecurity.IsValidContributor()){
				context.Response.Redirect("~/FormatPage.aspx?path=SiteConfig/accessdenied.format.html");
			}

			SiteConfig siteConfig = SiteConfig.GetSiteConfig();
			
			string entryId;
			string commentId;
			string referralPermalink;
			string type;
			string redirectUrl = SiteUtilities.GetStartPageUrl();

			entryId = context.Request.QueryString["entryId"];
			commentId = context.Request.QueryString["commentId"];
			referralPermalink = context.Request.QueryString["referralPermalink"];
			type = context.Request.QueryString["type"];

			// make sure the entry param is there
			if (entryId == null || entryId.Length == 0) {
				context.Response.Redirect(SiteUtilities.GetStartPageUrl(siteConfig));
				return;
			}
			else {
				try {

					ILoggingDataService logService = LoggingDataServiceFactory.GetService(SiteConfig.GetLogPathFromCurrentContext());
					IBlogDataService dataService = BlogDataServiceFactory.GetService(SiteConfig.GetContentPathFromCurrentContext(), logService );
			
					Entry entry = dataService.GetEntry( entryId );
					if ( entry != null ) {
						if (commentId != null && commentId.Length > 0) {

							ISpamBlockingService spamBlockingService = siteConfig.SpamBlockingService;
							if (spamBlockingService != null) 
							{
								Comment comment = dataService.GetCommentById(entryId, commentId);
								if ( (comment != null) && (comment.SpamState == SpamState.Spam) )
								{
									try 
									{
										spamBlockingService.ReportNotSpam(comment);
									}
									catch(Exception ex)
									{
										logService.AddEvent(new EventDataItem(EventCodes.Error, String.Format("Unable to report comment {0} incorrectly marked as spam. Original exception: {1}", comment.EntryId, ex), SiteUtilities.GetPermaLinkUrl(entryId)));
									}
								}
							}

							dataService.ApproveComment(entryId, commentId);

							logService.AddEvent(
								new EventDataItem( 
								EventCodes.CommentApproved, commentId, 
								SiteUtilities.GetPermaLinkUrl(entryId)));
	
							redirectUrl = SiteUtilities.GetCommentViewUrl(entryId);
						}
					}
				}
				catch( Exception exc ) {
					// absorb
					ErrorTrace.Trace(System.Diagnostics.TraceLevel.Error,exc);
				}
			}

			context.Response.Redirect(redirectUrl);
		}
	}
}
