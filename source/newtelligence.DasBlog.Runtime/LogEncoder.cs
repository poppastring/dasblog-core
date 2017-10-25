using System.Text.RegularExpressions;

namespace newtelligence.DasBlog.Runtime
{
	public static class LogEncoder
	{
		static readonly Regex badTagsRegEx =
			new Regex("<(?!span|/span|br/|br|/br|p|/p|a|/a)([^\"]+?|.+?\".*?\".*?)>",
			          RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.Singleline);

		public static string Encode(string input)
		{
			if (input != null)
			{
				return input.Replace("<", "&lt;").Replace(">", "&gt;");
			}
			else
			{
				return null;
			}
		}

		public static string EncodeBadTags(string input)
		{
			if (input != null)
			{
				foreach (Match match in badTagsRegEx.Matches(input))
				{
					input = input.Replace(match.Value, match.Value.Replace("<", "&lt;").Replace(">", "&gt;"));
				}

				return input;
			}
			else
			{
				return null;
			}
		}
	}
}