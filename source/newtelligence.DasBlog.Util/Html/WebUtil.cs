using System;
using System.Collections.Generic;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace newtelligence.DasBlog.Util.Html
{
	public static class WebUtil
	{
		public static string CreateEnableDisableScript(CheckBox parent, params Control[] controls)
		{
			if (parent == null)
			{
				throw new ArgumentNullException("parent");
			}

			if (controls.Length == 0)
			{
				throw new ArgumentOutOfRangeException("controls");
			}

			string[] controlClientIDs = Array.ConvertAll<Control, string>(controls,
			                                                              delegate(Control control) { return control.ClientID; });

			return CreateEnableDisableScript(parent, controlClientIDs);
		}

		public static string CreateEnableDisableScript(CheckBox parent, ListControl control)
		{
			if (parent == null)
			{
				throw new ArgumentNullException("parent");
			}

			List<string> controlClientIDs = new List<string>();
			for (int i = 0; i < control.Items.Count; i++)
			{
				controlClientIDs.Add(String.Format("{0}_{1}", control.ClientID, i));
			}

			return CreateEnableDisableScript(parent, controlClientIDs.ToArray());
		}

		public static string CreateEnableDisableScript(CheckBox parent, string[] controlClientIDs)
		{
			if (!parent.Page.ClientScript.IsClientScriptBlockRegistered(typeof(WebUtil), "EnableDisableScript"))
			{
				parent.Page.ClientScript.RegisterClientScriptBlock(typeof(WebUtil), "EnableDisableScript",
				                                                   Resources.WebUtil.PerformCheck);
			}

			string result = String.Format("javascript:performCheck('{0}', '{1}');",
			                              parent.ClientID,
			                              String.Join(",", controlClientIDs));

			return result;
		}
	}
}