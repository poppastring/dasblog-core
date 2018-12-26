using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace DasBlog.Core.Extensions
{
	public static class StringExtensions
	{
		public static string RemoveLineBreaks(this string text)
		{
			text = text.Replace("\r", " ");
			text = text.Replace("\n", " ");
			return text;
		}

		public static string StripHtml(this string text)
		{
			text = Regex.Replace(text, "<.*?>", string.Empty, RegexOptions.Compiled);
			text = text.Replace("<", "");
			text = text.Replace(">", "");
			text = text.Replace("&quot;", "");
			return text;
		}

		public static string RemoveDoubleSpaceCharacters(this string text)
		{
			return Regex.Replace(text, "[ ]+", " ");
		}

		public static string CutLongString(this string text, int length)
		{
			if (text.Trim() != string.Empty)
			{
				if (text.Length > length)
				{
					text = text.Substring(0, length);
					int positionLastSpace = text.LastIndexOf(" ");
					if (positionLastSpace > -1 && positionLastSpace < length)
					{
						text = text.Substring(0, positionLastSpace);
					}
				}
				text += " ...";
			}
			return text;
		}

		public static string RemoveQuotationMarks(this string text)
		{
			const char singleQuotationMark = (char)39;
			text = text.Replace((char)34, singleQuotationMark);   // "
			text = text.Replace((char)168, singleQuotationMark);  // ¨
			text = text.Replace((char)8220, singleQuotationMark); // “
			text = text.Replace((char)8221, singleQuotationMark); // ”
			text = text.Replace((char)8222, singleQuotationMark); // „

			return text;
		}
	}
}
