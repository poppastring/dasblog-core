using newtelligence.DasBlog.Runtime.Proxies;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

namespace newtelligence.DasBlog.Web.Core
{
    public class SeoMetaTags
    {
        public string MetaDescription { get; set; }
        public string MetaKeywords { get; set; }
        public string TwitterCard { get; set; }
        public string TwitterSite { get; set; }
        public string TwitterCreator { get; set; }
        public string TwitterImage { get; set; }
        public string FaceBookAdmins { get; set; }
        public string FaceBookAppID { get; set; }

        private static string MetaConfigFile
        {
            get { return Path.Combine(SiteConfig.GetConfigPathFromCurrentContext(), "metaConfig.xml"); }
        }

        public SeoMetaTags GetMetaTags()
        {
            var cache = CacheFactory.GetCache();                   
            var seoMetaTags = cache["seoMetaTags"] as SeoMetaTags;            
            if (seoMetaTags == null)
            {
                seoMetaTags = GetMetaTagsFromConfig();
                cache.Insert("seoMetaTags", seoMetaTags, new System.Web.Caching.CacheDependency(MetaConfigFile));                
            }
            return seoMetaTags;
        }

        public static void Save(SeoMetaTags seoConfig)
        {
            System.Security.Principal.WindowsImpersonationContext wi = Impersonation.Impersonate();

            XmlSerializer ser = new XmlSerializer(typeof(SeoMetaTags));

            using (StreamWriter writer = new StreamWriter(MetaConfigFile))
            {
                ser.Serialize(writer, seoConfig);
            }

            wi.Undo();
        }

        private SeoMetaTags GetMetaTagsFromConfig()
        {
            if (!File.Exists(MetaConfigFile))
            {
                return null;
            }

            SeoMetaTags seoMetaTags = new SeoMetaTags();

            using (XmlReader reader = XmlReader.Create(MetaConfigFile))
            {
                while (reader.Read())
                {
                    if (reader.NodeType == XmlNodeType.Element)
                    {
                        switch (reader.Name)
                        {
                            case "MetaDescription":
                                seoMetaTags.MetaDescription = reader.ReadString();
                                break;
                            case "MetaKeywords":
                                seoMetaTags.MetaKeywords = reader.ReadString();
                                break;
                            case "TwitterCard":
                                seoMetaTags.TwitterCard = reader.ReadString();
                                break;
                            case "TwitterSite":
                                seoMetaTags.TwitterSite = reader.ReadString();
                                break;
                            case "TwitterCreator":
                                seoMetaTags.TwitterCreator = reader.ReadString();
                                break;
                            case "TwitterImage":
                                seoMetaTags.TwitterImage = reader.ReadString();
                                break;
                            case "FaceBookAdmins":
                                seoMetaTags.FaceBookAdmins = reader.ReadString();
                                break;
                            case "FaceBookAppID":
                                seoMetaTags.FaceBookAppID = reader.ReadString();
                                break;
                        }
                    }
                }
            }
            return seoMetaTags;
        }
    }
}