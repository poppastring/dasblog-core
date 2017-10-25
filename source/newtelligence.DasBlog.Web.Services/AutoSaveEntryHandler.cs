using System;
using System.Web;
using newtelligence.DasBlog.Runtime;
using newtelligence.DasBlog.Web.Core;

namespace newtelligence.DasBlog.Web.Services
{
	/// <summary>
	/// Summary description for AutoSaveEntryHandler.
	/// </summary>
	public class AutoSaveEntryHandler : IHttpHandler
	{
		public AutoSaveEntryHandler()
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

			string entryId = "";
			string author = "";
			string title = "";
			string textToSave = "";
			string redirectUrl = SiteUtilities.GetStartPageUrl();


			System.Xml.XmlTextReader xtr = new System.Xml.XmlTextReader(context.Request.InputStream);
			try
			{
				while (!xtr.EOF)
				{
					xtr.Read();
					if (xtr.Name=="entryid")
					{
						entryId = xtr.ReadInnerXml();
					}
					if (xtr.Name=="author")
					{
						author = xtr.ReadInnerXml();
					}
					if (xtr.Name=="title" && xtr.NodeType == System.Xml.XmlNodeType.Element)
					{
						// Ensure this is the start element before moving forward to the CDATA.
						xtr.Read();  // Brings us to the CDATA inside "title"
						title = xtr.Value;
					}
					if (xtr.Name=="posttext" && xtr.NodeType == System.Xml.XmlNodeType.Element)
					{
						xtr.Read();
						textToSave = xtr.Value;
					}
				}
			}
			finally
			{
				xtr.Close();
			}


			// make sure the entry param is there
			if (entryId == null || entryId.Length == 0) 
			{
				context.Response.Redirect(SiteUtilities.GetStartPageUrl(siteConfig));
				return;
			}
			else
			{
				ILoggingDataService logService = LoggingDataServiceFactory.GetService(SiteConfig.GetLogPathFromCurrentContext());
				IBlogDataService dataService = BlogDataServiceFactory.GetService(SiteConfig.GetContentPathFromCurrentContext(), logService );
			
				// First, attempt to get the entry.  If the entry exists, then get it for editing 
				// and save.  If not, create a brand new entry and save it instead.
				Entry entry = dataService.GetEntry(entryId);
				if ( entry != null )
				{
					entry = dataService.GetEntryForEdit( entryId );
					Entry modifiedEntry = entry.Clone();
					modifiedEntry.Content = textToSave;
					modifiedEntry.Title = title;
					modifiedEntry.Author = author;
					modifiedEntry.Syndicated = false;
					modifiedEntry.IsPublic = false;
					modifiedEntry.ModifiedUtc = DateTime.Now.ToUniversalTime();
					modifiedEntry.Categories = "";
					dataService.SaveEntry(modifiedEntry, null);

					//context.Request.Form["textbox1"];
					
					logService.AddEvent(
						new EventDataItem( 
						EventCodes.EntryChanged, entryId, 
						context.Request.RawUrl));
				}
				else
				{
					// This is a brand new entry.  Create the entry and save it.
					entry = new Entry();
					entry.EntryId = entryId;
					entry.CreatedUtc = DateTime.Now.ToUniversalTime();
					entry.ModifiedUtc = DateTime.Now.ToUniversalTime();
					entry.Title = title;
					entry.Author = author;
					entry.Content = textToSave;
					entry.Syndicated = false;
					entry.IsPublic = false;
					dataService.SaveEntry(entry, null);

					//context.Request.Form["textbox1"];
					
					logService.AddEvent(
						new EventDataItem( 
						EventCodes.EntryAdded, entryId, 
						context.Request.RawUrl));
				}
			}
		}
	}
}
