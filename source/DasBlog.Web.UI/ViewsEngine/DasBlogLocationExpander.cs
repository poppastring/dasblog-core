using Microsoft.AspNetCore.Mvc.Razor;
using System.Collections.Generic;
using System.Linq;

namespace DasBlog.Web.ViewsEngine
{
    public class DasBlogLocationExpander : IViewLocationExpander
    {
        private const string _themeLocation = "/Themes/{0}";
        private string _theme;

        public DasBlogLocationExpander(string theme)
        {
            _theme = string.Format(_themeLocation, theme);
        }

        public IEnumerable<string> ExpandViewLocations(ViewLocationExpanderContext context, IEnumerable<string> viewLocations)
        {
            return viewLocations.Select(s => s.Replace("/Views/Shared", _theme));
        }

        public void PopulateValues(ViewLocationExpanderContext context)
        {

        }
    }
}
