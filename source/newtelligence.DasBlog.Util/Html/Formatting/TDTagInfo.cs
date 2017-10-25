namespace newtelligence.DasBlog.Runtime.Html.Formatting 
{
	internal class TDTagInfo : TagInfo 
	{
		public TDTagInfo() : base("td", FormattingFlags.None, WhiteSpaceType.NotSignificant, WhiteSpaceType.NotSignificant, ElementType.Other) 
		{
		}

		public override bool CanContainTag(TagInfo info) 
		{
			if (info.Type == ElementType.Any) 
			{
				return true;
			}

			if ((info.Type == ElementType.Inline) |
				(info.Type == ElementType.Block)) 
			{
				return true;
			}

			return false;
		}
	}
}
