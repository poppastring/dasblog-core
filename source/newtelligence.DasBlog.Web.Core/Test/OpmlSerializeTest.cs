using System;
using System.Collections.Generic;
using System.Text;

using NUnit.Framework;
using System.Xml.Serialization;
using System.IO;


namespace newtelligence.DasBlog.Web.Core.Test
{
    [TestFixture()]
    public class OpmlSerializeTest
    {

        [Test]
        public void Serialize()
        {
            Opml opml = new Opml();
OpmlOutline a           =new OpmlOutline();
            a.title = "a";
            a.xmlUrl ="a.xml";
            opml.body.outline.Add(a);

            XmlSerializer ser = new XmlSerializer(typeof(Opml));
            using (Stream s = File.OpenWrite("opml.txt"))
            {
                ser.Serialize(s, opml);
            }

        }

        [Test]
        public void DeSerialize()
        {

            XmlSerializer ser = new XmlSerializer(typeof(Opml));
            using (Stream s = File.OpenRead("..\\blogroll.opml"))
            {
                ser.Deserialize(s);
            }

        }


    }
}
