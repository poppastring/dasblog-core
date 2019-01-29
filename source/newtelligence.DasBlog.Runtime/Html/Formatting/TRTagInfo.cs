namespace newtelligence.DasBlog.Runtime.Html.Formatting 
{
	internal class TRTagInfo : TagInfo 
	{
		public TRTagInfo() : base("tr", FormattingFlags.None, WhiteSpaceType.NotSignificant, WhiteSpaceType.NotSignificant, ElementType.Other) 
		{
		}

		public override bool CanContainTag(TagInfo info) 
		{
			if (info.Type == ElementType.Any) 
			{
				return true;
			}

			if ((info.TagName.ToLower().Equals("th")) |
				(info.TagName.ToLower().Equals("td"))) 
			{
				return true;
			}

			return false;
		}
	}
}