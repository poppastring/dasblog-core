namespace DasBlog.Services.ActivityLogs
{
	public interface IActivityRepoFactory
	{
		IActivityRepo GetRepo();
	}
}
