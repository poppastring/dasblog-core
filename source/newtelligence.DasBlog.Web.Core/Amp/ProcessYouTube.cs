using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace newtelligence.DasBlog.Web.Core.Amp
{
    public class ProcessYouTube : BaseAmpProcess
    {
        private const string youtubeReplacement = "<amp-youtube width=\"480\" height=\"270\" layout=\"responsive\" data-videoid=\"{0}\"></amp-youtube>";

        public ProcessYouTube() : base(@"<iframe.*? src=[""'](.+?)[""'].*?></iframe>") { }

        protected override string TagMatchEvaluator(Match match)
        {
            string htmlTag = match.ToString();

            if (match != null && match.Groups != null && match.Groups.Count > 1)
            {
                var uri = new Uri(match.Groups[1].Value);

                string[] videoid = uri.LocalPath.Split('/');
                if (videoid.Count() > 2)
                {
                    htmlTag = string.Format(youtubeReplacement, videoid[2]);
                }
            }

            return htmlTag;
        }
    }
}
