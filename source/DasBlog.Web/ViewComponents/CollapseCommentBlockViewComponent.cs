using DasBlog.Core.Common;
using DasBlog.Web.Models.BlogViewModels;
using Microsoft.AspNetCore.Mvc;

namespace DasBlog.Web.ViewComponents
{
	public class CollapseCommentBlockViewComponent : ViewComponent
	{
		public IViewComponentResult Invoke(PostViewModel post, bool? showPageControl = null)
		{
			var effectiveShowPageControl = showPageControl
				?? (ViewData[Constants.ShowPageControl] as bool?)
				?? false;

			var vm = new CollapseCommentBlockViewModel
			{
				Post = post,
				ShowPageControl = effectiveShowPageControl
			};
			return View(vm);
		}
	}

	public class CollapseCommentBlockViewModel
	{
		public PostViewModel Post { get; set; }
		public bool ShowPageControl { get; set; }
	}
}
