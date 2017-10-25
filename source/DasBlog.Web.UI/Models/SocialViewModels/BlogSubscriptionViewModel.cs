using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DasBlog.Web.UI.Models.SocialViewModels
{
    public class BlogSubscriptionViewModel
    {
        public string BlogTitle { get; set; }
        public string RSSUrl { get; set; }
        public string ATOMUrl { get; set; }
        public string RsdUrl { get; set; }
        public string MicroSummaryUrl { get; set; }
    }
}
