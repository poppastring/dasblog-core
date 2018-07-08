using System.Collections.Generic;
using DasBlog.Core.Security;

namespace DasBlog.Core.Services.Interfaces
{
	public interface ILocalUserDataService
	{
		IEnumerable<User> LoadUsers();
	}
}
