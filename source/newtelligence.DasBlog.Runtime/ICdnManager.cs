namespace newtelligence.DasBlog.Runtime
{
	/// <summary>
	/// When the site is configured with a "CdnRoot" setting,
	/// this manager is used to create content URIs for the CDN location.
	/// </summary>
	public interface ICdnManager
	{
		/// <summary>
		/// Manages URI creation for resources when CDN is configured.
		/// </summary>
		/// <param name="uri">The URI of a file hosted locally.</param>
		/// <returns>The URI of a file when it is hosted in CDN.</returns>
		string ApplyCdnUri(string uri);
	}
}
