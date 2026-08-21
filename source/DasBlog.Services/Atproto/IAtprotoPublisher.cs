using System.Threading;
using System.Threading.Tasks;

namespace DasBlog.Services.Atproto
{
	public interface IAtprotoPublisher
	{
		Task<string> PublishPublicationAsync(CancellationToken cancellationToken = default);
		Task<bool> DeletePublicationAsync(CancellationToken cancellationToken = default);
		string GetPublicationUri();
	}
}
