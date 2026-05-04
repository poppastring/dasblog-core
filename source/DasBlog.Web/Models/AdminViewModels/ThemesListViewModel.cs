using System.Collections.Generic;
using System.Linq;
using DasBlog.Services;
using DasBlog.Web.Services;

namespace DasBlog.Web.Models.AdminViewModels
{
	public class ThemesListViewModel
	{
		private readonly IDasBlogSettings dasBlogSettings;
		public string Name { get; set; }

		public ThemesListViewModel(IDasBlogSettings dasBlogSettings)
		{
			this.dasBlogSettings = dasBlogSettings;
		}

		public List<ThemesListViewModel> Init()
		{
			var manager = new ThemeManager(dasBlogSettings);
			return manager.ListThemes()
				.Select(t => new ThemesListViewModel(dasBlogSettings) { Name = t.Name })
				.ToList();
		}
	}
}
