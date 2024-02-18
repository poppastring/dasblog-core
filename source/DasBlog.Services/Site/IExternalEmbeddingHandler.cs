using System.Threading.Tasks;

namespace DasBlog.Services.Site
{
	public interface IExternalEmbeddingHandler
	{
		Task<string> InjectCategoryLinksAsync(string content);
		Task<string> InjectDynamicEmbeddingsAsync(string content);
		Task<string> InjectIconsForBareLinksAsync(string content);
	}
}
