using System;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Xml.Serialization;
using newtelligence.DasBlog.Runtime;


namespace newtelligence.DasBlog.Web.Core
{
	/// <summary>
	/// Summary description for SideBarOpmlList.
	/// </summary>
	[DefaultProperty("FileName"), 
		ToolboxData("<{0}:SideBarOpmlList runat=server></{0}:SideBarOpmlList>")]
	public class SideBarOpmlList : System.Web.UI.WebControls.WebControl
	{
        string unresolvedFileName;
        string fileName;
		bool renderDescription;
        
	
		[Bindable(true), 
			Category("Data"), 
			DefaultValue("")] 
		public string FileName
        {
            get
            {
                return unresolvedFileName;
            }
            set
            {
                unresolvedFileName = value;
            }
        }

		public bool RenderDescription
		{
			get
			{
				return renderDescription;
			}
			set
			{
				renderDescription = value;
			}
		}
                       
        private void RenderOutlinesIntoTable( Table table, OpmlOutlineCollection outlines, int depth, bool renderDescription )
        {
            SharedBasePage requestPage = Page as SharedBasePage;
            foreach (OpmlOutline outline in outlines)
            {
                TableRow row = new TableRow();
                TableCell cell = new TableCell();
                cell.CssClass = "blogRollCellStyle";
                table.Rows.Add(row);
                row.Cells.Add(cell);

                if ( outline.outline == null || outline.outline.Count == 0 )
                {
                    // if there are no sub items, we just emit the link
                    if ( outline.xmlUrl != null && outline.xmlUrl.Length > 0 )
                    {
                        HyperLink xmlLink = new HyperLink();
                        xmlLink.CssClass = "blogRollXmlLinkStyle";
                        xmlLink.NavigateUrl = outline.xmlUrl;
						
						if (requestPage.SiteConfig.UseFeedSchemeForSyndication)
						{
							if (outline.xmlUrl.StartsWith("https")) 
							{
								xmlLink.NavigateUrl =  String.Concat("feed:", new Uri(outline.xmlUrl).ToString());
							}	
							else 
							{
								xmlLink.NavigateUrl = String.Concat("feed:", new Uri(outline.xmlUrl).ToString().Remove(0,5));
							}
						}
                                
                        System.Web.UI.WebControls.Image img = new System.Web.UI.WebControls.Image();
                        img.AlternateText = "[Feed]";
                        switch ( outline.type != null ? outline.type.ToLower() : "" )
                        {
                            case "rss" : img.ImageUrl = requestPage.GetThemedImageUrl("feedButton");break;
                            case "atom" : img.ImageUrl = requestPage.GetThemedImageUrl("atomButton");break;
                            case "opml" : img.ImageUrl = requestPage.GetThemedImageUrl("opmlButton");break;
                            default: img.ImageUrl = requestPage.GetThemedImageUrl("feedButton");break;
                        }
                         
                        xmlLink.Controls.Add(img);
                        cell.Controls.Add(xmlLink);
                    }
                    cell.Controls.Add( new LiteralControl("&nbsp;"));
                    HyperLink catLink = new HyperLink();
                    catLink.CssClass = "blogRollLinkStyle";
                    catLink.Text = outline.title;
                    catLink.NavigateUrl = outline.htmlUrl;                  
                    cell.Controls.Add(catLink);
                    
					if (renderDescription)
					{
						if ( outline.description != null && outline.description.Length > 0 )
						{
							HtmlGenericControl outlineDesc = new HtmlGenericControl("div");                        
							outlineDesc.Attributes.Add("class","blogRollDescriptionStyle");
							outlineDesc.InnerText = outline.description;
							cell.Controls.Add(new LiteralControl("<br />"));
							cell.Controls.Add(outlineDesc);
						}
					}
                }
                else
                {
                    // if there are subitems, we ignore this link and render 
                    // it as an outline
                    NamingPanel panel = new NamingPanel();
                    cell.Controls.Add(panel);
 
                    Table outlineHead = new Table();
                    outlineHead.CssClass = "blogRollNestedOutlineHeaderTableStyle";
                    panel.Controls.Add( outlineHead );

                    Panel collapsablePanel = new Panel();
                    panel.Controls.Add( collapsablePanel );
                    collapsablePanel.CssClass = "blogRollCollapsed";
                    collapsablePanel.ID = "panel";

                    TableCell outlineBadge;
                    TableCell outlineTitle;
                    outlineHead.Rows.Add(new TableRow());
                    outlineHead.Rows[0].Cells.Add(outlineBadge = new TableCell());
                    outlineHead.Rows[0].Cells.Add(outlineTitle = new TableCell());
                    outlineBadge.CssClass = "blogRollNestedOutlineBadgeCellStyle";
                    outlineTitle.CssClass = "blogRollNestedOutlineTitleCellStyle";

                    if ( outline.description != null && outline.description.Length > 0 )
                    {
                        TableCell outlineDesc;
                        outlineHead.Rows.Add(new TableRow());
                        outlineHead.Rows[1].Cells.Add(new TableCell());
                        outlineHead.Rows[1].Cells.Add(outlineDesc = new TableCell() );
                        outlineDesc.CssClass = "blogRollDescriptionStyle";
                        outlineDesc.Text = outline.description;
                    }

                    HyperLink hlBadge = new HyperLink();
                    hlBadge.CssClass = "blogRollNestedOutlineBadgeStyle";
                    outlineBadge.Controls.Add(hlBadge);
                    System.Web.UI.WebControls.Image imgBadge = new System.Web.UI.WebControls.Image();
                    hlBadge.Controls.Add(imgBadge);
                    imgBadge.ID = "img";
                    imgBadge.AlternateText = "";
                    hlBadge.NavigateUrl = "javascript:br_toggleExpansionStatus( '"+collapsablePanel.ClientID+"', '"+imgBadge.ClientID+"' )";
                    
                    imgBadge.ImageUrl = requestPage.GetThemedImageUrl("outlinearrow");
                    imgBadge.ToolTip = requestPage.CoreStringTables.GetString("tooltip_expand");
                    

                    HyperLink hlHead = new HyperLink();
                    hlHead.CssClass = "blogRollNestedOutlineTitleStyle";
                    outlineTitle.Controls.Add(hlHead);
                    hlHead.NavigateUrl = outline.htmlUrl;
                    hlHead.Text = outline.title;
                    

                    Table list = new Table();
                    list.CssClass = "blogRollNestedOutlineBodyTableStyle";
                    collapsablePanel.Controls.Add( list );
                                        
                    RenderOutlinesIntoTable( list, outline.outline, depth+1, renderDescription );
                }
            }

            
        }

        protected override void OnPreRender(EventArgs e)
        {
            SharedBasePage requestPage = Page as SharedBasePage;

            base.OnPreRender (e);
            if ( !Page.ClientScript.IsClientScriptBlockRegistered(this.GetType(),"script"))
            {
                string prefix =
                    String.Format("var br_img_expanded = '{0}';\nvar br_img_collapsed = '{1}';",
                          requestPage.GetThemedImageUrl("outlinedown"),
                          requestPage.GetThemedImageUrl("outlinearrow"));

                string script;
                using ( StreamReader rs = new StreamReader(GetType().Assembly.GetManifestResourceStream(GetType().Namespace+".SideBarOpmlListJS.txt")) )
                {
                    script = rs.ReadToEnd();
                }
                Page.ClientScript.RegisterClientScriptBlock(this.GetType(),"script",
                        String.Format("<script type=\"text/javascript\">\n<!--\n{0}\n{1}\n// --></script>",prefix,script));
            }
            if (!Page.ClientScript.IsClientScriptBlockRegistered(this.GetType(), "style"))
            {
                string script;
                using ( StreamReader rs = new StreamReader(GetType().Assembly.GetManifestResourceStream(GetType().Namespace+".SideBarOpmlListCSS.txt")) )
                {
                    script = rs.ReadToEnd();
                }
                Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "style", "");
                requestPage.InsertInPageHeader(String.Format("<style type=\"text/css\">{0}</style>", script));
            }
        }

        /// <summary> 
        /// Render this control to the output parameter specified.
        /// </summary>
        /// <param name="output"> The HTML writer to write out to </param>
        protected override void Render(HtmlTextWriter output)
        {
            HtmlGenericControl section = new HtmlGenericControl("div");
            section.Attributes["class"] = "blogRollContainerStyle";
            this.Controls.Add(section);
            
            Table list = new Table();
            list.CssClass = "blogRollTableStyle";
                        
            
            section.Controls.Add(list);


            //TODO: restructure: code-reuse != copy+paste

            try
            {
                bool foundOpml = false;
                
                Opml nav = null;
                string fullPath;

                if ( unresolvedFileName.StartsWith("http:") )
                {
                    try
                    {
                        Uri externalUri = new Uri(unresolvedFileName);
                        if ( externalUri.Scheme != "file" )                    
                        {
                            foundOpml = true;
                            WebRequest wq = WebRequest.Create(unresolvedFileName);
                            using (WebResponse wr = wq.GetResponse())
                            {
                                XmlSerializer ser;
                                using (Stream s = wr.GetResponseStream())
                                {
                                    ser = new XmlSerializer(typeof(Opml));
                                    nav = (Opml)ser.Deserialize(s);
                                }
                            }
                        }
                        else
                        {
                            unresolvedFileName = externalUri.LocalPath;
                        }
                    }
                    catch( Exception exc )
                    {
                        ErrorTrace.Trace(System.Diagnostics.TraceLevel.Error,exc);
                        foundOpml = false;
                    }
                }

                if ( foundOpml == false )
                {
                    if ( File.Exists( unresolvedFileName ) ) 
                        fullPath = unresolvedFileName;
                    else
                        fullPath = SiteUtilities.MapPath(unresolvedFileName);

                    if ((File.Exists(fullPath)) && (foundOpml == false))
                    {
                        foundOpml = true;
                        fileName = fullPath;
                        XmlSerializer ser;
                        using (Stream s = File.OpenRead(fileName))
                        {
                            ser = new XmlSerializer(typeof(Opml));
                            nav = (Opml)ser.Deserialize(s);
                        }
                    }
                }
                
                if ( foundOpml == false )
                {
                    fullPath = SiteUtilities.MapPath("~/SiteConfig/blogroll.opml");
                    if ((File.Exists(fullPath)))
                    {
                        foundOpml = true;
                        fileName = fullPath;

                        XmlSerializer ser;
                        using (Stream s = File.OpenRead(fullPath))
                        {
                            ser = new XmlSerializer(typeof(Opml));
                            nav = (Opml)ser.Deserialize(s);
                        }
                    }
                }

                if ( foundOpml == false )
                {
                    string urlPath = SiteUtilities.MapPath("~/SiteConfig/opml.txt");
                    if ((File.Exists(urlPath)))
                    {
                        foundOpml = true;
                        string url;
                        fileName = urlPath;

                        using (StreamReader t = File.OpenText(urlPath))
                        {
                            url = t.ReadLine();
                        }

                        WebRequest wq = WebRequest.Create(url);
                        using (WebResponse wr = wq.GetResponse())
                        {
                            XmlSerializer ser;
                            using (Stream s = wr.GetResponseStream())
                            {
                                ser = new XmlSerializer(typeof(Opml));
                                nav = (Opml)ser.Deserialize(s);
                            }
                        }
                    }
                }

                

                if (foundOpml == true)
                {
                    RenderOutlinesIntoTable( list, nav.body.outline, 0, renderDescription );
                }
                else
                {
                    section.Controls.Add(new LiteralControl("Add 'blogroll.opml' to your SiteConfig directory<br />"));
                }
            }
            catch( Exception exc )
            {
                ErrorTrace.Trace(System.Diagnostics.TraceLevel.Error,exc);
                section.Controls.Add(new LiteralControl("There was an error processing '" + fileName + "'<br />"));
            }
            section.RenderControl( output );
        }
   
	}
}
