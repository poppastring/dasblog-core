using System;
using System.Resources;
using newtelligence.DasBlog.Runtime;

namespace newtelligence.DasBlog.Web
{
    /// <summary>
    /// This class keeps a singleton reference to the application's
    /// resource table
    /// </summary>
    public class ApplicationResourceTable
    {
        private static ResourceManager rm;

        static ApplicationResourceTable()
        {
            rm = new ResourceManager("newtelligence.DasBlog.Web.StringTables.StringTables", typeof(Global).Assembly );
        }

        public static ResourceManager Get()
        {
            return rm;
        }

		public static string GetSpamStateDescription(SpamState spamState)
		{
			if (spamState == SpamState.NotChecked)
			{
				return rm.GetString("text_spamstate_notchecked");
			} 
			if (spamState == SpamState.NotSpam)
			{
				return rm.GetString("text_spamstate_notspam");
			}
			if (spamState == SpamState.Spam)
			{
				return rm.GetString("text_spamstate_spam");
			}
			return String.Empty;
		}
    }
}
