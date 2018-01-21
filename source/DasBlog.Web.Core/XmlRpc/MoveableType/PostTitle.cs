using CookComputing.XmlRpc;
using System;
using System.Collections.Generic;
using System.Text;

namespace DasBlog.Web.Core.XmlRpc.MoveableType
{
    public struct PostTitle
    {
        [XmlRpcMember(Description = "This is in the timezone of the weblog blogid.")]
        public DateTime dateCreated;

        public string postid;

        public string userid;

        public string title;
    }
}