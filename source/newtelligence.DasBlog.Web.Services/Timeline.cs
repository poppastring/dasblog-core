using System;
using System.Xml.Serialization;
using System.Collections.ObjectModel;
using System.Collections.Generic;


/// <remarks/>
[System.Xml.Serialization.XmlRootAttribute("data")]
public class timeline 
{
    
	public timeline(){}

    /// <remarks/>
    [System.Xml.Serialization.XmlElementAttribute("event")]
    public eventCollection events;
}

/// <remarks/>
public class @event 
{
	public @event(){}

	public @event(DateTime start, DateTime end, bool isDuration, string title, string link)
	{
		this.start = start;
		this.startStringSpecified = true;
		this.end = end;
		this.isDuration = isDuration;
		this.title = title;
		this.link = link;
	}

	public @event(DateTime start, bool isDuration, string title, string link)
	{
		this.start = start;
		this.startStringSpecified = true;
		this.isDuration = isDuration;
		this.title = title;
		this.link = link;
	}

	const string DATEFORMAT = @"MMM dd yyyy HH\:mm\:ss \G\M\T";

	[System.Xml.Serialization.XmlAttributeAttribute("start")]
	public string startString
	{
		get
		{
			return start.ToString(DATEFORMAT,System.Globalization.CultureInfo.InvariantCulture);
		}
		set
		{
			start = DateTime.ParseExact(value,DATEFORMAT,System.Globalization.CultureInfo.InvariantCulture);
			startStringSpecified = true;
		}
	}

	[System.Xml.Serialization.XmlAttributeAttribute("end")]
	public string endString
	{
		get
		{
			return end.ToString(DATEFORMAT,System.Globalization.CultureInfo.InvariantCulture);
		}
		set
		{
			endStringSpecified = true;
			end = DateTime.ParseExact(value,DATEFORMAT,System.Globalization.CultureInfo.InvariantCulture);
		}
	}

	[XmlIgnore] 
	public DateTime start;

	[XmlIgnore] 
	public DateTime end;

	[System.Xml.Serialization.XmlAttributeAttribute("isDuration")]
	public bool isDuration = false;

	[XmlIgnore] 
	public bool startStringSpecified = false;

	[XmlIgnore] 
	public bool endStringSpecified = false;

	[System.Xml.Serialization.XmlAttributeAttribute("title")]
	public string title = "";

	[System.Xml.Serialization.XmlAttributeAttribute("link")]
	public string link = "";

	[System.Xml.Serialization.XmlAttributeAttribute("image")]
	public string image = "";

	[System.Xml.Serialization.XmlText]
	public string text = "";

}

    /// <summary>
    /// A collection of elements of type @event
    /// </summary>
public class @eventCollection: Collection<@event>
{
	/// <summary>
	/// Initializes a new empty instance of the @eventCollection class.
	/// </summary>
	public @eventCollection()
        :base()
	{
		// empty
	}

	/// <summary>
	/// Initializes a new instance of the @eventCollection class, containing elements
	/// copied from a list.
	/// </summary>
	/// <param name="items">
	/// The IList{T} whose elements are to be added to the new @eventCollection.
	/// </param>
	public @eventCollection(IList<@event> list)
        :base(list)
	{
		// empty
	}


}

