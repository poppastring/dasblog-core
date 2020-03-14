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
		private readonly List<string> defaultfolders = new List<string> { "dasblog", "fulcrum", "median"};
		public string Name { get; set; }

		public ThemesListViewModel(IDasBlogSettings dasBlogSettings)
		{
			this.dasBlogSettings = dasBlogSettings;

			var dir = Directory.GetDirectories(Path.Combine(dasBlogSettings.WebRootDirectory, "Themes"));
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
