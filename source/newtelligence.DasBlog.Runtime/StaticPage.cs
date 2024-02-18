using NodaTime;
using System;
using System.Collections;
using System.Globalization;
using System.IO;
using System.Net;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace newtelligence.DasBlog.Runtime
{
	[Serializable]
	[XmlRoot(Namespace=Data.NamespaceURI)]
	[XmlType(Namespace=Data.NamespaceURI)]
	public class StaticPage 
	{
        string _content;
        string _name;

        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
            }
        }
        public string Content
        {
            get
            {
                return _content;
            }
            set
            {
                _content = value;
            }
        }
    }
}
