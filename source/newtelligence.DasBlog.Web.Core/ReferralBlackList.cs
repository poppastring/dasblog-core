using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Web;
using newtelligence.DasBlog.Runtime;

namespace newtelligence.DasBlog.Web.Core
{
	/// <summary>
	/// Enumeration used for the return code of <see cref="IBlogDataService.SaveEntry"/>
	/// </summary>
	public enum BlacklistUpdateState
	{
		None, 
		Updated, 
		Failed 
	}

	public interface IBlackList
	{
		void Initialize(string blacklist);
		BlacklistUpdateState UpdateBlacklist();
		Match IsBlacklisted(string url);
	}

	public static class ReferralBlackListFactory
	{
        private static Dictionary<string, IBlackList> blacklists = new Dictionary<string, IBlackList>();

		private static bool InitializeBlacklist(IBlackList referrerBlacklist, string path)
		{
			ILoggingDataService loggingService = LoggingDataServiceFactory.GetService(SiteConfig.GetLogPathFromCurrentContext());

			try
			{
				referrerBlacklist.Initialize(path);
				BlacklistUpdateState updateState = referrerBlacklist.UpdateBlacklist();
					
				if (updateState == BlacklistUpdateState.Failed)
				{
					loggingService.AddEvent(
						new EventDataItem(EventCodes.Error, referrerBlacklist.ToString() + " could not be updated: ", "InitializeBlacklist"));
				}
				else if (updateState == BlacklistUpdateState.Updated)
				{
					loggingService.AddEvent(
						new EventDataItem(EventCodes.ApplicationStartup, referrerBlacklist.ToString() + " updated: ", "InitializeBlacklist"));
				}
				
				return true;
			}
			catch (Exception ex)
			{
				loggingService.AddEvent(new EventDataItem(EventCodes.Error, referrerBlacklist.ToString() + " could not be initialized: " + ex.ToString(), "InitializeBlacklist"));
			}

			return false;
		}

		public static void AddBlacklist(IBlackList blackList, string path)
		{
			if (blacklists.ContainsKey(blackList.GetType().Name) == false)
			{
				bool success = InitializeBlacklist(blackList, path);
				if (success) blacklists.Add(blackList.GetType().Name, blackList);
			}
			else
			{
                IBlackList referrerBlacklist = blacklists[blackList.GetType().Name];
				InitializeBlacklist(referrerBlacklist, path);
			}
		}

		public static void RemoveBlacklist(Type type)
		{
			ILoggingDataService loggingService = LoggingDataServiceFactory.GetService(SiteConfig.GetLogPathFromCurrentContext());
			if (blacklists.ContainsKey(type.Name)== true)
			{
				try
				{
					blacklists.Remove(type.Name);
				}
				catch (Exception ex)
				{
					loggingService.AddEvent(new EventDataItem(EventCodes.Error, type.Name.ToString() + " could not be removed: " + ex.ToString(), ""));
				}
			}
		}

		public static IBlackList[] Lists
		{
			get
			{

                List<IBlackList> list = new List<IBlackList>();
				foreach (IBlackList referralBlacklist in blacklists.Values)
				{
					list.Add(referralBlacklist);
				}

				return list.ToArray();
			}
		}
	}

	public class ReferralBlackList
	{
		public static bool IsBlockedReferrer(string referrer)
		{
			return IsBlockedReferrer(referrer, null);
		}

		public static bool IsBlockedReferrer(string referrer, string entryTitle)
		{
			HttpContext context = HttpContext.Current;
			if (referrer == null || referrer.Length == 0 || context == null)
			{
				return false;
			}

			SiteConfig siteConfig = SiteConfig.GetSiteConfig();

			// return if this is a local page
			// bail if we are getting a local referral from our website
			if (context.Request.UrlReferrer!= null && 
				SiteUtilities.ReferralFromSelf(siteConfig, context.Request.UrlReferrer.ToString()) == true)
			{
				return false;
			}

            ILoggingDataService loggingService = LoggingDataServiceFactory.GetService(SiteConfig.GetLogPathFromCurrentContext());
            bool retVal = false;
			Match match = null;
			
			foreach (IBlackList referralBlacklist in ReferralBlackListFactory.Lists)
			{
				try
				{
					match = referralBlacklist.IsBlacklisted(referrer);
					if (match!= null)
					{
						retVal = match.Success;
						if (retVal) break;
					}
				}
				catch (Exception ex)
				{
                    loggingService.AddEvent(new EventDataItem(EventCodes.Error, ex.ToString(), referralBlacklist.ToString() + " Regular Expression"));
					return false;
				}
			}

			// if either Blacklist has a return value of true, we block the referral and log the event
			if(context != null && retVal == true)		
			{
				if (siteConfig.LogBlockedReferrals)
				{
					if (entryTitle != null && entryTitle != "")
					{
                        loggingService.AddEvent(new EventDataItem(EventCodes.ItemReferralBlocked, context.Request.UserHostAddress + " because of \"" + match.Value + "\"", context.Request.RawUrl, referrer, entryTitle));
					}
					else
					{
                        loggingService.AddEvent(new EventDataItem(EventCodes.ReferralBlocked, context.Request.UserHostAddress + " because of \"" + match.Value + "\"", context.Request.RawUrl, referrer));
					}
				}

				if(siteConfig.EnableReferralUrlBlackList404s == true)
				{
					context.Response.StatusCode = 404;
					context.Response.SuppressContent = true;
					context.Response.End();
				}
			}

			return retVal;
		}
	}
}
