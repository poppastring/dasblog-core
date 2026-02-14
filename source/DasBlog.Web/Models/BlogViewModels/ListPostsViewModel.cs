using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DasBlog.Web.Models.BlogViewModels
{
    public class ListPostsViewModel
    {
        public IList<PostViewModel> Posts { get; set; }
    }
}
