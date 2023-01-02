using System;
using System.Net.Mail;
using DasBlog.Core.Security;
using DasBlog.Services.ConfigFile.Interfaces;
using newtelligence.DasBlog.Runtime;
using NodaTime;
using Org.BouncyCastle.Crypto.Agreement.Srp;

namespace DasBlog.Services
{
	public interface IDasBlogSettings
	{
		ISiteConfig SiteConfiguration { get; set; }
		IMetaTags MetaTags { get; set; }
		ISiteSecurityConfig SecurityConfiguration { get; }

		string WebRootDirectory { get; }

		string RssUrl { get; }
		string PingBackUrl { get; }
		string CategoryUrl { get; }
		string ArchiveUrl { get; }
		string MicroSummaryUrl { get; }
		string RsdUrl { get; }

		string ShortCutIconUrl { get; }
		string ThemeCssUrl { get; }

		string RelativeToRoot(string relative);
		string GetBaseUrl();
		string GetPermaLinkUrl(string entryId);
		string GetPermaTitle(string titleurl);
		string GetTrackbackUrl(string entryId);
		string GetEntryCommentsRssUrl(string entryId);
		string GetCommentViewUrl(string entryId);
		string GetCategoryViewUrl(string category);
		string GetCategoryViewUrlName(string category);
		string GetRssCategoryUrl(string category);
		User GetUser(string userName);
		User GetUserByEmail(string email);
		void AddUser(User user);
		DateTimeZone GetConfiguredTimeZone();
		DateTime GetContentLookAhead();
		string FilterHtml(string input);
		bool AreCommentsPermitted(DateTime blogpostdate);
		string CompressTitle(string title);
		bool IsAdmin(string gravatarhash);
		string GeneratePostUrl(Entry entry);
		SendMailInfo GetMailInfo(MailMessage emailmessage);
		DateTime GetDisplayTime(DateTime datetime);
		DateTime GetCreateTime(DateTime datetime);
		string GetRssEntryUrl(string entryId);
	}
}
