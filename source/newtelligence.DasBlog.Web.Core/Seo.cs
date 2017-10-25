using System;
using System.Web;
using newtelligence.DasBlog.Runtime;
using System.Text.RegularExpressions;
using System.Diagnostics;

namespace newtelligence.DasBlog.Web.Core
{
    public static class Seo
    {
        private const string CanonicalLinkPattern = "<link rel=\"canonical\" href=\"{0}\" />\r\n";
        private const string MetaDescriptionTagPattern = "<meta name=\"description\" content=\"{0}\" />\r\n";
        private const string MetaKeywordTagPattern = "<meta name=\"keywords\" content=\"{0}\" />\r\n";
        private const string MetaNoindexFollowPattern = "<meta name=\"robots\" content=\"noindex,follow\" />\r\n";

        private const string MetaTwitterCardPattern = "<meta name=\"twitter:card\" content=\"{0}\" />\r\n ";
        private const string MetaTwitterSitePattern = "<meta name=\"twitter:site\" content=\"{0}\" />\r\n";
        private const string MetaTwitterCreatorPattern = "<meta name=\"twitter:creator\" content=\"{0}\" />\r\n";
        private const string MetaTwitterTitlePattern = "<meta name=\"twitter:title\" content=\"{0}\" />\r\n";
        private const string MetaTwitterDescriptionPattern = "<meta name=\"twitter:description\" content=\"{0}\" />\r\n";
        private const string MetaTwitterImagePattern = "<meta name=\"twitter:image\" content=\"{0}\" />\r\n";

        private const string MetaFaceBookUrlPattern = "<meta name=\"og:url\" content=\"{0}\" />\r\n";
        private const string MetaFaceBookImagePattern = "<meta name=\"og:image\" content=\"{0}\" />\r\n";
        private const string MetaFaceBookTitlePattern = "<meta name=\"og:title\" content=\"{0}\" />\r\n";
        private const string MetaFaceBookDescriptionPattern = "<meta name=\"og:description\" content=\"{0}\" />\r\n";
        private const string MetaFaceBookTypePattern = "<meta name=\"og:type\" content=\"blog\" />\r\n";
        private const string MetaFaceBookAdminsPattern = "<meta name=\"fb:admins\" content=\"{0}\" />\r\n";
        private const string MetaFaceBookAppIDPattern = "<meta name=\"fb:app_id\" content=\"{0}\" />\r\n";

        private const string MetaSchemeOpenScript = "<script type=\"application/ld+json\"> \r\n{\r\n";
        private const string MetaSchemeContext = "\"@context\": \"http://schema.org\",";
        private const string MetaSchemeType = "\"@type\": \"BlogPosting\",";
        private const string MetaSchemeNewsType = "\"@type\": \"NewsArticle\",";
        private const string MetaSchemeHeadline = "\"headline\": \"{0}\",";
        private const string MetaSchemeDatePublished = "\"datePublished\": \"{0}\",";
        private const string MetaSchemeDescription = "\"description\": \"{0}\",";
        private const string MetaSchemeUrl = "\"url\": \"{0}\",";
        private const string MetaSchemeAuthor = "\"author\": \"{0}\",";
        private const string MetaSchemePublisher = "\"publisher\": {{ \"@type\": \"Organization\", \"name\": \"{0}\", \"logo\": \"{1}\" }},";
        private const string MetaSchemeImage = "\"image\": \"{0}\"";
        private const string MetaSchemeCloseScript = "}\r\n</script>\r\n";

        private const string MetaAMPLinkPattern = "<link rel=\"amphtml\" href=\"{0}?amp=1\" />\r\n";

        public static string CreateSeoMetaInformation(EntryCollection weblogEntries, IBlogDataService dataService)
        {
            string metaTags = "\r\n";
            string currentUrl = HttpContext.Current.Request.Url.AbsoluteUri;

            if (currentUrl.IndexOf("categoryview.aspx", StringComparison.OrdinalIgnoreCase) > -1 || currentUrl.IndexOf("default.aspx?month=", StringComparison.OrdinalIgnoreCase) > -1)
            {
                metaTags += MetaNoindexFollowPattern;
            }
            else if (currentUrl.IndexOf("permalink.aspx", StringComparison.OrdinalIgnoreCase) > -1)
            {
                if (weblogEntries.Count >= 1)
                {
                    Entry entry = weblogEntries[0];
                    metaTags = GetMetaTags(metaTags, entry);
                }
            }
            else if (currentUrl.IndexOf("commentview.aspx", StringComparison.OrdinalIgnoreCase) > -1 && !string.IsNullOrEmpty(HttpContext.Current.Request.QueryString["guid"]))
            {
                Entry entry = dataService.GetEntry(HttpContext.Current.Request.QueryString["guid"]);
                if (entry != null)
                {
                    metaTags = GetMetaTags(metaTags, entry);
                }
            }
            else if (currentUrl.IndexOf("commentview.aspx", StringComparison.OrdinalIgnoreCase) > -1 && !string.IsNullOrEmpty(HttpContext.Current.Request.QueryString["title"]))
            {
                Entry entry = dataService.GetEntry(HttpContext.Current.Request.QueryString["title"]);
                if (entry != null)
                {
                    metaTags = GetMetaTags(metaTags, entry);
                }
            }
            else if (currentUrl.IndexOf("default.aspx", StringComparison.OrdinalIgnoreCase) > -1)
            {
                var smt = new SeoMetaTags();
                smt = smt.GetMetaTags();
                if (smt != null)
                {
                    if (!string.IsNullOrEmpty(smt.MetaDescription))
                    {
                        metaTags += string.Format(MetaDescriptionTagPattern, smt.MetaDescription);
                    }

                    if (!string.IsNullOrEmpty(smt.MetaKeywords))
                    {
                        metaTags += string.Format(MetaKeywordTagPattern, smt.MetaKeywords);
                    }
                }
                metaTags += string.Format(CanonicalLinkPattern, SiteUtilities.GetBaseUrl());
            }
            return metaTags;
        }

        public static string CreateAMPSeoMetaInformation(EntryCollection weblogEntries, IBlogDataService dataService)
        {
            string metaTags = "\r\n";
            string blogPostDescription;
            string postImage = string.Empty;

            if (weblogEntries.Count >= 1)
            {
                Entry entry = weblogEntries[0];
                metaTags += string.Format(CanonicalLinkPattern, SiteUtilities.GetPermaLinkUrl(entry));

                blogPostDescription = entry.Content; 

                try
                {
                    postImage = FindFirstImage(blogPostDescription);
                }
                catch (Exception ex)
                {
                    Trace.WriteLine("Exception looking for Images and Video: " + ex.ToString());
                    postImage = string.Empty;
                }

                SiteConfig siteConfig = SiteConfig.GetSiteConfig();

                metaTags += MetaSchemeOpenScript;
                metaTags += MetaSchemeContext;
                metaTags += MetaSchemeNewsType;
                metaTags += string.Format(MetaSchemeHeadline, entry.Title);
                metaTags += string.Format(MetaSchemeDatePublished, entry.CreatedUtc.ToString("yyyy-MM-dd"));
                metaTags += string.Format(MetaSchemeAuthor, siteConfig.Copyright);
                metaTags += string.Format(MetaSchemePublisher, siteConfig.Title, siteConfig.ChannelImageUrl);
                metaTags += string.Format(MetaSchemeImage, siteConfig.ChannelImageUrl);
                metaTags += MetaSchemeCloseScript;
            }

            return metaTags;
        }

        private static string GetMetaTags(string metaTags, Entry entry)
        {
            // Canonical Tag
            metaTags += string.Format(CanonicalLinkPattern, SiteUtilities.GetPermaLinkUrl(entry));

            // Meta Description
            string blogPostDescription;
            string postImage;
            string postVideo;

            blogPostDescription = entry.Content;

            //This is dangerous stuff, don't freak out if it doesn't work.
            try
            {
                postImage = FindFirstImage(blogPostDescription);
                postVideo = FindFirstYouTubeVideo(blogPostDescription);
            }
            catch(Exception ex)
            {
                Trace.WriteLine("Exception looking for Images and Video: " + ex.ToString());
                postImage = string.Empty;
                postVideo = string.Empty;
            }
            blogPostDescription = StipHTMLFromText(blogPostDescription);

            var smt = new SeoMetaTags();
            smt = smt.GetMetaTags();

            if (blogPostDescription.Length == 0)
                blogPostDescription = smt.MetaDescription;

            metaTags += string.Format(MetaDescriptionTagPattern, blogPostDescription);

            // Meta Keywords
            if (!string.IsNullOrEmpty(entry.Categories))
            {
                metaTags += string.Format(MetaKeywordTagPattern, entry.Categories.Replace(';', ','));
            }

            //Twitter SEO Integration
            metaTags += string.Format(MetaTwitterCardPattern, smt.TwitterCard);
            metaTags += string.Format(MetaTwitterSitePattern, smt.TwitterSite);
            metaTags += string.Format(MetaTwitterCreatorPattern, smt.TwitterCreator);
            metaTags += string.Format(MetaTwitterTitlePattern, entry.Title);
            metaTags += string.Format(MetaTwitterDescriptionPattern, blogPostDescription.CutLongString(120));

            SiteConfig siteConfig = SiteConfig.GetSiteConfig();
            if (siteConfig.AMPPagesEnabled)
            {
                metaTags += string.Format(MetaAMPLinkPattern, SiteUtilities.GetPermaLinkUrl(entry));
            }

            if (postImage.Length > 0)
            {
                metaTags += string.Format(MetaTwitterImagePattern, postImage);
            }
            else if(postVideo.Length > 0)
            {
                metaTags += string.Format(MetaTwitterImagePattern, postVideo);
            }
            else
            {
                metaTags += string.Format(MetaTwitterImagePattern, smt.TwitterImage);
            }

            //FaceBook OG Integration
            metaTags += string.Format(MetaFaceBookUrlPattern, SiteUtilities.GetPermaLinkUrl(entry));
            metaTags += string.Format(MetaFaceBookTitlePattern, entry.Title);

            if (postImage.Length > 0)
            {
                metaTags += string.Format(MetaFaceBookImagePattern, postImage);
            }
            else if (postVideo.Length > 0)
            {
                metaTags += string.Format(MetaFaceBookImagePattern, postVideo);
            }
            else
            {
                metaTags += string.Format(MetaFaceBookImagePattern, smt.TwitterImage);
            }

            metaTags += string.Format(MetaFaceBookDescriptionPattern, blogPostDescription.CutLongString(120));
            metaTags += MetaFaceBookTypePattern;
            metaTags += string.Format(MetaFaceBookAdminsPattern, smt.FaceBookAdmins);
            metaTags += string.Format(MetaFaceBookAppIDPattern, smt.FaceBookAppID);

            //Scheme.org meta data integration
            metaTags += MetaSchemeOpenScript;
            metaTags += MetaSchemeContext;
            metaTags += MetaSchemeType;
            metaTags += string.Format(MetaSchemeHeadline, entry.Title);
            metaTags += string.Format(MetaSchemeDatePublished, entry.CreatedUtc.ToString("yyyy-MM-dd"));
            metaTags += string.Format(MetaSchemeDescription, blogPostDescription.CutLongString(240));
            metaTags += string.Format(MetaSchemeUrl, SiteUtilities.GetPermaLinkUrl(entry));
            metaTags += string.Format(MetaSchemeImage, postImage);
            metaTags += MetaSchemeCloseScript;

            return metaTags;
        }

        private static string FindFirstImage(string blogcontent)        
        {
            string firstimage = string.Empty;

            //Look for all the img src tags...
            Regex urlRx = new Regex("<img.+?src=[\"'](.+?)[\"'].*?>", RegexOptions.IgnoreCase | RegexOptions.Compiled);

            MatchCollection matches = urlRx.Matches(blogcontent);

            if (matches != null && matches.Count > 0)
            {
                if (matches[0].Groups != null && matches[0].Groups.Count > 0)
                {
                    firstimage = matches[0].Groups[1].Value.Trim();
                }
            }

            return firstimage.Trim();
        }

        private static string FindFirstYouTubeVideo(string blogcontent)
        {
            string firstVideo = string.Empty;

            //Look for all the img src tags...
            Regex urlRx = new Regex("<iframe.+?src=[\"'](.+?)[\"'].*?>", RegexOptions.IgnoreCase | RegexOptions.Compiled);

            MatchCollection matches = urlRx.Matches(blogcontent);

            if (matches != null && matches.Count > 0)
            {
                if (matches[0].Groups != null && matches[0].Groups.Count > 0)
                {
                    firstVideo = matches[0].Groups[1].Value.Trim();
                }
            }

            return firstVideo.Trim();
        }

        private static string StipHTMLFromText(string text)
        {
            text = HttpUtility.HtmlDecode(text);
            text = text.RemoveLineBreaks();
            text = text.StripHtml();
            text = text.RemoveDoubleSpaceCharacters();
            text = text.Trim();
            text = text.CutLongString(160);
            text = text.RemoveQuotationMarks();

            return text;
        }
    }
}
