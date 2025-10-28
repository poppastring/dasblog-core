using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;

namespace Subtext.Akismet
{
	/// <summary>
	/// Class used to make the actual HTTP requests.
	/// </summary>
	/// <remarks>
	/// Yeah, I know you're thinking this is overkill, but it makes it 
	/// easier to write tests to have this layer of abstraction from the 
	/// underlying Http request.
	/// </remarks>
	public class HttpClient
	{
		private static readonly System.Net.Http.HttpClient httpClient = new System.Net.Http.HttpClient();

		/// <summary>
		/// Posts the request and returns a text response.  
		/// This is all that is needed for Akismet.
		/// </summary>
		/// <param name="url">The URL.</param>
		/// <param name="userAgent">The user agent.</param>
		/// <param name="timeout">The timeout.</param>
		/// <param name="formParameters">The properly formatted parameters.</param>
		/// <returns></returns>
		public virtual string PostRequest(Uri url, string userAgent, int timeout, string formParameters)
		{
			System.Net.ServicePointManager.Expect100Continue = false;
			
			var request = new HttpRequestMessage(System.Net.Http.HttpMethod.Post, url);
			request.Headers.Add("User-Agent", userAgent);
			request.Content = new StringContent(formParameters, Encoding.UTF8, "application/x-www-form-urlencoded");
			
			using (var cts = new System.Threading.CancellationTokenSource(timeout))
			{
				var response = httpClient.SendAsync(request, cts.Token).Result;
				
				if (response.StatusCode < HttpStatusCode.OK || response.StatusCode >= HttpStatusCode.Ambiguous)
					throw new InvalidResponseException(string.Format("The service was not able to handle our request. Http Status '{0}'.", response.StatusCode), response.StatusCode);

				string responseText = response.Content.ReadAsStringAsync().Result;
				
				return responseText;
			}
		}
	}
}
