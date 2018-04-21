using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DasBlog.Web.Models.BlogViewModels
{
    public class CategoryViewModel
    {
		public string Category { get; set; }
		public string CategoryUrl { get; set; }
		public bool Checked { get; set; }
	}
}
