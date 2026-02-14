using DasBlog.Managers.Interfaces;
using DasBlog.Services;
using Microsoft.Extensions.Logging;
using newtelligence.DasBlog.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;

namespace DasBlog.Managers
{
	public class CommentManager : ICommentManager
	{
		private readonly IBlogDataService dataService;
		private readonly ILogger logger;
		private readonly IDasBlogSettings dasBlogSettings;
		private const int COMMENT_PAGE_SIZE = 5;

		public CommentManager(ILogger<CommentManager> logger, IDasBlogSettings dasBlogSettings, IBlogDataService dataService)
		{
			this.logger = logger;
			this.dasBlogSettings = dasBlogSettings;
			this.dataService = dataService;
		}

		public CommentSaveState AddComment(string postid, Comment comment)
		{
			var saveState = CommentSaveState.Failed;
			var entry = dataService.GetEntry(postid);

			if (!dasBlogSettings.SiteConfiguration.EnableComments || !entry.AllowComments)
			{
				return CommentSaveState.SiteCommentsDisabled;
			}

			if (entry != null)
			{
				var targetComment = DateTime.UtcNow.AddDays(-1 * dasBlogSettings.SiteConfiguration.DaysCommentsAllowed);

				if ((targetComment > entry.CreatedUtc))
				{
					return CommentSaveState.PostCommentsDisabled;
				}

				// FilterHtml html encodes anything we don't like
				string filteredText = dasBlogSettings.FilterHtml(comment.Content);
				comment.Content = filteredText;

				if (dasBlogSettings.SiteConfiguration.SendCommentsByEmail)
				{
					var actions = ComposeMailForUsers(entry, comment);
					dataService.AddComment(comment, actions);
				}
				else
				{
					dataService.AddComment(comment);
				}

				
				saveState = CommentSaveState.Added;
			}
			else
			{
				saveState = CommentSaveState.NotFound;
			}

			return saveState;
		}

		public CommentSaveState DeleteComment(string postid, string commentid)
		{
			CommentSaveState est = CommentSaveState.Failed;

			Entry entry = dataService.GetEntry(postid);

			if (entry != null && !string.IsNullOrEmpty(commentid))
			{
				dataService.DeleteComment(postid, commentid);

				est = CommentSaveState.Deleted;
			}
			else
			{
				est = CommentSaveState.NotFound;
			}

			return est;
		}

		public CommentSaveState ApproveComment(string postid, string commentid)
		{
			CommentSaveState est = CommentSaveState.Failed;
			Entry entry = dataService.GetEntry(postid);

			if (entry != null && !string.IsNullOrEmpty(commentid))
			{
				dataService.ApproveComment(postid, commentid);

				est = CommentSaveState.Approved;
			}
			else
			{
				est = CommentSaveState.NotFound;
			}

			return est;
		}

		public CommentCollection GetComments(string postid, bool allComments)
		{
			return dataService.GetCommentsFor(postid, allComments);
		}

		public CommentCollection GetAllComments()
		{
			return dataService.GetAllComments();
		}

		public List<Comment> GetCommentsFrontPage()
		{
			var comments = dataService.GetAllComments().OrderByDescending(d => d.CreatedUtc).ToList();

			return comments.Take(COMMENT_PAGE_SIZE).ToList();
		}

		public List<Comment> GetCommentsForPage(int pageIndex)
		{
			var comments = dataService.GetAllComments().OrderByDescending(d => d.CreatedUtc).ToList();

			return comments.Skip((pageIndex) * COMMENT_PAGE_SIZE).Take(COMMENT_PAGE_SIZE).ToList();
		}

		private object[] ComposeMailForUsers(Entry entry, Comment c)
		{
			var actions = new List<object>();
			
			foreach (var user in dasBlogSettings.SecurityConfiguration.Users)
			{
				if (string.IsNullOrWhiteSpace(user.EmailAddress))
					continue;

				if (user.NotifyOnAllComment || (user.NotifyOnOwnComment && entry.Author.ToUpper() == user.Name.ToUpper()))
				{
					var sendMailInfo = ComposeMail(c);
					sendMailInfo.Message.To.Add(user.EmailAddress);
					actions.Add(sendMailInfo);
				}
			}

			return actions.ToArray();
		}

		private SendMailInfo ComposeMail(Comment c)
		{
			var emailMessage = new MailMessage();

			if (!string.IsNullOrWhiteSpace(dasBlogSettings.SiteConfiguration.NotificationEMailAddress))
			{
				emailMessage.To.Add(dasBlogSettings.SiteConfiguration.NotificationEMailAddress);
			}
			else
			{
				emailMessage.To.Add(dasBlogSettings.SiteConfiguration.Contact);
			}

			emailMessage.Subject = string.Format("Weblog comment by '{0}' from '{1}' on '{2}'", c.Author, c.AuthorHomepage, c.TargetTitle);

			if (dasBlogSettings.SiteConfiguration.CommentsRequireApproval)
			{
				emailMessage.Body = string.Format("{0}\r\nComments page: {1}\r\n\r\nRequires approval.\r\n\r\nCommentor Email: {2}\r\n\r\nIP Address: {3}\r\n\r\nLogin Here: {4}",
				   WebUtility.HtmlDecode(c.Content),
				   dasBlogSettings.GetCommentViewUrl(c.TargetEntryId),
				   c.AuthorEmail,
				   c.AuthorIPAddress,
				   dasBlogSettings.RelativeToRoot("account/login"));
			}
			else
			{
				emailMessage.Body = string.Format("{0}\r\nComments page: {1}\r\n\r\nCommentor Email: {2}\r\n\r\nIP Address: {3}\r\n\r\nLogin Here: {4}",
				   WebUtility.HtmlDecode(c.Content),
				   dasBlogSettings.GetCommentViewUrl(c.TargetEntryId),
				   c.AuthorEmail,
				   c.AuthorIPAddress,
				   dasBlogSettings.RelativeToRoot("account/login"));
			}

			emailMessage.IsBodyHtml = false;
			emailMessage.BodyEncoding = System.Text.Encoding.UTF8;

			emailMessage.From = new MailAddress(GetFromEmail());

			return dasBlogSettings.GetMailInfo(emailMessage);
		}

		private string GetFromEmail()
		{
			if (string.IsNullOrWhiteSpace(dasBlogSettings.SiteConfiguration.SmtpFromEmail))
			{
				return dasBlogSettings.SiteConfiguration.SmtpUserName?.Trim();
			}

			return dasBlogSettings.SiteConfiguration.SmtpFromEmail?.Trim();
		}
	}
}
