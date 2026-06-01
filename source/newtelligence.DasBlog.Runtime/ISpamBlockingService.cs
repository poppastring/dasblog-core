
namespace newtelligence.DasBlog.Runtime
{
	public interface ISpamBlockingService
	{
		/// <summary>
		/// True when the underlying spam-blocking service is configured and ready to make remote calls.
		/// When false, all other members are guaranteed to be cheap no-ops.
		/// </summary>
		bool IsEnabled { get; }

		bool IsSpam(IFeedback feedback);
		void ReportSpam(IFeedback feedback);
		void ReportNotSpam(IFeedback feedback);
	}
}
