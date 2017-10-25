using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using newtelligence.DasBlog.Web.Core;

namespace newtelligence.DasBlog.Web.TextEditors
{
    public class TinyMCEAdapter : EditControlAdapter
    {
        private TextBox control;

        public TinyMCEAdapter()
        {
            control = new TextBox();
            control.TextMode = TextBoxMode.MultiLine;
            control.CssClass = "MCE";
            control.ID = "MCEID";
        }

        public override void Initialize()
        {
            string insertMCEHandler = string.Format("<script language=\"javascript\" type=\"text/javascript\" src=\"//tinymce.cachefly.net/4.1/tinymce.min.js\"></script><script language=\"javascript\" type=\"text/javascript\" src=\"tinymce/plugins/code/plugin.min.js\"></script>");

            if (this.control.Visible)
            {
                insertMCEHandler += @"<script language=""javascript"" type=""text/javascript"">tinymce.init({selector:'.MCE',plugins:'code'});</script>";
            }

            this.Control.Page.ClientScript.RegisterClientScriptBlock(this.GetType(),"insertMCEHandler", insertMCEHandler);
        }

        public override bool HasText()
        {
            return !string.IsNullOrEmpty(this.Text);
        }

        public override Control Control
        {
            get { return control; }
        }

        public override string Text
        {
            get { return control.Text; }
            set { control.Text = value; }
        }

        public override Unit Width
        {
            get { return control.Width; }
            set { control.Width = value;  }
        }

        public override Unit Height
        {
            get { return control.Height; }
            set { control.Height = value; }
        }
    }
}