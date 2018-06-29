using System;
using System.Collections.Generic;
using System.Security.Principal;
using System.Text;
using Microsoft.AspNetCore.Http;
using DasBlog.Services.Interfaces;
using System.Security.Claims;

namespace DasBlog.Services
{
	public class PrincipalService : IPrincipalService
	{
		private readonly IHttpContextAccessor httpContextAccessor;
		public ClaimsPrincipal GetPrincipal() => httpContextAccessor.HttpContext.User;
		public PrincipalService(IHttpContextAccessor httpContextAccessor)
		{
			this.httpContextAccessor = httpContextAccessor;
		}
	}
}
