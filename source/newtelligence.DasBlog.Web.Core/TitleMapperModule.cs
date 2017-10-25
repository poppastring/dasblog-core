using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using System.Web;

namespace newtelligence.DasBlog.Web.Core
{

    public class TitleMapperModule : IHttpModule
    {
        /// <summary>
        ///  A description of the regular expression:
        ///  
        ///  [Year]: A named capture group. [(?:\d{4})]
        ///      Match expression but don't capture it. [\d{4}]
        ///          Any digit, exactly 4 repetitions
        ///  /
        ///  [Month]: A named capture group. [\d{1,2}]
        ///      Any digit, between 1 and 2 repetitions
        ///  /
        ///  [Day]: A named capture group. [\d{1,2}]
        ///      Any digit, between 1 and 2 repetitions
        /// </summary>
        static readonly Regex _yearMonthDayPattern =
            new Regex(@"(?<Year>(?:\d{4}))/(?<Month>\d{1,2})/(?<Day>\d{1,2})",
                      RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.IgnorePatternWhitespace |
                      RegexOptions.Compiled);
        
        /// <summary>
        /// A hard-coded list of strings that can be used as a replacement for the space in a post title permalink.
        /// The string at position 0, is considered the default string.
        /// </summary>
        private static readonly ReadOnlyCollection<string> titlePermalinkSpaceReplacementOptions = new ReadOnlyCollection<string>(new List<string>(new string[2]{ "+", "-" }));
        
        /// <summary>
        /// Gets the title permalink space replacement options.
        /// </summary>
        /// <value>The title permalink space replacements.</value>
        public static ReadOnlyCollection<string> TitlePermalinkSpaceReplacementOptions { get { return titlePermalinkSpaceReplacementOptions; } }

        /// <summary>
        /// Gets the default title permalink space replacement.
        /// </summary>
        /// <value>The default title permalink space replacement.</value>
        public static string DefaultTitlePermalinkSpaceReplacement { get { return TitlePermalinkSpaceReplacementOptions[0]; } }

        //AGB - 10 June 2008
        static readonly string[] HttpHandlerStrings = TryGetExclusions();

        //AGB - 10 June 2008 - helper method to initialize the HttpHandlerStrings array - falling back to the 
        //original values if the new TitleMapperModuleSectionHandler configuration section is not present.
        private static string[] TryGetExclusions()
        {
            string[] exclusions;

            if (TitleMapperModuleSectionHandler.Settings != null && TitleMapperModuleSectionHandler.Settings.Exclusions.Count > 0)
            {
                exclusions = new string[TitleMapperModuleSectionHandler.Settings.Exclusions.Count];
                for (int i = 0; i < exclusions.Length; i++)
                {
                    exclusions[i] = TitleMapperModuleSectionHandler.Settings.Exclusions[i].Path;
                }
            }
            else
            {
                exclusions = new string[]
			    {
				    "CaptchaImage.aspx",
				    "aggbug.ashx",
				    "blogger.aspx",
				    "pingback.aspx",
				    "trackback.aspx",
				    "get_aspx_ver.aspx"                    
			    };
            }

            return exclusions;
        }

        #region IHttpModule Members
        public void Init(HttpApplication context)
        {
            SiteConfig config = SiteConfig.GetSiteConfig();
            if (config.EnableTitlePermaLink)
            {
                context.BeginRequest += HandleBeginRequest;
            }
        }

        public void Dispose()
        {
        }
        #endregion

        static void HandleBeginRequest(object sender, EventArgs e)
        {
            HttpApplication app = sender as HttpApplication;
            if (app == null)
            {
                return;
            }

            string requestUrl = app.Context.Request.Url.ToString();
            if (requestUrl.Contains(",") || requestUrl.Contains("?"))
            {
                return;
            }

            // need to detect the default url because that is also pointing default.aspx
            bool fileExists = File.Exists(app.Context.Server.MapPath(app.Context.Request.FilePath)) || 
                app.Context.Request.ApplicationPath.TrimEnd('/') == app.Context.Request.FilePath.TrimEnd('/');

            // TODO: there has to be a way to also see if the request is from an HttpHander since File.Exists won't work.
            //if (!requestUrl.EndsWith(".aspx", StringComparison.InvariantCultureIgnoreCase) ||
            //    fileExists)
            //{
            //    return;
            //}

            if (fileExists) return;

            string title = app.Context.Request.Url.Segments[app.Context.Request.Url.Segments.Length - 1];

            if (Array.Exists(HttpHandlerStrings, delegate(string x)
            {
                // use case insensitive compare to check for match
                return String.Compare(x, title, StringComparison.OrdinalIgnoreCase) == 0;
            }))
            {
                return;
            }

            //TODO: If a config option says ExtensionLess...
            bool extensionlessPosts = SiteConfig.GetSiteConfig().ExtensionlessUrls;
            if (extensionlessPosts)
            {
                if (!fileExists && requestUrl.EndsWith(".aspx"))
                {
                    string newUrl = requestUrl.Replace(".aspx", "");
                    app.Context.Response.RedirectPermanent(newUrl);
                    return;
                }
            }

            title = title.Replace(".aspx", "");
            
            //replace any of the custom spacer strings with ""
            foreach (string spacer in TitlePermalinkSpaceReplacementOptions)
            {
                title = title.Replace(spacer, "");    
            }
            
            title = title.Replace(" ", "");
            title = title.Replace("%2b", "");
            title = title.Replace("%20", "");

            string requestParams = String.Format("title={0}", title);

            // Try to be more specific by matching the date pattern.
            Match match = _yearMonthDayPattern.Match(app.Context.Request.Url.LocalPath);
            if (match.Groups.Count == 4)
            {
                try
                {
                    int year = Convert.ToInt32(match.Groups["Year"].Value);
                    int month = Convert.ToInt32(match.Groups["Month"].Value);
                    int day = Convert.ToInt32(match.Groups["Day"].Value);

                    DateTime dateTime = new DateTime(year, month, day);
                    if (dateTime != DateTime.MinValue)
                    {
                        requestParams = String.Format("{0}&date={1}", requestParams, dateTime.ToString("yyyy-MM-dd"));
                    }
                }
                catch (ArgumentException)
                {
                }
            }

            // Append original query to the list of parameters, skip the leading "?".
            if (!String.IsNullOrEmpty(app.Context.Request.Url.Query))
            {
                requestParams = String.Format("{0}&{1}", requestParams, app.Context.Request.Url.Query.Substring(1));
            }


            SiteConfig config = SiteConfig.GetSiteConfig();

            //and NOT doing amp...no comments when AMP is active...
            if (config.EnableComments && config.ShowCommentsWhenViewingEntry
                && String.IsNullOrWhiteSpace(app.Context.Request.QueryString["amp"]))
            {
                requestUrl = String.Format("~/CommentView.aspx?{0}", requestParams);
            }
            else
            {
                requestUrl = String.Format("~/Permalink.aspx?{0}", requestParams);
            }

            app.Context.RewritePath(requestUrl);
        }
    }
}
