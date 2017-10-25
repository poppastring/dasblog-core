using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace newtelligence.DasBlog.Web.Core.Amp
{
    public class ProcessTwitter : BaseAmpProcess
    {
        private const string twitterReplacement = "<amp-twitter width = \"390\" height=\"330\" layout=\"responsive\" data-tweetid=\"{0}\" data-cards=\"hidden\"></amp-twitter>";

        public ProcessTwitter() : base(@"<script .*?platform.twitter.com.*?></script>")
        {
        }

        protected override string TagMatchEvaluator(Match match)
        {
            string twitterTag = match.ToString();

            if (match != null && match.Groups != null && match.Groups.Count > 1)
            {
                twitterTag = string.Format(twitterReplacement, match.Groups[1].Value);
            }

            return string.Empty;
        }
    }
}
