using CookComputing.XmlRpc;

namespace DasBlog.Core.XmlRpc.MetaWeblog
{
    public struct MediaType
    {
        public string name;

        [XmlRpcMissingMapping(MappingAction.Ignore)]
        public string type;

        public byte[] bits;
    }
}
