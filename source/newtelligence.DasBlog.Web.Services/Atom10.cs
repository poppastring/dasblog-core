#region Copyright (c) 2003, newtelligence AG. All rights reserved.
/*
// Copyright (c) 2003, newtelligence AG. (http://www.newtelligence.com)
// Original BlogX Source Code: Copyright (c) 2003, Chris Anderson (http://simplegeek.com)
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, are permitted
// provided that the following conditions are met:
//
// (1) Redistributions of source code must retain the above copyright notice, this list of
// conditions and the following disclaimer.
// (2) Redistributions in binary form must reproduce the above copyright notice, this list of
// conditions and the following disclaimer in the documentation and/or other materials
// provided with the distribution.
// (3) Neither the name of the newtelligence AG nor the names of its contributors may be used
// to endorse or promote products derived from this software without specific prior
// written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS
// OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY
// AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
// DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE,
// DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER
// IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT
// OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
// -------------------------------------------------------------------------
//
// Original BlogX source code (c) 2003 by Chris Anderson (http://simplegeek.com)
//
// newtelligence is a registered trademark of newtelligence Aktiengesellschaft.
//
// For portions of this software, the some additional copyright notices may apply
// which can either be found in the license.txt file included in the source distribution
// or following this notice.
//
*/
#endregion

using System;
using System.Xml;
using System.Xml.Serialization;

namespace newtelligence.DasBlog.Web.Services.Atom10
{
	[XmlRoot("feed", Namespace="http://www.w3.org/2005/Atom")]
	public class AtomRoot
	{
		public AtomRoot()
		{
			Namespaces = new XmlSerializerNamespaces();
			entries = new AtomEntryCollection();
			links = new AtomLinkCollection();
			contributors = new AtomParticipantCollection();
		}

		[XmlNamespaceDeclarations]
		public XmlSerializerNamespaces Namespaces;

		string xmlbase;
		[XmlAttribute("base", Form=System.Xml.Schema.XmlSchemaForm.Qualified, Namespace="http://www.w3.org/XML/1998/namespace")]
		public string XmlBase { get { return xmlbase; } set { xmlbase = value; } }

		string lang;
		[XmlAttribute("lang", Form=System.Xml.Schema.XmlSchemaForm.Qualified, Namespace="http://www.w3.org/XML/1998/namespace")]
		public string Lang { get { return lang; } set { lang = value; } }

		string title;
		[XmlElement("title")]
		public string Title { get { return title; } set { title = value; } }

		AtomLinkCollection links;
		[XmlElement("link")]
		public AtomLinkCollection Links { get { return links; } set { links = value; } }

		string logo;
		[XmlElement("logo")]
		public string Logo { get { return logo; } set { logo = value; } }

		string icon;
		[XmlElement("icon")]
		public string Icon { get { return icon; } set { icon = value; } }

		DateTime modified;
		[XmlIgnore]
		public DateTime ModifiedUtc { get { return modified; } set { modified = value; } }
        [XmlElement("updated")]
        public DateTime ModifiedLocalTime { get { return (ModifiedUtc==DateTime.MinValue||ModifiedUtc==DateTime.MaxValue)?ModifiedUtc:ModifiedUtc.ToLocalTime(); } set { ModifiedUtc = (value==DateTime.MinValue||value==DateTime.MaxValue)?value:value.ToUniversalTime(); } }

		AtomParticipant author;
		[XmlElement("author")]
		public AtomParticipant Author { get { return author; } set { author = value; } }

		string tagline;
		[XmlElement("subtitle")]
		public string Tagline { get { return tagline; } set { tagline = value; } }

		string id;
		[XmlElement("id")]
		public string Id { get { return id; } set { id = value; } }

		string copyright;
		[XmlElement("rights")]
		public string Copyright { get { return copyright; } set { copyright = value; } }

		AtomGenerator generator;
		[XmlElement("generator")]
		public AtomGenerator Generator { get { return generator; } set { generator = value; } }

		AtomParticipantCollection contributors;
		[XmlElement("contributor")]
		public AtomParticipantCollection Contributors { get { return contributors; } set { contributors = value; } }

		AtomEntryCollection entries;
		[XmlElement("entry")]
		public AtomEntryCollection Entries { get { return entries; } set { entries = value; } }

        [XmlAnyElement]
        public XmlElement[] anyElements;
        [XmlAnyAttribute]
        public XmlAttribute[] anyAttributes;
	}

	[XmlRoot("entry")]
	public class AtomLink
	{
		string rel;
		[XmlAttribute("rel")]
		public string Rel { get { return rel; } set { rel = value; } }

		string type;
		[XmlAttribute("type")]
		public string Type { get { return type; } set { type = value; } }

		string href;
		[XmlAttribute("href")]
		public string Href { get { return href; } set { href = value; } }

		[XmlAnyElement]
		public XmlElement[] anyElements;
		[XmlAnyAttribute]
		public XmlAttribute[] anyAttributes;

		public AtomLink()
		{
		}

		public AtomLink(string href)
		{
			this.Rel = "alternate";
			this.Type = "text/html";
			this.Href = href;
		}

		public AtomLink(string href, string rel, string type)
		{
			this.Rel = rel;
			this.Type = type;
			this.Href = href;
		}

	}

	[XmlRoot("generator")]
	public class AtomGenerator
	{
		[XmlAttribute("uri")]
		public string Uri { get { return "http://dasblog.info/";}set{} }

		string version;
		[XmlAttribute("version")]
		public string Version {get { return version;} set{version = value;}}

		[XmlText]
		public string Text { get { return "DasBlog"; } set{} }

        [XmlAnyElement]
        public XmlElement[] anyElements;
        [XmlAnyAttribute]
        public XmlAttribute[] anyAttributes;
	}


	[XmlRoot("category")]
	public class AtomCategory
	{
		public AtomCategory() {}

		private string _term;
		[XmlAttribute("term")]
		public string Term{get{return _term;}set{_term = value;}}

		private string _label;
		[XmlAttribute("label")]
		public string Label{get{return _label;}set{_label = value;}}

		private string _scheme;
		[XmlAttribute("scheme")]
		public string Scheme{get{return _scheme;}set{_scheme = value;}}
	}


	[XmlRoot("entry")]
	public class AtomEntry
	{
		public AtomEntry() {}

		public AtomEntry(string title, AtomLink link, string id, DateTime issued, DateTime modified)
		{
			this.Title = title;
			this.Link = link;
			this.Id = id;
			this.IssuedUtc = issued;
			this.ModifiedUtc = modified;
		}

		string title;
		[XmlElement("title")]
		public string Title { get { return title; } set { title = value; } }

		AtomLink link;
		[XmlElement("link")]
		public AtomLink Link { get { return link; } set { link = value; } }

		string id;
		[XmlElement("id")]
		public string Id { get { return id; } set { id = value; } }

		DateTime issued;
		[XmlIgnore]
		public DateTime IssuedUtc { get { return issued; } set { issued = value; } }
        [XmlElement("published")]
        public DateTime IssuedLocalTime { get { return (IssuedUtc==DateTime.MinValue||IssuedUtc==DateTime.MaxValue)?IssuedUtc:IssuedUtc.ToLocalTime(); } set { IssuedUtc = (value==DateTime.MinValue||value==DateTime.MaxValue)?value:value.ToUniversalTime(); } }

		DateTime modified;
		[XmlIgnore]
		public DateTime ModifiedUtc { get { return modified; } set { modified = value; } }
        [XmlElement("updated")]
        public DateTime ModifiedLocalTime { get { return (ModifiedUtc==DateTime.MinValue||ModifiedUtc==DateTime.MaxValue)?ModifiedUtc:ModifiedUtc.ToLocalTime(); } set { ModifiedUtc = (value==DateTime.MinValue||value==DateTime.MaxValue)?value:value.ToUniversalTime(); } }

		private AtomCategoryCollection categories;
		[XmlElement("category")]
		public AtomCategoryCollection Categories {get { return categories; } set { categories = value; } }

		string summary;
		[XmlElement("summary")]
		public string Summary { get { return summary; } set { summary = value; } }

		AtomParticipant author;
		[XmlElement("author")]
		public AtomParticipant Author { get { return author; } set { author = value; } }

		AtomContent content;
		[XmlElement("content")]
		public AtomContent Content { get { return content; } set { content = value; } }

        [XmlAnyElement]
        public XmlElement[] anyElements;
        [XmlAnyAttribute]
        public XmlAttribute[] anyAttributes;
	}

	public class AtomParticipant
	{
		string name;
		[XmlElement("name")]
		public string Name { get { return name; } set { name = value; } }

		string url;
		[XmlElement("url")]
		public string Url { get { return url; } set { url = value; } }

		string email;
		[XmlElement("email")]
		public string EMail { get { return email; } set { email = value; } }

        [XmlAnyElement]
        public XmlElement[] anyElements;
        [XmlAnyAttribute]
        public XmlAttribute[] anyAttributes;
	}

	[XmlRoot("content")]
	public class AtomContent
	{
		string type;
		[XmlAttribute("type")]
		public string Type { get { return type; } set { type = value; } }

		string lang;
		[XmlAttribute("lang", Form=System.Xml.Schema.XmlSchemaForm.Qualified, Namespace="http://www.w3.org/XML/1998/namespace")]
		public string Lang { get { return lang; } set { lang = value; } }

		string textContent;
		[System.Xml.Serialization.XmlText]
		public string TextContent { get { return textContent; } set { textContent = value; } }

        [XmlAnyElement]
        public XmlElement[] anyElements;
        [XmlAnyAttribute]
        public XmlAttribute[] anyAttributes;
	}

	public enum AtomContentMode
	{
		[XmlEnum(Name = "xml")]
		Xml,
		[XmlEnum(Name = "escaped")]
		Escaped,
		[XmlEnum(Name = "base64")]
		Base64,
	}

	/// <summary>
	/// A collection of elements of type AtomEntry
	/// </summary>
	public class AtomEntryCollection: System.Collections.CollectionBase
	{
		/// <summary>
		/// Initializes a new empty instance of the AtomEntryCollection class.
		/// </summary>
		public AtomEntryCollection()
		{
			// empty
		}

		/// <summary>
		/// Initializes a new instance of the AtomEntryCollection class, containing elements
		/// copied from an array.
		/// </summary>
		/// <param name="entries">
		/// The array whose elements are to be added to the new AtomEntryCollection.
		/// </param>
		public AtomEntryCollection(AtomEntry[] entries)
		{
			this.AddRange(entries);
		}

		/// <summary>
		/// Initializes a new instance of the AtomEntryCollection class, containing elements
		/// copied from another instance of AtomEntryCollection
		/// </summary>
		/// <param name="entries">
		/// The AtomEntryCollection whose elements are to be added to the new AtomEntryCollection.
		/// </param>
		public AtomEntryCollection(AtomEntryCollection entries)
		{
			this.AddRange(entries);
		}

		/// <summary>
		/// Adds the elements of an array to the end of this AtomEntryCollection.
		/// </summary>
		/// <param name="entries">
		/// The array whose elements are to be added to the end of this AtomEntryCollection.
		/// </param>
		public virtual void AddRange(AtomEntry[] entries)
		{
			foreach (AtomEntry entry in entries)
			{
				this.List.Add(entry);
			}
		}

		/// <summary>
		/// Adds the elements of another AtomEntryCollection to the end of this AtomEntryCollection.
		/// </summary>
		/// <param name="entries">
		/// The AtomEntryCollection whose elements are to be added to the end of this AtomEntryCollection.
		/// </param>
		public virtual void AddRange(AtomEntryCollection entries)
		{
			foreach (AtomEntry entry in entries)
			{
				this.List.Add(entry);
			}
		}

		/// <summary>
		/// Adds an instance of type AtomEntry to the end of this AtomEntryCollection.
		/// </summary>
		/// <param name="value">
		/// The AtomEntry to be added to the end of this AtomEntryCollection.
		/// </param>
		public virtual void Add(AtomEntry value)
		{
			this.List.Add(value);
		}

		/// <summary>
		/// Determines whether a specfic AtomEntry value is in this AtomEntryCollection.
		/// </summary>
		/// <param name="value">
		/// The AtomEntry value to locate in this AtomEntryCollection.
		/// </param>
		/// <returns>
		/// true if value is found in this AtomEntryCollection;
		/// false otherwise.
		/// </returns>
		public virtual bool Contains(AtomEntry value)
		{
			return this.List.Contains(value);
		}

		/// <summary>
		/// Return the zero-based index of the first occurrence of a specific value
		/// in this AtomEntryCollection
		/// </summary>
		/// <param name="value">
		/// The AtomEntry value to locate in the AtomEntryCollection.
		/// </param>
		/// <returns>
		/// The zero-based index of the first occurrence of the _ELEMENT value if found;
		/// -1 otherwise.
		/// </returns>
		public virtual int IndexOf(AtomEntry value)
		{
			return this.List.IndexOf(value);
		}

		/// <summary>
		/// Inserts an element into the AtomEntryCollection at the specified index
		/// </summary>
		/// <param name="index">
		/// The index at which the AtomEntry is to be inserted.
		/// </param>
		/// <param name="value">
		/// The AtomEntry to insert.
		/// </param>
		public virtual void Insert(int index, AtomEntry value)
		{
			this.List.Insert(index, value);
		}

		/// <summary>
		/// Gets or sets the AtomEntry at the given index in this AtomEntryCollection.
		/// </summary>
		public virtual AtomEntry this[int index]
		{
			get
			{
				return (AtomEntry) this.List[index];
			}
			set
			{
				this.List[index] = value;
			}
		}

		/// <summary>
		/// Removes the first occurrence of a specific AtomEntry from this AtomEntryCollection.
		/// </summary>
		/// <param name="value">
		/// The AtomEntry value to remove from this AtomEntryCollection.
		/// </param>
		public virtual void Remove(AtomEntry value)
		{
			this.List.Remove(value);
		}

		/// <summary>
		/// Type-specific enumeration class, used by AtomEntryCollection.GetEnumerator.
		/// </summary>
		public class Enumerator: System.Collections.IEnumerator
		{
			private System.Collections.IEnumerator wrapped;

			public Enumerator(AtomEntryCollection collection)
			{
				this.wrapped = ((System.Collections.CollectionBase)collection).GetEnumerator();
			}

			public AtomEntry Current
			{
				get
				{
					return (AtomEntry) (this.wrapped.Current);
				}
			}

			object System.Collections.IEnumerator.Current
			{
				get
				{
					return (AtomEntry) (this.wrapped.Current);
				}
			}

			public bool MoveNext()
			{
				return this.wrapped.MoveNext();
			}

			public void Reset()
			{
				this.wrapped.Reset();
			}
		}

		/// <summary>
		/// Returns an enumerator that can iterate through the elements of this AtomEntryCollection.
		/// </summary>
		/// <returns>
		/// An object that implements System.Collections.IEnumerator.
		/// </returns>
		public new virtual AtomEntryCollection.Enumerator GetEnumerator()
		{
			return new AtomEntryCollection.Enumerator(this);
		}
	}


	/// <summary>
	/// A collection of elements of type AtomParticipant
	/// </summary>
	public class AtomParticipantCollection: System.Collections.CollectionBase
	{
		/// <summary>
		/// Initializes a new empty instance of the AtomParticipantCollection class.
		/// </summary>
		public AtomParticipantCollection()
		{
			// empty
		}

		/// <summary>
		/// Initializes a new instance of the AtomParticipantCollection class, containing elements
		/// copied from an array.
		/// </summary>
		/// <param name="participants">
		/// The array whose elements are to be added to the new AtomParticipantCollection.
		/// </param>
		public AtomParticipantCollection(AtomParticipant[] participants)
		{
			this.AddRange(participants);
		}

		/// <summary>
		/// Initializes a new instance of the AtomParticipantCollection class, containing elements
		/// copied from another instance of AtomParticipantCollection
		/// </summary>
		/// <param name="participants">
		/// The AtomParticipantCollection whose elements are to be added to the new AtomParticipantCollection.
		/// </param>
		public AtomParticipantCollection(AtomParticipantCollection participants)
		{
			this.AddRange(participants);
		}

		/// <summary>
		/// Adds the elements of an array to the end of this AtomParticipantCollection.
		/// </summary>
		/// <param name="participants">
		/// The array whose elements are to be added to the end of this AtomParticipantCollection.
		/// </param>
		public virtual void AddRange(AtomParticipant[] participants)
		{
			foreach (AtomParticipant participant in participants)
			{
				this.List.Add(participant);
			}
		}

		/// <summary>
		/// Adds the elements of another AtomParticipantCollection to the end of this AtomParticipantCollection.
		/// </summary>
		/// <param name="participants">
		/// The AtomParticipantCollection whose elements are to be added to the end of this AtomParticipantCollection.
		/// </param>
		public virtual void AddRange(AtomParticipantCollection participants)
		{
			foreach (AtomParticipant participant in participants)
			{
				this.List.Add(participant);
			}
		}

		/// <summary>
		/// Adds an instance of type AtomParticipant to the end of this AtomParticipantCollection.
		/// </summary>
		/// <param name="value">
		/// The AtomParticipant to be added to the end of this AtomParticipantCollection.
		/// </param>
		public virtual void Add(AtomParticipant value)
		{
			this.List.Add(value);
		}

		/// <summary>
		/// Determines whether a specfic AtomParticipant value is in this AtomParticipantCollection.
		/// </summary>
		/// <param name="value">
		/// The AtomParticipant value to locate in this AtomParticipantCollection.
		/// </param>
		/// <returns>
		/// true if value is found in this AtomParticipantCollection;
		/// false otherwise.
		/// </returns>
		public virtual bool Contains(AtomParticipant value)
		{
			return this.List.Contains(value);
		}

		/// <summary>
		/// Return the zero-based index of the first occurrence of a specific value
		/// in this AtomParticipantCollection
		/// </summary>
		/// <param name="value">
		/// The AtomParticipant value to locate in the AtomParticipantCollection.
		/// </param>
		/// <returns>
		/// The zero-based index of the first occurrence of the _ELEMENT value if found;
		/// -1 otherwise.
		/// </returns>
		public virtual int IndexOf(AtomParticipant value)
		{
			return this.List.IndexOf(value);
		}

		/// <summary>
		/// Inserts an element into the AtomParticipantCollection at the specified index
		/// </summary>
		/// <param name="index">
		/// The index at which the AtomParticipant is to be inserted.
		/// </param>
		/// <param name="value">
		/// The AtomParticipant to insert.
		/// </param>
		public virtual void Insert(int index, AtomParticipant value)
		{
			this.List.Insert(index, value);
		}

		/// <summary>
		/// Gets or sets the AtomParticipant at the given index in this AtomParticipantCollection.
		/// </summary>
		public virtual AtomParticipant this[int index]
		{
			get
			{
				return (AtomParticipant) this.List[index];
			}
			set
			{
				this.List[index] = value;
			}
		}

		/// <summary>
		/// Removes the first occurrence of a specific AtomParticipant from this AtomParticipantCollection.
		/// </summary>
		/// <param name="value">
		/// The AtomParticipant value to remove from this AtomParticipantCollection.
		/// </param>
		public virtual void Remove(AtomParticipant value)
		{
			this.List.Remove(value);
		}

		/// <summary>
		/// Type-specific enumeration class, used by AtomParticipantCollection.GetEnumerator.
		/// </summary>
		public class Enumerator: System.Collections.IEnumerator
		{
			private System.Collections.IEnumerator wrapped;

			public Enumerator(AtomParticipantCollection collection)
			{
				this.wrapped = ((System.Collections.CollectionBase)collection).GetEnumerator();
			}

			public AtomParticipant Current
			{
				get
				{
					return (AtomParticipant) (this.wrapped.Current);
				}
			}

			object System.Collections.IEnumerator.Current
			{
				get
				{
					return (AtomParticipant) (this.wrapped.Current);
				}
			}

			public bool MoveNext()
			{
				return this.wrapped.MoveNext();
			}

			public void Reset()
			{
				this.wrapped.Reset();
			}
		}

		/// <summary>
		/// Returns an enumerator that can iterate through the elements of this AtomParticipantCollection.
		/// </summary>
		/// <returns>
		/// An object that implements System.Collections.IEnumerator.
		/// </returns>
		public new virtual AtomParticipantCollection.Enumerator GetEnumerator()
		{
			return new AtomParticipantCollection.Enumerator(this);
		}
	}


	/// <summary>
	/// A collection of elements of type AtomContent
	/// </summary>
	public class AtomContentCollection: System.Collections.CollectionBase
	{
		/// <summary>
		/// Initializes a new empty instance of the AtomContentCollection class.
		/// </summary>
		public AtomContentCollection()
		{
			// empty
		}

		/// <summary>
		/// Initializes a new instance of the AtomContentCollection class, containing elements
		/// copied from an array.
		/// </summary>
		/// <param name="content">
		/// The array whose elements are to be added to the new AtomContentCollection.
		/// </param>
		public AtomContentCollection(AtomContent[] content)
		{
			this.AddRange(content);
		}

		/// <summary>
		/// Initializes a new instance of the AtomContentCollection class, containing elements
		/// copied from another instance of AtomContentCollection
		/// </summary>
		/// <param name="content">
		/// The AtomContentCollection whose elements are to be added to the new AtomContentCollection.
		/// </param>
		public AtomContentCollection(AtomContentCollection content)
		{
			this.AddRange(content);
		}

		/// <summary>
		/// Adds the elements of an array to the end of this AtomContentCollection.
		/// </summary>
		/// <param name="content">
		/// The array whose elements are to be added to the end of this AtomContentCollection.
		/// </param>
		public virtual void AddRange(AtomContent[] content)
		{
			foreach (AtomContent contentEntry in content)
			{
				this.List.Add(contentEntry);
			}
		}

		/// <summary>
		/// Adds the elements of another AtomContentCollection to the end of this AtomContentCollection.
		/// </summary>
		/// <param name="content">
		/// The AtomContentCollection whose elements are to be added to the end of this AtomContentCollection.
		/// </param>
		public virtual void AddRange(AtomContentCollection content)
		{
			foreach (AtomContent contentEntry in content)
			{
				this.List.Add(contentEntry);
			}
		}

		/// <summary>
		/// Adds an instance of type AtomContent to the end of this AtomContentCollection.
		/// </summary>
		/// <param name="value">
		/// The AtomContent to be added to the end of this AtomContentCollection.
		/// </param>
		public virtual void Add(AtomContent value)
		{
			this.List.Add(value);
		}

		/// <summary>
		/// Determines whether a specfic AtomContent value is in this AtomContentCollection.
		/// </summary>
		/// <param name="value">
		/// The AtomContent value to locate in this AtomContentCollection.
		/// </param>
		/// <returns>
		/// true if value is found in this AtomContentCollection;
		/// false otherwise.
		/// </returns>
		public virtual bool Contains(AtomContent value)
		{
			return this.List.Contains(value);
		}

		/// <summary>
		/// Return the zero-based index of the first occurrence of a specific value
		/// in this AtomContentCollection
		/// </summary>
		/// <param name="value">
		/// The AtomContent value to locate in the AtomContentCollection.
		/// </param>
		/// <returns>
		/// The zero-based index of the first occurrence of the _ELEMENT value if found;
		/// -1 otherwise.
		/// </returns>
		public virtual int IndexOf(AtomContent value)
		{
			return this.List.IndexOf(value);
		}

		/// <summary>
		/// Inserts an element into the AtomContentCollection at the specified index
		/// </summary>
		/// <param name="index">
		/// The index at which the AtomContent is to be inserted.
		/// </param>
		/// <param name="value">
		/// The AtomContent to insert.
		/// </param>
		public virtual void Insert(int index, AtomContent value)
		{
			this.List.Insert(index, value);
		}

		/// <summary>
		/// Gets or sets the AtomContent at the given index in this AtomContentCollection.
		/// </summary>
		public virtual AtomContent this[int index]
		{
			get
			{
				return (AtomContent) this.List[index];
			}
			set
			{
				this.List[index] = value;
			}
		}

		/// <summary>
		/// Removes the first occurrence of a specific AtomContent from this AtomContentCollection.
		/// </summary>
		/// <param name="value">
		/// The AtomContent value to remove from this AtomContentCollection.
		/// </param>
		public virtual void Remove(AtomContent value)
		{
			this.List.Remove(value);
		}

		/// <summary>
		/// Type-specific enumeration class, used by AtomContentCollection.GetEnumerator.
		/// </summary>
		public class Enumerator: System.Collections.IEnumerator
		{
			private System.Collections.IEnumerator wrapped;

			public Enumerator(AtomContentCollection collection)
			{
				this.wrapped = ((System.Collections.CollectionBase)collection).GetEnumerator();
			}

			public AtomContent Current
			{
				get
				{
					return (AtomContent) (this.wrapped.Current);
				}
			}

			object System.Collections.IEnumerator.Current
			{
				get
				{
					return (AtomContent) (this.wrapped.Current);
				}
			}

			public bool MoveNext()
			{
				return this.wrapped.MoveNext();
			}

			public void Reset()
			{
				this.wrapped.Reset();
			}
		}

		/// <summary>
		/// Returns an enumerator that can iterate through the elements of this AtomContentCollection.
		/// </summary>
		/// <returns>
		/// An object that implements System.Collections.IEnumerator.
		/// </returns>
		public new virtual AtomContentCollection.Enumerator GetEnumerator()
		{
			return new AtomContentCollection.Enumerator(this);
		}
	}

	/// <summary>
	/// A collection of elements of type AtomCategory
	/// </summary>
	public class AtomCategoryCollection: System.Collections.CollectionBase
	{
		/// <summary>
		/// Initializes a new empty instance of the AtomCategoryCollection class.
		/// </summary>
		public AtomCategoryCollection()
		{
			// empty
		}

		/// <summary>
		/// Initializes a new instance of the AtomCategoryCollection class, containing elements
		/// copied from an array.
		/// </summary>
		/// <param name="entries">
		/// The array whose elements are to be added to the new AtomCategoryCollection.
		/// </param>
		public AtomCategoryCollection(AtomCategory[] entries)
		{
			this.AddRange(entries);
		}

		/// <summary>
		/// Initializes a new instance of the AtomCategoryCollection class, containing elements
		/// copied from another instance of AtomCategoryCollection
		/// </summary>
		/// <param name="entries">
		/// The AtomCategoryCollection whose elements are to be added to the new AtomCategoryCollection.
		/// </param>
		public AtomCategoryCollection(AtomCategoryCollection entries)
		{
			this.AddRange(entries);
		}

		/// <summary>
		/// Adds the elements of an array to the end of this AtomCategoryCollection.
		/// </summary>
		/// <param name="entries">
		/// The array whose elements are to be added to the end of this AtomCategoryCollection.
		/// </param>
		public virtual void AddRange(AtomCategory[] entries)
		{
			foreach (AtomCategory entry in entries)
			{
				this.List.Add(entry);
			}
		}

		/// <summary>
		/// Adds the elements of another AtomCategoryCollection to the end of this AtomCategoryCollection.
		/// </summary>
		/// <param name="entries">
		/// The AtomCategoryCollection whose elements are to be added to the end of this AtomCategoryCollection.
		/// </param>
		public virtual void AddRange(AtomCategoryCollection entries)
		{
			foreach (AtomCategory entry in entries)
			{
				this.List.Add(entry);
			}
		}

		/// <summary>
		/// Adds an instance of type AtomCategory to the end of this AtomCategoryCollection.
		/// </summary>
		/// <param name="value">
		/// The AtomCategory to be added to the end of this AtomCategoryCollection.
		/// </param>
		public virtual void Add(AtomCategory value)
		{
			this.List.Add(value);
		}

		/// <summary>
		/// Determines whether a specfic AtomCategory value is in this AtomCategoryCollection.
		/// </summary>
		/// <param name="value">
		/// The AtomCategory value to locate in this AtomCategoryCollection.
		/// </param>
		/// <returns>
		/// true if value is found in this AtomCategoryCollection;
		/// false otherwise.
		/// </returns>
		public virtual bool Contains(AtomCategory value)
		{
			return this.List.Contains(value);
		}

		/// <summary>
		/// Return the zero-based index of the first occurrence of a specific value
		/// in this AtomCategoryCollection
		/// </summary>
		/// <param name="value">
		/// The AtomCategory value to locate in the AtomCategoryCollection.
		/// </param>
		/// <returns>
		/// The zero-based index of the first occurrence of the _ELEMENT value if found;
		/// -1 otherwise.
		/// </returns>
		public virtual int IndexOf(AtomCategory value)
		{
			return this.List.IndexOf(value);
		}

		/// <summary>
		/// Inserts an element into the AtomCategoryCollection at the specified index
		/// </summary>
		/// <param name="index">
		/// The index at which the AtomCategory is to be inserted.
		/// </param>
		/// <param name="value">
		/// The AtomCategory to insert.
		/// </param>
		public virtual void Insert(int index, AtomCategory value)
		{
			this.List.Insert(index, value);
		}

		/// <summary>
		/// Gets or sets the AtomCategory at the given index in this AtomCategoryCollection.
		/// </summary>
		public virtual AtomCategory this[int index]
		{
			get
			{
				return (AtomCategory) this.List[index];
			}
			set
			{
				this.List[index] = value;
			}
		}

		/// <summary>
		/// Removes the first occurrence of a specific AtomCategory from this AtomCategoryCollection.
		/// </summary>
		/// <param name="value">
		/// The AtomCategory value to remove from this AtomCategoryCollection.
		/// </param>
		public virtual void Remove(AtomCategory value)
		{
			this.List.Remove(value);
		}

		/// <summary>
		/// Type-specific enumeration class, used by AtomCategoryCollection.GetEnumerator.
		/// </summary>
		public class Enumerator: System.Collections.IEnumerator
		{
			private System.Collections.IEnumerator wrapped;

			public Enumerator(AtomCategoryCollection collection)
			{
				this.wrapped = ((System.Collections.CollectionBase)collection).GetEnumerator();
			}

			public AtomCategory Current
			{
				get
				{
					return (AtomCategory) (this.wrapped.Current);
				}
			}

			object System.Collections.IEnumerator.Current
			{
				get
				{
					return (AtomCategory) (this.wrapped.Current);
				}
			}

			public bool MoveNext()
			{
				return this.wrapped.MoveNext();
			}

			public void Reset()
			{
				this.wrapped.Reset();
			}
		}

		/// <summary>
		/// Returns an enumerator that can iterate through the elements of this AtomCategoryCollection.
		/// </summary>
		/// <returns>
		/// An object that implements System.Collections.IEnumerator.
		/// </returns>
		public new virtual AtomCategoryCollection.Enumerator GetEnumerator()
		{
			return new AtomCategoryCollection.Enumerator(this);
		}
	}


	/// <summary>
	/// A collection of elements of type AtomLink
	/// </summary>
	public class AtomLinkCollection: System.Collections.CollectionBase
	{
		/// <summary>
		/// Initializes a new empty instance of the AtomLinkCollection class.
		/// </summary>
		public AtomLinkCollection()
		{
			// empty
		}

		/// <summary>
		/// Initializes a new instance of the AtomLinkCollection class, containing elements
		/// copied from an array.
		/// </summary>
		/// <param name="participants">
		/// The array whose elements are to be added to the new AtomLinkCollection.
		/// </param>
		public AtomLinkCollection(AtomLink[] participants)
		{
			this.AddRange(participants);
		}

		/// <summary>
		/// Initializes a new instance of the AtomLinkCollection class, containing elements
		/// copied from another instance of AtomLinkCollection
		/// </summary>
		/// <param name="participants">
		/// The AtomLinkCollection whose elements are to be added to the new AtomLinkCollection.
		/// </param>
		public AtomLinkCollection(AtomLinkCollection participants)
		{
			this.AddRange(participants);
		}

		/// <summary>
		/// Adds the elements of an array to the end of this AtomLinkCollection.
		/// </summary>
		/// <param name="participants">
		/// The array whose elements are to be added to the end of this AtomLinkCollection.
		/// </param>
		public virtual void AddRange(AtomLink[] participants)
		{
			foreach (AtomLink participant in participants)
			{
				this.List.Add(participant);
			}
		}

		/// <summary>
		/// Adds the elements of another AtomLinkCollection to the end of this AtomLinkCollection.
		/// </summary>
		/// <param name="participants">
		/// The AtomLinkCollection whose elements are to be added to the end of this AtomLinkCollection.
		/// </param>
		public virtual void AddRange(AtomLinkCollection participants)
		{
			foreach (AtomLink participant in participants)
			{
				this.List.Add(participant);
			}
		}

		/// <summary>
		/// Adds an instance of type AtomLink to the end of this AtomLinkCollection.
		/// </summary>
		/// <param name="value">
		/// The AtomLink to be added to the end of this AtomLinkCollection.
		/// </param>
		public virtual void Add(AtomLink value)
		{
			this.List.Add(value);
		}

		/// <summary>
		/// Determines whether a specfic AtomLink value is in this AtomLinkCollection.
		/// </summary>
		/// <param name="value">
		/// The AtomLink value to locate in this AtomLinkCollection.
		/// </param>
		/// <returns>
		/// true if value is found in this AtomLinkCollection;
		/// false otherwise.
		/// </returns>
		public virtual bool Contains(AtomLink value)
		{
			return this.List.Contains(value);
		}

		/// <summary>
		/// Return the zero-based index of the first occurrence of a specific value
		/// in this AtomLinkCollection
		/// </summary>
		/// <param name="value">
		/// The AtomLink value to locate in the AtomLinkCollection.
		/// </param>
		/// <returns>
		/// The zero-based index of the first occurrence of the _ELEMENT value if found;
		/// -1 otherwise.
		/// </returns>
		public virtual int IndexOf(AtomLink value)
		{
			return this.List.IndexOf(value);
		}

		/// <summary>
		/// Inserts an element into the AtomLinkCollection at the specified index
		/// </summary>
		/// <param name="index">
		/// The index at which the AtomLink is to be inserted.
		/// </param>
		/// <param name="value">
		/// The AtomLink to insert.
		/// </param>
		public virtual void Insert(int index, AtomLink value)
		{
			this.List.Insert(index, value);
		}

		/// <summary>
		/// Gets or sets the AtomLink at the given index in this AtomLinkCollection.
		/// </summary>
		public virtual AtomLink this[int index]
		{
			get
			{
				return (AtomLink) this.List[index];
			}
			set
			{
				this.List[index] = value;
			}
		}

		/// <summary>
		/// Removes the first occurrence of a specific AtomLink from this AtomLinkCollection.
		/// </summary>
		/// <param name="value">
		/// The AtomLink value to remove from this AtomLinkCollection.
		/// </param>
		public virtual void Remove(AtomLink value)
		{
			this.List.Remove(value);
		}

		/// <summary>
		/// Type-specific enumeration class, used by AtomLinkCollection.GetEnumerator.
		/// </summary>
		public class Enumerator: System.Collections.IEnumerator
		{
			private System.Collections.IEnumerator wrapped;

			public Enumerator(AtomLinkCollection collection)
			{
				this.wrapped = ((System.Collections.CollectionBase)collection).GetEnumerator();
			}

			public AtomLink Current
			{
				get
				{
					return (AtomLink) (this.wrapped.Current);
				}
			}

			object System.Collections.IEnumerator.Current
			{
				get
				{
					return (AtomLink) (this.wrapped.Current);
				}
			}

			public bool MoveNext()
			{
				return this.wrapped.MoveNext();
			}

			public void Reset()
			{
				this.wrapped.Reset();
			}
		}

		/// <summary>
		/// Returns an enumerator that can iterate through the elements of this AtomLinkCollection.
		/// </summary>
		/// <returns>
		/// An object that implements System.Collections.IEnumerator.
		/// </returns>
		public new virtual AtomLinkCollection.Enumerator GetEnumerator()
		{
			return new AtomLinkCollection.Enumerator(this);
		}
	}


}
