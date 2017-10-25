using System;
using System.IO;
using System.Net;
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
			HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;

			if (null != request)
			{			
				request.UserAgent = userAgent;
				request.Timeout = timeout;
				request.Method = "POST";
				request.ContentLength = formParameters.Length;
				request.ContentType = "application/x-www-form-urlencoded; charset=utf-8";
				request.KeepAlive = true;

				using (StreamWriter myWriter = new StreamWriter(request.GetRequestStream()))
				{
					myWriter.Write(formParameters);
				}
			}

			HttpWebResponse response = (HttpWebResponse)request.GetResponse();
			if (response.StatusCode < HttpStatusCode.OK && response.StatusCode >= HttpStatusCode.Ambiguous)
				throw new InvalidResponseException(string.Format("The service was not able to handle our request. Http Status '{0}'.", response.StatusCode), response.StatusCode);

			string responseText;
			using(StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.ASCII)) //They only return "true" or "false"
			{
				responseText = reader.ReadToEnd();
			}
			
			return responseText;
		}
	}
}
