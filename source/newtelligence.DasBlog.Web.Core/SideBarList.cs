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


namespace newtelligence.DasBlog.Web.Core
{
    using System;
    using System.ComponentModel;
    using System.IO;
    using System.Web.UI;
    using System.Web.UI.HtmlControls;
    using System.Web.UI.WebControls;
    using System.Xml.Serialization;
    using newtelligence.DasBlog.Runtime;

    /// <summary>
    ///		Summary description for SideBarList.
    /// </summary>
    [DefaultProperty("FileName"), 
       ToolboxData("<{0}:SideBarList runat=server></{0}:SideBarList>")]
    public class SideBarList : System.Web.UI.WebControls.WebControl
    {
        string fileName = "navigatorLinks.xml";
        bool renderAsList = false;

        public SideBarList() { } // default constructor (incase I missed anything - KenH)

        public SideBarList(bool asList)
        {
            renderAsList = asList;
        }

        public string FileName
        {
            get
            {
                return fileName;
            }
            set
            {
                fileName = value;
            }
        }


        protected override void Render(HtmlTextWriter writer)
        {
            HtmlGenericControl section = new HtmlGenericControl("div");
            section.Attributes["class"] = "navigatorLinksContainerStyle";
            this.Controls.Add(section);

            string controlType = "table";
            string controlClass = "navigatorLinksTableStyle";
            string listItemClass = "navigatorLinksCellStyle";

            if (renderAsList)
            {
                controlType = "ul";
                controlClass = "navigatorLinksListStyle";
                listItemClass = "navigatorLinksListItemStyle";
            }

            HtmlGenericControl list = new HtmlGenericControl(controlType);
            list.Attributes.Add("class", controlClass);
            section.Controls.Add(list);

            //FIX: Why do we support 2 different schema's for the SideBarList content???

            try
            {
                string fullPath = Path.Combine(SiteConfig.GetContentPathFromCurrentContext(), fileName);
                if (File.Exists(fullPath))
                {
                    NavigatorXml nav;
                    using (Stream s = File.OpenRead(fullPath))
                    {
                        XmlSerializer ser = new XmlSerializer(typeof(NavigatorXml));
                        nav = (NavigatorXml)ser.Deserialize(s);
                    }
                    foreach (NavigatorItem navitem in nav.Items)
                    {
                        HyperLink catLink = new HyperLink();
                        catLink.CssClass = "navigatorLinksLinkStyle";
                        catLink.Text = navitem.Name;
                        catLink.NavigateUrl = navitem.Url;

                        if (renderAsList)
                        {
                            HtmlGenericControl cell = new HtmlGenericControl("li");
                            cell.Attributes.Add("class", listItemClass);
                            list.Controls.Add(cell);
                            cell.Controls.Add(catLink);
                        }
                        else
                        {
                            TableRow row = new TableRow();
                            TableCell cell = new TableCell();
                            cell.CssClass = listItemClass;
                            list.Controls.Add(row);
                            row.Cells.Add(cell);
                            cell.Controls.Add(catLink);
                        }
                    }
                }
                else
                {
                    NavigationRoot nav;

                    fullPath = Path.Combine(SiteConfig.GetConfigPathFromCurrentContext(), fileName);
                    if (File.Exists(fullPath))
                    {
                        using (Stream s = File.OpenRead(fullPath))
                        {
                            XmlSerializer ser = new XmlSerializer(typeof(NavigationRoot));
                            nav = (NavigationRoot)ser.Deserialize(s);
                        }
                        foreach (NavigationLink navitem in nav.Items)
                        {
                            HyperLink catLink = new HyperLink();
                            catLink.CssClass = "navigatorLinksLinkStyle";
                            catLink.Text = navitem.Name;
                            catLink.NavigateUrl = navitem.Url;

                            if (renderAsList)
                            {
                                HtmlGenericControl cell = new HtmlGenericControl("li");
                                cell.Attributes.Add("class", listItemClass);
                                list.Controls.Add(cell);
                                cell.Controls.Add(catLink);
                            }
                            else
                            {
                                TableRow row = new TableRow();
                                TableCell cell = new TableCell();
                                cell.CssClass = listItemClass;
                                list.Controls.Add(row);
                                row.Cells.Add(cell);
                                cell.Controls.Add(catLink);
                            }
                        }
                    }
                    else
                    {
                        section.Controls.Add(new LiteralControl("Add '" + fileName + "' to your SiteConfig directory<br />"));
                    }
                }
            }
            catch (Exception exc)
            {
                ErrorTrace.Trace(System.Diagnostics.TraceLevel.Error, exc);
                section.Controls.Add(new LiteralControl("There was an error processing '" + fileName + "'<br />"));
            }

            section.RenderControl( writer );
        }
    }

    [XmlRoot("navigator")]
    public class NavigatorXml
    {
        NavigatorItemCollection items = new NavigatorItemCollection();

        [XmlElement("item")]
        public NavigatorItemCollection Items { get { return items; } set { items = value; } }
    }

    public class NavigatorItem
    {
        string name;
        string url;

        [XmlAttribute("name")]
        public string Name { get { return name; } set { name = value; } }
        [XmlAttribute("pagename")]
        public string Url { get { return url; } set { url = value; } } 
    }
    #region Collection
    /// <summary>
    /// A collection of elements of type NavigatorItem
    /// </summary>
    public class NavigatorItemCollection: System.Collections.CollectionBase
    {
        /// <summary>
        /// Initializes a new empty instance of the NavigatorItemCollection class.
        /// </summary>
        public NavigatorItemCollection()
        {
            // empty
        }

        /// <summary>
        /// Initializes a new instance of the NavigatorItemCollection class, containing elements
        /// copied from an array.
        /// </summary>
        /// <param name="items">
        /// The array whose elements are to be added to the new NavigatorItemCollection.
        /// </param>
        public NavigatorItemCollection(NavigatorItem[] items)
        {
            this.AddRange(items);
        }

        /// <summary>
        /// Initializes a new instance of the NavigatorItemCollection class, containing elements
        /// copied from another instance of NavigatorItemCollection
        /// </summary>
        /// <param name="items">
        /// The NavigatorItemCollection whose elements are to be added to the new NavigatorItemCollection.
        /// </param>
        public NavigatorItemCollection(NavigatorItemCollection items)
        {
            this.AddRange(items);
        }

        /// <summary>
        /// Adds the elements of an array to the end of this NavigatorItemCollection.
        /// </summary>
        /// <param name="items">
        /// The array whose elements are to be added to the end of this NavigatorItemCollection.
        /// </param>
        public virtual void AddRange(NavigatorItem[] items)
        {
            foreach (NavigatorItem item in items)
            {
                this.List.Add(item);
            }
        }

        /// <summary>
        /// Adds the elements of another NavigatorItemCollection to the end of this NavigatorItemCollection.
        /// </summary>
        /// <param name="items">
        /// The NavigatorItemCollection whose elements are to be added to the end of this NavigatorItemCollection.
        /// </param>
        public virtual void AddRange(NavigatorItemCollection items)
        {
            foreach (NavigatorItem item in items)
            {
                this.List.Add(item);
            }
        }

        /// <summary>
        /// Adds an instance of type NavigatorItem to the end of this NavigatorItemCollection.
        /// </summary>
        /// <param name="value">
        /// The NavigatorItem to be added to the end of this NavigatorItemCollection.
        /// </param>
        public virtual void Add(NavigatorItem value)
        {
            this.List.Add(value);
        }

        /// <summary>
        /// Determines whether a specfic NavigatorItem value is in this NavigatorItemCollection.
        /// </summary>
        /// <param name="value">
        /// The NavigatorItem value to locate in this NavigatorItemCollection.
        /// </param>
        /// <returns>
        /// true if value is found in this NavigatorItemCollection;
        /// false otherwise.
        /// </returns>
        public virtual bool Contains(NavigatorItem value)
        {
            return this.List.Contains(value);
        }

        /// <summary>
        /// Return the zero-based index of the first occurrence of a specific value
        /// in this NavigatorItemCollection
        /// </summary>
        /// <param name="value">
        /// The NavigatorItem value to locate in the NavigatorItemCollection.
        /// </param>
        /// <returns>
        /// The zero-based index of the first occurrence of the _ELEMENT value if found;
        /// -1 otherwise.
        /// </returns>
        public virtual int IndexOf(NavigatorItem value)
        {
            return this.List.IndexOf(value);
        }

        /// <summary>
        /// Inserts an element into the NavigatorItemCollection at the specified index
        /// </summary>
        /// <param name="index">
        /// The index at which the NavigatorItem is to be inserted.
        /// </param>
        /// <param name="value">
        /// The NavigatorItem to insert.
        /// </param>
        public virtual void Insert(int index, NavigatorItem value)
        {
            this.List.Insert(index, value);
        }

        /// <summary>
        /// Gets or sets the NavigatorItem at the given index in this NavigatorItemCollection.
        /// </summary>
        public virtual NavigatorItem this[int index]
        {
            get
            {
                return (NavigatorItem) this.List[index];
            }
            set
            {
                this.List[index] = value;
            }
        }

        /// <summary>
        /// Removes the first occurrence of a specific NavigatorItem from this NavigatorItemCollection.
        /// </summary>
        /// <param name="value">
        /// The NavigatorItem value to remove from this NavigatorItemCollection.
        /// </param>
        public virtual void Remove(NavigatorItem value)
        {
            this.List.Remove(value);
        }

        /// <summary>
        /// Type-specific enumeration class, used by NavigatorItemCollection.GetEnumerator.
        /// </summary>
        public class Enumerator: System.Collections.IEnumerator
        {
            private System.Collections.IEnumerator wrapped;

            public Enumerator(NavigatorItemCollection collection)
            {
                this.wrapped = ((System.Collections.CollectionBase)collection).GetEnumerator();
            }

            public NavigatorItem Current
            {
                get
                {
                    return (NavigatorItem) (this.wrapped.Current);
                }
            }

            object System.Collections.IEnumerator.Current
            {
                get
                {
                    return (NavigatorItem) (this.wrapped.Current);
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
        /// Returns an enumerator that can iterate through the elements of this NavigatorItemCollection.
        /// </summary>
        /// <returns>
        /// An object that implements System.Collections.IEnumerator.
        /// </returns>        
        public new virtual NavigatorItemCollection.Enumerator GetEnumerator()
        {
            return new NavigatorItemCollection.Enumerator(this);
        }
    }
    #endregion
}
