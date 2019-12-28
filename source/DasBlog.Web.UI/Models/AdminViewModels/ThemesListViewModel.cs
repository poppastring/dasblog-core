using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using DasBlog.Services;
using Microsoft.Extensions.Hosting;

namespace DasBlog.Web.Models.AdminViewModels
{
	public class ThemesListViewModel
	{
		private IDasBlogSettings dasBlogSettings;
		private List<string> defaultfolders = new List<string> { "dasblog", "fulcrum", "median"};
		public string Name { get; set; }

		public ThemesListViewModel(IDasBlogSettings dasBlogSettings)
		{
			this.dasBlogSettings = dasBlogSettings;

			var dir = Directory.GetDirectories(string.Format("{0}\\Themes", dasBlogSettings.WebRootDirectory));
			dir.Select(f => Path.GetDirectoryName(f));
			
			defaultfolders.AddRange(dir.Select(f => Path.GetFileName(f)).ToList());
		}

		public List<ThemesListViewModel> Init()
		{
			var themelist = defaultfolders.Distinct(StringComparer.CurrentCultureIgnoreCase).Select(o => new ThemesListViewModel(dasBlogSettings) {Name = o }).ToList();

			return themelist;
		}
	}
}
