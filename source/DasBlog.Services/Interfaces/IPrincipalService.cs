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
	/// The Principal is guaranteed to be up-to-date for each request by
	/// the time action methods are hit.
	/// </summary>
    public interface IPrincipalService
    {
		ClaimsPrincipal GetPrincipal();
    }
}
