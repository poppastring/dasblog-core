using System;
using System.Collections.Generic;
using System.Text;

namespace DasBlog.Core.Configuration
{
    public class MetaTags : IMetaTags
    {
        public string MetaDescription { get; set; }
        public string MetaKeywords  { get; set; }
        public string TwitterCard  { get; set; }
        public string TwitterSite  { get; set; }
        public string TwitterCreator  { get; set; }
        public string TwitterImage  { get; set; }
        public string FaceBookAdmins  { get; set; }
        public string FaceBookAppID  { get; set; }
    }
}
