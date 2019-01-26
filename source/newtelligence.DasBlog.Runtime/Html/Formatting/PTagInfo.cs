namespace newtelligence.DasBlog.Runtime.Html.Formatting 
{
	internal class PTagInfo : TagInfo 
	{
		public PTagInfo() : base("p", FormattingFlags.None, WhiteSpaceType.NotSignificant, WhiteSpaceType.NotSignificant, ElementType.Block) 
		{
		}

		public override bool CanContainTag(TagInfo info) 
		{
			if (info.Type == ElementType.Any) 
			{
				return true;
			}

			if ((info.Type == ElementType.Inline) |
				(info.TagName.ToLower().Equals("table")) |
				(info.TagName.ToLower().Equals("hr"))) 
			{
				return true;
			}

			return false;
		}
	}
}