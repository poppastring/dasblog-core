using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Principal;
using System.Security.Claims;

namespace DasBlog.Services.Interfaces
{
	/// <summary>
	/// The service provides the Principal details.  Typically
	/// used to detect whether the logged in user has
	/// admin priveleges.
	/// </summary>
    public interface IPrincipalService
    {
		ClaimsPrincipal GetPrincipal();
    }
}
