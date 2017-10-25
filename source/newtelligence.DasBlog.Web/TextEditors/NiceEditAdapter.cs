using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using newtelligence.DasBlog.Web.Core;

namespace newtelligence.DasBlog.Web.TextEditors
{
    public class NiceEditAdapter : EditControlAdapter
    {
        private TextBox control;

        public NiceEditAdapter()
        {

            control = new TextBox();
            control.TextMode = TextBoxMode.MultiLine;
            control.ID = "editor";
        }

        public override void Initialize()
        {
            string niceScriptUrl = "\"//js.nicedit.com/nicEdit-latest.js\"";
            string insertNiceHandler = string.Format("<script language=\"javascript\" type=\"text/javascript\" src={0}></script>", niceScriptUrl);

            if (this.control.Visible)
            {
                insertNiceHandler += @"<script type=""text/javascript"">bkLib.onDomLoaded(nicEditors.allTextAreas);</script>";
            }

            this.Control.Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "insertNiceHandler", insertNiceHandler);
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