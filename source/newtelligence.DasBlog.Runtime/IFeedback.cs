
namespace newtelligence.DasBlog.Runtime
{
	public interface IFeedback
	{
		string Author { get; }
		string AuthorEmail { get; }
		string AuthorHomepage { get; }
		string AuthorIPAddress { get; }
		string AuthorUserAgent { get; }
		string FeedbackType { get; }
		string Content { get; }
		string Referer { get; }
		string TargetEntryId { get; }
	}
}
