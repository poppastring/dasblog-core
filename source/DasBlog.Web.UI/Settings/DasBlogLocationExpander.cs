using Microsoft.AspNetCore.Mvc.Razor;
using System.Collections.Generic;
using System.Linq;

namespace DasBlog.Web.Settings
{
    public class DasBlogLocationExpander : IViewLocationExpander
    {
        private const string themeLocation = "/Themes/{0}/{1}";
		private string theme;

        public DasBlogLocationExpander(string theme)
        {
            this.theme = string.Format(themeLocation, theme, "{0}.cshtml");
		}

        public IEnumerable<string> ExpandViewLocations(ViewLocationExpanderContext context, IEnumerable<string> viewLocations)
        {
			var listlocations = viewLocations.ToList();
			listlocations.Insert(0, theme);
			return listlocations;
		}

        public void PopulateValues(ViewLocationExpanderContext context)
        {

        }
    }
}
