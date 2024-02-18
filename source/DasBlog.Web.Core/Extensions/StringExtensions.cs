using System;
using System.Collections.Generic;
using System.Net;
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

		public static string StripHTMLFromText(this string text)
		{
			text = WebUtility.HtmlDecode(text);
			text = text.RemoveLineBreaks();
			text = text.StripHtml();
			text = text.RemoveDoubleSpaceCharacters();
			text = text.Trim();
			text = text.RemoveQuotationMarks();

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

		public static string FindFirstImage(this string blogcontent)
		{
			var firstimage = string.Empty;

			//Look for all the img src tags...
			var urlRx = new Regex("<img.+?src=[\"'](.+?)[\"'].*?>", RegexOptions.IgnoreCase | RegexOptions.Compiled);

			var matches = urlRx.Matches(blogcontent);

			if (matches != null && matches.Count > 0)
			{
				if (matches[0].Groups != null && matches[0].Groups.Count > 0)
				{
					firstimage = matches[0].Groups[1].Value.Trim();
				}
			}

			return firstimage.Trim();
		}

		public static string FindHeroImage(this string blogcontent)
		{
			var heroImage = string.Empty;

			var regex = new Regex("<img[^>]*class=\"hero-image\"[^>]*src=\"([^\"]+)\"[^>]*>", RegexOptions.IgnoreCase | RegexOptions.Compiled);

			var match = regex.Match(blogcontent);

			if (match.Success)
			{
				heroImage = match.Groups[1].Value.Trim();
			}

			if (string.IsNullOrEmpty(heroImage))
			{
				heroImage = FindFirstImage(blogcontent);
			}

			return heroImage;
		}

		public static string FindFirstYouTubeVideo(this string blogcontent)
		{
			var firstVideo = string.Empty;

			//Look for all the img src tags...
			var urlRx = new Regex("<iframe.+?src=[\"'](.+?)[\"'].*?>", RegexOptions.IgnoreCase | RegexOptions.Compiled);

			var matches = urlRx.Matches(blogcontent);

			if (matches != null && matches.Count > 0)
			{
				if (matches[0].Groups != null && matches[0].Groups.Count > 0)
				{
					firstVideo = matches[0].Groups[1].Value.Trim();
				}
			}

			return firstVideo.Trim();
		}
	}
}
