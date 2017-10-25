using System;
using System.Web;
using newtelligence.DasBlog.Runtime;
using newtelligence.DasBlog.Web.Services.Rss20;
using System.Xml;
using System.Xml.Serialization;
using newtelligence.DasBlog.Web.Core;

namespace newtelligence.DasBlog.Web.Services
{
	/// <summary>
	/// Summary description for CommentAPI.
	/// </summary>
	public class CommentAPI : IHttpHandler
	{
		public CommentAPI()
		{
		}


        public void ProcessRequest( HttpContext context )
        {
            if( context.Request.ContentType =="text/xml" &&
                context.Request.RequestType =="POST" &&
                context.Request.QueryString["guid"] != null )
            {
                try
                {
                    ILoggingDataService logService = LoggingDataServiceFactory.GetService(SiteConfig.GetLogPathFromCurrentContext());
                    IBlogDataService dataService = BlogDataServiceFactory.GetService(SiteConfig.GetContentPathFromCurrentContext(), logService);
                    DataCache cache = CacheFactory.GetCache();
                    
					Entry entry = dataService.GetEntry(context.Request.QueryString["guid"]);

					if (entry != null && DasBlog.Web.Core.SiteUtilities.AreCommentsAllowed(entry, SiteConfig.GetSiteConfig()))
					{
						XmlSerializer ser = new XmlSerializer(typeof(RssItem));
						RssItem item = (RssItem)ser.Deserialize(context.Request.InputStream);
						if ( item != null )
						{
							Comment c = new Comment();
							c.Initialize();
							foreach( XmlElement el in item.anyElements )
							{
								if ( el.NamespaceURI == "http://purl.org/dc/elements/1.1/" &&
									el.LocalName == "creator")
								{
									c.Author = el.InnerText;
									break;
								}
							}
							c.AuthorEmail = item.Author;
							c.AuthorHomepage = item.Link;
							c.AuthorIPAddress = context.Request.UserHostAddress;
							c.Content = context.Server.HtmlEncode(item.Description);
							c.TargetEntryId = entry.EntryId;
							c.TargetTitle = "";
							dataService.AddComment(c);
							
							// TODO: no comment mail?

							// break the caching
							cache.Remove("BlogCoreData");
							context.Response.StatusCode = 200;
							context.Response.SuppressContent = true;
							context.Response.End();
						}
					}
					else if (entry != null && !entry.AllowComments)
					{
						context.Response.StatusCode = 403; // Forbidden
						context.Response.SuppressContent = true;
						context.Response.End();
					}
					else if (entry == null)
					{
						context.Response.StatusCode = 404; // Not Found
						context.Response.SuppressContent = true;
						context.Response.End();
					}
                }
                catch(Exception exc)
                {
                    ErrorTrace.Trace(System.Diagnostics.TraceLevel.Error,exc);
                }
            }
        }

        public bool IsReusable
        {
            get
            {
                return true;
            }
        }
	}
}
