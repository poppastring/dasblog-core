namespace newtelligence.DasBlog.Runtime.Proxies
{
	/// <summary>
	/// Summary description for UserAgent.
	/// </summary>
	public class Utils
	{
		private Utils()
		{
			
		}

		public static string GetUserAgent()
		{
			string version = typeof(Utils).Assembly.GetName().Version.ToString();
			return "newtelligence dasBlog/" + version;
		}
	}
}
