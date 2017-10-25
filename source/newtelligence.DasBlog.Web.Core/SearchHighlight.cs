using System;
using System.ComponentModel;
using System.IO;
using System.Web.UI;

namespace newtelligence.DasBlog.Web.Core
{
	/// <summary>
	/// Summary description for SearchHighlight.
	/// </summary>
	[DefaultProperty("Text"), 
		ToolboxData("<{0}:SearchHighlight runat=server></{0}:SearchHighlight>")]
	public class SearchHighlight : System.Web.UI.WebControls.WebControl
	{
        [Bindable(true),
            Category("Appearance"),
            DefaultValue("")]
        public string Text
        {
            get;
            set;
        }

		protected override void OnPreRender(EventArgs e)
		{
			base.OnPreRender (e);

			SharedBasePage requestPage = Page as SharedBasePage;

			if ( !Page.ClientScript.IsClientScriptBlockRegistered(this.GetType(),"script"))
			{
				string script;
				using ( StreamReader rs = new StreamReader(GetType().Assembly.GetManifestResourceStream(GetType().Namespace+".SearchHighlightJS.txt")) )
				{
					script = rs.ReadToEnd();
				}

				script = script.Replace("<%siteRoot%>", requestPage.SiteConfig.Root);

				Page.ClientScript.RegisterClientScriptBlock(this.GetType(),"script",
					String.Format("<script type=\"text/javascript\">\n<!--\n{0}\n// -->\n</script>",script));
			}

			if ( !Page.ClientScript.IsClientScriptBlockRegistered(this.GetType(),"style"))
			{
				string script;
				using ( StreamReader rs = new StreamReader(GetType().Assembly.GetManifestResourceStream(GetType().Namespace+".SearchHighlightCSS.txt")) )
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
			output.Write(Text);
		}
	}
}
