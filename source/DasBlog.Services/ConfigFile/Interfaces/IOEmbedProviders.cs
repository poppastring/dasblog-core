using System.Collections.Generic;

namespace DasBlog.Services.ConfigFile.Interfaces
{
	public interface IOEmbedProviders 
	{
		public List<OEmbedProvider> Providers { get; set; }
	}

}
