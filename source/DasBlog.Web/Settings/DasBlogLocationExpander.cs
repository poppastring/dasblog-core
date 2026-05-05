using DasBlog.Services.ConfigFile;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Linq;

namespace DasBlog.Web.Settings
{
	public class DasBlogLocationExpander : IViewLocationExpander
	{
		private const string THEME_KEY = "__theme";

		public IEnumerable<string> ExpandViewLocations(ViewLocationExpanderContext context, IEnumerable<string> viewLocations)
		{
			var theme = context.Values[THEME_KEY];
			var themeLocation = $"/Themes/{theme}/{{0}}.cshtml";
			var listlocations = viewLocations.ToList();
			listlocations.Insert(0, themeLocation);
			return listlocations;
		}

		public void PopulateValues(ViewLocationExpanderContext context)
		{
			var siteConfig = context.ActionContext.HttpContext.RequestServices
				.GetRequiredService<IOptionsMonitor<SiteConfig>>().CurrentValue;
			context.Values[THEME_KEY] = siteConfig.Theme;
		}
	}
}
