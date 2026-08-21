using System.Threading;
using System.Threading.Tasks;

namespace DasBlog.Services.Atproto
{
	public interface IAtprotoRepositoryClient
	{
		Task<AtprotoSession> CreateSessionAsync(string pdsUrl, string handle, string appPassword, CancellationToken cancellationToken = default);
		Task PutPublicationAsync(AtprotoSession session, string rkey, AtprotoPublication publication, CancellationToken cancellationToken = default);
		Task DeletePublicationAsync(AtprotoSession session, string rkey, CancellationToken cancellationToken = default);
	}
}
