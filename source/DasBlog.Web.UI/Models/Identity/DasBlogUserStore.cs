using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using DasBlog.Web.Core;
using DasBlog.Web.Core.Configuration;
using DasBlog.Web.Core.Security;
using Microsoft.AspNetCore.Identity;

namespace DasBlog.Web.UI.Models.Identity
{
	public class DasBlogUserStore : IUserStore<DasBlogUser>, IUserPasswordStore<DasBlogUser>, IUserEmailStore<DasBlogUser>, IUserClaimStore<DasBlogUser>
	{
		private readonly IDasBlogSettings _dasBlogSettings;
		private readonly IMapper _mapper;

		public DasBlogUserStore(IDasBlogSettings dasBlogSettings, IMapper mapper)
		{
			_dasBlogSettings = dasBlogSettings;
			_mapper = mapper;
		}

		#region IUserStore

		public void Dispose()
		{
		}

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

			return Task.FromResult(user.UserName);
		}

		public Task SetUserNameAsync(DasBlogUser user, string userName, CancellationToken cancellationToken)
		{
			throw new System.NotImplementedException();
		}

		public Task<string> GetNormalizedUserNameAsync(DasBlogUser user, CancellationToken cancellationToken)
		{
			throw new System.NotImplementedException();
		}

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
				var mappedUser = _mapper.Map<User>(user);
				_dasBlogSettings.AddUser(mappedUser);
			}
			catch (Exception e)
			{
				return Task.FromResult(IdentityResult.Failed(new IdentityError { Description = e.Message }));
			}

			return Task.FromResult(IdentityResult.Success);
		}

		public Task<IdentityResult> UpdateAsync(DasBlogUser user, CancellationToken cancellationToken)
		{
			throw new System.NotImplementedException();
		}

		public Task<IdentityResult> DeleteAsync(DasBlogUser user, CancellationToken cancellationToken)
		{
			throw new System.NotImplementedException();
		}

		public Task<DasBlogUser> FindByIdAsync(string userId, CancellationToken cancellationToken)
		{
			throw new System.NotImplementedException();
		}

		public Task<DasBlogUser> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
		{
			cancellationToken.ThrowIfCancellationRequested();
			if (normalizedUserName == null)
			{
				throw new ArgumentNullException(nameof(normalizedUserName));
			}

			var user = _dasBlogSettings.SecurityConfiguration.Users.Find(u => u.EmailAddress.Equals(normalizedUserName, StringComparison.InvariantCultureIgnoreCase));
			var dasBlogUser = _mapper.Map<DasBlogUser>(user);
			return Task.FromResult(dasBlogUser);
		}

		#endregion

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
			var user = _dasBlogSettings.SecurityConfiguration.Users.Find(u => u.EmailAddress.ToUpper() == normalizedEmail);
			var dasBlogUser = _mapper.Map<DasBlogUser>(user);
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

		#endregion

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
			if (role.Equals("admin", StringComparison.InvariantCultureIgnoreCase))
			{
				return new List<Claim>()
				{
					new Claim("http://dasblog.org/claims/addpost", "Add Post"),
					new Claim("http://dasblog.org/claims/editpost", "Edit Post"),
					new Claim("http://dasblog.org/claims/editsitesettings", "Edit Site Settings")
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


		#endregion
	}
}
