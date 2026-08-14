namespace DasBlog.Services.Atproto
{
	public interface IAtprotoSettingsResolver
	{
		bool IsEnabled();
		string GetHandle();
		string GetPdsUrl();
		string GetPublicationRkey();
		string GetAppPassword();
	}
}
