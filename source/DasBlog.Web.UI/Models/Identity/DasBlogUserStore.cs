using System.Threading;
using System.Threading.Tasks;
using DasBlog.Web.Core.Configuration;
using Microsoft.AspNetCore.Identity;

namespace DasBlog.Web.UI.Models.Identity
{
	public class DasBlogUserStore : IUserStore<DasBlogUser>
	{
		private readonly ISiteSecurityConfig _siteSecurityConfig;

		public DasBlogUserStore(ISiteSecurityConfig siteSecurityConfig)
		{
			_siteSecurityConfig = siteSecurityConfig;
		}

		public void Dispose()
		{
		}

		public async Task<string> GetUserIdAsync(DasBlogUser user, CancellationToken cancellationToken)
		{
			throw new System.NotImplementedException();
		}

		public async Task<string> GetUserNameAsync(DasBlogUser user, CancellationToken cancellationToken)
		{
			throw new System.NotImplementedException();
		}

		public async Task SetUserNameAsync(DasBlogUser user, string userName, CancellationToken cancellationToken)
		{
			throw new System.NotImplementedException();
		}

		public async Task<string> GetNormalizedUserNameAsync(DasBlogUser user, CancellationToken cancellationToken)
		{
			throw new System.NotImplementedException();
		}

		public async Task SetNormalizedUserNameAsync(DasBlogUser user, string normalizedName, CancellationToken cancellationToken)
		{
			throw new System.NotImplementedException();
		}

		public async Task<IdentityResult> CreateAsync(DasBlogUser user, CancellationToken cancellationToken)
		{
			throw new System.NotImplementedException();
		}

		public async Task<IdentityResult> UpdateAsync(DasBlogUser user, CancellationToken cancellationToken)
		{
			throw new System.NotImplementedException();
		}

		public async Task<IdentityResult> DeleteAsync(DasBlogUser user, CancellationToken cancellationToken)
		{
			throw new System.NotImplementedException();
		}

		public async Task<DasBlogUser> FindByIdAsync(string userId, CancellationToken cancellationToken)
		{
			throw new System.NotImplementedException();
		}

		public async Task<DasBlogUser> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
		{
			throw new System.NotImplementedException();
		}
	}
}
