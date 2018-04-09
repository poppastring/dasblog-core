using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace DasBlog.Core.Configuration
{
    [Serializable]
    [XmlType(Namespace = "urn:newtelligence-com:dasblog:config")]
    [XmlRoot(Namespace = "urn:newtelligence-com:dasblog:config")]
    public class ContentFilter
    {
        string expression = "";
        string mapTo;
        bool isRegex = false;

        public ContentFilter()
        {
        }

        public ContentFilter(string expression, string mapTo)
        {
            this.expression = expression;
            this.mapTo = mapTo;
        }

        [XmlAttribute("find")]
        public string Expression
        {
            get { return expression; }
            set { expression = value; }
        }

        [XmlAttribute("replace")]
        public string MapTo
        {
            get { return mapTo; }
            set { mapTo = value; }
        }

        [XmlAttribute("isregex")]
        public bool IsRegEx
        {
            get { return isRegex; }
            set { isRegex = value; }
        }

        [XmlAnyElement]
        public XmlElement[] anyElements;
        [XmlAnyAttribute]
        public XmlAttribute[] anyAttributes;
    }
}
