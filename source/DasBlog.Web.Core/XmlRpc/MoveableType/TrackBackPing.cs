using CookComputing.XmlRpc;
using System;
using System.Collections.Generic;
using System.Text;

namespace DasBlog.Web.Core.XmlRpc.MoveableType
{
    public struct TrackbackPing
    {
        [XmlRpcMember(Description = "The title of the entry sent in the ping.")]
        public string pingTitle;

        [XmlRpcMember(Description = "The URL of the entry.")]
        public string pingURL;

        [XmlRpcMember(Description = "The IP address of the host that sent the ping.")]
        public string pingIP;
    }
}