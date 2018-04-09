using System;

namespace DasBlog.Web.Models.SocialViewModels
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
