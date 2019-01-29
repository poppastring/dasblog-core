namespace newtelligence.DasBlog.Runtime.Html.Formatting 
{
	internal class LITagInfo : TagInfo 
	{
		public LITagInfo() : base("li", FormattingFlags.None, WhiteSpaceType.NotSignificant, WhiteSpaceType.CarryThrough) 
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