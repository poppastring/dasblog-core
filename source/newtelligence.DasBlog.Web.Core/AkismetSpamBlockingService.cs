using System;
using newtelligence.DasBlog.Runtime;
using Subtext.Akismet;
using AkismetComment = Subtext.Akismet.Comment;

namespace newtelligence.DasBlog.Web.Core
{
	/// <summary>
	/// Uses the Akismet service to determine if a comment is SPAM
	/// </summary>
	/// <remarks>http://akismet.com/</remarks>
	public class AkismetSpamBlockingService : ISpamBlockingService
	{
		AkismetClient akismetClient;

		public AkismetSpamBlockingService(string apiKey, string blogUrl)
		{
			akismetClient = new AkismetClient(apiKey, new Uri(blogUrl));	
		}

		public bool IsSpam(IFeedback feedback)
		{
			IComment akismetFormattedComment = ConvertToAkismetComment(feedback);
			return akismetClient.CheckCommentForSpam(akismetFormattedComment);
		}

		public void ReportSpam(IFeedback feedback)
		{
			IComment akismetFormattedComment = ConvertToAkismetComment(feedback);
			akismetClient.SubmitSpam(akismetFormattedComment);
		}

		public void ReportNotSpam(IFeedback feedback)
		{
			IComment akismetFormattedComment = ConvertToAkismetComment(feedback);
			akismetClient.SubmitHam(akismetFormattedComment);
		}

		private AkismetComment ConvertToAkismetComment(IFeedback feedback)
		{
			System.Net.IPAddress ipAddress = System.Net.IPAddress.None;
			if (feedback.AuthorIPAddress != null)
			{
				try 
				{
					ipAddress = System.Net.IPAddress.Parse(feedback.AuthorIPAddress);
				}
				catch(FormatException){}
			}
			AkismetComment comment = new AkismetComment(ipAddress, feedback.AuthorUserAgent);
			comment.Author = feedback.Author;
			comment.AuthorEmail = feedback.AuthorEmail;
			if (feedback.AuthorHomepage != null && feedback.AuthorHomepage.Length > 0)
			{
				try 
				{
					comment.AuthorUrl = new Uri(feedback.AuthorHomepage);
				}
				catch(UriFormatException){}			
			}
			comment.Content = feedback.Content;
			comment.Referer = feedback.Referer;
			if (feedback.TargetEntryId != null & feedback.TargetEntryId.Trim().Length > 0)
			{
				try 
				{
					comment.Permalink = new Uri(SiteUtilities.GetPermaLinkUrl(feedback.TargetEntryId));
				}
				catch(UriFormatException){}
			}
			comment.CommentType = feedback.FeedbackType;
			return comment;
		}

	}
}
