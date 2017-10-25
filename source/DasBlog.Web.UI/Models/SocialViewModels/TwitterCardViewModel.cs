using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DasBlog.Web.UI.Models.SocialViewModels
{
    public class TwitterCardViewModel
    {
        public string Card { get; set; }
        public string Site { get; set; }
        public string Creator { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
    }
}
