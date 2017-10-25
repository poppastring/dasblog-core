using System.Web.UI;

namespace newtelligence.DasBlog.Web.Core
{
    /// <summary>
    /// No, this class is absolutely not a joke. It turns a panel
    /// into a naming container. Required for some script wizardry
    /// </summary>
    [ToolboxData("<{0}:NamingPanel runat=server></{0}:NamingPanel>")]
	public class NamingPanel : System.Web.UI.WebControls.Panel, INamingContainer
	{
        public NamingPanel()
        {
            this.Style.Add("padding","0px");
            this.Style.Add("margin","0px");
        }
    }
}                                                         
