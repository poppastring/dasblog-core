using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DasBlog.Web.Models.BlogViewModels
{
	public class ArchiveListViewModel
	{
		public Dictionary<int, IList<PostViewModel>> MonthEntries { get; } = new Dictionary<int, IList<PostViewModel>>();
	}
}
