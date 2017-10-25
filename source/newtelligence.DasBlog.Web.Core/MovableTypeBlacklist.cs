using System;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace newtelligence.DasBlog.Web.Core
{
	/// <summary>
	/// MovableTypeBlacklist is a class that can check if a url is on the MT-Blacklist Master Copy
	/// http://www.jayallen.org/comment_spam/blacklist.txt.
	/// </summary>
	public class MovableTypeBlacklist : IBlackList
	{	
		static private string pattern = null;
		static private Regex blackListRegex = null;
		private static string localPath;
		private static object blackListRegexLock = new object();
		private static ReaderWriterLock blacklistReaderLock = new ReaderWriterLock();
		private static Uri internetAddress = new Uri("http://www.jayallen.org/comment_spam/blacklist.txt");
		private static bool loaded = false;

		/// <summary>
		/// The local path to the blacklist.txt file.
		/// </summary>
		public static string LocalPath
		{
			get { return localPath; }
		}

		/// <summary>
		/// The <see cref="Uri"/> for the internet address of the MT-Blacklist Master Copy.
		/// </summary>
		/// <example>http://www.jayallen.org/comment_spam/blacklist.txt</example>
		public static Uri InternetAddress
		{
			get { return internetAddress; }
		}

		public void Initialize(string localPath)
		{
			Initialize(localPath, internetAddress);
		}
		/// <summary>
		/// Initializes the <see cref="MovableTypeBlacklist"/>.
		/// </summary>
		/// <param name="localPath">The absolute path to the local blacklist.txt file</param>
		/// <param name="internetAddress">The <see cref="Uri"/> for the internet address of the MT-Blacklist Master Copy.</param>
		public void Initialize(string localPath, Uri internetAddress)
		{
			// allow the user to change the internetAddress and reload
			if (MovableTypeBlacklist.InternetAddress != internetAddress || !loaded)
			{
				MovableTypeBlacklist.localPath = localPath;
				MovableTypeBlacklist.internetAddress = internetAddress;

				if (!File.Exists(localPath))
				{
					try
					{
						UpdateBlacklist();
					}
					catch (Exception ex)
					{
						throw ex;
					}
				}

				lock (blackListRegexLock)
				{
					if (pattern == null)
					{
						string newBlackList = OpenFile();
						if (newBlackList != null && newBlackList.Length > 0)
						{
							pattern = newBlackList.Replace(';', '|');
							blackListRegex = new Regex(pattern,RegexOptions.IgnoreCase|RegexOptions.Singleline);
						}
						else
						{
							throw new NullReferenceException("blacklist file is empty");
						}
					}
				}
			}

			loaded = true;
		}

		private static void WriteToFile(string contents, DateTime lastModified)
		{
			if (contents != null && contents.Length > 0)
			{
				blacklistReaderLock.AcquireWriterLock(250);

				try
				{
					using (StreamWriter sw = File.CreateText(localPath))
					{
						sw.Write(contents);
					}

					File.SetCreationTimeUtc(localPath, lastModified);
				}
				catch(Exception e)
				{
					throw e;
				}
				finally
				{
					blacklistReaderLock.ReleaseLock();
				}
			}
		}

		private static string OpenFile()
		{
			StringBuilder sb = new StringBuilder();

			blacklistReaderLock.AcquireReaderLock(250);

			try
			{
				using (StreamReader sr = new StreamReader(localPath)) 
				{
					string line = null;
					// Read and display lines from the file until the end of 
					// the file is reached.
					while ((line = sr.ReadLine()) != null) 
					{
						if (!line.StartsWith("#"))
						{
							if (line.IndexOf('#') > -1)
							{
								line = line.Substring(0, line.IndexOf('#'));
							}

							sb.Append(line.TrimEnd(' '));
							sb.Append(";");
						}
					}
				}
			}
			catch(Exception e)
			{
				throw e;
			}
			finally
			{
				blacklistReaderLock.ReleaseLock();
			}

			return sb.ToString().TrimEnd(';');
		}

		/// <summary>
		/// Determines if the url is blacklisted.
		/// </summary>
		/// <param name="url">The url to check.</param>
		/// <returns>A <see cref="Match"/>.</returns>
		public Match IsBlacklisted(string url)
		{
			if (!MovableTypeBlacklist.loaded)
			{
				return null;
			}

			try
			{
				Match match = null;

				lock (blackListRegexLock)
				{
					match = blackListRegex.Match(url);
				}

				return match;
			}
			catch (Exception ex)
			{
				throw new Exception(String.Format("An error occured trying to determine if {0} is blacklisted", url), ex.InnerException);
			}
		}
	
		/// <summary>
		/// Downloads the MT-Blacklist Master Copy.
		/// </summary>
		/// <remarks>
		/// The file will only get downloaded a maximum of once per day. The file is downloaded using
		/// gzip compression from the server.
		/// </remarks>
		public BlacklistUpdateState UpdateBlacklist()
		{
			// only check the blacklist once a day
			DateTime fileLastWritten = DateTime.MinValue;

			if (File.Exists(localPath))
				fileLastWritten = File.GetLastWriteTime(localPath);

			if (fileLastWritten.Day != DateTime.Now.Day)
			{
				HttpWebResponse response = null;
				try
				{
					string requestBody = null;
					HttpWebRequest webRequest = WebRequest.Create(internetAddress) as HttpWebRequest;
			
					webRequest.Method="GET";

					if (File.Exists(localPath))
						webRequest.IfModifiedSince = File.GetCreationTimeUtc(localPath);
					
					// TODO: if we really care we can decompress this to non gzip'ed data
					// webRequest.Headers.Add("Accept-Encoding", "gzip, deflate");
			
					// uncomment to see traffic in fiddlertool
					// GlobalProxySelection.Select = new WebProxy("127.0.0.1", 8888);
					response = webRequest.GetResponse() as HttpWebResponse;

					if (response.StatusCode == HttpStatusCode.OK)
					{
						using (StreamReader requestReader = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
						{
							requestBody = requestReader.ReadToEnd();
						}
			
						DateTime lastModified = response.LastModified;
						
						WriteToFile(requestBody, lastModified);
						return BlacklistUpdateState.Updated;
					}

					return BlacklistUpdateState.Failed;
				}
				catch
				{
					return BlacklistUpdateState.Failed;
				}
				finally
				{
					if (response != null) response.Close();
				}
			}

			return BlacklistUpdateState.None;
		}
	}
}
