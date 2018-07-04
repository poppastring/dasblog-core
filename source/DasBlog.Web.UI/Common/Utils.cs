using System.Text.RegularExpressions;

namespace DasBlog.Web.Common
{
	internal static class Utils
	{
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
