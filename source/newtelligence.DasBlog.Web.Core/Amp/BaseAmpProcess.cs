using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace newtelligence.DasBlog.Web.Core.Amp
{
    public abstract class BaseAmpProcess : IProcessTag
    {
        protected readonly string processRegEx;
        protected const string tagReplacementTemplate = "<tag src=\"{0}\" layout=\"responsive\" width=\"{1}\" height=\"{2}\" ></tag>";

        public BaseAmpProcess(string regex)
        {
            processRegEx = regex;
        }

        public string ReplaceTag(string content)
        {
            Regex urlRx = new Regex(processRegEx, RegexOptions.IgnoreCase | RegexOptions.Compiled);

            return urlRx.Replace(content, new MatchEvaluator(TagMatchEvaluator));
        }

        protected abstract string TagMatchEvaluator(Match match);
    }
}
