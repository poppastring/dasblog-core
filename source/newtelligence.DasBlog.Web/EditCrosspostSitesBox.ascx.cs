using System;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using newtelligence.DasBlog.Web.Core;
using newtelligence.DasBlog.Runtime;
using newtelligence.DasBlog.Runtime.Proxies;
using System.Collections;

namespace newtelligence.DasBlog.Web
{

	/// <summary>                                             
	///		Summary description for EditNavigatorLinksBox.
	/// </summary>
	public partial class EditCrosspostSitesBox : SharedBaseControl
	{
		[TransientPageState]
		public CrosspostSiteCollection crosspostSites;
        
		protected System.Web.UI.WebControls.RequiredFieldValidator validatorRFname;
		protected System.Web.UI.WebControls.Panel Panel1;
		private CrossPostServerInfo[] crossPostServerInfo;
		protected System.Resources.ResourceManager resmgr;
       
		private void SaveSites( )
		{
			SiteConfig siteConfig = SiteConfig.GetSiteConfig(SiteConfig.GetConfigFilePathFromCurrentContext());
			siteConfig.CrosspostSites.Clear();
			siteConfig.CrosspostSites.AddRange(crosspostSites);
			XmlSerializer ser = new XmlSerializer(typeof(SiteConfig));
			using (StreamWriter writer = new StreamWriter(SiteConfig.GetConfigFilePathFromCurrentContext()))
			{
				ser.Serialize(writer, siteConfig);
			}
		}

		protected string TruncateString( string s, int max )
		{
			if ( s == null )
			{
				return "";
			}
			else if ( s.Length < max )
			{
				return s;
			}
			else if ( s.Length >= max )
			{
				return s.Substring(0,max)+"...";
			}
			return s;
		}

		private void LoadSites( )
		{
			SiteConfig siteConfig = SiteConfig.GetSiteConfig(SiteConfig.GetConfigFilePathFromCurrentContext());
			crosspostSites = new CrosspostSiteCollection(siteConfig.CrosspostSites);

			// luke@jurasource.co.uk 24-MAR-04
			// ensure that each allBlogNames property has at
			// least one entry, namely the blog name
			foreach (CrosspostSite site in crosspostSites)
			{
				if ( site.AllBlogNames == null || site.AllBlogNames.Length == 0 )
					site.AllBlogNames = new string[]{site.BlogName};
			}
		}

		public ICollection GetServerInfo() 
		{
			LoadCrossPostServerInfo();

			DataTable dt = new DataTable();
			DataRow dr;
 
			dt.Columns.Add(new DataColumn("Name", typeof(string)));
			dt.Columns.Add(new DataColumn("Value", typeof(string)));
 
			foreach(CrossPostServerInfo info in crossPostServerInfo) 
			{
				dr = dt.NewRow();
 
				dr[0] = info.Name;
				dr[1] = info.Name;
 
				dt.Rows.Add(dr);
			}
 
			DataView dv = new DataView(dt);
			return dv;
		}


		private void LoadCrossPostServerInfo()
		{
			string fullPath = SiteConfig.GetConfigPathFromCurrentContext() + "CrossPostServerInfo.xml";
			
			if (File.Exists(fullPath))
			{
				FileStream fileStream = newtelligence.DasBlog.Util.FileUtils.OpenForRead(fullPath);

				if ( fileStream != null )
				{
					try
					{
						XmlSerializer ser = new XmlSerializer(typeof(CrossPostServerInfo[]));
						StreamReader reader = new StreamReader(fileStream);
						crossPostServerInfo = (CrossPostServerInfo[])ser.Deserialize(reader);
					}
					catch(Exception e)
					{
						ErrorTrace.Trace(System.Diagnostics.TraceLevel.Error,e);
					}
					finally
					{
						fileStream.Close();
					}
				}
			}
			else
			{
				CrossPostServerInfo[] crossPostServerInfo = new CrossPostServerInfo[1];
				CrossPostServerInfo c1 = new CrossPostServerInfo();
				c1.Name = "DasBlog";
				c1.Endpoint = "blogger.aspx";
				c1.Port = 80;
				c1.Service = 1;
				crossPostServerInfo[0] = c1;
			}
		}

		private void BindGrid()
		{
			crosspostSitesGrid.DataSource = crosspostSites;
            crosspostSitesGrid.Columns[0].HeaderText = resmgr.GetString("text_profile");
			DataBind();
		}

		protected void Page_Load(object sender, System.EventArgs e)
		{
			if (SiteSecurity.IsInRole("admin") == false) 
			{
				Response.Redirect("~/FormatPage.aspx?path=SiteConfig/accessdenied.format.html");
			}

			resmgr = ((System.Resources.ResourceManager)ApplicationResourceTable.Get());

			if ( !IsPostBack || crosspostSites == null )
			{
				LoadSites( );			
			}

			BindGrid();		
		
		}

		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
		{
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			InitializeComponent();
			base.OnInit(e);
		}
		
		/// <summary>
		///		Required method for Designer support - do not modify
		///		the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.crosspostSitesGrid.ItemCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.crosspostSitesGrid_ItemCommand);
			this.crosspostSitesGrid.CancelCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.crosspostSitesGrid_CancelCommand);
			this.crosspostSitesGrid.EditCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.crosspostSitesGrid_EditCommand);
			this.crosspostSitesGrid.UpdateCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.crosspostSitesGrid_UpdateCommand);
			this.crosspostSitesGrid.DeleteCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.crosspostSitesGrid_DeleteCommand);

		}
		#endregion

		private void crosspostSitesGrid_EditCommand(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
		{
			crosspostSitesGrid.EditItemIndex = e.Item.ItemIndex;
			DataBind();
            
			// luke@jurasource.co.uk
			// Ensure the correct item in the blog names ddl is selected
			DropDownList allBlogNames = (DropDownList) crosspostSitesGrid.Items[e.Item.ItemIndex].FindControl("listAllBlogNames");
			CrosspostSite site = crosspostSites[e.Item.ItemIndex];

            // james.snape@gmail.com
            // There seems to be an issue with Blogger and accounts with spaces in
            // as it sometimes results in two enties in this list split on the space char.
            // This ensures the page displays even when the list item is missing.
            if (allBlogNames.Items.FindByValue(site.BlogName) != null)
            {
                allBlogNames.SelectedValue = site.BlogName;
            }
		}

		private void crosspostSitesGrid_DeleteCommand(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
		{
			crosspostSites.RemoveAt(e.Item.DataSetIndex);
			crosspostSitesGrid.EditItemIndex = -1;
			DataBind();
			changesAlert.Visible = true;
		}

		private void crosspostSitesGrid_CancelCommand(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
		{
			crosspostSitesGrid.EditItemIndex = -1;
			DataBind();
            
		}

		private void crosspostSitesGrid_UpdateCommand(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
		{
			CrosspostSite site = crosspostSites[e.Item.DataSetIndex];
			site.ProfileName = ((TextBox)e.Item.FindControl("textProfileName")).Text;
			site.HostName = ((TextBox)e.Item.FindControl("textHostName")).Text;
			site.Port = int.Parse(((TextBox)e.Item.FindControl("textPort")).Text);
			site.Endpoint = ((TextBox)e.Item.FindControl("textEndpoint")).Text;
			site.Username = ((TextBox)e.Item.FindControl("textUsername")).Text;
			site.ApiType = ((DropDownList)e.Item.FindControl("listApiType")).SelectedValue;
			TextBox textPassword = ((TextBox)e.Item.FindControl("textPassword"));
			if ( textPassword.Text.Length > 0 )
			{
				site.Password = textPassword.Text;
			}
			// luke@jurasource.co.uk 24-MAR-04
			// Ensure the correct blog is selected from the ddl
			DropDownList allBlogNames = (DropDownList) e.Item.FindControl("listAllBlogNames");
			if ( allBlogNames != null && allBlogNames.SelectedItem.Text != null )
			{
				site.BlogName = allBlogNames.SelectedItem.Text;
				site.BlogId = ((TextBox)e.Item.FindControl(("textBlogId"))).Text;
			}

			crosspostSitesGrid.EditItemIndex = -1;
			DataBind();
            
			changesAlert.Visible = true;
		}                                          

		private void crosspostSitesGrid_PageIndexChanged(object source, System.Web.UI.WebControls.DataGridPageChangedEventArgs e)
		{
			crosspostSitesGrid.CurrentPageIndex = e.NewPageIndex;
			DataBind();
            
		}

		private void crosspostSitesGrid_ItemCommand(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
		{
			if ( e.CommandName == "AddItem" )
			{
				CrosspostSite newSite = new CrosspostSite();
				newSite.ProfileName = "New Profile";
				newSite.Port=80;
				// luke@jurasource 24-MAR-04
				// init all blog names
				newSite.AllBlogNames = new string[]{""};
				crosspostSites.Insert(0,newSite);
				crosspostSitesGrid.CurrentPageIndex = 0;
				crosspostSitesGrid.EditItemIndex = 0;
                
				BindGrid();
			}
			else if ( e.CommandName == "autoFill" )
			{
				DropDownList dropDownList = (DropDownList)e.Item.FindControl("blogSoftware");
				TextBox textHostName = (TextBox)e.Item.FindControl("textHostName");
				TextBox textEndpoint = (TextBox)e.Item.FindControl("textEndpoint");
				TextBox textBoxBlogURL = (TextBox)e.Item.FindControl("textBoxBlogURL");
				DropDownList dropDownApi = (DropDownList)e.Item.FindControl("listApiType");

				string rawBlogUrl = textBoxBlogURL.Text;
				rawBlogUrl = rawBlogUrl.Replace("http://", "");
				string[] blogUrl = rawBlogUrl.Split('/');
				textHostName.Text = blogUrl[0];

				string endpoint = "";
				for (int index = 1; index < blogUrl.Length; index++)
				{
					endpoint += "/" + blogUrl[index];
				}

				CrossPostServerInfo currentInfo = (CrossPostServerInfo)crossPostServerInfo.GetValue(dropDownList.SelectedIndex);
				
				if (endpoint.Length != 0  && endpoint.EndsWith("/") == false)
					endpoint += "/";

				textEndpoint.Text = endpoint + currentInfo.Endpoint;
				dropDownApi.SelectedIndex = currentInfo.Service;
				
			}
			else if ( e.CommandName =="testConnection" )
			{
				Label labelTestError;

				labelTestError = ((Label)e.Item.FindControl("labelTestError"));
				labelTestError.Text = "";
				try
				{
					TextBox textBlogName;
					TextBox textPassword;
                                        
					CrosspostSite site = crosspostSites[e.Item.DataSetIndex];
					site.ProfileName = ((TextBox)e.Item.FindControl("textProfileName")).Text;
					site.HostName = ((TextBox)e.Item.FindControl("textHostName")).Text;
					site.Port = int.Parse(((TextBox)e.Item.FindControl("textPort")).Text);
					site.Endpoint = ((TextBox)e.Item.FindControl("textEndpoint")).Text;
					site.Username = ((TextBox)e.Item.FindControl("textUsername")).Text;
					site.ApiType = ((DropDownList)e.Item.FindControl("listApiType")).SelectedValue;
					textPassword = ((TextBox)e.Item.FindControl("textPassword"));
					if ( textPassword.Text.Length > 0 )
					{
						site.Password = textPassword.Text;
					}
					textBlogName = ((TextBox)e.Item.FindControl("textBlogName"));
                    

					UriBuilder uriBuilder = new UriBuilder("http",site.HostName,site.Port,site.Endpoint);
					BloggerAPIClientProxy proxy = new BloggerAPIClientProxy();
					proxy.Url = uriBuilder.ToString();
					proxy.UserAgent="newtelligence dasBlog/1.4";
					bgBlogInfo[] blogInfos = proxy.blogger_getUsersBlogs("",site.Username, site.Password);
					if ( blogInfos.Length > 0 )
					{
						// luke@jurasource.co.uk 24-MAR-04
						// refresh all the blog names for this crosspost site
						string[] allBlogNames = new string[blogInfos.Length];
						
						for (int blog=0; blog<blogInfos.Length; blog++)
							allBlogNames[blog] = blogInfos[blog].blogName;

						site.AllBlogNames = allBlogNames;

						// Default the crosspost blog to the first one
						site.BlogName = textBlogName.Text = blogInfos[0].blogName;
						site.BlogId = blogInfos[0].blogid;
					}                                       

					BindGrid();
                    

				}
				catch( Exception exc )
				{
					labelTestError.Text = exc.Message;             
				}
				finally
				{

				}
			}
		}

        

		protected void buttonSaveChanges_Click(object sender, System.EventArgs e)
		{
			SaveSites();
			changesAlert.Visible = false;
		}

		protected void crosspostSitesGrid_Load(object sender, System.EventArgs e)
		{
			
		}
	}
}
