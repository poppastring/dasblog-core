using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace DasBlog.Web.UI.Models.Identity
{
	public class DasBlogUserStore : IUserStore<ApplicationUser>
	{
		public void Dispose()
		{
		}

		public async Task<string> GetUserIdAsync(ApplicationUser user, CancellationToken cancellationToken)
		{
			throw new System.NotImplementedException();
		}

		public async Task<string> GetUserNameAsync(ApplicationUser user, CancellationToken cancellationToken)
		{
			throw new System.NotImplementedException();
		}

		public async Task SetUserNameAsync(ApplicationUser user, string userName, CancellationToken cancellationToken)
		{
			throw new System.NotImplementedException();
		}

		public async Task<string> GetNormalizedUserNameAsync(ApplicationUser user, CancellationToken cancellationToken)
		{
			throw new System.NotImplementedException();
		}

		public async Task SetNormalizedUserNameAsync(ApplicationUser user, string normalizedName, CancellationToken cancellationToken)
		{
			throw new System.NotImplementedException();
		}

		public async Task<IdentityResult> CreateAsync(ApplicationUser user, CancellationToken cancellationToken)
		{
			throw new System.NotImplementedException();
		}

		public async Task<IdentityResult> UpdateAsync(ApplicationUser user, CancellationToken cancellationToken)
		{
			throw new System.NotImplementedException();
		}

		public async Task<IdentityResult> DeleteAsync(ApplicationUser user, CancellationToken cancellationToken)
		{
			throw new System.NotImplementedException();
		}

		public async Task<ApplicationUser> FindByIdAsync(string userId, CancellationToken cancellationToken)
		{
			throw new System.NotImplementedException();
		}

		public async Task<ApplicationUser> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
		{
			throw new System.NotImplementedException();
		}
	}
}
