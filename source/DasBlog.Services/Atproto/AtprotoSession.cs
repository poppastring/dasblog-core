namespace DasBlog.Services.Atproto
{
	public sealed class AtprotoSession
	{
		public string Did { get; init; }
		public string AccessJwt { get; init; }
		public string PdsUrl { get; init; }
	}
}
