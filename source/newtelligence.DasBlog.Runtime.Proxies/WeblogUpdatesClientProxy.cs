using CookComputing.XmlRpc;

namespace newtelligence.DasBlog.Runtime.Proxies
{
	/// <summary>
	/// This is the client proxy for notifying weblogs.com on update
	/// </summary>
	public class WeblogUpdatesClientProxy : XmlRpcClientProtocol
	{
        public WeblogUpdatesClientProxy(string endpointUrl)
        {
			this.Url = endpointUrl;
			this.UserAgent = Utils.GetUserAgent();
        }

        [XmlRpcMethod("weblogUpdates.ping")]
        public WeblogUpdatesReply Ping( string weblogName, string weblogUrl )
        {
            return (WeblogUpdatesReply)this.Invoke("Ping", new object[]{weblogName, weblogUrl});
        }
	}


    /// <summary>
    /// This is the client proxy for notifying blo.gs on update
    /// </summary>
    public class ExtendedWeblogUpdatesClientProxy :XmlRpcClientProtocol
    {
        public ExtendedWeblogUpdatesClientProxy(string endpointUrl)
        {
            this.Url = endpointUrl;
        }

        [XmlRpcMethod("weblogUpdates.ping")]
        public WeblogUpdatesReply Ping( string weblogName, string weblogUrl )
        {
            return (WeblogUpdatesReply)this.Invoke("Ping", new object[]{weblogName, weblogUrl});
        }

        [XmlRpcMethod("weblogUpdates.extendedPing")]
        public WeblogUpdatesReply ExtendedPing( string weblogName, string weblogUrl, string checkUrl, string rssUrl )
        {
            return (WeblogUpdatesReply)this.Invoke("ExtendedPing", new object[]{weblogName, weblogUrl, checkUrl, rssUrl});
        }
    }

    [XmlRpcMissingMapping(MappingAction.Ignore)]
    public struct WeblogUpdatesReply
    {
        [XmlRpcMember]
        public bool flerror;
        [XmlRpcMember]
        public string message;
    }
}
