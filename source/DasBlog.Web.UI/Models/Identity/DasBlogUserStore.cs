using System;
using System.Diagnostics.Contracts;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using DasBlog.Web.Core.Configuration;
using DasBlog.Web.Core.Security;
using Microsoft.AspNetCore.Identity;

namespace DasBlog.Web.UI.Models.Identity
{
	public class DasBlogUserStore : IUserStore<DasBlogUser>
	{
		private readonly ISiteSecurityConfig _siteSecurityConfig;
		private readonly IMapper _mapper;

		public DasBlogUserStore(ISiteSecurityConfig siteSecurityConfig, IMapper mapper)
		{
			_siteSecurityConfig = siteSecurityConfig;
			_mapper = mapper;
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
			var userName = _siteSecurityConfig.Users.Find(u => u.EmailAddress == user.Email)?.EmailAddress;
			return await Task.FromResult(userName);
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
			cancellationToken.ThrowIfCancellationRequested();
			Contract.Requires<ArgumentNullException>(user != null, $"{nameof(user)} is null");

			var mappedUser = _mapper.Map<User>(user);
			try
			{
				_siteSecurityConfig.Users.Add(mappedUser);
			}
			catch (Exception e)
			{
				return IdentityResult.Failed(new IdentityError { Description = e.Message });
			}

			return await Task.FromResult(IdentityResult.Success);
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
