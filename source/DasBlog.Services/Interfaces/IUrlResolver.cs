using newtelligence.DasBlog.Runtime;

namespace DasBlog.Services
{
	/// <summary>
	/// URL generation and resolution for blog resources.
	/// </summary>
	public interface IUrlResolver
	{
		string RssUrl { get; }
		string PingBackUrl { get; }
		string CategoryUrl { get; }
		string ArchiveUrl { get; }
		string MicroSummaryUrl { get; }
		string RsdUrl { get; }
		string ShortCutIconUrl { get; }
		string ThemeCssUrl { get; }

		string RelativeToRoot(string relative);
		string GetBaseUrl();
		string GetPermaLinkUrl(string entryId);
		string GetPermaTitle(string titleurl);
		string GetTrackbackUrl(string entryId);
		string GetEntryCommentsRssUrl(string entryId);
		string GetCommentViewUrl(string entryId);
		string GetCategoryViewUrl(string category);
		string GetCategoryViewUrlName(string category);
		string GetRssCategoryUrl(string category);
		string GeneratePostUrl(Entry entry);
	}
}
