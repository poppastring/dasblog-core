using CookComputing.XmlRpc;

namespace DasBlog.Web.Core.XmlRpc.MetaWeblog
{
    public struct MediaType
    {
        public string name;

        [XmlRpcMissingMapping(MappingAction.Ignore)]
        public string type;

        public byte[] bits;
    }
}
