namespace DasBlog.Tests.Support
{
	public class DasBlogISandboxOptions
	{
		/// <summary>
		/// e.g. "Vanilla"
		/// the environment must be set up in the file system e.g.
		/// source/DasBlog.Tests/Resources/Environments/Vanilla
		/// </summary>
		public string Environment { get; set; }
	}
}
