using System.Collections.Generic;
using System.Threading.Tasks;
using DasBlog.Core.Security;
using Microsoft.AspNetCore.Mvc;
using DasBlog.Services.Users;

namespace DasBlog.Web.Components
{
	public class UserList : ViewComponent
	{
		private IUserService _userService;
		public UserList( IUserService userService)
		{
			this._userService = userService;
           
		}
		public async Task<IViewComponentResult> InvokeAsync()
		{
			IEnumerable<User> list = _userService.GetAllUsers();
			await Task.Yield();
			return View(list);
		}
        
	}
}
