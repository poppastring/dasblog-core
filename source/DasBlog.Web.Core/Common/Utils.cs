using System;
using System.Linq.Expressions;
using System.Text.RegularExpressions;

namespace DasBlog.Core.Common
{
	public static class Utils
	{
		/// DUPLICATE of newtelligence.DasBlog.Util.HtmlHelper.EncodeCategoryUrl
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

	public static class Veriifier
	{
		public static void VerifyParam(Expression<Func<bool>> pred)
		{
			if (!pred.Compile()())
			{
				throw new Exception($"The following expectation was not met {pred}");
							// a bloke's got to have a bit of fun
			}
		}
	}
}
