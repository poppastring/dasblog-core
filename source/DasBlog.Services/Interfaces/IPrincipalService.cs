using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Principal;

namespace DasBlog.Services.Interfaces
{
    public interface IPrincipalService
    {
		IPrincipal GetPrincipal();
    }
}
