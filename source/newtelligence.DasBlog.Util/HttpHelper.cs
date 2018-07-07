using System.Text;
using System.Text.RegularExpressions;

namespace newtelligence.DasBlog.Util
{
    public static class HttpHelper
    {
	    /// <summary>
	    /// replaces almost all non-alpha-numeric characters with '-'
	    /// has asimilar effect to HtmlEncoding but produces more agreeable urls.
	    /// Note that in dasBlog this function produced camel case for
	    /// dasBlog-core it now produces snake case
	    /// </summary>
	    /// <param name="urlString">e.g. marks&spencer</param>
	    /// <returns>e.g. marks-spencer</returns>
        public static string GetURLSafeString(string urlString)
        {
	        return EncodeCategoryUrl(urlString, "-");
        }
	    /// DUPLICATE of DasBlog.Web.Common.Utils.EncodeCategoryUrl
	    /// <summary>
	    /// converts a category display text to a safe url using separator (typically '-') to replace
	    /// dangwerous characters
	    /// </summary>
	    /// <param name="displayText">e.g Pots&amp;Pans</param>
	    /// <param name="separator">typically config TitlePermalinkSpaceReplacement - '-'</param>
	    /// <returns>pots-pans</returns>
	    public static string EncodeCategoryUrl(string displayText, string separator)
		    => Regex.Replace(displayText.ToLower(), @"[^A-Za-z0-9_\.~]+", separator);

    }
}
