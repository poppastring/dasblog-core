using System;
using System.Collections.Specialized;
using System.IO;
using System.Web.Caching;
using System.Xml.Serialization;
using newtelligence.DasBlog.Runtime;

namespace newtelligence.DasBlog.Web.Core
{
    [XmlType(Namespace = "urn:newtelligence-com:dasblog:config")]
    [XmlRoot(Namespace = "urn:newtelligence-com:dasblog:config")]
    public class RobotDefinition
    {
        public StringCollection Domains
        {
            get;
            set;
        }

        public StringCollection UserAgents
        {
            get;
            set;
        }

        public bool IsRobot(LogDataItem logDataItem)
        {
            if (Domains.Contains(logDataItem.UserDomain.Trim()) ||
                IsRobotUserAgent(logDataItem.UserAgent.Trim()))
            {
                return true;
            }

            return false;
        }

        bool IsRobotUserAgent(string userAgent)
        {
            if (String.IsNullOrEmpty(userAgent))
            {
                return false;
            }

            foreach (string robotUserAgent in UserAgents)
            {
                if (userAgent.IndexOf(robotUserAgent, StringComparison.InvariantCultureIgnoreCase) != -1)
                {
                    return true;
                }
            }

            return false;
        }

        #region Get Robots Definition
        public static RobotDefinition GetRobotDefinition()
        {
            DataCache cache = CacheFactory.GetCache();

            RobotDefinition definition = (RobotDefinition)cache["RobotDefinition"];

            if (definition == null)
            {
                definition = GetRobotDefinition(GetConfigFilePathFromCurrentContext());

                cache.Insert("RobotDefinition", definition, new CacheDependency(GetConfigFilePathFromCurrentContext()));
            }

            return definition;
        }

        public static RobotDefinition GetRobotDefinition(string configPath)
        {
            RobotDefinition definition;
            XmlSerializer ser = new XmlSerializer(typeof(RobotDefinition));

            using (StreamReader reader = new StreamReader(configPath))
            {
                //XmlNamespaceUpgradeReader xnur = new XmlNamespaceUpgradeReader(reader, "", "urn:newtelligence-com:dasblog:config");
                definition = ser.Deserialize(reader) as RobotDefinition;
            }

            return definition;
        }

        // FIX: hardcoded path
        static string GetConfigFilePathFromCurrentContext()
        {
            return SiteUtilities.MapPath("~/SiteConfig/robots.config");
        }
        #endregion
    }
}