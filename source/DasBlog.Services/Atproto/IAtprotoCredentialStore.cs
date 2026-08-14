namespace DasBlog.Services.Atproto
{
	public interface IAtprotoCredentialStore
	{
		string GetAppPassword();
		void SaveAppPassword(string appPassword);
	}
}
