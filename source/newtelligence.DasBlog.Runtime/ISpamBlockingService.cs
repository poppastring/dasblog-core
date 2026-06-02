
using System.Threading;
using System.Threading.Tasks;

namespace newtelligence.DasBlog.Runtime
{
	public interface ISpamBlockingService
	{
		/// <summary>
		/// True when the underlying spam-blocking service is configured and ready to make remote calls.
		/// When false, all other members are guaranteed to be cheap no-ops.
		/// </summary>
		bool IsEnabled { get; }

		Task<bool> IsSpamAsync(IFeedback feedback, CancellationToken cancellationToken = default);

		/// <summary>
		/// Fire-and-forget. Reports a confirmed-spam comment to the upstream service.
		/// Returns immediately; failures are logged but never propagated.
		/// </summary>
		void ReportSpam(IFeedback feedback);

		/// <summary>
		/// Fire-and-forget. Reports a false-positive (ham) to the upstream service.
		/// Returns immediately; failures are logged but never propagated.
		/// </summary>
		void ReportNotSpam(IFeedback feedback);
	}
}
