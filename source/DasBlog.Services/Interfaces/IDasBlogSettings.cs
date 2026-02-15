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
		ISiteConfig SiteConfiguration { get; }
		IMetaTags MetaTags { get; }
		ISiteSecurityConfig SecurityConfiguration { get; }

		IOEmbedProviders OEmbedProviders { get; }

		string WebRootDirectory { get; }
	}
}
