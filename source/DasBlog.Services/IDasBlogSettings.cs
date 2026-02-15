using System;
using System.Net.Mail;
using DasBlog.Core.Security;
using DasBlog.Services.ConfigFile.Interfaces;
using newtelligence.DasBlog.Runtime;
using NodaTime;

namespace DasBlog.Services
{
	public interface IDasBlogSettings : IUrlResolver, ITimeZoneService, IContentProcessor, IMailProvider, IUserManager
	{
		ISiteConfig SiteConfiguration { get; set; }
		IMetaTags MetaTags { get; set; }
		ISiteSecurityConfig SecurityConfiguration { get; }

		IOEmbedProviders OEmbedProviders { get; set; }

		string WebRootDirectory { get; }
	}
}
