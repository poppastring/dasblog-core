using DasBlog.Web.Models.BlogViewModels;
using Microsoft.AspNetCore.Mvc;

namespace DasBlog.Web.ViewComponents
{
	public class CommentBlockViewComponent : ViewComponent
	{
		public IViewComponentResult Invoke(ListCommentsViewModel comments)
		{
			return View(comments);
		}
	}
}
