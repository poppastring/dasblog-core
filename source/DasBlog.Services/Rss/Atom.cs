using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;

namespace DasBlog.Services.Rss.Atom
{
    /// <summary>
    /// Atom 1.0 Feed Root Element
    /// </summary>
    [XmlRoot("feed", Namespace = "http://www.w3.org/2005/Atom")]
    public class AtomRoot
    {
        private readonly string generator;
        private List<AtomEntry> entries;

        public AtomRoot()
        {
            generator = "dasBlog Core " + GetType().Assembly.GetName().Version;
            entries = new List<AtomEntry>();
            Links = new List<AtomLink>();
        }

        [XmlElement("title")]
        public AtomText Title { get; set; }

        [XmlElement("subtitle")]
        public AtomText Subtitle { get; set; }

        [XmlElement("id")]
        public string Id { get; set; }

        [XmlElement("updated")]
        public string Updated { get; set; }

        [XmlElement("generator")]
        public string Generator => generator;

        [XmlElement("rights")]
        public string Rights { get; set; }

        [XmlElement("icon")]
        public string Icon { get; set; }

        [XmlElement("logo")]
        public string Logo { get; set; }

        [XmlElement("link")]
        public List<AtomLink> Links { get; set; }

        [XmlElement("author")]
        public AtomPerson Author { get; set; }

        [XmlElement("entry")]
        public List<AtomEntry> Entries
        {
            get => entries;
            set => entries = value;
        }

        [XmlAnyElement]
        public XmlElement[] AnyElements { get; set; }
    }

    /// <summary>
    /// Atom Entry Element
    /// </summary>
    public class AtomEntry
    {
        public AtomEntry()
        {
            Links = new List<AtomLink>();
            Categories = new List<AtomCategory>();
        }

        [XmlElement("title")]
        public AtomText Title { get; set; }

        [XmlElement("id")]
        public string Id { get; set; }

        [XmlElement("updated")]
        public string Updated { get; set; }

        [XmlElement("published")]
        public string Published { get; set; }

        [XmlElement("author")]
        public AtomPerson Author { get; set; }

        [XmlElement("link")]
        public List<AtomLink> Links { get; set; }

        [XmlElement("category")]
        public List<AtomCategory> Categories { get; set; }

        [XmlElement("summary")]
        public AtomText Summary { get; set; }

        [XmlElement("content")]
        public AtomContent Content { get; set; }

        [XmlAnyElement]
        public XmlElement[] AnyElements { get; set; }
    }

    /// <summary>
    /// Atom Text Construct (for title, subtitle, summary, rights)
    /// </summary>
    public class AtomText
    {
        [XmlAttribute("type")]
        public string Type { get; set; }

        [XmlText]
        public string Text { get; set; }

        public AtomText() { }

        public AtomText(string text, string type = "text")
        {
            Text = text;
            Type = type;
        }
    }

    /// <summary>
    /// Atom Content Element
    /// </summary>
    public class AtomContent
    {
        [XmlAttribute("type")]
        public string Type { get; set; }

        [XmlAttribute("src")]
        public string Src { get; set; }

        [XmlText]
        public string Text { get; set; }

        public AtomContent() { }

        public AtomContent(string text, string type = "html")
        {
            Text = text;
            Type = type;
        }
    }

    /// <summary>
    /// Atom Link Element
    /// </summary>
    public class AtomLink
    {
        [XmlAttribute("href")]
        public string Href { get; set; }

        [XmlAttribute("rel")]
        public string Rel { get; set; }

        [XmlAttribute("type")]
        public string Type { get; set; }

        [XmlAttribute("hreflang")]
        public string HrefLang { get; set; }

        [XmlAttribute("title")]
        public string Title { get; set; }

        [XmlAttribute("length")]
        public string Length { get; set; }

        public AtomLink() { }

        public AtomLink(string href, string rel = null, string type = null)
        {
            Href = href;
            Rel = rel;
            Type = type;
        }
    }

    /// <summary>
    /// Atom Person Construct (for author, contributor)
    /// </summary>
    public class AtomPerson
    {
        [XmlElement("name")]
        public string Name { get; set; }

        [XmlElement("uri")]
        public string Uri { get; set; }

        [XmlElement("email")]
        public string Email { get; set; }

        public AtomPerson() { }

        public AtomPerson(string name, string email = null, string uri = null)
        {
            Name = name;
            Email = email;
            Uri = uri;
        }
    }

    /// <summary>
    /// Atom Category Element
    /// </summary>
    public class AtomCategory
    {
        [XmlAttribute("term")]
        public string Term { get; set; }

        [XmlAttribute("scheme")]
        public string Scheme { get; set; }

        [XmlAttribute("label")]
        public string Label { get; set; }

        public AtomCategory() { }

        public AtomCategory(string term, string label = null)
        {
            Term = term;
            Label = label;
        }
    }
}
