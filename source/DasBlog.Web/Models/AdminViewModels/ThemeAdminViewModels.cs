using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using DasBlog.Web.Services;

namespace DasBlog.Web.Models.AdminViewModels
{
	public class ThemeAdminListViewModel
	{
		public IReadOnlyList<ThemeInfo> Themes { get; set; } = new List<ThemeInfo>();
		public string ActiveTheme { get; set; }
	}

	public class CreateThemeViewModel
	{
		[Required]
		[Display(Name = "New theme name")]
		[RegularExpression("^[A-Za-z0-9_-]+$",
			ErrorMessage = "Theme names may only contain letters, numbers, hyphens, and underscores.")]
		[StringLength(64, MinimumLength = 1)]
		public string Name { get; set; }

		[Required]
		[Display(Name = "Copy from")]
		public string SourceTheme { get; set; } = "dasblog";

		public IReadOnlyList<string> AvailableSources { get; set; } = new List<string>();
	}

	public class ThemeEditViewModel
	{
		public ThemeInfo Theme { get; set; }
		public IReadOnlyList<ThemeFileInfo> Files { get; set; } = new List<ThemeFileInfo>();
	}

	public class ThemeFileEditViewModel
	{
		public string ThemeName { get; set; }
		public string RelativePath { get; set; }
		public string Content { get; set; }
		public bool IsReadOnly { get; set; }
		public bool IsDefaultTheme { get; set; }
		public bool IsActiveTheme { get; set; }
		public IReadOnlyList<ThemeBackupInfo> Backups { get; set; } = new List<ThemeBackupInfo>();
	}
}
