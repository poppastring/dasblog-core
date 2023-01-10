using System;

namespace newtelligence.DasBlog.Runtime
{
	public static class CdnManagerFactory
	{
		public static ICdnManager GetService(string cdnFrom, string cdnTo)
		{
			return new CdnManager(cdnFrom, cdnTo);
		}
	}

	internal sealed class CdnManager : ICdnManager
	{
		private readonly string cdnFrom;
		private readonly string cdnTo;

		/// <summary>
		/// Setting up the cdn manager to return cdn uris when that is configured.
		/// </summary>
		/// <param name="cdnFrom">The binary hosting path to be replaced.</param>
		/// <param name="cdnTo">The cdn binary hosting path to change to.</param>
		public CdnManager(string cdnFrom, string cdnTo)
		{
			this.cdnFrom = string.IsNullOrWhiteSpace(cdnFrom) ? null : cdnFrom;
			this.cdnTo = string.IsNullOrWhiteSpace(cdnTo) || this.cdnFrom == null ? null : cdnTo;
		}

		public string ApplyCdnUri(string uri)
		{
			// If the cdnUri is null then we can just return the uri.
			return cdnTo == null ? uri : uri.Replace(cdnFrom, cdnTo);
		}
	}
}
