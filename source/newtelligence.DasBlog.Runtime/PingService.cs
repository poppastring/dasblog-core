using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Xml.Serialization;

namespace newtelligence.DasBlog.Runtime
{
    /// <summary>
    /// Summary description for CrossPostServerInfo.
    /// </summary>
    public class PingService
    {
        public enum PingApiType
        {
            Basic,
            Extended
        }

        private string name;
        private string url;
        private string endpoint;
        private PingApiType pingApiType = PingApiType.Basic;

        public string Endpoint
        {
            get { return endpoint; }
            set { endpoint = value; }
        }

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public string Url
        {
            get { return url; }
            set { url = value; }
        }

        public PingApiType PingApi
        {
            get { return pingApiType; }
            set { pingApiType = value; }
        }

        [XmlIgnore]
        public string Hyperlink
        {
            get
            {
                return String.Format("<a href=\"{0}\">{1}</a>", this.url, this.name);
            }
        }

        public PingService()
        {
        }

        public static PingServiceCollection GetDefaultPingServices()
        {
            PingServiceCollection pingServiceCollection = new PingServiceCollection();
            pingServiceCollection.Add(GetWebLogsDotCom());
            pingServiceCollection.Add(GetBloGs());
            return pingServiceCollection;
        }

        public static PingService GetWebLogsDotCom()
        {
            PingService ping = new PingService();
            ping.Name = "Weblogs";
            ping.Url = "http://www.weblogs.com";
            ping.Endpoint = "http://rpc.weblogs.com/RPC2";
            ping.PingApi = PingService.PingApiType.Basic;
            return ping;
        }

        public static PingService GetBloGs()
        {
            PingService ping = new PingService();
            ping.Name = "blo.gs";
            ping.Url = "http://www.blo.gs";
            ping.Endpoint = "http://ping.blo.gs/";
            ping.PingApi = PingService.PingApiType.Extended;
            return ping;
        }
    }

    /// <summary>
    /// A collection of elements of type PingServiceInfo
    /// </summary>
    [Serializable]
    public class PingServiceCollection : Collection<PingService>
    {
        /// <summary>
        /// Initializes a new empty instance of the PingServiceCollection class.
        /// </summary>
        public PingServiceCollection()
            : base()
        {
            // empty
        }

        /// <summary>
        /// Initializes a new instance of the PingServiceCollection class, containing elements
        /// copied from an array.
        /// </summary>
        /// <param name="items">
        /// The array whose elements are to be added to the new PingServiceCollection.
        /// </param>
        public PingServiceCollection(IList<PingService> items)
            : base()
        {
            if (items == null) {
                throw new ArgumentNullException("items");
            }

            this.AddRange(items);

        }

        /// <summary>
        /// Adds the elements of an array to the end of this PingServiceCollection.
        /// </summary>
        /// <param name="items">
        /// The array whose elements are to be added to the end of this PingServiceCollection.
        /// </param>
        public virtual void AddRange(IEnumerable<PingService> items)
        {
            if (items == null)
            {
                throw new ArgumentNullException("items");
            }

            foreach (PingService item in items)
            {
                this.Items.Add(item);
            }
        }


    }
}
