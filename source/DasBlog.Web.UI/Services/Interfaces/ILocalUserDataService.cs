using System.Collections.Generic;
using DasBlog.Core.Security;

namespace DasBlog.Web.Services.Interfaces
{
	public interface ILocalUserDataService
	{
		IEnumerable<User> LoadUsers();
	}
}
