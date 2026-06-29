namespace DasBlog.Services
{
	public interface IMastodonSettingsResolver
	{
		string GetMastodonServerUrl();
		string GetMastodonAccount();
	}
}
