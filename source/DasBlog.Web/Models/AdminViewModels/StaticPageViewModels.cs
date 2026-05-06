using System.Collections.Generic;
using DasBlog.Web.Services;

namespace DasBlog.Web.Models.AdminViewModels
{
	public class StaticPageListViewModel
	{
		public IReadOnlyList<StaticPageItemInfo> Pages { get; set; } = new List<StaticPageItemInfo>();
	}

	public class StaticPageEditViewModel
	{
		public string Name { get; set; }
		public string DisplayTitle { get; set; }
		public string PublicUrlPath { get; set; }
		public string Content { get; set; }
		public bool Exists { get; set; }
		public IReadOnlyList<StaticPageBackupInfo> Backups { get; set; } = new List<StaticPageBackupInfo>();
	}
}
