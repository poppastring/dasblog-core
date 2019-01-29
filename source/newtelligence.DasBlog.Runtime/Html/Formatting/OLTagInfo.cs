namespace newtelligence.DasBlog.Runtime.Html.Formatting 
{
	internal class OLTagInfo : TagInfo 
	{
		public OLTagInfo() : base("ol", FormattingFlags.None, WhiteSpaceType.NotSignificant, WhiteSpaceType.NotSignificant, ElementType.Block) 
		{
		}

		public override bool CanContainTag(TagInfo info) 
		{
			if (info.Type == ElementType.Any) 
			{
				return true;
			}

			if (info.TagName.ToLower().Equals("li")) 
			{
				return true;
			}

			return false;
		}
	}
}