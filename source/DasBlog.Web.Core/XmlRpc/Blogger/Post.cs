using CookComputing.XmlRpc;
using System;
using System.Collections.Generic;
using System.Text;

namespace DasBlog.Core.XmlRpc.Blogger
{
    public struct Post
    {
        /// <summary>
        /// 
        /// </summary>
        public System.DateTime dateCreated;
        /// <summary>
        /// 
        /// </summary>
        [XmlRpcMember(
             Description = "Depending on server may be either string or integer. "
             + "Use Convert.ToInt32(userid) to treat as integer or "
             + "Convert.ToString(userid) to treat as string")]
        public object userid;
        /// <summary>
        /// 
        /// </summary>
        public string postid;
        /// <summary>
        /// 
        /// </summary>
        public string content;
    }
}
