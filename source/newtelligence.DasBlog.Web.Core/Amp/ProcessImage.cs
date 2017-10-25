using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace newtelligence.DasBlog.Web.Core.Amp
{
    public class ProcessImage : BaseAmpProcess
    {
        public ProcessImage() : base(@"<img.+? src=[""'](.+?)[""'].*? width=[""'](.+?)[""'].*? height=[""'](.+?)[""'].*?>") { }

        protected override string TagMatchEvaluator(Match match)
        {
            string imageTag = match.ToString();

            if (match != null && match.Groups != null && match.Groups.Count > 1)
            {
                imageTag = tagReplacementTemplate.Replace("tag", "amp-img");
                imageTag = string.Format(imageTag, match.Groups[1].Value, match.Groups[2].Value, match.Groups[3].Value);
            }

            return imageTag;
        }
    }
}
