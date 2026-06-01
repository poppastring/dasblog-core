using System;
using System.Globalization;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading;

namespace Subtext.Akismet
{
	/// <summary>
	/// The client class used to communicate with the
	/// <see href="http://akismet.com/">Akismet</see> service.
	/// </summary>
	public class AkismetClient
	{
		private static readonly HttpClient sharedHttpClient = new HttpClient();
		private static readonly string version = typeof(AkismetClient).Assembly.GetName().Version.ToString();
		private static readonly Uri verifyUrl = new Uri("https://rest.akismet.com/1.1/verify-key");
		private static readonly Uri checkUrl = new Uri("https://rest.akismet.com/1.1/comment-check");
		private static readonly Uri submitSpamUrl = new Uri("https://rest.akismet.com/1.1/submit-spam");
		private static readonly Uri submitHamUrl = new Uri("https://rest.akismet.com/1.1/submit-ham");

		private string apiKey;
		private string userAgent;
		private int timeout = 5000;
		private Uri blogUrl;

		/// <summary>
		/// Initializes a new instance of the <see cref="AkismetClient"/> class.
		/// </summary>
		/// <param name="apiKey">The Akismet API key.</param>
		/// <param name="blogUrl">The root url of the blog.</param>
		public AkismetClient(string apiKey, Uri blogUrl)
		{
			if (apiKey == null)
				throw new ArgumentNullException(nameof(apiKey), "The akismet Api Key must be specified");

			if (blogUrl == null)
				throw new ArgumentNullException(nameof(blogUrl), "The blog's url must be specified");

			this.apiKey = apiKey;
			this.blogUrl = blogUrl;
		}

		/// <summary>
		/// Gets or sets the Akismet API key.
		/// </summary>
		public string ApiKey
		{
			get { return this.apiKey ?? string.Empty; }
			set { this.apiKey = value ?? string.Empty; }
		}

		/// <summary>
		/// Gets or sets the User Agent for the Akismet Client.
		/// Do not confuse this with the user agent for the comment being checked.
		/// </summary>
		public string UserAgent
		{
			get { return this.userAgent ?? BuildUserAgent("DasBlog", version); }
			set { this.userAgent = value; }
		}

		/// <summary>
		/// Helper method for building a user agent string in the format preferred by Akismet.
		/// </summary>
		public static string BuildUserAgent(string applicationName, string appVersion)
		{
			return string.Format("{0}/{1} | Akismet/1.11", applicationName, appVersion);
		}

		/// <summary>
		/// Gets or sets the timeout in milliseconds for the http request to Akismet.
		/// By default 5000 (5 seconds).
		/// </summary>
		public int Timeout
		{
			get { return this.timeout; }
			set { this.timeout = value; }
		}

		/// <summary>
		/// Gets or sets the root URL to the blog.
		/// </summary>
		public Uri BlogUrl
		{
			get { return this.blogUrl; }
			set { this.blogUrl = value; }
		}

		/// <summary>
		/// Verifies the API key. You really only need to call this once, perhaps at startup.
		/// </summary>
		public bool VerifyApiKey()
		{
			string parameters = "api_key=" + WebUtility.UrlEncode(this.ApiKey)
								+ "&blog=" + WebUtility.UrlEncode(this.BlogUrl.ToString());
			string result = PostRequest(verifyUrl, parameters);

			if (string.IsNullOrEmpty(result))
				throw new InvalidResponseException("Akismet returned an empty response");

			return string.Compare("valid", result, StringComparison.OrdinalIgnoreCase) == 0;
		}

		/// <summary>
		/// Checks the comment and returns true if it is spam, otherwise false.
		/// </summary>
		public bool CheckCommentForSpam(IComment comment)
		{
			string result = SubmitComment(comment, checkUrl);

			if (string.IsNullOrEmpty(result))
				throw new InvalidResponseException("Akismet returned an empty response");

			if (result != "true" && result != "false")
				throw new InvalidResponseException(string.Format("Received the response '{0}' from Akismet. Probably a bad API key.", result));

			return bool.Parse(result);
		}

		/// <summary>
		/// Submits a comment to Akismet that should have been flagged as SPAM, but was not.
		/// </summary>
		public void SubmitSpam(IComment comment)
		{
			SubmitComment(comment, submitSpamUrl);
		}

		/// <summary>
		/// Submits a comment to Akismet that should not have been flagged as SPAM (a false positive).
		/// </summary>
		public void SubmitHam(IComment comment)
		{
			SubmitComment(comment, submitHamUrl);
		}

		private string SubmitComment(IComment comment, Uri url)
		{
			var parameters = new StringBuilder();
			parameters.Append("api_key=").Append(WebUtility.UrlEncode(this.ApiKey));
			parameters.Append("&blog=").Append(WebUtility.UrlEncode(this.blogUrl.ToString()));
			parameters.Append("&blog_charset=UTF-8");
			parameters.Append("&user_ip=").Append(comment.IpAddress);
			parameters.Append("&user_agent=").Append(WebUtility.UrlEncode(comment.UserAgent));
			parameters.Append("&comment_date_gmt=").Append(WebUtility.UrlEncode(DateTime.UtcNow.ToString("o", CultureInfo.InvariantCulture)));

			if (!string.IsNullOrEmpty(comment.Referer))
				parameters.Append("&referrer=").Append(WebUtility.UrlEncode(comment.Referer));

			if (comment.Permalink != null)
				parameters.Append("&permalink=").Append(WebUtility.UrlEncode(comment.Permalink.ToString()));

			if (!string.IsNullOrEmpty(comment.CommentType))
				parameters.Append("&comment_type=").Append(WebUtility.UrlEncode(comment.CommentType));

			if (!string.IsNullOrEmpty(comment.Author))
				parameters.Append("&comment_author=").Append(WebUtility.UrlEncode(comment.Author));

			if (!string.IsNullOrEmpty(comment.AuthorEmail))
				parameters.Append("&comment_author_email=").Append(WebUtility.UrlEncode(comment.AuthorEmail));

			if (comment.AuthorUrl != null)
				parameters.Append("&comment_author_url=").Append(WebUtility.UrlEncode(comment.AuthorUrl.ToString()));

			if (!string.IsNullOrEmpty(comment.Content))
				parameters.Append("&comment_content=").Append(WebUtility.UrlEncode(comment.Content));

			if (comment.ServerEnvironmentVariables != null)
			{
				foreach (string key in comment.ServerEnvironmentVariables)
				{
					parameters.Append('&').Append(key).Append('=').Append(WebUtility.UrlEncode(comment.ServerEnvironmentVariables[key]));
				}
			}

			return PostRequest(url, parameters.ToString()).ToLower(CultureInfo.InvariantCulture);
		}

		private string PostRequest(Uri url, string formParameters)
		{
			using var request = new HttpRequestMessage(HttpMethod.Post, url);
			request.Headers.TryAddWithoutValidation("User-Agent", this.UserAgent);
			request.Content = new StringContent(formParameters ?? string.Empty, Encoding.UTF8, "application/x-www-form-urlencoded");

			using var cts = new CancellationTokenSource(TimeSpan.FromMilliseconds(this.timeout));
			using var response = sharedHttpClient.SendAsync(request, cts.Token).GetAwaiter().GetResult();

			if (!response.IsSuccessStatusCode)
			{
				throw new InvalidResponseException(
					string.Format("The service was not able to handle our request. Http Status '{0}'.", response.StatusCode),
					response.StatusCode);
			}

			return response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
		}
	}
}
