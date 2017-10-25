
namespace newtelligence.DasBlog.Runtime
{
	public interface ISpamBlockingService
	{
		bool IsSpam(IFeedback feedback);
		void ReportSpam(IFeedback feedback);
		void ReportNotSpam(IFeedback feedback);
	}
}
