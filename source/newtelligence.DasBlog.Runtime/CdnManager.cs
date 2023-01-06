using System;

namespace newtelligence.DasBlog.Runtime
{
	public static class CdnManagerFactory
	{
		public static ICdnManager GetService(string rootUri, string cdnUri)
		{
			return new CdnManager(rootUri, cdnUri);
		}
	}

	internal sealed class CdnManager : ICdnManager
	{
		private readonly string rootUri;
		private readonly string cdnUri;

		/// <summary>
		/// Setting up the cdn manager to return cdn uris when that is configured.
		/// </summary>
		/// <param name="rootUri">The root of the </param>
		/// <param name="cdnUri"></param>
		public CdnManager(string rootUri, string cdnUri)
		{
			this.rootUri = 
				string.IsNullOrWhiteSpace(rootUri) || !Uri.TryCreate(rootUri, UriKind.Absolute, out _) ?
				null :
				rootUri;
			this.cdnUri = 
				string.IsNullOrWhiteSpace(cdnUri) || !Uri.TryCreate(cdnUri, UriKind.Absolute, out _) ? 
				null : 
				cdnUri;
			if (this.rootUri == null) this.cdnUri = null;
		}

		public string ApplyCdnUri(string uri)
		{
			// If the cdnUri is null then we can just return the uri.
			return cdnUri == null ? uri : uri.Replace(rootUri, cdnUri);
		}
	}
}
