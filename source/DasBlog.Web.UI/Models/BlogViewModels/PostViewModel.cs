using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DasBlog.Web.UI.Models.BlogViewModels
{
    public class PostViewModel
    {
        public string Content { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Author { get; set; }
        public string PermaLink { get; set; }
        public string EntryId { get; set; }
        public IList<CategoryViewModel> Categories { get; set; }
        public bool AllowComments { get; set; }
        public bool IsPublic { get; set; }
        public DateTime CreatedDateTime { get; set; }
    }
}
