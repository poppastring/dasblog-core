using Microsoft.AspNetCore.Mvc.Razor;
using System.Collections.Generic;
using System.Linq;

namespace DasBlog.Web.Settings
{
    public class DasBlogLocationExpander : IViewLocationExpander
    {
        private const string _themeLocation = "/Themes/{0}/{1}";

		private string _theme;

        public DasBlogLocationExpander(string theme)
        {
            _theme = string.Format(_themeLocation, theme, "{0}.cshtml");
		}

        public IEnumerable<string> ExpandViewLocations(ViewLocationExpanderContext context, IEnumerable<string> viewLocations)
        {
			var listlocations = viewLocations.ToList();
			listlocations.Add(_theme);
			return listlocations;
		}

        public void PopulateValues(ViewLocationExpanderContext context)
        {

        }
    }
}
