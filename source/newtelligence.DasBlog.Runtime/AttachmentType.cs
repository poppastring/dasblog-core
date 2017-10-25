using System.Xml.Serialization;

namespace newtelligence.DasBlog.Runtime
{
	[XmlRoot(Namespace=Data.NamespaceURI)]
	[XmlType(Namespace=Data.NamespaceURI)]
	public enum AttachmentType
	{
		Enclosure, Attachment, Picture, Unknown
	}
}
