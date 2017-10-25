using System;
using System.Collections.Specialized;
using System.Net;

namespace Subtext.Akismet
{
	/// <summary>
	/// Defines the base information about a comment submitted to 
	/// Akismet.
	/// </summary>
	public interface IComment
	{
		/// <summary>
		/// The name submitted with the comment.
		/// </summary>
		string Author {get;}
		
		/// <summary>
		/// The email submitted with the comment.
		/// </summary>
		string AuthorEmail {get;}

		/// <summary>
		/// The url submitted if provided.
		/// </summary>
		Uri AuthorUrl { get;}

		/// <summary>
		/// Content of the comment
		/// </summary>
		string Content { get;}

		/// <summary>
		/// The HTTP_REFERER header value of the 
		/// originating comment.
		/// </summary>
		string Referer { get;}

		/// <summary>
		/// Permanent location of the entry the comment was 
		/// submitted to.
		/// </summary>
		Uri Permalink { get;}

		/// <summary>
		/// User agent of the requester. (Required)
		/// </summary>
		string UserAgent { get;}

		/// <summary>
		/// May be one of the following: {blank}, "comment", "trackback", "pingback", or a made-up value 
		/// like "registration".
		/// </summary>
		string CommentType { get;}

		/// <summary>
		/// IPAddress of the submitter
		/// </summary>
		IPAddress IpAddress { get;}

		/// <summary>
		/// Optional collection of various server environment variables. 
		/// </summary>
		NameValueCollection ServerEnvironmentVariables { get; }
	}
}
