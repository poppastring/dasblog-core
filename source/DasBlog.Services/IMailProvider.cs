using System.Net.Mail;
using newtelligence.DasBlog.Runtime;

namespace DasBlog.Services
{
	/// <summary>
	/// Email configuration and mail message preparation.
	/// </summary>
	public interface IMailProvider
	{
		SendMailInfo GetMailInfo(MailMessage emailmessage);
	}
}
