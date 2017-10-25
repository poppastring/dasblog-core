using System;
using System.Collections.Specialized;
using System.Net;

namespace Subtext.Akismet
{
	public class Comment : IComment
	{
		private NameValueCollection serverEnvironmentVariables = new NameValueCollection();
		private IPAddress ipAddress;
		private string commentType;
		private string userAgent;
		private Uri permalink;
		private string referer;
		private string content;
		private Uri authorUrl;
		private string authorEmail;
		private string author;

		/// <summary>
		/// Initializes a new instance of the <see cref="Comment"/> class.
		/// </summary>
		/// <param name="authorIpAddress">The author ip address.</param>
		/// <param name="authorUserAgent">The author user agent.</param>
		public Comment(IPAddress authorIpAddress, string authorUserAgent)
		{
			this.ipAddress = authorIpAddress;
			this.userAgent = authorUserAgent;
		}
		
		/// <summary>
		/// The name submitted with the comment.
		/// </summary>
		public string Author
		{
			get { return this.author; }
			set { this.author = value;}
		}

		/// <summary>
		/// The email submitted with the comment.
		/// </summary>
		public string AuthorEmail
		{
			get { return this.authorEmail; }
			set { this.authorEmail = value;}
		}

		/// <summary>
		/// The url submitted if provided.
		/// </summary>
		public Uri AuthorUrl
		{
			get { return this.authorUrl; }
			set { this.authorUrl = value; }
		}

		/// <summary>
		/// Content of the comment
		/// </summary>
		public string Content
		{
			get { return this.content; }
			set { this.content = value; }
		}

		/// <summary>
		/// The HTTP_REFERER header value of the 
		/// originating comment.
		/// </summary>
		public string Referer
		{
			get { return this.referer; }
			set { this.referer = value; }
		}

		/// <summary>
		/// Permanent location of the entry the comment was 
		/// submitted to.
		/// </summary>
		public Uri Permalink
		{
			get { return this.permalink; }
			set { this.permalink = value; }
		}

		/// <summary>
		/// User agent of the requester. (Required)
		/// </summary>
		public string UserAgent
		{
			get { return this.userAgent; }
		}

		/// <summary>
		/// May be one of the following: {blank}, "comment", "trackback", "pingback", or a made-up value 
		/// like "registration".
		/// </summary>
		public string CommentType
		{
			get { return this.commentType; }
			set { this.commentType = value; }
		}

		/// <summary>
		/// IPAddress of the submitter
		/// </summary>
		public IPAddress IpAddress
		{
			get { return this.ipAddress; }
		}

		/// <summary>
		/// Optional collection of various server environment variables. 
		/// </summary>
		public NameValueCollection ServerEnvironmentVariables
		{
			get { return this.serverEnvironmentVariables; }
		}
	}
}