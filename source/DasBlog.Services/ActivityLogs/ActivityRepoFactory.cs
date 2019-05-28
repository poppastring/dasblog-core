using Microsoft.Extensions.Options;

namespace DasBlog.Services.ActivityLogs
{
	public class ActivityRepoFactory : IActivityRepoFactory
	{
		public string Path { get; set; }

		public ActivityRepoFactory(IOptions<ActivityRepoOptions> options)
		{
			Path = options.Value.Path;
		}
		public IActivityRepo GetRepo()
		{
			return new ActivityRepo(Path);
		}
	}
}
