using DasBlog.Web.Models.BlogViewModels;

namespace DasBlog.Web.Services.Interfaces
{
	public interface IBlogPostViewModelCreator
	{
		PostViewModel CreateBlogPostVM();
		void AddAllLanguages(PostViewModel pvm);

	}
}
