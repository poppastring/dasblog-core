using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.ComponentModel;
using System.Collections;
using newtelligence.DasBlog.Runtime;
using System.IO;

namespace newtelligence.DasBlog.Web.Core
{
	/// <summary>
	/// Summary description for CategoryList.
	/// </summary>
	[DefaultProperty("Categories"), 
		ToolboxData("<{0}:CategoryList runat=server></{0}:CategoryList>")]
	public class CategoryList : System.Web.UI.WebControls.WebControl
	{
		private CategoryCacheEntryCollection categories = new CategoryCacheEntryCollection();
		
		private bool showFeedBadge = true;

		public CategoryList(bool showFeedBadge) : base()
		{
			this.showFeedBadge = showFeedBadge;
		}

		public CategoryList() : base()
		{

		}

		[Bindable(true), Category("Data"), DefaultValue("")] 
		public CategoryCacheEntryCollection Categories 
		{
			get
			{
				return categories;
			}

			set
			{
				categories = value;
			}
		}


        private void RenderCategoriesIntoTable( Table table, string[] parentPath )
        {
            SharedBasePage requestPage = Page as SharedBasePage;
            int parentPathLength = parentPath.Length;

            foreach (CategoryCacheEntry categoryEntry in categories )
            {
                string[] entryPath = categoryEntry.CategoryPath;
                int entryPathLength = categoryEntry.CategoryPath.Length;

                // If the path shorter of equal to the parent path or if
                // the path is more than one item longer than the parent path, 
                // we can skip this item. We seek for descendants only.
                if ( entryPathLength != parentPathLength+1 )
                    continue;

                bool identicalSubpaths = true;

                // if we don't have the right subpath, skip as well
                for (int j=0;j<parentPath.Length;j++)
                {
                    if ( entryPath[j].Trim().ToUpper() != parentPath[j].Trim().ToUpper() )
                    {
                        identicalSubpaths = false;
                        break;
                    }
                   
                }   
                if ( !identicalSubpaths )
                {
                    continue;
                }


                TableRow row = new TableRow();
                TableCell cell = new TableCell();
                cell.CssClass = "categoryListCellStyle";
                table.Rows.Add(row);
                row.Cells.Add(cell);

				HyperLink rssLink = new HyperLink();
				if(showFeedBadge)
				{
					rssLink.CssClass = "categoryListXmlLinkStyle";
					if (requestPage.SiteConfig.UseFeedSchemeForSyndication)
					{
						rssLink.NavigateUrl = SiteUtilities.GetFeedCategoryUrl(requestPage.SiteConfig,categoryEntry.Name);
					}
					else 
					{
						rssLink.NavigateUrl = SiteUtilities.GetRssCategoryUrl(requestPage.SiteConfig,categoryEntry.Name);
					}
					System.Web.UI.WebControls.Image rssImg = new System.Web.UI.WebControls.Image();
					rssImg.ImageUrl = requestPage.GetThemedImageUrl("feedButton");
                    rssImg.AlternateText = "[RSS]";    
					rssLink.Attributes.Add("rel", "tag");
					rssLink.Controls.Add(rssImg);
				}
				                
                /* PARKED
                HyperLink atomLink = new HyperLink();
                atomLink.CssClass = "categoryListXmlLinkStyle";
                atomLink.NavigateUrl = Utils.GetAtomCategoryUrl(requestPage.SiteConfig,categoryEntry.Name);
                System.Web.UI.WebControls.Image atomImg = new System.Web.UI.WebControls.Image();
                atomImg.ImageUrl = requestPage.GetThemedImageUrl("atomButton");                        
                atomLink.Controls.Add(atomImg);
                */
                

                // the entryPath is a leaf on the parentPath ?
                if ( !categories.HasChildrenInCollection( categoryEntry ) )
                {
                    // we just emit the link
					if(showFeedBadge)
					{
						cell.Controls.Add(rssLink);
					}
					cell.Controls.Add(new LiteralControl("&nbsp;"));
                    /*cell.Controls.Add(atomLink);
                    cell.Controls.Add( new LiteralControl("&nbsp;"));*/

                    HyperLink catLink = new HyperLink();
                    catLink.CssClass = "categoryListLinkStyle";
                    catLink.Text = categoryEntry.DisplayName;
                    catLink.NavigateUrl = SiteUtilities.GetCategoryViewUrl(requestPage.SiteConfig,categoryEntry.Name);
					catLink.Attributes.Add("rel", "tag");
                    cell.Controls.Add(catLink);
                }
                else
                {
                    // if there are subitems, we ignore this link and render 
                    // it as an categoryEntry
                    NamingPanel panel = new NamingPanel();
                    cell.Controls.Add(panel);
 
                    Table categoryEntryHead = new Table();
                    categoryEntryHead.CellPadding=0;categoryEntryHead.CellSpacing=0;
                    categoryEntryHead.CssClass = "categoryListNestedOutlineHeaderTableStyle";
                    panel.Controls.Add( categoryEntryHead );

                    Panel collapsablePanel = new Panel();
                    Panel containerPanel = new Panel();
                    panel.Controls.Add( collapsablePanel );
                    containerPanel.CssClass = "categoryListNestedOutlineContainer";
                    collapsablePanel.Controls.Add( containerPanel );
                    collapsablePanel.CssClass = "categoryListExpanded";
                    collapsablePanel.ID = "panel";

                    TableCell categoryEntryBadge;
                    TableCell categoryEntryTitle;
                    categoryEntryHead.Rows.Add(new TableRow());
                    categoryEntryHead.Rows[0].Cells.Add(categoryEntryBadge = new TableCell());
                    categoryEntryHead.Rows[0].Cells.Add(categoryEntryTitle = new TableCell());
                    categoryEntryBadge.CssClass = "categoryListNestedOutlineBadgeCellStyle";
                    categoryEntryTitle.CssClass = "categoryListNestedOutlineTitleCellStyle";

                    
                    HyperLink expandableLink = new HyperLink();
               
                    categoryEntryBadge.Controls.Add(rssLink);
                    categoryEntryBadge.Controls.Add(new LiteralControl("&nbsp;"));
                    categoryEntryBadge.Controls.Add(expandableLink);
                    categoryEntryBadge.Controls.Add(new LiteralControl("&nbsp;"));
                    /*categoryEntryBadge.Controls.Add(atomLink);
                    categoryEntryBadge.Controls.Add( new LiteralControl("&nbsp;"));*/
                    

                    expandableLink.CssClass = "categoryListNestedOutlineBadgeStyle";
                    System.Web.UI.WebControls.Image imgBadge = new System.Web.UI.WebControls.Image();
                    expandableLink.Controls.Add(imgBadge);
                    imgBadge.ID = "img";
                    expandableLink.NavigateUrl = "javascript:ct_toggleExpansionStatus( '"+collapsablePanel.ClientID+"', '"+imgBadge.ClientID+"' )";
                    imgBadge.ImageUrl = requestPage.GetThemedImageUrl("outlinedown");
                    imgBadge.ToolTip = requestPage.CoreStringTables.GetString("tooltip_expand");
                    
               

                    HyperLink hlHead = new HyperLink();
                    hlHead.CssClass = "categoryListNestedOutlineTitleStyle";
                    categoryEntryTitle.Controls.Add(hlHead);
                    hlHead.NavigateUrl = SiteUtilities.GetCategoryViewUrl(requestPage.SiteConfig,categoryEntry.Name);
                    hlHead.Text = categoryEntry.DisplayName;
                    

                    Table list = new Table();
                    list.CellPadding=0;list.CellSpacing=0;
                    list.CssClass = "categoryListNestedOutlineBodyTableStyle";
                    containerPanel.Controls.Add( list );
                                        
                    RenderCategoriesIntoTable( list, categoryEntry.CategoryPath );
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
                    String.Format("var ct_img_expanded = '{0}';\nvar ct_img_collapsed = '{1}';",
                    requestPage.GetThemedImageUrl("outlinedown"),
                    requestPage.GetThemedImageUrl("outlinearrow"));

                string script;
                using ( StreamReader rs = new StreamReader(GetType().Assembly.GetManifestResourceStream(GetType().Namespace+".CategoryListJS.txt")) )
                {
                    script = rs.ReadToEnd();
                }
                Page.ClientScript.RegisterClientScriptBlock(this.GetType(),"script",
                    String.Format("<script type=\"text/javascript\">\n<!--\n{0}\n{1}\n// --></script>",prefix,script));
            }
            if ( !Page.ClientScript.IsClientScriptBlockRegistered(this.GetType(),"style"))
            {
                string script;
                using ( StreamReader rs = new StreamReader(GetType().Assembly.GetManifestResourceStream(GetType().Namespace+".CategoryListCSS.txt")) )
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
            section.Attributes["class"] = "categoryListContainerStyle";
            this.Controls.Add(section);
            

            if ((categories != null) && (categories.Count > 0))
            {
                Table list = new Table();
                list.CssClass = "categoryListTableStyle";
                    
                section.Controls.Add(list);
                RenderCategoriesIntoTable( list, new string[0] );
            }
            else
            {
                section.Controls.Add(new LiteralControl(""));
            }
            
            section.RenderControl( output );
        }
	}
}
