using System;
using System.Text.RegularExpressions;

namespace newtelligence.DasBlog.Web.Core
{
	/// <summary>
	/// Summary description for ContentReferralBlacklist.
	/// </summary>
	public class ReferralUrlBlacklist : IBlackList
	{
		private static bool loaded = false;
		private static Regex blackListRegex = null;
		private static string blacklist = null;
		private static object blackListLock = new object();

		public void Initialize(string newBlackList)
		{
			lock(blackListLock)
			{
				if (blacklist == null || blacklist != newBlackList)
				{
					if (newBlackList != null && newBlackList.Length > 0)
					{
						blacklist = newBlackList;
						blacklist = blacklist.Replace(";","|");
						blacklist = blacklist.Replace("+","\\+");
						blacklist = blacklist.Replace("*","\\*");
						blackListRegex = new Regex(blacklist,RegexOptions.Compiled|RegexOptions.IgnoreCase|RegexOptions.IgnorePatternWhitespace);
					}
					else
					{
						throw new NullReferenceException("blacklist string is empty");
					}
				}
			}

			loaded = true;
		}

		public Match IsBlacklisted(string url)
		{
			if (!loaded)
			{
				return null;
			}

			try
			{
				Match match = null;
				Uri urlReferrer = new Uri(url);

				// we want to remove the Query from the url as it may contain keywords that easily match
				// the black list. 
				// However, we CHECK against the stripped referrer and log the COMPLETE referral!
				string strippedReferrer = (urlReferrer != null) ? urlReferrer.Scheme + "://" + urlReferrer.Authority + urlReferrer.AbsolutePath : String.Empty;

				lock (blackListLock)
				{
					match = blackListRegex.Match(strippedReferrer);
				}

				return match;
			}
			catch (Exception ex)
			{
				throw new Exception(String.Format("An error occured trying to determine if {0} is blacklisted", url), ex.InnerException);
			}
		}

		public BlacklistUpdateState UpdateBlacklist()
		{
			return BlacklistUpdateState.None;
		}
	}
}
