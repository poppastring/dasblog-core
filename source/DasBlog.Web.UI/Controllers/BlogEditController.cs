using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using newtelligence.DasBlog.Runtime;
using DasBlog.Web.Core;
using DasBlog.Web.Repositories.Interfaces;
using Microsoft.AspNetCore.Http;

namespace DasBlog.Web.UI.Controllers
{
	//Authentication...
	[Route("Blogger")]
	public class BlogEditController : Controller
	{
		private IArchiveRepository _archiveRepository;
		private IHttpContextAccessor _httpContextAccessor;

		public BlogEditController(IArchiveRepository archiveRepository, IHttpContextAccessor httpContextAccessor)
		{
			_archiveRepository = archiveRepository;
			_httpContextAccessor = httpContextAccessor;
		}
	}
}
