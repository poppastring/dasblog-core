using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DasBlog.Web.UI.Models.SocialViewModels
{
    public class OpenGraphViewModel
    {
        public string Url { get; set; }
        public string Title { get; set; }
        public string Image { get; set; }
        public string Description { get; set; }
        public string Type { get; set; }
    }
}
