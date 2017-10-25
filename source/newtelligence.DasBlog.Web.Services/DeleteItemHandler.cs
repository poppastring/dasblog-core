using System;
using System.Web;
using newtelligence.DasBlog.Runtime;
using newtelligence.DasBlog.Web.Core;


namespace newtelligence.DasBlog.Web.Services
{
	/// <summary>
	/// Summary description for DeleteItemHandler.
	/// </summary>
	public class DeleteItemHandler : IHttpHandler 
	{
		public DeleteItemHandler()
		{
			
		}

		public bool IsReusable
		{
			get
			{
				return true;
			}
		}

		public void ProcessRequest( HttpContext context )
		{
			if (!SiteSecurity.IsValidContributor()) 
			{
				context.Response.Redirect("~/FormatPage.aspx?path=SiteConfig/accessdenied.format.html");
			}

			SiteConfig siteConfig = SiteConfig.GetSiteConfig();

			string entryId;
			string commentId;
			string referralPermalink;
			string type;
			string redirectUrl = SiteUtilities.GetStartPageUrl();
			bool reportAsSpam;

			entryId = context.Request.QueryString["entryId"];
			commentId = context.Request.QueryString["commentId"];
			referralPermalink = context.Request.QueryString["referralPermalink"];
			type = context.Request.QueryString["type"];
			reportAsSpam = context.Request.QueryString["report"] != null;

			// make sure the entry param is there
			if (entryId == null || entryId.Length == 0) 
			{
				context.Response.Redirect(SiteUtilities.GetStartPageUrl(siteConfig));
				return;
			}
			else
			{
				try
				{
					ILoggingDataService logService = LoggingDataServiceFactory.GetService(SiteConfig.GetLogPathFromCurrentContext());
					IBlogDataService dataService = BlogDataServiceFactory.GetService(SiteConfig.GetContentPathFromCurrentContext(), logService );
			
					Entry entry = dataService.GetEntry( entryId );
					if ( entry != null )
					{
						if (commentId != null && commentId.Length > 0)
						{
							if (reportAsSpam)
							{
								ISpamBlockingService spamBlockingService = siteConfig.SpamBlockingService;
								if (spamBlockingService != null)
								{
									Comment comment = dataService.GetCommentById(entryId, commentId);
									if ( (comment != null) && (comment.SpamState != SpamState.Spam) )
									{
										try
										{
											spamBlockingService.ReportSpam(comment);
										}
										catch(Exception ex)
										{
											logService.AddEvent(new EventDataItem(EventCodes.Error, String.Format("Unable to report comment {0} as spam. Original exception: {1}", comment.EntryId, ex), SiteUtilities.GetPermaLinkUrl(entryId)));
										}
									}
								}
							}
							dataService.DeleteComment(entryId, commentId);

							logService.AddEvent(
								new EventDataItem( 
								EventCodes.CommentDeleted, commentId, 
								SiteUtilities.GetPermaLinkUrl(entryId)));
	
							redirectUrl = SiteUtilities.GetCommentViewUrl(entryId);
						}
						else if (referralPermalink != null && referralPermalink.Length > 0)
						{
							TrackingType trackingType = TrackingType.Referral;

							if (type != null && type.Length != 0)
							{
								trackingType = (TrackingType)Enum.Parse(typeof(TrackingType), type);
							}

							dataService.DeleteTracking(entryId, referralPermalink, trackingType);

							logService.AddEvent(
								new EventDataItem( 
								EventCodes.ItemReferralDeleted, referralPermalink, 
								SiteUtilities.GetPermaLinkUrl(entryId)));

							redirectUrl = SiteUtilities.GetPermaLinkUrl(entryId);
						}
						else // it must be an entry we are deleting
						{
							SiteUtilities.DeleteEntry(entryId, siteConfig, logService, dataService);
							redirectUrl = SiteUtilities.GetStartPageUrl();
						}
					}
				}
				catch( Exception exc )
				{
					// absorb
					ErrorTrace.Trace(System.Diagnostics.TraceLevel.Error,exc);
				}
			}

			context.Response.Redirect(redirectUrl);
		}
	}
}
