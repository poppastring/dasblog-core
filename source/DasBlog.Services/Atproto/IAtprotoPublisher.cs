using System.Threading;
using System.Threading.Tasks;

namespace DasBlog.Services.Atproto
{
	public interface IAtprotoPublisher
	{
		Task<string> EnsurePublicationAsync(CancellationToken cancellationToken = default);
	}
}
