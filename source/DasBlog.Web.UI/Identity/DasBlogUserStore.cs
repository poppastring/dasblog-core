using AutoMapper;
using DasBlog.Core.Security;
using DasBlog.Services;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace DasBlog.Web.Identity
{
	public class DasBlogUserStore : IUserStore<DasBlogUser>, IUserPasswordStore<DasBlogUser>, IUserEmailStore<DasBlogUser>, IUserClaimStore<DasBlogUser>
	{
		private readonly IDasBlogSettings dasBlogSettings;
		private readonly IMapper mapper;

		public DasBlogUserStore(IDasBlogSettings dasBlogSettings, IMapper mapper)
		{
			this.dasBlogSettings = dasBlogSettings;
			this.mapper = mapper;
		}

		#region IUserStore

		public Task<string> GetUserIdAsync(DasBlogUser user, CancellationToken cancellationToken)
		{
			if (user == null)
			{
				throw new ArgumentNullException(nameof(user));
			}

			return Task.FromResult(user.UserName);
		}

		public Task<string> GetUserNameAsync(DasBlogUser user, CancellationToken cancellationToken)
		{
			if (user == null)
			{
				throw new ArgumentNullException(nameof(user));
			}

			return Task.FromResult(user.DisplayName);
		}

		public Task SetUserNameAsync(DasBlogUser user, string userName, CancellationToken cancellationToken) => throw new NotImplementedException();

		public Task<string> GetNormalizedUserNameAsync(DasBlogUser user, CancellationToken cancellationToken) => throw new NotImplementedException();

		public Task SetNormalizedUserNameAsync(DasBlogUser user, string normalizedName, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
			if (user == null)
			{
				throw new ArgumentNullException(nameof(user));
			}

			user.NormalizedUserName = normalizedName ?? throw new ArgumentNullException(nameof(normalizedName));
			return Task.FromResult<object>(null);
		}

		public Task<IdentityResult> CreateAsync(DasBlogUser user, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
			if (user == null)
			{
				throw new ArgumentNullException(nameof(user));
			}

			try
			{
				var mappedUser = mapper.Map<User>(user);

				dasBlogSettings.AddUser(mappedUser);
			}
			catch (Exception e)
			{
				return Task.FromResult(IdentityResult.Failed(new IdentityError { Description = e.Message }));
			}

			return Task.FromResult(IdentityResult.Success);
		}

		public Task<IdentityResult> UpdateAsync(DasBlogUser user, CancellationToken cancellationToken) => throw new NotImplementedException();

		public Task<IdentityResult> DeleteAsync(DasBlogUser user, CancellationToken cancellationToken) => throw new NotImplementedException();

		public Task<DasBlogUser> FindByIdAsync(string userId, CancellationToken cancellationToken)
		{
			return FindByNameAsync(userId, cancellationToken);
		}

		public Task<DasBlogUser> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
			if (normalizedUserName == null)
			{
				throw new ArgumentNullException(nameof(normalizedUserName));
			}

			var user = dasBlogSettings.SecurityConfiguration.Users.Find(u => u.EmailAddress.Equals(normalizedUserName, StringComparison.InvariantCultureIgnoreCase));
			var dasBlogUser = mapper.Map<DasBlogUser>(user);
			return Task.FromResult(dasBlogUser);
		}

		#endregion IUserStore

		#region IUserEmailStore

		public Task SetEmailAsync(DasBlogUser user, string email, CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}

		public Task<string> GetEmailAsync(DasBlogUser user, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
			if (user == null)
			{
				throw new ArgumentNullException(nameof(user));
			}

			return Task.FromResult(user.Email);
		}

		public Task<bool> GetEmailConfirmedAsync(DasBlogUser user, CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}

		public Task SetEmailConfirmedAsync(DasBlogUser user, bool confirmed, CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}

		public Task<DasBlogUser> FindByEmailAsync(string normalizedEmail, CancellationToken cancellationToken)
		{
			var user = dasBlogSettings.SecurityConfiguration.Users.Find(u => u.EmailAddress.ToUpper() == normalizedEmail);
			var dasBlogUser = mapper.Map<DasBlogUser>(user);
			return Task.FromResult(dasBlogUser);
		}

		public Task<string> GetNormalizedEmailAsync(DasBlogUser user, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
			if (user == null)
			{
				throw new ArgumentNullException(nameof(user));
			}

			return Task.FromResult(user.NormalizedEmail);
		}

		public Task SetNormalizedEmailAsync(DasBlogUser user, string normalizedEmail, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
			if (user == null)
			{
				throw new ArgumentNullException(nameof(user));
			}

			user.NormalizedEmail = normalizedEmail ?? throw new ArgumentNullException(nameof(normalizedEmail));
			return Task.FromResult<object>(null);
		}

		#endregion IUserEmailStore

		#region IUserPasswordStore

		public Task SetPasswordHashAsync(DasBlogUser user, string passwordHash, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
			if (user == null)
			{
				throw new ArgumentNullException(nameof(user));
			}

			user.PasswordHash = passwordHash ?? throw new ArgumentNullException(nameof(passwordHash));
			return Task.FromResult<object>(null);
		}

		public Task<string> GetPasswordHashAsync(DasBlogUser user, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
			if (user == null)
			{
				throw new ArgumentNullException(nameof(user));
			}

			return Task.FromResult(user.PasswordHash);
		}

		public Task<bool> HasPasswordAsync(DasBlogUser user, CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}

		public Task<IList<Claim>> GetClaimsAsync(DasBlogUser user, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
			if (user == null)
			{
				throw new ArgumentNullException(nameof(user));
			}

			IList<Claim> claims = DefineClaims(user.Role);

			return Task.FromResult(claims);
		}

		private IList<Claim> DefineClaims(string role)
		{
			if (!string.IsNullOrEmpty(role) && role.Equals("admin", StringComparison.InvariantCultureIgnoreCase))
			{
				return new List<Claim>()
				{
					new Claim("http://dasblog.org/claims/addpost", "Add Post"),
					new Claim("http://dasblog.org/claims/editpost", "Edit Post"),
					new Claim("http://dasblog.org/claims/editsitesettings", "Edit Site Settings")
					,new Claim(ClaimTypes.Role, "admin")
				};
			}

			return new List<Claim>()
				{
					new Claim("http://dasblog.org/claims/addpost", "Add Post"),
					new Claim("http://dasblog.org/claims/editpost", "Edit Post"),
				};
		}

		public Task AddClaimsAsync(DasBlogUser user, IEnumerable<Claim> claims, CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}

		public Task ReplaceClaimAsync(DasBlogUser user, Claim claim, Claim newClaim, CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}

		public Task RemoveClaimsAsync(DasBlogUser user, IEnumerable<Claim> claims, CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}

		public Task<IList<DasBlogUser>> GetUsersForClaimAsync(Claim claim, CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}

		#region IDisposable Support

		private bool disposedValue = false; // To detect redundant calls

		protected virtual void Dispose(bool disposing)
		{
			if (!disposedValue)
			{
				if (disposing)
				{
					// TODO: dispose managed state (managed objects).
				}

				// TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
				// TODO: set large fields to null.

				disposedValue = true;
			}
		}

		// TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
		// ~DasBlogUserStore() {
		//   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
		//   Dispose(false);
		// }

		// This code added to correctly implement the disposable pattern.
		public void Dispose()
		{
			// Do not change this code. Put cleanup code in Dispose(bool disposing) above.
			Dispose(true);
			// TODO: uncomment the following line if the finalizer is overridden above.
			// GC.SuppressFinalize(this);
		}

		#endregion IDisposable Support

		#endregion IUserPasswordStore
	}
}
