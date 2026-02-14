using System;

namespace DasBlog.Services
{
	/// <summary>
	/// HTML filtering and content processing for comments and titles.
	/// </summary>
	public interface IContentProcessor
	{
		string FilterHtml(string input);
		bool AreCommentsPermitted(DateTime blogpostdate);
		string CompressTitle(string title);
	}
}
