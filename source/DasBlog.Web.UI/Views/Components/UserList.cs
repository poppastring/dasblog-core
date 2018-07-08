using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DasBlog.Core.Security;
using Microsoft.AspNetCore.Mvc;
using DasBlog.Core.Services.Interfaces;

namespace DasBlog.Web.Components
{
	public class UserList : ViewComponent
	{
		private ILocalUserDataService _dataService;
		public UserList( ILocalUserDataService dataService)
		{
			this._dataService = dataService;
           
		}
		public async Task<IViewComponentResult> InvokeAsync()
		{
			IEnumerable<User> list = _dataService.LoadUsers();
			await Task.Yield();
			return View(list);
		}
        
	}
}
