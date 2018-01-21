using CookComputing.XmlRpc;
using System;
using System.Collections.Generic;
using System.Text;

namespace DasBlog.Web.Core.XmlRpc.MoveableType
{
    public struct Category
    {
        public string categoryId;

        [XmlRpcMissingMapping(MappingAction.Ignore)]
        public string categoryName;
        [XmlRpcMissingMapping(MappingAction.Ignore)]
        public bool isPrimary;
    }
}