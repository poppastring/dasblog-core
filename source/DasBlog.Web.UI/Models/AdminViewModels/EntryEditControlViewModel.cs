using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DasBlog.Web.Models.AdminViewModels
{
	public class EntryEditControlViewModel
	{
		public string Name { get; set; }

		public List<EntryEditControlViewModel> Init()
		{
			return new List<EntryEditControlViewModel>() {
				new EntryEditControlViewModel { Name = "Froala" },
				new EntryEditControlViewModel { Name = "TinyMce" },
				new EntryEditControlViewModel { Name = "NicEdit" },
				new EntryEditControlViewModel { Name = "TextArea" }

			};
		}
	}
}
