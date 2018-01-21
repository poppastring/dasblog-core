using CookComputing.XmlRpc;
using System;
using System.Collections.Generic;
using System.Text;

namespace DasBlog.Web.UI.Core.XmlRpc.MoveableType
{
    public struct TextFilter
    {
        [XmlRpcMember(Description = "unique string identifying a text formatting plugin")]
        public string key;

        [XmlRpcMember(Description = "readable description to be displayed to a user")]
        public string value;
    }
}
