using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using newtelligence.DasBlog.Web.Core;

namespace newtelligence.DasBlog.Web.TextEditors
{
	/// <summary>
	/// Converts the TextBox control to the interface needed by dasBlog
	/// </summary>
	public class TextBoxAdapter : EditControlAdapter
	{
		TextBox _Control;

		public TextBoxAdapter()
		{
			_Control = new System.Web.UI.WebControls.TextBox();
			_Control.ID="editControl";
			_Control.TextMode = System.Web.UI.WebControls.TextBoxMode.MultiLine;
		}

		public override Control Control
		{
			get
			{
				return _Control;
			}
		}

		public override string Text 
		{ 
			get { return _Control.Text; }
			set { _Control.Text = value; }
		}

		public override bool HasText() 
		{
			return (_Control.Text.Trim().Length > 0 && _Control.Text.Trim() != "<p></p>");
		}

		public override Unit Width
		{
			get { return _Control.Width; }
			set { _Control.Width = value; }
		}

		public override Unit Height
		{
			get { return _Control.Height; }
			set { _Control.Height = value; }
		}
	}
}
