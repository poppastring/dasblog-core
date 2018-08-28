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
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using newtelligence.DasBlog.Runtime;
using newtelligence.DasBlog.Util;
using NodaTime;

namespace newtelligence.DasBlog.Web.Core
{
    public interface IPageFormatInfo
    {
        Control Bodytext { get; }
    }

    // register this as "newtelligence.DasBlog.Macros"
    public class MacroSectionHandler : NameValueSectionHandler
    {
        public static string ConfigSectionName
        {
            get { return "newtelligence.DasBlog.Macros"; }
        }

        public MacroSectionHandler()
        {
        }

        protected override string KeyAttributeName
        {
            get { return "macro"; }
        }

        protected override string ValueAttributeName
        {
            get { return "type"; }
        }
    }

    public static class MacrosFactory
    {
        public static RadioProperties CreateRadioPropertiesInstance(SharedBasePage page)
        {
            string typeName = typeof(RadioProperties).AssemblyQualifiedName;
            NameValueCollection macroMap = ConfigurationManager.GetSection(MacroSectionHandler.ConfigSectionName) as NameValueCollection;
            if (macroMap != null && macroMap["radioprops"] != null)
            {
                typeName = macroMap["radioprops"];
            }
            return (RadioProperties)Activator.CreateInstance(Type.GetType(typeName), new object[] { page });
        }

        public static EditRadioProperties CreateEditRadioPropertiesInstance(SharedBasePage page)
        {
            string typeName = typeof(EditRadioProperties).AssemblyQualifiedName;
            NameValueCollection macroMap = ConfigurationManager.GetSection(MacroSectionHandler.ConfigSectionName) as NameValueCollection;
            if (macroMap != null && macroMap["editradioprops"] != null)
            {
                typeName = macroMap["editradioprops"];
            }
            return (EditRadioProperties)Activator.CreateInstance(Type.GetType(typeName), new object[] { page });
        }

        public static NewtelligenceProperties CreateNewtelligencePropertiesInstance(SharedBasePage page)
        {
            string typeName = typeof(NewtelligenceProperties).AssemblyQualifiedName;
            NameValueCollection macroMap = ConfigurationManager.GetSection(MacroSectionHandler.ConfigSectionName) as NameValueCollection;
            if (macroMap != null && macroMap["newtelligenceprops"] != null)
            {
                typeName = macroMap["newtelligenceprops"];
            }
            return (NewtelligenceProperties)Activator.CreateInstance(Type.GetType(typeName), new object[] { page });
        }

        public static RadioMacros CreateRadioMacrosInstance(SharedBasePage page)
        {
            string typeName = typeof(RadioMacros).AssemblyQualifiedName;
            NameValueCollection macroMap = ConfigurationManager.GetSection(MacroSectionHandler.ConfigSectionName) as NameValueCollection;
            if (macroMap != null && macroMap["radiomacros"] != null)
            {
                typeName = macroMap["radiomacros"];
            }
            return (RadioMacros)Activator.CreateInstance(Type.GetType(typeName), new object[] { page });
        }

        public static EditRadioMacros CreateEditRadioMacrosInstance(SharedBasePage page)
        {
            string typeName = typeof(EditRadioMacros).AssemblyQualifiedName;
            NameValueCollection macroMap = ConfigurationManager.GetSection(MacroSectionHandler.ConfigSectionName) as NameValueCollection;
            if (macroMap != null && macroMap["editradiomacros"] != null)
            {
                typeName = macroMap["editradiomacros"];
            }
            return (EditRadioMacros)Activator.CreateInstance(Type.GetType(typeName), new object[] { page });
        }

        public static EditMacros CreateEditMacrosInstance(SharedBasePage page)
        {
            string typeName = typeof(EditMacros).AssemblyQualifiedName;
            NameValueCollection macroMap = ConfigurationManager.GetSection(MacroSectionHandler.ConfigSectionName) as NameValueCollection;
            if (macroMap != null && macroMap["Editmacros"] != null)
            {
                typeName = macroMap["Editmacros"];
            }
            return (EditMacros)Activator.CreateInstance(Type.GetType(typeName), new object[] { page });
        }

        public static WeblogMacros CreateWeblogMacrosInstance(SharedBasePage page)
        {
            string typeName = typeof(WeblogMacros).AssemblyQualifiedName;
            NameValueCollection macroMap = ConfigurationManager.GetSection(MacroSectionHandler.ConfigSectionName) as NameValueCollection;
            if (macroMap != null && macroMap["weblogmacros"] != null)
            {
                typeName = macroMap["weblogmacros"];
            }
            return (WeblogMacros)Activator.CreateInstance(Type.GetType(typeName), new object[] { page });
        }

        public static Macros CreateMacrosInstance(SharedBasePage page)
        {
            string typeName = typeof(Macros).AssemblyQualifiedName;
            NameValueCollection macroMap = ConfigurationManager.GetSection(MacroSectionHandler.ConfigSectionName) as NameValueCollection;
            if (macroMap != null && macroMap["macros"] != null)
            {
                typeName = macroMap["macros"];
            }
            return (Macros)Activator.CreateInstance(Type.GetType(typeName), new object[] { page });
        }

        public static DayMacros CreateDayMacrosInstance(SharedBasePage page)
        {
            string typeName = typeof(DayMacros).AssemblyQualifiedName;
            NameValueCollection macroMap = ConfigurationManager.GetSection(MacroSectionHandler.ConfigSectionName) as NameValueCollection;
            if (macroMap != null && macroMap["daymacros"] != null)
            {
                typeName = macroMap["daymacros"];
            }
            return (DayMacros)Activator.CreateInstance(Type.GetType(typeName), new object[] { page });
        }

        public static object CreateCustomMacrosInstance(SharedBasePage page, Entry item, string name)
        {
            NameValueCollection macroMap = ConfigurationManager.GetSection(MacroSectionHandler.ConfigSectionName) as NameValueCollection;
            if (macroMap != null && macroMap[name] != null)
            {
                string typeName = macroMap[name];
                return Activator.CreateInstance(Type.GetType(typeName), new object[] { page, item });
            }
            return null;
        }

    }

    /// <summary>
    /// This class provides a compatible implementation of the 
    /// macros available in Userland's Radio application.
    /// </summary>
    public class RadioMacros
    {
        protected SharedBasePage requestPage;

        /// <summary>
        /// RadioMacros' constructor
        /// </summary>
        /// <param name="page">The page we are rendering.</param>
        public RadioMacros(SharedBasePage page)
        {
            requestPage = page;
        }

        /// <summary>
        /// Returns an absolute URL to an image based on a partial URL 
        /// contained in a design template.
        /// </summary>
        /// <param name="img">Relative URL to image.</param>
        /// <returns></returns>
        public virtual Control ImageUrl(string img)
        {
            return new LiteralControl(new Uri(new Uri(SiteUtilities.GetBaseUrl(requestPage.SiteConfig)), requestPage.BlogTheme.ImageDirectory).ToString() + "/" + img);
        }

        /// <summary>
        /// Returns an absolute URL to an image based on a partial URL 
        /// contained in a design template.
        /// </summary>
        /// <param name="img">Relative URL to image.</param>
        /// <returns></returns>
        public virtual Control ImageUrl(string img, params string[] args)
        {
            return new LiteralControl(new Uri(new Uri(SiteUtilities.GetBaseUrl(requestPage.SiteConfig)), requestPage.BlogTheme.ImageDirectory).ToString() + "/" + img);
        }

        /// <summary>
        /// Returns an absolute URL to an image based on a partial URL 
        /// relative to the Radio system path. 
        /// </summary>
        /// <param name="img">Relative URL to image.</param>
        /// <returns></returns>
        /// <remarks>
        /// Renders one &lt;img&gt; tag with CSS class "simpleImageStyle"
        /// </remarks>
        public virtual Control ImageRef(string img, params string[] args)
        {
            Image imgCtl = new Image();
            imgCtl.CssClass = "simpleImageStyle";
            imgCtl.ImageUrl = new Uri(new Uri(SiteUtilities.GetBaseUrl(requestPage.SiteConfig)), requestPage.BlogTheme.ImageDirectory).ToString() + "/" + img;
            return imgCtl;
        }

        /// <summary>
        /// This macro renders a set of &lt;link&gt; and &lt;meta&gt; tags
        /// for including in the &lt;head&gt; section of the page. 
        /// </summary>
        /// <returns></returns>
        [Obsolete]
        public virtual Control HeadLinks()
        {
            PlaceHolder placeHolder = new PlaceHolder();
            return placeHolder;
        }

        /// <summary>
        /// This macro renders an OPML outline. The outline is displayed as an
        /// hierarchical list. Each item with subitems is displayed with an icon
        /// on the left hand side. The default image is "images/outlinearrow.gif" for
        /// the collapsed state and "images/outlinedown.gif" for the expanded state.
        /// Both file names are relative to the site root. Each item without subitems
        /// is rendered with an RSS icon on the left hand side. The image used is
        /// "images/xmlButton.gif". 
        /// </summary>
        /// <param name="url">Absolute URL to the outline or relative local path</param>
        /// <returns></returns>
        /// <remarks>
        /// Renders a complex control with the following CSS styles:<br/>
        /// <table>
        ///    <tr><td>Outermost container (&lt;div&gt;)<td></td>blogRollContainerStyle</td></tr>
        ///    <tr><td>Contained table (&lt;table&gt;)<td></td>blogRollTableStyle</td></tr>
        ///    <tr><td>Table cells (&lt;td&gt;)<td></td>blogRollCellStyle</td></tr>
        ///    <tr><td>RSS Hyperlink (&lt;a&gt;)<td></td>blogRollXmlLinkStyle</td></tr>
        ///    <tr><td>Text Hyperlink (&lt;div&gt;)<td></td>blogRollLinkStyle</td></tr>
        ///    <tr><td>Nested outline header table (&lt;table&gt;)<td></td>blogRollNestedOutlineHeaderTableStyle</td></tr>
        ///    <tr><td>Collapsed nested outline (&lt;div&gt;)<td></td>blogRollCollapsed (this must be changed in or removed from "scripts/outline.css" to override the default)</td></tr>
        ///    <tr><td>Expanded nested outline (&lt;div&gt;)<td></td>blogRollExpanded (this must be changed in or removed from "scripts/outline.css" to override the default)</td></tr>
        ///    <tr><td>Nested outline header image cell (&lt;td&gt;)<td></td>blogRollNestedOutlineBadgeCellStyle</td></tr>
        ///    <tr><td>Nested outline header text cell (&lt;td&gt;)<td></td>blogRollNestedOutlineTitleCellStyle</td></tr>
        ///    <tr><td>Nested outline header image link (&lt;a&gt;)<td></td>blogRollNestedOutlineBadgeStyle</td></tr>
        ///    <tr><td>Nested outline header image link (&lt;a&gt;)<td></td>blogRollNestedOutlineTitleStyle</td></tr>
        ///    <tr><td>Nested outline header image cell (&lt;td&gt;)<td></td>blogRollNestedOutlineBadgeStyle</td></tr>
        ///    <tr><td>Nested outline body table (&lt;table&gt;)<td></td>blogRollNestedOutlineBodyTableStyle (sub-elements repeat with above styles)</td></tr>
        /// </table>
        /// </remarks>
        public virtual Control BlogRoll(string url)
        {
            SideBarOpmlList list = new SideBarOpmlList();
            list.FileName = url;
            list.RenderDescription = requestPage.SiteConfig.EnableBlogrollDescription;
            return list;
        }

        /// <summary>
        /// The URL of this weblog.
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        /// <remarks>Renders plain text</remarks>
        public virtual Control WeblogUrl(params string[] args)
        {
            return new LiteralControl(SiteUtilities.GetStartPageUrl(requestPage.SiteConfig));
        }

        /// <summary>
        /// Ignored in this version.
        /// </summary>
        /// <returns>Renders nothing</returns>
        public virtual Control EditThisPageButton()
        {
            return new LiteralControl("");
        }

        /// <summary>
        /// Renders the administrator bar, if the user is logged and is member
        /// of the "admin" role.
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public virtual Control EditorsOnlyMenu(params string[] args)
        {
            // Load the admin bar if we have a valid contributor - the control itself
            // will handle what is visible for the type of role.
            if ((!requestPage.HideAdminTools) && SiteSecurity.IsValidContributor())
            {
                return this.requestPage.LoadControl("AdminNavBar.ascx");
            }
            else
            {
                return new LiteralControl("");
            }
        }

        /// <summary>
        /// Generates a text link to the (equivalent of) the Radio coffee mug.
        /// </summary>
        /// <returns></returns>
        /// <remarks>
        /// Renders one &lt;a&gt; tag with the CSS class "xmlCoffeeMugStyle"
        /// </remarks>
        public virtual Control XmlCoffeeMug()
        {
            HyperLink xmlLink = new HyperLink();
            xmlLink.CssClass = "xmlCoffeeMugStyle";
            xmlLink.NavigateUrl = "http://127.0.0.1:5335/system/pages/subscriptions?url=" + SiteUtilities.GetRssUrl();
            xmlLink.Target = "";
            Image img = new Image();
            img.CssClass = "xmlCoffeeMugImageStyle";
            img.ImageUrl = requestPage.GetThemedImageUrl("xmlCoffeeMug");
            img.ToolTip = img.AlternateText = requestPage.CoreStringTables.GetString("text_subscribe");
            xmlLink.Controls.Add(img);
            return xmlLink;
        }

        /// <summary>
        /// Generates a link to the author's email address or some page
        /// providing feedback capability. If SiteConfig.ObfuscateEmail is enabled this
        /// address will be Obfuscated.
        /// </summary>
        /// <returns></returns>
        /// <remarks>Renders one &lt;a&gt; tag with the CSS class "mailToStyle" and 
        /// an embedded &lt;img&gt; tage with the CSS class "mailToImageStyle"</remarks>
        public virtual Control MailTo()
        {
            HyperLink xmlLink = new HyperLink();
            xmlLink.CssClass = "mailToStyle";

            string messageSubject = (requestPage.TitleOverride != null && requestPage.TitleOverride.Length > 0) ? requestPage.SiteConfig.Title + " - " + requestPage.TitleOverride : requestPage.SiteConfig.Title;
            messageSubject = messageSubject.Replace("'", @"\'");

            string emailAddress = requestPage.SiteConfig.Contact;

            if (requestPage.SiteConfig.ObfuscateEmail)
            {
                xmlLink.NavigateUrl = SiteUtilities.GetObfuscatedEmailUrl(emailAddress, messageSubject);
            }
            else
            {
                xmlLink.NavigateUrl = String.Format("mailto:{0}?Subject={1}", requestPage.SiteConfig.Contact, messageSubject);
            }

            Image img = new Image();
            img.CssClass = "mailToImageStyle";
            img.ToolTip = img.AlternateText = requestPage.CoreStringTables.GetString("text_send_mail");
            img.ImageUrl = requestPage.GetThemedImageUrl("mailTo");
            xmlLink.Controls.Add(img);
            return xmlLink;
        }




        /// <summary>
        /// Renders the time of the last site update, expressed in GMT
        /// </summary>
        /// <returns></returns>
        /// <remarks>Renders plans text</remarks>
        public virtual Control GetLastUpdate()
        {
            if (requestPage.SiteConfig.AdjustDisplayTimeZone)
            {
                return new LiteralControl(requestPage.SiteConfig.GetConfiguredTimeZone().FormatAdjustedUniversalTime(requestPage.DataService.GetLastEntryUpdate()));
            }
            else
            {
                return new LiteralControl(requestPage.DataService.GetLastEntryUpdate().ToString("U") + " UTC");
            }
        }

        /// <summary>
        /// Unused, provided for backwards template-compatibility with Radio Userland only
        /// </summary>
        /// <returns></returns>
        public virtual Control StaticSiteStatsImage()
        {
            return new LiteralControl("");
        }

        /// <summary>
        /// Renders a search hyperlink, pointing to Google.com. The search expression
        /// is the title as passed the macro.
        /// </summary>
        /// <param name="title"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public virtual Control GoogleIt(string title, params string[] args)
        {
            HyperLink xmlLink = new HyperLink();
            xmlLink.NavigateUrl = "http://www.google.com/search?q=" + HttpUtility.UrlEncode(title);
            if (args != null && args.Length == 1)
            {
                xmlLink.Text = args[0];
            }
            else
            {
                xmlLink.Text = "GoogleIt";
            }
            return xmlLink;
        }
    }

    public class EditRadioMacros : RadioMacros
    {
        public EditRadioMacros(SharedBasePage page)
            : base(page)
        {
        }

        /// <summary>
        /// Creates the EditEntryBox control.
        /// </summary>
        /// <returns></returns>
        public virtual Control WeblogEditBox()
        {
            PlaceHolder bodyPlaceHolder = new PlaceHolder();
            bodyPlaceHolder.Controls.Add(requestPage.LoadControl("EditEntryBox.ascx"));
            return bodyPlaceHolder;
        }

        /// <summary>
        /// Creates an empty placeholder.
        /// </summary>
        /// <returns></returns>
        public virtual Control WeblogRecentPosts()
        {
            PlaceHolder bodyPlaceHolder = new PlaceHolder();
            return bodyPlaceHolder;
        }

        /// <summary>
        /// Renders plain text: 'cloud links'
        /// </summary>
        /// <returns></returns>
        public virtual Control CloudLinks()
        {
            return new LiteralControl("");
        }

        /// <summary>
        /// Renders plain text: 'status center'
        /// </summary>
        /// <returns></returns>
        public virtual Control StatusCenter()
        {
            return new LiteralControl("");
        }

        /// <summary>
        /// Renders plain text: 'support center'
        /// </summary>
        /// <returns></returns>
        public virtual Control SupportCenter()
        {
            return new LiteralControl("");
        }
    }

    public class WeblogMacros
    {
        protected SharedBasePage requestPage;

        public WeblogMacros(SharedBasePage page)
        {
            requestPage = page;
        }

        public virtual Control getUrl()
        {
            return new LiteralControl(SiteUtilities.GetStartPageUrl());
        }

        /// <summary>
        /// Draws the calendar.
        /// </summary>
        /// <returns></returns>
        public virtual Control DrawCalendar()
        {
            WeblogCalendar calendar = new WeblogCalendar();
            return calendar;
        }
    }

    public class RadioProperties
    {
        protected RadioMacros radioMacros;
        protected SharedBasePage requestPage;
        protected WeblogMacros weblogMacros;

        public RadioProperties(SharedBasePage page)
        {
            requestPage = page;
            radioMacros = InitRadioMacros();
            weblogMacros = InitWeblogMacros();
        }

        public virtual RadioMacros InitRadioMacros()
        {
            return MacrosFactory.CreateRadioMacrosInstance(requestPage);
        }

        public virtual WeblogMacros InitWeblogMacros()
        {
            return MacrosFactory.CreateWeblogMacrosInstance(requestPage);
        }

        public RadioMacros macros
        {
            get { return radioMacros; }
        }

        public WeblogMacros weblog
        {
            get { return weblogMacros; }
        }
    }

    public class EditRadioProperties : RadioProperties
    {
        public EditRadioProperties(SharedBasePage page)
            : base(page)
        {
        }

        public override RadioMacros InitRadioMacros()
        {
            return MacrosFactory.CreateEditRadioMacrosInstance(requestPage);
        }

        public override WeblogMacros InitWeblogMacros()
        {
            return MacrosFactory.CreateWeblogMacrosInstance(requestPage);
        }
    }

    public class NewtelligenceProperties
    {
        protected SharedBasePage requestPage;

        public NewtelligenceProperties(SharedBasePage page)
        {
            requestPage = page;
        }

        private Control InternalDrawCategories(bool feed)
        {
            CategoryList control = new CategoryList(feed);
            control.Categories = requestPage.DataService.GetCategories();
            return control;

            //			//HACK: SDH: We need to figure something out here. I'm doing caching of a dynamic control
            //			// after it's IDs have already been decided. It's happening after PreRender, so 
            //			// Render will fail if any controls in the tree have the same name...for now, 
            //			// I'm hiding this control on pages that use FindControl, as that will cause an exception...
            //			// Ultimately we need to get Partial Caching Controls to work with our Macro system.
            //			// This control is far too expensive to let happen all the time.
            //			if (HttpContext.Current.Request.Url.LocalPath.EndsWith("Login.aspx") || HttpContext.Current.Request.Url.LocalPath.IndexOf("Edit") != -1)
            //			{
            //				return new LiteralControl(String.Empty);
            //			}
            //
            //			string languageFilter = HttpContext.Current.Request.Headers["Accept-Language"];
            //			string key = "CategoryList" + feed.ToString() + languageFilter;
            //			CategoryList control = HttpContext.Current.Cache[key] as CategoryList;
            //			if (control == null)
            //			{
            //				control = new CategoryList(feed);
            //				control.Categories = requestPage.DataService.GetCategories();
            //				HttpContext.Current.Cache.Insert(key,control,null,DateTime.Now.AddMinutes(20),TimeSpan.Zero);
            //			}
            //			return control;
        }

        public virtual Control DrawCategories()
        {
            return InternalDrawCategories(true);
        }




        public virtual Control DrawCategoriesWithoutFeed()
        {
            return InternalDrawCategories(false);
        }

        public virtual Control DrawMainCategories()
        {
            return InternalDrawCategories(true);
        }

        public virtual Control DrawSubCategories()
        {
            return new LiteralControl("");
        }

        public virtual Control DrawArchiveMonths()
        {
            return new ArchiveMonthsList();

            //			//HACK: SDH: We need to figure something out here. I'm doing caching of a dynamic control
            //			// after it's IDs have already been decided. It's happening after PreRender, so 
            //			// Render will fail if any controls in the tree have the same name...for now, 
            //			// I'm hiding this control on pages that use FindControl, as that will cause an exception...
            //			// Ultimately we need to get Partial Caching Controls to work with our Macro system.
            //			// This control is far too expensive to let happen all the time.
            //			if (HttpContext.Current.Request.Url.LocalPath.EndsWith("Login.aspx") || HttpContext.Current.Request.Url.LocalPath.IndexOf("Edit") != -1)
            //			{
            //				return new LiteralControl(String.Empty);
            //			}
            //
            //			// SDH: I can't get Partial Caching Control to work with our dynamic macro-created controls. Thoughts anyone?
            //			string languageFilter = HttpContext.Current.Request.Headers["Accept-Language"];
            //			string key = "ArchiveMonths" + languageFilter;
            //			ArchiveMonthsList control = HttpContext.Current.Cache[key] as ArchiveMonthsList;
            //			if (control == null)
            //			{
            //				control = new ArchiveMonthsList();
            //				HttpContext.Current.Cache.Insert(key,control,null,DateTime.Now.AddMinutes(20),TimeSpan.Zero);
            //			}
            //			return control;
        }

        public virtual Control AdminBar()
        {
            if (!requestPage.HideAdminTools && SiteSecurity.IsValidContributor())
            {


                return this.requestPage.LoadControl("AdminNavBar.ascx");
            }
            else
            {
                return new LiteralControl("");
            }
        }

        public virtual Control Disclaimer(string disclaimerPath)
        {
            if (disclaimerPath == null)
            {
                disclaimerPath = "SiteConfig/disclaimer.format.html";
            }

            string fullpath = SiteUtilities.MapPath(disclaimerPath);
            string value = "";
            using (StreamReader reader = new StreamReader(fullpath))
            {
                value = reader.ReadToEnd();
            }
            return new LiteralControl(value);
        }

        public virtual Control LoginBox()
        {
            // OmarS: we no longer allow the LoginBox on the main page for a number of reasons
            /*
            if (!requestPage.HideAdminTools && !requestPage.Request.IsAuthenticated && !requestPage.Request.Path.EndsWith("login.aspx"))
            {
                return this.requestPage.LoadControl("SignIn.ascx");
            }
            else if (!requestPage.HideAdminTools && requestPage.Request.IsAuthenticated)
            {
                return this.requestPage.LoadControl("Logout.ascx");
            }
            else if (requestPage.HideAdminTools)
            {
                HyperLink hl = new HyperLink();
                hl.Text = requestPage.CoreStringTables.GetString("text_log_on");
                hl.NavigateUrl = "Login.aspx";
                return hl;
            }
            else
            {
                return new LiteralControl("");
            }
            */

            if (!requestPage.HideAdminTools && requestPage.Request.IsAuthenticated && !requestPage.Request.Path.ToLower().EndsWith("login.aspx"))
            {
                return this.requestPage.LoadControl("Logout.ascx");
            }
            else
            {
                HyperLink hl = new HyperLink();
                hl.Text = requestPage.CoreStringTables.GetString("text_log_on");
                hl.NavigateUrl = "Login.aspx";
                return hl;
            }
        }

        public virtual Control LogoutBox()
        {
            if (!requestPage.HideAdminTools && requestPage.Request.IsAuthenticated)
            {
                return this.requestPage.LoadControl("Logout.ascx");
            }
            else
            {
                return new LiteralControl("");
            }
        }

        public virtual Control Search()
        {
            return this.requestPage.LoadControl("Search.ascx");
        }

        /// <summary>
        /// Renders a control only if the user is logged in and is an administrator.
        /// Associated with the macro &lt;%ASPNETAdminControl(...)%&gt;
        /// </summary>
        /// <param name="controlName">String with relative path to ASCX control</param>
        public virtual Control ASPNETAdminControl(string controlName)
        {
            if (!requestPage.HideAdminTools && requestPage.Request.IsAuthenticated && SiteSecurity.IsInRole("admin"))
            {
                return this.requestPage.LoadControl(controlName);
            }
            else
            {
                return new LiteralControl("");
            }
        }

        /// <summary>
        /// Renders a control only if the user is logged in
        /// Associated with the macro &lt;%ASPNETLoggedInControl(...)%&gt;
        /// </summary>
        /// <param name="controlName">String with relative path to ASCX control</param>
        public virtual Control ASPNETLoggedInControl(string controlName)
        {
            if (!requestPage.HideAdminTools && requestPage.Request.IsAuthenticated)
            {
                return this.requestPage.LoadControl(controlName);
            }
            else
            {
                return new LiteralControl("");
            }
        }

        public virtual Control ASPNETControl(string controlName)
        {
            return this.requestPage.LoadControl(controlName);
        }
    }

    /// <summary>
    /// Macros always return controls
    /// </summary>
    public class Macros
    {
        protected RadioProperties _radio;
        protected NewtelligenceProperties _newtelligence;
        protected SharedBasePage requestPage;

        public Macros(SharedBasePage page)
        {
            requestPage = page;
            _radio = InitRadioProperties();
            _newtelligence = MacrosFactory.CreateNewtelligencePropertiesInstance(page);
        }

        public virtual RadioProperties InitRadioProperties()
        {
            return MacrosFactory.CreateRadioPropertiesInstance(requestPage);
        }

        public virtual Control StyleSheet(string styleSheetName, string media)
        {
            PlaceHolder placeHolder = new PlaceHolder();
            HtmlGenericControl linkTag = new HtmlGenericControl("link");
            linkTag.Attributes.Add("media", media);
            linkTag.Attributes.Add("rel", "stylesheet");
            linkTag.Attributes.Add("type", "text/css");
            string url = new Uri(new Uri(SiteUtilities.GetBaseUrl(requestPage.SiteConfig)), requestPage.BlogTheme.TemplateDirectory).ToString() + "/" + styleSheetName;
            linkTag.Attributes.Add("href", url);
            placeHolder.Controls.Add(linkTag);
            return placeHolder;
        }

        public virtual Control StyleSheet(string styleSheetName)
        {
            return StyleSheet(styleSheetName, "all");
        }

        public virtual Control BaseUrl()
        {
            return new LiteralControl(SiteUtilities.GetBaseUrl());
        }

        public virtual Control DrawPostPagingNext()
        {
            if (requestPage.IsAggregatedView &&
                requestPage.CategoryName.Equals("Frontpage") &&
                requestPage.TitleOverride == null && // != null if category, date, month, title or guid query strings have been passed.
                requestPage.Request.QueryString["q"] == null && // Don't display paging for SearchView.aspx.
                requestPage.WeblogEntries.Count > 0 &&
                requestPage.PageIndex > 0)
            {
                HyperLink lnkNext = new HyperLink();

                if (requestPage.PageIndex > 1)
                {
                    lnkNext.NavigateUrl = SiteUtilities.GetPagedViewUrl(requestPage.PageIndex - 1);
                }
                else if (requestPage.PageIndex == 1)
                {
                    lnkNext.NavigateUrl = SiteUtilities.GetStartPageUrl();
                }

                LiteralControl litNext = new LiteralControl(requestPage.CoreStringTables.GetString("text_next_posts"));

                HtmlGenericControl spanNextSign = new HtmlGenericControl("span");
                spanNextSign.InnerHtml = "&raquo;";

                // Build control hierarchy.
                lnkNext.Controls.Add(litNext);
                lnkNext.Controls.Add(spanNextSign);

                return lnkNext;
            }

            return new LiteralControl(String.Empty);
        }

        public virtual Control DrawPostPagingPrevious()
        {
            if (!requestPage.IsLastPage &&
                requestPage.IsAggregatedView &&
                requestPage.CategoryName.Equals("Frontpage") &&
                requestPage.TitleOverride == null && // != null if category, date, month, title or guid query strings have been passed.
                requestPage.Request.QueryString["q"] == null && // Don't display paging for SearchView.aspx.
                requestPage.WeblogEntries.Count > 0)
            {
                HyperLink lnkPrevious = new HyperLink();
                lnkPrevious.NavigateUrl = SiteUtilities.GetPagedViewUrl(requestPage.PageIndex + 1);

                LiteralControl litPrevious = new LiteralControl(requestPage.CoreStringTables.GetString("text_previous_posts"));

                HtmlGenericControl spanPreviousSign = new HtmlGenericControl("span");
                spanPreviousSign.InnerHtml = "&laquo;";

                // Build control hierarchy.
                lnkPrevious.Controls.Add(spanPreviousSign);
                lnkPrevious.Controls.Add(litPrevious);

                return lnkPrevious;
            }

            return new LiteralControl(String.Empty);
        }

        /// <summary>
        /// Weblog 'Title' as defined through the tag &lt;Title&gt;
        /// in the configuration file of the Web application.
        /// This macro should be used to render the web page title in
        /// the title html tags. 
        /// If _titleOveride is not null or empty it will be 
        /// appended to the SiteName and rendered in title of the page.
        /// Corresponds to the macro: &lt;%title%&gt;
        /// Different from <see cref="newtelligence.DasBlog.Web.Core.Macros.SiteName"/>.
        /// <seealso cref="Macros.SiteName"/>
        /// </summary>
        /// <remarks>The output is plain text.</remarks>
        public virtual Control Title
        {
            get
            {
                if (requestPage.TitleOverride != null && requestPage.TitleOverride.Length > 0)
                {
                    return new LiteralControl(requestPage.TitleOverride + " - " + requestPage.SiteConfig.Title);
                }
                else
                {
                    return new LiteralControl(requestPage.SiteConfig.Title);
                }
            }
        }

        /// <summary>
        /// Weblog 'SubTitle' as defined through the tag &lt;Subtitle&gt;
        /// in the configuration file of the Web application.
        /// Corresponds to the macro: &lt;%subtitle%&gt;
        /// </summary>
        /// <remarks>The output is plain text.</remarks>
        public virtual Control Subtitle
        {
            get { return new LiteralControl(requestPage.SiteConfig.Subtitle); }
        }

        /// <summary>
        /// Emits text 'newtelligence DasBlog' and version number.
        /// Corresponds to the macro: &lt;%radioBadge%&gt;
        /// </summary>
        /// <remarks>The output is plain text.</remarks>
        public virtual Control RadioBadge
        {
            get { return new LiteralControl("newtelligence dasBlog " + GetType().Assembly.GetName().Version); }
        }

        /// <summary>
        /// Hyperlink to the main RSS feed.
        /// Corresponds to the macro: &lt;%rssLink%&gt;
        /// </summary>
        /// <remarks>The output is HTML hyperlinked HTML image.</remarks>
        public virtual Control RssLink
        {
            get
            {
                HyperLink xmlLink = new HyperLink();
                xmlLink.CssClass = "rssLinkStyle";
                if (requestPage.SiteConfig.UseFeedSchemeForSyndication)
                {
                    xmlLink.NavigateUrl = SiteUtilities.GetFeedUrl();
                }
                else
                {
                    xmlLink.NavigateUrl = SiteUtilities.GetRssUrl();
                }
                Image img = new Image();
                img.ImageUrl = requestPage.GetThemedImageUrl("feed-icon-16x16");
                img.CssClass = "rssLinkImageStyle";
                img.ToolTip = img.AlternateText = "RSS 2.0";
                xmlLink.Controls.Add(img);
                return xmlLink;
            }
        }

        /// <summary>
        /// Hyperlink to the main RSS feed.
        /// Corresponds to the macro: &lt;%rssLinkUrl(linktext,tooltip)%&gt;
        /// </summary>
        /// <param name="text">Provide a text you want as the link text, e.g. "XML". Default will be "RSS".</param>
        /// <param name="titleText">link title text (tooltip)</param>
        /// <remarks>The output is HTML hyperlinked HTML. Default link text is "RSS".</remarks>
        public virtual Control RssLinkUrl(string text, string titleText)
        {
            HyperLink xmlLink = new HyperLink();
            xmlLink.ID = "rssLinkUrl";
            xmlLink.CssClass = "rssLinkStyle";
            xmlLink.NavigateUrl = SiteUtilities.GetRssUrl();
            xmlLink.Text = (text == null || text.Length == 0 ? "RSS" : text);
            xmlLink.ToolTip = (titleText == null || titleText.Length == 0 ? "RSS 2.0" : titleText);
            return xmlLink;
        }

        /// <summary>
        /// Provides just the RSS feed url that can be used in HTML code from inside the template.
        /// </summary>
        /// <returns>Outputs a literal control with the URL for the RSS feed.</returns>
        public virtual Control RssUrl()
        {
            LiteralControl rssUrl = new LiteralControl();
            rssUrl.Text = SiteUtilities.GetRssUrl();
            return rssUrl;
        }

        /// <summary>
        /// Hyperlink to the main RSS feed.
        /// Corresponds to the macro: &lt;%rssLinkUrl()%&gt;
        /// </summary>
        /// <remarks>The output is HTML hyperlinked HTML. Default link text is "RSS".</remarks>
        public virtual Control RssLinkUrl()
        {
            return this.RssLinkUrl(null, null);
        }

        /// <summary>
        /// Hyperlink to the main RSS feed.
        /// Corresponds to the macro: &lt;%rssLinkUrl(linktext)%&gt;
        /// </summary>
        /// <param name="text">Provide a text you want as the link text, e.g. "XML". Default will be "RSS".</param>
        /// <remarks>The output is HTML hyperlinked HTML. Default link text is "RSS".</remarks>
        public virtual Control RssLinkUrl(string text)
        {
            return this.RssLinkUrl(text, null);
        }

        /// <summary>
        /// Hyperlink to the main RSS feed, but with protocol scheme "feed".
        /// Corresponds to the macro: &lt;%feedLink%&gt;
        /// </summary>
        /// <remarks>The output is HTML hyperlinked HTML image.</remarks>
        public virtual Control FeedLink
        {
            get
            {
                HyperLink xmlLink = new HyperLink();
                xmlLink.CssClass = "feedLinkStyle";
                if (requestPage.SiteConfig.UseFeedSchemeForSyndication)
                {
                    xmlLink.NavigateUrl = SiteUtilities.GetFeedUrl();
                }
                else
                {
                    xmlLink.NavigateUrl = SiteUtilities.GetRssUrl();
                }
                Image img = new Image();
                img.ImageUrl = requestPage.GetThemedImageUrl("feed-icon-16x16");
                img.CssClass = "feedLinkImageStyle";
                img.ToolTip = img.AlternateText = "Feed your aggregator (RSS 2.0)";
                xmlLink.Controls.Add(img);
                return xmlLink;
            }
        }

        /// <summary>
        /// Hyperlink to the main RSS feed, but with protocol scheme "feed".
        /// Corresponds to the macro: &lt;%feedLinkUrl(linktext,tooltip)%&gt;
        /// </summary>
        /// <param name="text">Provide a text you want as the link text</param>
        /// <param name="titleText">link title text (tooltip)</param>
        /// <remarks>The output is HTML hyperlinked HTML.</remarks>
        public virtual Control FeedLinkUrl(string text, string titleText)
        {
            HyperLink xmlLink = new HyperLink();
            xmlLink.ID = "feedLinkUrl";
            xmlLink.CssClass = "feedLinkStyle";
            xmlLink.NavigateUrl = SiteUtilities.GetFeedUrl();
            xmlLink.Text = (text == null || text.Length == 0 ? "FEED" : text);
            xmlLink.ToolTip = (titleText == null || titleText.Length == 0 ? "Feed your aggregator (RSS 2.0)" : titleText);
            return xmlLink;
        }

        /// <summary>
        /// Hyperlink to the main RSS feed, but with protocol scheme "feed".
        /// Corresponds to the macro: &lt;%feedLinkUrl()%&gt;
        /// </summary>
        /// <remarks>The output is HTML hyperlinked HTML.</remarks>
        public virtual Control FeedLinkUrl()
        {
            return this.FeedLinkUrl(null, null);
        }

        /// <summary>
        /// Hyperlink to the main RSS feed, but with protocol scheme "feed".
        /// Corresponds to the macro: &lt;%feedLinkUrl(linktext)%&gt;
        /// </summary>
        /// <param name="text">Provide a text you want as the link text</param>
        /// <remarks>The output is HTML hyperlinked HTML.</remarks>
        public virtual Control FeedLinkUrl(string text)
        {
            return this.FeedLinkUrl(text, null);
        }

        /// <summary>
        /// Hyperlink to the (Atom) feed.
        /// Corresponds to the macro: &lt;%atomLink%&gt;
        /// </summary>
        /// <remarks>The output is HTML Hyperlink.</remarks>
        public virtual Control AtomLink
        {
            get
            {
                HyperLink xmlLink = new HyperLink();
                xmlLink.CssClass = "atomLinkStyle";
                xmlLink.NavigateUrl = SiteUtilities.GetAtomUrl();

                Image img = new Image();
                img.ImageUrl = requestPage.GetThemedImageUrl("atomButton");
                img.CssClass = "rssLinkImageStyle";
                img.ToolTip = img.AlternateText = "Atom 1.0";
                xmlLink.Controls.Add(img);
                return xmlLink;
            }
        }

        /// <summary>
        /// Hyperlink to the main (Atom) feed.
        /// Corresponds to the macro: &lt;%atomLinkUrl(linktext,tooltip)%&gt;
        /// </summary>
        /// <param name="text">Provide a text you want as the link text. Default will be "ATOM".</param>
        /// <param name="titleText">link title text (tooltip)</param>
        /// <remarks>The output is HTML hyperlinked HTML. Default link text is "ATOM".</remarks>
        public virtual Control AtomLinkUrl(string text, string titleText)
        {
            HyperLink xmlLink = new HyperLink();
            xmlLink.ID = "atomLinkUrl";
            xmlLink.CssClass = "atomLinkStyle";
            xmlLink.NavigateUrl = SiteUtilities.GetAtomUrl();
            xmlLink.Text = (text == null || text.Length == 0 ? "ATOM" : text);
            xmlLink.ToolTip = (titleText == null || titleText.Length == 0 ? "Atom 1.0" : titleText);
            return xmlLink;
        }

        /// <summary>
        /// Hyperlink to the main (Atom) feed.
        /// Corresponds to the macro: &lt;%atomLinkUrl()%&gt;
        /// </summary>
        /// <remarks>The output is HTML hyperlinked HTML. Default link text is "ATOM".</remarks>
        public virtual Control AtomLinkUrl()
        {
            return this.AtomLinkUrl(null, null);
        }

        /// <summary>
        /// Hyperlink to the main (Atom) feed.
        /// Corresponds to the macro: &lt;%atomLinkUrl(linktext)%&gt;
        /// </summary>
        /// <param name="text">Provide a text you want as the link text. Default will be "ATOM".</param>
        /// <remarks>The output is HTML hyperlinked HTML. Default link text is "ATOM".</remarks>
        public virtual Control AtomLinkUrl(string text)
        {
            return this.AtomLinkUrl(text, null);
        }

        /// <summary>
        /// Returns just the URL for the ATOM feed for the current blog.
        /// </summary>
        /// <returns>The output is just the literal URL for the ATOM feed</returns>
        public virtual Control AtomUrl()
        {
            LiteralControl atomUrl = new LiteralControl();
            atomUrl.Text = SiteUtilities.GetAtomUrl();

            return atomUrl;
        }

        /// <summary>
        /// Hyperlink to the main RSS feed.
        /// Corresponds to the macro: &lt;%cdfLink%&gt;
        /// </summary>
        /// <remarks>The output is HTML hyperlinked HTML image.</remarks>
        public virtual Control CdfLink
        {
            get
            {
                HyperLink xmlLink = new HyperLink();
                xmlLink.CssClass = "cdfLinkStyle";
                xmlLink.NavigateUrl = SiteUtilities.GetCdfUrl();
                Image img = new Image();
                img.ImageUrl = requestPage.GetThemedImageUrl("cdfButton");
                img.CssClass = "cdfLinkImageStyle";
                img.ToolTip = img.AlternateText = "CDF";
                xmlLink.Controls.Add(img);

                return xmlLink;
            }
        }

        /// <summary>
        /// Current year.
        /// Corresponds to the macro: &lt;%year%&gt;
        /// </summary>
        /// <remarks>The output is plain text.</remarks>
        public virtual Control Year
        {
            get
            {
                DateTime dt = DateTime.Now.ToUniversalTime();
                if (requestPage.SiteConfig.AdjustDisplayTimeZone)
                {
                    WindowsTimeZone tz = requestPage.SiteConfig.GetConfiguredTimeZone();
                    dt = tz.ToLocalTime(dt);
                }
                return new LiteralControl(dt.Year.ToString());
            }
        }

        /// <summary>
        /// Current month.
        /// Corresponds to the macro: &lt;%month%&gt;
        /// </summary>
        /// <remarks>The output is plain text.</remarks>
        public virtual Control Month
        {
            get
            {
                DateTime dt = DateTime.Now.ToUniversalTime();
                if (requestPage.SiteConfig.AdjustDisplayTimeZone)
                {
                    WindowsTimeZone tz = requestPage.SiteConfig.GetConfiguredTimeZone();
                    dt = tz.ToLocalTime(dt);
                }
                return new LiteralControl(dt.Month.ToString());
            }
        }

        /// <summary>
        /// Current day.
        /// Corresponds to the macro: &lt;%day%&gt;
        /// </summary>
        /// <remarks>The output is plain text.</remarks>
        public virtual Control Day
        {
            get
            {
                DateTime dt = DateTime.Now.ToUniversalTime();
                if (requestPage.SiteConfig.AdjustDisplayTimeZone)
                {
                    WindowsTimeZone tz = requestPage.SiteConfig.GetConfiguredTimeZone();
                    dt = tz.ToLocalTime(dt);
                }
                return new LiteralControl(dt.Day.ToString());
            }
        }

        /// <summary>
        /// Current day formatted as short date.
        /// Corresponds to the macro: &lt;%today%&gt;
        /// </summary>
        /// <remarks>The output is plain text.</remarks>
        public virtual Control Today
        {
            get
            {
                DateTime dt = DateTime.Now.ToUniversalTime();
                if (requestPage.SiteConfig.AdjustDisplayTimeZone)
                {
                    WindowsTimeZone tz = requestPage.SiteConfig.GetConfiguredTimeZone();
                    dt = tz.ToLocalTime(dt);
                }
                return new LiteralControl(dt.ToString("d"));
            }
        }

        /// <summary>
        /// Current time (UTC).
        /// Corresponds to the macro: &lt;%now%&gt;
        /// </summary>
        /// <remarks>The output is plain text.</remarks>
        public virtual Control Now
        {
            get
            {
                if (requestPage.SiteConfig.AdjustDisplayTimeZone)
                {
                    return new LiteralControl(requestPage.SiteConfig.GetConfiguredTimeZone().FormatAdjustedUniversalTime(DateTime.Now.ToUniversalTime()));
                }
                else
                {
                    return new LiteralControl(DateTime.Now.ToUniversalTime().ToString("U") + " UTC");
                }
            }
        }

        /// <summary>
        /// Current time formatted as short time.
        /// Corresponds to the macro: &lt;%time%&gt;
        /// </summary>
        /// <remarks>The output is plain text.</remarks>
        public virtual Control Time
        {
            get
            {
                DateTime dt = DateTime.Now.ToUniversalTime();
                if (requestPage.SiteConfig.AdjustDisplayTimeZone)
                {
                    WindowsTimeZone tz = requestPage.SiteConfig.GetConfiguredTimeZone();
                    dt = tz.ToLocalTime(dt);
                }
                return new LiteralControl(dt.ToString("T"));
            }
        }

        /// <summary>
        /// Copyright owner as defined through the tag &lt;Copyright&gt;
        /// in the configuration file of the Web application.
        /// Corresponds to the macro: &lt;%authorName%&gt;
        /// </summary>
        /// <remarks>The output is plain text.</remarks>
        public virtual Control AuthorName
        {
            get { return new LiteralControl(requestPage.SiteConfig.Copyright); }
        }


        /// <summary>
        /// hCard for the copyright owner as defined in the tag &lt;Copyright&gt;
        /// in the configuration file of the Web application.
        /// Corresponds to the macro: &lt;%hCardAuthorName%&gt;
        /// </summary>
        /// <remarks>See http://microformats.org/wiki/hcard</remarks>
        public virtual Control hCardAuthor
        {
            get
            {
                // <span class="vcard">
                //		<a class="url fn" href="http://localhost/dasblog">Copyright owner</a>

                // </span>

                HtmlGenericControl div = new HtmlGenericControl("span");
                div.Attributes.Add("class", "vcard");

                HtmlAnchor nameUrl = new HtmlAnchor();
                nameUrl.Attributes.Add("class", "url fn");
                nameUrl.HRef = requestPage.SiteConfig.Root;
                nameUrl.InnerText = requestPage.SiteConfig.Copyright;

                div.Controls.Add(nameUrl);

                return div;
            }
        }

        /// <summary>
        /// Control for paging in the category view. 
        /// Draws previous link, x of y page and next link; where appropiate.
        /// </summary>
        /// <returns></returns>
        public virtual Control DrawCategoryPaging()
        {
            if (this.requestPage.CategoryName != String.Empty && SiteConfig.GetSiteConfig().CategoryAllEntries == false)
            {
                if (HttpContext.Current.Items.Contains("page") == true && HttpContext.Current.Items.Contains("maxpage") == true)
                {
                    HtmlGenericControl div = new HtmlGenericControl("span");
                    div.Attributes.Add("class", "categoryPaging");

                    int page = (int)HttpContext.Current.Items["page"];
                    int maxpage = (int)HttpContext.Current.Items["maxpage"];

                    if (page != 1)
                    {
                        HtmlAnchor previous = new HtmlAnchor();
                        previous.HRef = SiteUtilities.GetCategoryViewUrl(this.requestPage.CategoryName, page - 1);
                        previous.InnerText = requestPage.CoreStringTables.GetString("text_category_previouspage");
                        div.Controls.Add(previous);
                        div.Controls.Add(new LiteralControl(" "));
                    }

                    string pagexofy = requestPage.CoreStringTables.GetString("text_category_pagexofy");
                    div.Controls.Add(new LiteralControl(String.Format(pagexofy, page, maxpage, requestPage.CategoryName)));

                    if (page != maxpage)
                    {
                        div.Controls.Add(new LiteralControl(" "));
                        HtmlAnchor next = new HtmlAnchor();
                        next.HRef = SiteUtilities.GetCategoryViewUrl(this.requestPage.CategoryName, page + 1);
                        next.InnerText = requestPage.CoreStringTables.GetString("text_category_nextpage");
                        div.Controls.Add(next);
                    }
                    return div;
                }
            }
            return new LiteralControl(String.Empty);
        }

        /// <summary>
        /// Placeholder for rendering page content
        /// Corresponds to the macro: &lt;%bodytext%&gt;
        /// </summary>
        /// <remarks>The output is HTML.</remarks>
        public virtual Control Bodytext
        {
            get
            {
                IPageFormatInfo formatInfo = requestPage as IPageFormatInfo;
                if (formatInfo != null)
                {
                    return formatInfo.Bodytext;
                }
                else
                {
                    PlaceHolder bodyPlaceHolder = new PlaceHolder();

                    EntryCollection entries = requestPage.WeblogEntries;
                    if (entries != null)
                    {
                        List<DateTime> al = new List<DateTime>();
                        foreach (Entry entry in entries)
                        {
                            DateTime currentDay = entry.CreatedUtc;

                            if (requestPage.SiteConfig.AdjustDisplayTimeZone)
                            {
                                currentDay = requestPage.SiteConfig.GetConfiguredTimeZone().ToLocalTime(currentDay);
                            }

                            currentDay = currentDay.Date;

                            if (!al.Contains(currentDay))
                            {
                                al.Add(currentDay);
                            }
                        }
                        al.Sort();
                        for (int i = al.Count - 1; i >= 0; i--)
                        {
                            requestPage.ProcessDayTemplate(al[i], bodyPlaceHolder);
                        }
                    }
                    return bodyPlaceHolder;
                }
            }
        }

        public virtual Control TitleList()
        {
            return TitleList(int.MaxValue);
        }

        /// <summary>
        /// List of entry titles that are displayed on the current page.
        /// Corresponds to the macro: &lt;%titleList%&gt;
        /// </summary>
        /// <remarks>The output is HTML.</remarks>
        public virtual Control TitleList(int maxLength)
        {
            if (maxLength == 0) maxLength = int.MaxValue;
            Table list = new Table();
            list.CssClass = "titleListStyle";

            foreach (Entry entry in requestPage.WeblogEntries)
            {
                if (entry.Title != null && entry.Title.Length > 0)
                {
                    TableRow row = new TableRow();
                    TableCell cell = new TableCell();
                    //cell.CssClass = "titleListCellStyle";
                    list.Rows.Add(row);
                    row.Cells.Add(cell);

                    HyperLink titleLink = new HyperLink();
                    //titleLink.CssClass = "titleListLinkStyle";
                    titleLink.Text = TruncateString(entry.Title, maxLength);
                    titleLink.NavigateUrl = HttpContext.Current.Request.RawUrl + "#a" + entry.EntryId;
                    cell.Controls.Add(titleLink);
                }
            }

            //Don't return an empty table, its invalid XHTML
            if (list.Rows.Count == 0)
                return new LiteralControl();

            return list;
        }

        public class PopularSorter : IComparer<Entry>
        {
            #region IComparer Members

            public int Compare(Entry left, Entry right)
            {

                if (left == null) return -1;
                if (right == null) return 1;
                if (left.IsPublic == false) return -1;
                if (right.IsPublic == false) return 1;

                SiteConfig siteConfig = SiteConfig.GetSiteConfig();
                ILoggingDataService logService = LoggingDataServiceFactory.GetService(SiteConfig.GetLogPathFromCurrentContext());
                IBlogDataService dataService = BlogDataServiceFactory.GetService(SiteConfig.GetContentPathFromCurrentContext(), logService);

                TrackingCollection leftTrackings = dataService.GetTrackingsFor(left.EntryId);
                TrackingCollection rightTrackings = dataService.GetTrackingsFor(right.EntryId);

                CommentCollection leftComments = dataService.GetPublicCommentsFor(left.EntryId);
                CommentCollection rightComments = dataService.GetPublicCommentsFor(right.EntryId);
                int leftCount = leftTrackings.Count + leftComments.Count;
                int rightCount = rightTrackings.Count + rightComments.Count;
                return leftCount.CompareTo(rightCount);
            }
            #endregion
        }

        private static object popularListLock = new object();
        /// <summary>
        /// This macro is used like this: %lt;%popularList(10,20)%&gt; and will return a list of Popular entries, 
        /// where Popular entries have more "Trackings" than non-Popular ones. Trackings include referrals, pingbacks, trackbacks and comments.
        /// </summary>
        /// <remarks>
        /// The resulting table is cached in memory for 8 hours, as popularity of posts doesn't change THAT much, and updating 3 times a day seems more than reasonable.
        /// </remarks>
        /// <param name="numEntries">The number of entries to return</param>
        /// <param name="maxLength">The length at which to truncate the title</param>
        /// <returns>A Table of Popular Entries</returns>
        public virtual Control PopularList(int numEntries, int maxLength)
        {
            if (maxLength == 0) maxLength = 128;
            if (numEntries == 0) numEntries = 10;

            DataCache cache = CacheFactory.GetCache();

            Table list = cache["PopularEntriesTable"] as Table;
            if (list == null)
            {
                lock (popularListLock)
                {
                    if (list == null)
                    {
                        list = new Table();
                        list.CssClass = "titleListStyle";

                        IBlogDataService dataService = requestPage.DataService;

                        EntryCollection popularEntries = new EntryCollection();
                        DateTime[] daysWithEntries = dataService.GetDaysWithEntries(DateTimeZone.Utc);
                        CommentCollection comments = dataService.GetAllComments();
                        foreach (DateTime day in daysWithEntries)
                        {
                            EntryCollection entries = dataService.GetEntriesForDay(day, DateTimeZone.Utc, String.Empty, 1, int.MaxValue, String.Empty);
                            foreach (Entry potentialEntry in entries)
                            {

                                if (popularEntries.ContainsKey(potentialEntry.EntryId) == false)
                                {
                                    popularEntries.Add(potentialEntry);
                                }
                            }
                        }


                        popularEntries.Sort(new PopularSorter());
                        popularEntries.Reverse();
                        popularEntries.RemoveRange(numEntries - 1, popularEntries.Count - numEntries);

                        foreach (Entry entry in popularEntries)
                        {
                            if (entry.Title != null && entry.Title.Length > 0)
                            {
                                TableRow row = new TableRow();
                                TableCell cell = new TableCell();
                                //cell.CssClass = "titleListCellStyle";
                                list.Rows.Add(row);
                                row.Cells.Add(cell);

                                HyperLink titleLink = new HyperLink();
                                //titleLink.CssClass = "titleListLinkStyle";
                                titleLink.Text = TruncateString(entry.Title, maxLength);
                                titleLink.NavigateUrl = SiteUtilities.GetPermaLinkUrl(entry);
                                cell.Controls.Add(titleLink);
                            }
                        }
                        cache.Insert("PopularEntriesTable", list, DateTime.Now.AddHours(36));
                    }
                }
            }
            return list;
        }

        public static DateTime GetStartOfMonth(int Month, int Year)
        {
            return new DateTime(Year, Month, 1, 0, 0, 0, 0);
        }

        private static DateTime GetEndOfMonth(int Month, int Year)
        {
            return new DateTime(Year, Month,
                DateTime.DaysInMonth(Year, Month), 23, 59, 59, 999);
        }

        public static DateTime GetStartOfCurrentWeek()
        {
            int DaysToSubtract = (int)DateTime.UtcNow.DayOfWeek;
            DateTime dt = DateTime.UtcNow.Subtract(
                System.TimeSpan.FromDays(DaysToSubtract));
            return new DateTime(dt.Year, dt.Month, dt.Day, 0, 0, 0, 0);
        }

        public static DateTime GetEndOfCurrentWeek()
        {
            DateTime dt = GetStartOfCurrentWeek().AddDays(6);
            return new DateTime(dt.Year, dt.Month, dt.Day, 23, 59, 59,
                999);
        }

        public static DateTime GetStartOfYear(int Year)
        {
            return new DateTime(Year, 1, 1, 0, 0, 0, 0);
        }

        public static DateTime GetEndOfYear(int Year)
        {
            return new DateTime(Year, 12,
                DateTime.DaysInMonth(Year, 12), 23, 59, 59, 999);
        }

        private static object blogStatsLock = new object();

        /// <summary>
        /// This macro is used like this: %lt;%BlogStats()%&gt; and will return a set of Blog statistics, 
        /// including totalPosts, totalComments, postsThisWeek, postsThisMonth
        /// </summary>
        /// <remarks>
        /// The resulting data is cached in memory for 8 hours, as popularity of posts doesn't change THAT much, and updating 3 times a day seems more than reasonable.
        /// </remarks>
        /// <returns>A set of Blog Statistics</returns>
        public virtual Control BlogStats()
        {
            Literal lit = new Literal();
            const string key = "BlogStats";

            DataCache cache = CacheFactory.GetCache();

            BlogStatistics blogstats = cache[key] as BlogStatistics;
            if (blogstats == null)
            {
                lock (blogStatsLock)
                {
                    if (blogstats == null)
                    {
                        try
                        {
                            BlogStatistics newStats = new BlogStatistics();

                            IBlogDataService dataService = requestPage.DataService;

                            DateTime monthFirst = Macros.GetStartOfMonth(DateTime.UtcNow.Month, DateTime.UtcNow.Year);
                            DateTime monthLast = Macros.GetEndOfMonth(DateTime.UtcNow.Month, DateTime.UtcNow.Year);

                            DateTime weekFirst = Macros.GetStartOfCurrentWeek();
                            DateTime weekLast = Macros.GetEndOfCurrentWeek();

                            DateTime yearFirst = Macros.GetStartOfYear(DateTime.UtcNow.Year);
                            DateTime yearLast = Macros.GetEndOfYear(DateTime.UtcNow.Year);

                            DateTime[] daysWithEntries = dataService.GetDaysWithEntries(DateTimeZone.Utc);
                            foreach (DateTime day in daysWithEntries)
                            {
                                EntryCollection entries = dataService.GetEntriesForDay(day, DateTimeZone.Utc, String.Empty, 1, int.MaxValue, String.Empty);
                                newStats.AllEntriesCount += entries.Count;
                                foreach (Entry potentialEntry in entries)
                                {
                                    if (potentialEntry.CreatedUtc > monthFirst && potentialEntry.CreatedUtc <= monthLast)
                                    {
                                        newStats.MonthPostCount++;
                                    }

                                    if (potentialEntry.CreatedUtc > weekFirst && potentialEntry.CreatedUtc <= weekLast)
                                    {
                                        newStats.WeekPostCount++;
                                    }

                                    if (potentialEntry.CreatedUtc > yearFirst && potentialEntry.CreatedUtc <= yearLast)
                                    {
                                        newStats.YearPostCount++;
                                    }
                                }
                            }

                            CommentCollection comments = dataService.GetAllComments();

                            newStats.CommentCount = (comments == null ? 0 : comments.Count);

                            cache.Insert(key, newStats, DateTime.Now.AddHours(8));
                            // potential race condition
                            blogstats = newStats;
                        }
                        catch (Exception ex)
                        {
                            requestPage.LoggingService.AddEvent(new EventDataItem(EventCodes.Error, "BlogStats Error: " + ex.ToString(), String.Empty));
                        }
                    }
                }
            }

            // localize strings			
            string totalPostsString = requestPage.CoreStringTables.GetString("text_blogstats_totalposts");
            string thisWeekString = requestPage.CoreStringTables.GetString("text_blogstats_thisweek");
            string thisMonthString = requestPage.CoreStringTables.GetString("text_blogstats_thismonth");
            string thisYearString = requestPage.CoreStringTables.GetString("text_blogstats_thisyear");
            string commentsString = requestPage.CoreStringTables.GetString("text_blogstats_comments");

            // render content
            StringBuilder sb = new StringBuilder();
            sb.Append(totalPostsString + " " + blogstats.AllEntriesCount + "<br />");
            sb.Append(thisYearString + " " + blogstats.YearPostCount + "<br />");
            sb.Append(thisMonthString + " " + blogstats.MonthPostCount + "<br />");
            sb.Append(thisWeekString + " " + blogstats.WeekPostCount + "<br />");
            sb.Append(commentsString + " " + blogstats.CommentCount + "<br />");
            lit.Text = "<div class=\"blogStats\">" + sb.ToString() + "</div>";

            return lit;
        }

        public class UserInfo : IComparable
        {
            private string name = string.Empty;
            private string displayName = string.Empty;
            private int noPosts = 0;

            public string Name
            {
                get { return name; }
                set { name = value; }
            }

            public string DisplayName
            {
                get { return displayName; }
                set { displayName = value; }
            }

            public int NoPosts
            {
                get { return noPosts; }
                set { noPosts = value; }
            }

            public string Author
            {
                get
                {
                    if (displayName != null && displayName.Length > 0)
                        return displayName;
                    else return name;
                }
            }

            public UserInfo(string name, string displayName)
            {
                this.name = name;
                this.displayName = displayName;
            }

            public int CompareTo(object obj)
            {
                UserInfo userInfo = (UserInfo)obj;

                if (this.NoPosts > userInfo.NoPosts)
                    return -1;
                if (this.NoPosts < userInfo.NoPosts)
                    return 1;
                return 0;
            }
        }

        private static object blogTopPostersLock = new object();

        public bool TopPostersDayEntryCriteria(DayEntry entry)
        {
            return true;
        }

        public bool TopPostersEntryCriteria(Entry entry)
        {
            return true;
        }

        public virtual Control BlogTopPosters()
        {
            Literal lit = new Literal();
            string key = "BlogTopPosters";

            DataCache cache = CacheFactory.GetCache();


            string blogTopPosters = cache[key] as string;

            if (blogTopPosters == null)
            {
                lock (blogTopPostersLock)
                {
                    if (blogTopPosters == null)
                    {
                        try
                        {
                            string totalPostsString = requestPage.CoreStringTables.GetString("text_blogtopposters_totalposts");
                            string commentsString = requestPage.CoreStringTables.GetString("text_blogtopposters_comments");

                            List<UserInfo> userItems = new List<UserInfo>();
                            Dictionary<string, UserInfo> userKeys = new Dictionary<string, UserInfo>();

                            IBlogDataService dataService = requestPage.DataService;

                            EntryCollection entries = dataService.GetEntries(
                                TopPostersDayEntryCriteria,
                                TopPostersEntryCriteria,
                                int.MaxValue,
                                int.MaxValue);

                            foreach (Entry potentialEntry in entries)
                            {
                                if (userKeys.ContainsKey(potentialEntry.Author))
                                {
                                    userKeys[potentialEntry.Author].NoPosts++;
                                }
                                else
                                {
                                    User user = SiteSecurity.GetUser(potentialEntry.Author);

                                    if (user.Active)
                                    {
                                        UserInfo userInfo = new UserInfo(user.Name, user.DisplayName);
                                        userInfo.NoPosts = 1;
                                        userItems.Add(userInfo);
                                        userKeys[potentialEntry.Author] = userInfo;
                                    }
                                }
                            }

                            userItems.Sort();
                            int commentCount = dataService.GetAllComments().Count;

                            StringBuilder sb = new StringBuilder();
                            sb.Append("<table class=\"topPostersListTableStyle\">");
                            sb.Append("<col class=\"topPostersUserCellStyle\"/>");
                            sb.Append("<col class=\"topPostersNoPostsCellStyle\"/>");

                            foreach (UserInfo userInfo in userItems)
                            {
                                sb.AppendFormat("<tr><td><a href=\"{0}\">{1}</a></td><td>{2}</td></tr>",
                                    SiteUtilities.GetUserViewUrl(userInfo.Name),
                                    userInfo.Author,
                                    userInfo.NoPosts);
                            }

                            sb.AppendFormat("<tr><td colspan=\"2\"><hr class=\"topPostersDivider\"></td></tr>");
                            sb.AppendFormat("<tr><td>{0}</td><td>{1}</td></tr>", totalPostsString, entries.Count);
                            sb.AppendFormat("<tr><td>{0}</td><td>{1}</td></tr>", commentsString, commentCount);
                            sb.Append("</table>");

                            blogTopPosters = sb.ToString();
                            cache.Insert(key, blogTopPosters, DateTime.Now.AddHours(8));
                        }
                        catch (Exception ex)
                        {
                            requestPage.LoggingService.AddEvent(new EventDataItem(EventCodes.Error, "BlogTopPosters Error: " + ex.ToString(), String.Empty));
                        }
                    }
                }
            }

            lit.Text = blogTopPosters;
            return lit;
        }

        protected string TruncateString(string s, int max)
        {
            if (s == null)
            {
                return "";
            }
            else if (s.Length < max)
            {
                return s;
            }
            else if (s.Length >= max)
            {
                return s.Substring(0, max) + "...";
            }
            return s;
        }


        /// <summary>
        /// List of entry titles that appear on the current front page.
        /// Corresponds to the macro: &lt;%frontPageTitleList%&gt;
        /// </summary>
        /// <remarks>The output is HTML. Uses the same CSS classes as TitleList.</remarks>
        public virtual Control FrontPageTitleList
        {
            get
            {
                Table list = new Table();
                list.BorderWidth = Unit.Pixel(0);
                list.CellPadding = 0;
                list.CellSpacing = 0;
                list.CssClass = "titleListStyle";

                foreach (Entry entry in requestPage.GetFrontPageEntries())
                {
                    if (entry.CreatedUtc <= DateTime.UtcNow)
                    {
                        if (entry.Title != null & entry.Title.Length > 0)
                        {
                            TableRow row = new TableRow();
                            TableCell cell = new TableCell();
                            //cell.CssClass = "titleListCellStyle";
                            //cell.HorizontalAlign = HorizontalAlign.Center;
                            list.Rows.Add(row);
                            row.Cells.Add(cell);

                            HyperLink titleLink = new HyperLink();
                            //titleLink.CssClass = "titleListLinkStyle";
                            titleLink.Text = entry.Title;
                            titleLink.NavigateUrl = SiteUtilities.GetStartPageUrl(requestPage.SiteConfig) + "#a" + entry.EntryId;
                            cell.Controls.Add(titleLink);
                        }
                    }
                }
                return list;
            }
        }

        /// <summary>
        /// This is similar to <see>FrontPageTitleList</see>, with two key differences:
        /// 1) This is rendered as a unordered list, rather than a table - for all those who prefer to use 
        /// CSS-based layouts for flexibility (don't you?).
        /// 2) The links do not point to the front page anchor entries - instead, they link to the permalinked
        /// single entry.
        /// Corresponds to the macro: &lt;%RecentEntries%&gt;
        /// </summary>
        public virtual Control RecentEntries
        {
            get
            {
                // We're doing this the brute force way given the lack of a corresponding HTML
                // control.
                StringBuilder sb = new StringBuilder();
                sb.Append("<ul class='titleListStyle'>");
                foreach (Entry entry in requestPage.GetFrontPageEntries())
                {
                    if (entry.CreatedUtc <= DateTime.UtcNow)
                    {
                        if (entry.Title != null & entry.Title.Length > 0)
                        {
                            sb.Append("<li>");
                            sb.AppendFormat("<a class='titleListLinkStyle' href='{0}'>{1}</a>",
                                SiteUtilities.GetPermaLinkUrl(entry),
                                entry.Title);
                            sb.Append("</li>");
                        }
                    }
                }
                sb.Append("</ul>");
                return new LiteralControl(sb.ToString());
            }
        }

        /// <summary>
        /// Behaves as RecentEntries does, only it formats as a table (for those who prefer not to futz with CSS).
        /// Possibly redundant given that RecentEntries is a better method for rendering?
        /// Corresponds to the macro: &lt;%RecentEntriesAsTable%&gt;
        /// </summary>
        public virtual Control RecentEntriesAsTable
        {
            get
            {
                Table list = new Table();
                list.BorderWidth = Unit.Pixel(0);
                list.CellPadding = 0;
                list.CellSpacing = 0;
                list.CssClass = "titleListStyle";

                foreach (Entry entry in requestPage.GetFrontPageEntries())
                {
                    if (entry.CreatedUtc <= DateTime.UtcNow)
                    {
                        if (entry.Title != null & entry.Title.Length > 0)
                        {
                            TableRow row = new TableRow();
                            TableCell cell = new TableCell();
                            cell.CssClass = "titleListCellStyle";
                            list.Rows.Add(row);
                            row.Cells.Add(cell);

                            HyperLink titleLink = new HyperLink();
                            titleLink.CssClass = "titleListLinkStyle";
                            titleLink.Text = entry.Title;
                            if (entry.Link != null)
                            {
                                titleLink.NavigateUrl = entry.Link;
                            }
                            else
                            {
                                titleLink.NavigateUrl = SiteUtilities.GetPermaLinkUrl(entry);
                            }
                            cell.Controls.Add(titleLink);
                        }
                    }
                }
                return list;
            }
        }

        /// <summary>
        /// Weblog 'Title' as defined through the tag &lt;Title&gt;
        /// in the configuration file of the Web application.
        /// Corresponds to the macro: &lt;%title%&gt;
        /// Not the same as <see cref="Macros.Title"/>.
        /// <seealso cref="Macros.Title"/> which is the web page
        /// title as rendered in the title html tag.
        /// </summary>
        /// <remarks>The output is plain text.</remarks>
        public virtual Control SiteName
        {
            get { return new LiteralControl(requestPage.SiteConfig.Title); }
        }

        /// <summary>
        /// Provides a Hyperlink with the sitename and link
        /// to the root homepage as defined in SiteConfig.
        /// Corresponds to the macro: &lt;%siteNameLink%&gt;
        /// </summary>
        /// <remarks>The output is a hyperlink.</remarks>
        public virtual Control SiteNameLink
        {
            get { return new LiteralControl(String.Format("<a href=\"{0}\">{1}</a>", SiteUtilities.GetStartPageUrl(requestPage.SiteConfig), requestPage.SiteConfig.Title)); }
        }

        /// <summary>
        /// Site description as defined through the tag &lt;Description&gt;
        /// in the configuration file of the Web application.
        /// Corresponds to the macro: &lt;%description%&gt;
        /// </summary>
        /// <remarks>The output is plain text.</remarks>
        public virtual Control Description
        {
            get { return new LiteralControl(requestPage.SiteConfig.Description); }
        }

        /// <summary>
        /// Renders a list of user-definable hyperlinks.
        /// Corresponds to the macro: &lt;%navigatorLinks%&gt;
        /// </summary>
        /// <remarks>
        /// The output is HTML.
        /// <P/>
        /// List can be edited in the configuration file 'navigatorLinks.xml'.
        /// </remarks>
        public virtual Control NavigatorLinks
        {
            get { return new SideBarList(false); }
        }

        /// <summary>
        /// Renders a list of user-definable hyperlinks.
        /// Corresponds to the macro: &lt;%navigatorLinksList%&gt;
        /// </summary>
        /// <remarks>
        /// The output is HTML.
        /// <P/>
        /// List can be edited in the configuration file 'navigatorLinks.xml'.
        /// </remarks>
        public virtual Control NavigatorLinksList
        {
            get { return new SideBarList(true); }
        }

        /// <summary>
        /// The localString macro allows to define language/culture dependend string
        /// expressions in a template. An example for the format for the string is
        /// "English Expression|DE-CH:Schweizerisches Ausdr�ckli|DE:Deutscher Ausdruck"
        /// The default expression (usually English) always goes first and is used for
        /// all cultures than are not contained in the string. The following expressions
        /// are prefixed by their culture code followed by a colon. The substrings are 
        /// separated from each other by the pipe-symbol. The pipe-symbol itself can't be
        /// used in localString expressions.
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public virtual Control LocalString(string expression)
        {
            string[] localStrings = expression.Split('|');
            string cultureId = Thread.CurrentThread.CurrentCulture.Name;

            for (int i = 1; i < localStrings.Length; i++)
            {
                string culturePrefix = localStrings[i].Split(':')[0];
                if (cultureId.ToUpper().StartsWith(culturePrefix.ToUpper()))
                {
                    return new LiteralControl(localStrings[i].Substring(culturePrefix.Length + 1));
                }
            }
            return new LiteralControl(localStrings[0]);
        }

        public virtual Control DrawTagCloud()
        {
            string languageFilter = HttpContext.Current.Request.Headers["Accept-Language"];
            string key = "TagCloud" + languageFilter;

            DataCache cache = CacheFactory.GetCache();

            string control = cache[key] as string;
            if (control == null)
            {
                StringBuilder sb = new StringBuilder();
                int minCount = int.MaxValue;
                int maxCount = 0;
                Hashtable catEntryCount = new Hashtable();
                sb.Append("<span class=\"tagCloud\">");
                CategoryCacheEntryCollection categories = requestPage.DataService.GetCategories();
                foreach (CategoryCacheEntry category in categories)
                {
                    EntryCollection entries = requestPage.DataService.GetEntriesForCategory(category.Name, languageFilter);
                    catEntryCount.Add(category.Name, entries.Count);
                    minCount = Math.Min(minCount, entries.Count);
                    maxCount = Math.Max(maxCount, entries.Count);
                }
                int distribution = (maxCount - minCount) / 5;

                foreach (CategoryCacheEntry category in categories)
                {
                    int count = (int)catEntryCount[category.Name];

                    string cssClass = String.Empty;
                    if (count == minCount)
                        cssClass = "smallestTag";
                    else if (count == maxCount)
                        cssClass = "largestTag";
                    else if (count > (minCount + (distribution * 4)))
                        cssClass = "largerTag";
                    else if (count > (minCount + (distribution * 3)))
                        cssClass = "largeTag";
                    else if (count > (minCount + (distribution * 2)))
                        cssClass = "mediumTag";
                    else if (count > (minCount + (distribution)))
                        cssClass = "smallTag";
                    else
                        cssClass = "smallerTag";

                    sb.AppendFormat("<a class=\"{2}\" href=\"{1}\">{0}</a>", category.DisplayName + "&nbsp;(" + count.ToString() + ")", SiteUtilities.GetCategoryViewUrl(requestPage.SiteConfig, category.Name), cssClass);
                    sb.Append(" ");
                }
                sb.Append("</span>");
                control = sb.ToString();
                cache.Insert(key, control, DateTime.Now.AddMinutes(240));
            }
            return new LiteralControl(control);
        }

        public virtual RadioProperties Radio
        {
            get { return _radio; }
        }

        public virtual NewtelligenceProperties Newtelligence
        {
            get { return _newtelligence; }
        }


        public virtual Control MailTo()
        {
            HyperLink mailLink = new HyperLink();
            mailLink.CssClass = "mailToStyle";


            mailLink.NavigateUrl = SiteUtilities.RelativeToRoot("Email.aspx");

            Image img = new Image();
            img.CssClass = "mailToImageStyle";
            img.ToolTip = img.AlternateText = requestPage.CoreStringTables.GetString("text_send_mail");
            img.ImageUrl = requestPage.GetThemedImageUrl("mailTo");
            mailLink.Controls.Add(img);
            return mailLink;
        }
    }

    /// <summary>
    /// Like Macros, DayMacros always return controls.
    /// </summary>
    public class DayMacros : Macros
    {
        /// <summary>
        /// This value is expressed in display time (which depends on SiteConfig.AdjustDisplayTimeZone)
        /// </summary>
        protected DateTime day;

        /// <summary>
        /// DayMacros' constructor
        /// </summary>
        /// <param name="page">The page we are rendering</param>
        /// <param name="day">A day, expressed in display time, depending on SiteConfig.AdjustDisplayTimeZone</param>
        public DayMacros(SharedBasePage page, DateTime day)
            : base(page)
        {
            this.day = day;
        }

        /// <summary>
        /// Hyperlink to archived entries.
        /// Corresponds to the macro: &lt;%archiveLink%&gt;
        /// Works hand in hand with <see cref="DayMacros.LongDate"/>
        /// </summary>
        /// <remarks>The output is HTML hyperlinked HTML image.</remarks>
        public virtual Control ArchiveLink
        {
            get
            {
                HyperLink hl = new HyperLink();
                hl.CssClass = "archiveLinkStyle";
                Image img = new Image();
                img.ImageUrl = new Uri(new Uri(SiteUtilities.GetBaseUrl(requestPage.SiteConfig)), requestPage.GetThemedImageUrl("dayLink")).ToString();
                img.CssClass = "archiveLinkImageStyle";
                img.AlternateText = "#";
                hl.Controls.Add(img);
                hl.NavigateUrl = SiteUtilities.GetDayViewUrl(day);
                return hl;
            }
        }



        /// <summary>
        /// Protected field <see cref="DayMacros.day"/> formatted as long date.
        /// Corresponds to the macro: &lt;%longDate%&gt;
        /// Works hand in hand with <see cref="DayMacros.ArchiveLink"/>
        /// </summary>
        /// <remarks>The output is plain text.</remarks>
        public virtual Control LongDate
        {
            get { return new LiteralControl(day.ToString("D")); }
        }

        /// <summary>
        /// Protected field <see cref="DayMacros.day"/> formatted as whatever the
        /// user passes in as the format string.
        /// Corresponds to the macro: &lt;%dayFormattedDate(...)%&gt;
        /// Works hand in hand with <see cref="DayMacros.ArchiveLink"/>
        /// </summary>
        /// <param name="format">Standard format string</param>
        /// <remarks>The output is plain text.</remarks>
        public virtual Control dayFormattedDate(string format)
        {
            return new LiteralControl(day.ToString(format));
        }

        /// <summary>
        /// Placeholder for rendering weblog entries.
        /// Corresponds to the macro: &lt;%items%&gt;
        /// </summary>
        /// <remarks>The output is HTML.</remarks>
        public virtual Control Items
        {
            get
            {
                PlaceHolder itemPlaceHolder = new PlaceHolder();

                EntryCollection entries = requestPage.WeblogEntries;
                foreach (Entry entry in entries)
                {
                    if ((!requestPage.SiteConfig.AdjustDisplayTimeZone && day.Year == entry.CreatedUtc.Year && day.Month == entry.CreatedUtc.Month && day.Day == entry.CreatedUtc.Day) || (requestPage.SiteConfig.AdjustDisplayTimeZone && requestPage.SiteConfig.GetConfiguredTimeZone().ToLocalTime(entry.CreatedUtc).Date == new DateTime(day.Year, day.Month, day.Day).Date))
                    {
                        requestPage.ProcessItemTemplate(entry, itemPlaceHolder);
                    }
                }
                return itemPlaceHolder;
            }
        }
    }

    /// <summary>
    /// Like Macros and DayMacros, ItemMacros always return controls.
    /// </summary>
    public class ItemMacros : DayMacros
    {
        protected Entry entry;

        /// <summary>
        /// ItemMacros' constructor
        /// </summary>
        /// <param name="page">The page we are rendering</param>
        /// <param name="entry">An entry!</param>
        public ItemMacros(SharedBasePage page, Entry entry)
            : base(page, entry.CreatedUtc)
        {
            this.entry = entry;
        }

        /// <summary>
        /// The text of a single weblog entry.
        /// Corresponds to the macro: &lt;%itemText%&gt;
        /// </summary>
        /// <remarks>
        /// The output is plain text.
        /// <P/>
        /// To de/activate the content filter mechanism edit the tag
        /// &lt;ApplyContentFiltersToWeb&gt; in the SiteConfig file of the Web application.
        /// </remarks>
        public virtual Control ItemText
        {
            get
            {
                string bodyContent;

                if (requestPage.SiteConfig.ApplyContentFiltersToWeb)
                {
                    bodyContent = SiteUtilities.FilterContent(entry.EntryId, entry.Content);
                }
                else
                {
                    bodyContent = entry.Content;
                }

                if (SiteSecurity.IsInRole("admin") && entry.Language != null && entry.Language.Length > 0)
                {
                    CultureInfo ci = CultureInfo.InvariantCulture;
                    foreach (CultureInfo sci in CultureInfo.GetCultures(CultureTypes.AllCultures))
                    {
                        if (sci.Name.ToUpper() == entry.Language.ToUpper())
                        {
                            ci = sci;
                            break;
                        }
                    }
                    bodyContent = String.Format("<p style=\"color:red;font-size:smaller\">{0} {1}/{2} ({3})<p>{4}", requestPage.CoreStringTables.GetString("text_item_lang_note"), ci.EnglishName, ci.NativeName, entry.Language, bodyContent);
                }
                if (SiteSecurity.IsInRole("admin") && (entry.Categories == null || entry.Categories.Trim().Length == 0) && requestPage.SiteConfig.FrontPageCategory != null && requestPage.SiteConfig.FrontPageCategory.Length > 0)
                {
                    bodyContent = "<p style=\"color:red;font-size:smaller\">" + requestPage.CoreStringTables.GetString("text_item_cat_note") + "<p>" + bodyContent;
                }
                return new LiteralControl(bodyContent);
            }
        }

        /// <summary>
        /// The body of a single weblog entry. Corresponds to the macro: &lt;%itemBody%&gt;
        /// </summary>
        /// <remarks>
        /// The difference between the output of this and the itemText macro is that
        /// this macro depends on the &lt;ShowDescriptionInAggregatedViews&gt; setting in
        /// site.config and on whether the current page is an aggregated view. If both is the 
        /// case and the entry does have a description in addition to a content entry, the
        /// description and not the body content will be rendered. If the description is rendered,
        /// the macro will add a "Read more..." link at the bottom of the rendered output.
        /// <P/>
        /// To de/activate the content filter mechanism edit the tag
        /// &lt;ApplyContentFiltersToWeb&gt; in the SiteConfig file of the Web application.
        /// </remarks>
        public virtual Control ItemBody
        {
            get
            {
                bool isDescription = false;
                string bodyContent;

                if (requestPage.SiteConfig.ShowItemDescriptionInAggregatedViews && requestPage.IsAggregatedView)
                {
                    if (entry.Description != null && entry.Description.Trim().Length > 0)
                    {
                        bodyContent = entry.Description;
                        isDescription = true;
                    }
                    else
                    {
                        bodyContent = entry.Content;
                    }
                }
                else
                {
                    bodyContent = entry.Content;
                }

                if (requestPage.SiteConfig.ApplyContentFiltersToWeb)
                {
                    bodyContent = SiteUtilities.FilterContent(entry.EntryId, bodyContent);
                }

                if (isDescription)
                {
                    bodyContent += String.Format("<div class=\"itemReadMoreStyle\"><a href=\"{0}\" rel=\"bookmark\">{1}</a></div>", SiteUtilities.GetPermaLinkUrl(requestPage.SiteConfig, (ITitledEntry)entry), requestPage.CoreStringTables.GetString("text_read_more"));
                }

                if (!requestPage.HideAdminTools && entry.Language != null && entry.Language.Length > 0)
                {
                    CultureInfo ci = CultureInfo.InvariantCulture;
                    foreach (CultureInfo sci in CultureInfo.GetCultures(CultureTypes.AllCultures))
                    {
                        if (sci.Name.ToUpper() == entry.Language.ToUpper())
                        {
                            ci = sci;
                            break;
                        }
                    }
                    bodyContent = String.Format("<p style=\"color:red;font-size:smaller\">{0} {1}/{2} ({3})<p>{4}", requestPage.CoreStringTables.GetString("text_item_lang_note"), ci.EnglishName, ci.NativeName, entry.Language, bodyContent);
                }
                if (!requestPage.HideAdminTools && (entry.Categories == null || entry.Categories.Trim().Length == 0) && requestPage.SiteConfig.FrontPageCategory != null && requestPage.SiteConfig.FrontPageCategory.Length > 0)
                {
                    bodyContent = "<p style=\"color:red;font-size:smaller\">" + requestPage.CoreStringTables.GetString("text_item_cat_note") + "<p>" + bodyContent;
                }

                return new LiteralControl(bodyContent);
            }
        }

        /// <summary>
        /// The title of a single weblog entry.
        /// Corresponds to the macro: &lt;%itemTitle%&gt;
        /// </summary>
        /// <remarks>
        /// The output is plain text.
        /// <P/>
        /// To de/activate the content filter mechanism edit the tag
        /// &lt;ApplyContentFiltersToWeb&gt; in the SiteConfig file of the Web application.
        /// </remarks>
        public virtual Control ItemTitle
        {
            get
            {
                Control control;
                string title = entry.Title;

                if (requestPage.SiteConfig.EntryTitleAsLink || entry.Link != null)
                {
                    HyperLink hl = new HyperLink();
                    if (entry.Link != null)
                    {
                        hl.NavigateUrl = entry.Link;
                    }
                    else
                    {
                        hl.NavigateUrl = SiteUtilities.GetPermaLinkUrl(entry);
                        // only add the rel="bookmark" when it's actually a permalink
                        hl.Attributes.Add("rel", "bookmark");
                    }
                    hl.Text = entry.Title;
                    hl.CssClass = "TitleLinkStyle";
                    control = hl;
                }
                else
                {
                    if (requestPage.SiteConfig.ApplyContentFiltersToWeb)
                    {
                        control = new LiteralControl(SiteUtilities.FilterContent(entry.EntryId, title));
                    }
                    else
                    {
                        control = new LiteralControl(entry.Title);
                    }
                }
                return control;
            }
        }

        #region GeoRSS
        public virtual Control Latitude
        {
            get
            {
                Control control = null;

                if (requestPage.SiteConfig.EnableGeoRss)
                {
                    Nullable<double> latitude = new Nullable<double>();
                    if (entry.Latitude.HasValue)
                    {
                        latitude = entry.Latitude;
                    }
                    else
                    {
                        if (requestPage.SiteConfig.EnableDefaultLatLongForNonGeoCodedPosts)
                        {
                            latitude = requestPage.SiteConfig.DefaultLatitude;
                        }
                    }

                    if (latitude.HasValue)
                    {
                        control = new LiteralControl(String.Format(CultureInfo.InvariantCulture, "{0}", latitude));
                    }
                }

                return control;
            }
        }

        public virtual Control Longitude
        {
            get
            {
                Control control = null;

                if (requestPage.SiteConfig.EnableGeoRss)
                {
                    Nullable<double> longitude = new Nullable<double>();
                    if (entry.Longitude.HasValue)
                    {
                        longitude = entry.Longitude;
                    }
                    else
                    {
                        if (requestPage.SiteConfig.EnableDefaultLatLongForNonGeoCodedPosts)
                        {
                            longitude = requestPage.SiteConfig.DefaultLongitude;
                        }
                    }

                    if (longitude.HasValue)
                    {
                        control = new LiteralControl(String.Format(CultureInfo.InvariantCulture, "{0}", longitude));
                    }
                }

                return control;
            }
        }

        public virtual Control LatitudeLongitude(string prefix, string latLongSeparator, string suffix)
        {
            Control control = null;

            if (requestPage.SiteConfig.EnableGeoRss)
            {
                Nullable<double> latitude = new Nullable<double>();
                Nullable<double> longitude = new Nullable<double>();

                if (entry.Latitude.HasValue)
                {
                    latitude = entry.Latitude;
                }
                else
                {
                    if (requestPage.SiteConfig.EnableDefaultLatLongForNonGeoCodedPosts)
                    {
                        latitude = requestPage.SiteConfig.DefaultLatitude;
                    }
                }

                if (entry.Longitude.HasValue)
                {
                    longitude = entry.Longitude;
                }
                else
                {
                    if (requestPage.SiteConfig.EnableDefaultLatLongForNonGeoCodedPosts)
                    {
                        longitude = requestPage.SiteConfig.DefaultLongitude;
                    }
                }

                if (latitude.HasValue && longitude.HasValue)
                {
                    control = new LiteralControl(String.Format(CultureInfo.InvariantCulture, "{0}{1}{2}{3}{4}", prefix, latitude, latLongSeparator, longitude, suffix));
                }
            }

            return control;
        }
        #endregion

        public Control ItemTitleRaw
        {
            get
            {
                return new LiteralControl(entry.Title);
            }
        }

        /// <summary>
        /// The Description of a single weblog entry.
        /// Corresponds to the macro: &lt;%itemDescription%&gt;
        /// </summary>
        /// <remarks>
        /// The output is plain text.
        /// <P/>
        /// To de/activate the content filter mechanism edit the tag
        /// &lt;ApplyContentFiltersToWeb&gt; in the SiteConfig file of the Web application.
        /// </remarks>
        public virtual Control ItemDescription
        {
            get
            {
                if (requestPage.SiteConfig.ApplyContentFiltersToWeb)
                {
                    return new LiteralControl(SiteUtilities.FilterContent(entry.EntryId, entry.Description));
                }
                else
                {
                    return new LiteralControl(entry.Description);
                }
            }
        }

        /// <summary>
        /// The Author of a single weblog entry.
        /// Corresponds to the macro: &lt;%itemAuthor%&gt;
        /// </summary>
        /// <remarks>
        /// The output is plain text.
        /// </remarks>
        public virtual Control ItemAuthor
        {
            get
            {
                User user = SiteSecurity.GetUser(entry.Author);
                if (user != null)
                {
                    if (user.EmailAddress != null && user.EmailAddress.Length > 0)
                    {
                        StringBuilder builder = new StringBuilder();
                        builder.Append("<a href=\"");
                        if (requestPage.SiteConfig.ObfuscateEmail)
                        {
                            builder.Append(SiteUtilities.GetObfuscatedEmailUrl(user.EmailAddress, entry.Title.Replace("'", "\\'")));
                        }
                        else
                        {
                            builder.Append("mailto:");
                            builder.Append(user.EmailAddress);
                        }
                        builder.Append("\">");
                        if (user.DisplayName != null && user.DisplayName.Length > 0)
                        {
                            builder.Append(user.DisplayName);
                        }
                        else
                        {
                            builder.Append(entry.Author);
                        }
                        builder.Append("</a>");
                        return new LiteralControl(builder.ToString());
                    }
                }

                return new LiteralControl(entry.Author);
            }
        }

        /// <summary>
        /// The Author of a single weblog entry.
        /// Corresponds to the macro: &lt;%itemAuthorNoLink%&gt;
        /// </summary>
        /// <remarks>
        /// The output is plain text.
        /// </remarks>
        public virtual Control ItemAuthorNoLink
        {
            get
            {
                User user = SiteSecurity.GetUser(entry.Author);
                if (user != null && user.DisplayName != null && user.DisplayName.Length > 0)
                {
                    return new LiteralControl(user.DisplayName);
                }

                return new LiteralControl(entry.Author);
            }
        }

        public virtual Control PreviousLink(string previousNavigationText)
        {
            return PreviousLink(previousNavigationText, int.MaxValue);
        }

        /// <summary>
        /// Returns a Hyperlink control linking to the previous entry to this one.  
        /// The macro is of the form &lt;%previousLink("[Your String Here]",[Max Length of Title])%&gt;
        /// </summary>
        /// <param name="previousNavigationText">Whatever text should prefix the previous entry title
        /// (for example, double left angle brackets.)</param>
        /// <remarks>The text of this link is
        /// of the form "[previousNavigationText][Entry Title]" (spaces are inserted at the choice of the user).
        /// This string is not HTML-encoded so as to allow the user to insert HTML 
        /// blocks (e.g. images, etc.) in place of text if they so choose.</remarks>
        public virtual Control PreviousLink(string previousNavigationText, int maxLength)
        {
            // We only try to create a previous link in a situation where the user has loaded
            // a page with only one entry (i.e., a specific link), because this is where previous
            // and next links come in handy.
            // If the user, for example, is on the front page or a category page, the "previous" and "next" links
            // have no real point.
            if (this.requestPage.WeblogEntries.Count == 1)
            {
                EntryCollection cachedEntryCollection = this.requestPage.DataService.GetEntries(false);

                // Get our current entry from the cache.
                Entry ourCachedEntry = cachedEntryCollection[entry.EntryId];
                int ourCachedEntryIndex = cachedEntryCollection.IndexOf(ourCachedEntry);

                // The data service is sorted in reverse order, so to get the "next post" we need to look at 
                // the entry with an index 1 lower than our current entry.
                if (ourCachedEntryIndex < cachedEntryCollection.Count - 1)
                {
                    int previousEntryIndex = ourCachedEntryIndex + 1;
                    Entry previousEntry = null;
                    // The dataservice will not retrieve entries that have not been published.  So, if we 
                    // grab an id of an entry that hasn't been published yet, we need to go further up the list
                    // to get the previous link.  If there are no previous posts that have been published, there's
                    // no need to render the previous control.
                    do
                    {
                        previousEntry = cachedEntryCollection[previousEntryIndex];
                        ++previousEntryIndex;
                    } while (previousEntry == null && previousEntryIndex < cachedEntryCollection.Count - 1);

                    if (previousEntry != null)
                    {
                        HyperLink previousLink = new HyperLink();
                        previousLink.Text = previousNavigationText + TruncateString(previousEntry.Title, maxLength);
                        if (previousEntry.Link != null)
                        {
                            previousLink.NavigateUrl = previousEntry.Link;
                        }
                        else
                        {
                            previousLink.NavigateUrl = SiteUtilities.GetPermaLinkUrl(previousEntry);
                        }
                        return previousLink;
                    }
                }
            }
            return new LiteralControl("");
        }

        /// <summary>
        /// Returns a separator string.  This would typically be used in-between a previous entry link and a next
        /// entry link, but could be used for any purpose on a permalink page that is not the earliest or most
        /// recent post.
        /// Takes the format &lt;%previousNextSeparator("[your string here]")%&gt;.  This string is not HTML-encoded so
        /// as to allow the user to insert HTML blocks (e.g. images, etc.) in place of text if they so choose.
        /// </summary>
        /// <param name="separatorText">Text that will be displayed as the separator.</param>
        /// <returns></returns>
        public virtual Control PreviousNextSeparator(string separatorText)
        {
            string separator = "";
            if (this.requestPage.WeblogEntries.Count == 1 && separatorText != null)
            {
                EntryCollection cachedEntryCollection = this.requestPage.DataService.GetEntries(false);

                // Check if our entry is either the very first entry or the very last entry, 
                // in which case no separator is needed for display.  

                Entry currentEntry = cachedEntryCollection[entry.EntryId];
                int currentCachedEntryIndex = cachedEntryCollection.IndexOf(currentEntry);

                if (currentCachedEntryIndex > 0 &&
                        currentCachedEntryIndex < cachedEntryCollection.Count - 1)
                {
                    separator = separatorText;
                }
            }
            return new LiteralControl(separator);
        }

        /// <summary>
        /// Returns a link back to the main site.  This is pretty much the same as the site name link for the main 
        /// page macros - the only difference is that this is only displayed if we are on a single entry.
        /// </summary>
        /// <param name="pageLinkText"></param>
        /// <returns></returns>
        public virtual Control MainPageEntryLink(string pageLinkText)
        {
            if (this.requestPage.WeblogEntries.Count == 1)
            {
                return new LiteralControl(String.Format("<a href=\"{0}\" rel=\"home\">{1}</a>", SiteUtilities.GetStartPageUrl(requestPage.SiteConfig), pageLinkText));
            }
            return new LiteralControl("");
        }

        /// <summary>
        /// Returns a link back to the main site.  This is pretty much the same as the site name link for the main 
        /// page macros - the only difference is that this is only displayed if we are on a single entry.
        /// 
        /// This overload will take in an option seperator so taht you can pass in ("Main", "|") and it will
        /// render " | <a href="">Main</a> | " without hyperlinking the sepererator.
        /// </summary>
        /// <param name="pageLinkText"></param>
        /// <param name="pageLinkSeperator">The separator you wish to use like "|"</param>
        /// <returns></returns>
        public virtual Control MainPageEntryLink(string pageLinkText, string pageLinkSeperator)
        {
            if (this.requestPage.WeblogEntries.Count == 1)
            {
                return new LiteralControl(String.Format(" {2} <a href=\"{0}\" rel=\"home\">{1}</a> {2} ", SiteUtilities.GetStartPageUrl(requestPage.SiteConfig), pageLinkText, pageLinkSeperator));
            }
            return new LiteralControl("");
        }

        /// <summary>
        /// Returns a link to the most recent entry if we are on a single entry page.
        /// </summary>
        /// <param name="mostRecentLinkText"></param>
        /// <returns></returns>
        public virtual Control MostRecentEntryLink(string mostRecentLinkText)
        {
            if (this.requestPage.WeblogEntries.Count == 1 && mostRecentLinkText != null)
            {
                EntryCollection cachedEntryCollection = this.requestPage.DataService.GetEntries(false);

                // Grab the first entry out of the cache - this is our most recent post.

                int mostRecentIndex = 0;
                Entry mostRecentEntry = null;
                // Non-published entries don't get returned by the data service.  If our most recent entry in
                // the cache is non-published, keep moving until we find one that is.
                do
                {
                    mostRecentEntry = cachedEntryCollection[mostRecentIndex];
                    ++mostRecentIndex;
                } while (mostRecentEntry == null && mostRecentIndex < cachedEntryCollection.Count);

                if (mostRecentEntry != null)
                {
                    return new LiteralControl(String.Format("<a href=\"{0}\">{1}</a>",
                                    SiteUtilities.GetPermaLinkUrl(mostRecentEntry), mostRecentLinkText));
                }
            }
            return new LiteralControl("");
        }

        public virtual Control OnPagePreviousLink(string navigationText)
        {
            return OnPagePreviousLink(navigationText, "previous", null);
        }

        public virtual Control OnPagePreviousLink(string navigationText, string removeUrlFragmentRegex)
        {
            return OnPagePreviousLink(navigationText, "previous", removeUrlFragmentRegex);
        }

        public virtual Control OnPagePreviousLink(string navigationText, string cssClass, string removeUrlFragmentRegex)
        {
            if (requestPage.WeblogEntries.Count <= 1)
            {
                return new LiteralControl();
            }

            int currentIndex = requestPage.WeblogEntries.IndexOf(entry);
            if (currentIndex < requestPage.WeblogEntries.Count - 1)
            {
                Entry previousEntry = requestPage.WeblogEntries[currentIndex + 1];

                HyperLink previousLink = new HyperLink();
                previousLink.Controls.Add(LocalString(navigationText));
                previousLink.NavigateUrl = String.Format("{0}#a{1}", GetClientUrl(HttpContext.Current.Request.RawUrl, removeUrlFragmentRegex), previousEntry.EntryId);
                previousLink.CssClass = cssClass;
                return previousLink;
            }

            return new LiteralControl();
        }

        public virtual Control OnPageNextLink(string navigationText)
        {
            return OnPageNextLink(navigationText, "next", null);
        }

        public virtual Control OnPageNextLink(string navigationText, string removeUrlFragmentRegex)
        {
            return OnPageNextLink(navigationText, "next", removeUrlFragmentRegex);
        }

        public virtual Control OnPageNextLink(string navigationText, string cssClass, string removeUrlFragmentRegex)
        {
            if (requestPage.WeblogEntries.Count <= 1)
            {
                return new LiteralControl();
            }

            int currentIndex = requestPage.WeblogEntries.IndexOf(entry);
            if (currentIndex > 0)
            {
                Entry nextEntry = requestPage.WeblogEntries[currentIndex - 1];

                HyperLink nextLink = new HyperLink();
                nextLink.Controls.Add(LocalString(navigationText));
                nextLink.NavigateUrl = String.Format("{0}#a{1}", GetClientUrl(HttpContext.Current.Request.RawUrl, removeUrlFragmentRegex), nextEntry.EntryId);
                nextLink.CssClass = cssClass;
                return nextLink;
            }

            return new LiteralControl();
        }

        static string GetClientUrl(string url, string removeUrlFragmentRegex)
        {
            if (!String.IsNullOrEmpty(removeUrlFragmentRegex))
            {
                url = Regex.Replace(url,
                                    removeUrlFragmentRegex,
                                    String.Empty,
                                    RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.IgnoreCase);
            }

            return url;
        }

        /// <summary>
        /// Returns a Hyperlink control linking to the following entry beyond this one.  
        /// The macro is of the form &lt;%nextLink("[Your String Here]")%&gt;  
        /// </summary>  
        /// <param name="nextNavigationText">Whatever text should be appended to the next entry title 
        /// (for example, double right angle brackets.)</param>
        /// <remarks>The text of this link is
        /// of the form "[Entry Title][NextNavigationText]" (spaces are inserted at the choice of the user).
        /// This string is not HTML-encoded so as to allow the user to insert HTML 
        /// blocks (e.g. images, etc.) in place of text if they so choose. 
        /// </remarks>
        public virtual Control NextLink(string nextNavigationText, int maxLength)
        {
            // We only try to create a previous link in a situation where the user has loaded
            // a page with only one entry (i.e., a specific link), because this is where previous
            // and next links come in handy.
            // If the user, for example, is on the front page or a category page, the "previous" and "next" links
            // have no real point.
            if (this.requestPage.WeblogEntries.Count == 1)
            {
                // We subtract 5 seconds from the entry time, as otherwise OccursBefore will retrieve
                // our current entry in addition to the entries before it.
                EntryCollection cachedEntryCollection = this.requestPage.DataService.GetEntries(false);

                // Get our current entry from the cache.
                Entry ourCachedEntry = cachedEntryCollection[entry.EntryId];
                int ourCachedEntryIndex = cachedEntryCollection.IndexOf(ourCachedEntry);
                Entry nextEntry = null;

                // The data service is sorted in reverse order, so to get the "next post" we need to look at 
                // the entry with an index 1 lower than our current entry.
                if (ourCachedEntryIndex > 0)
                {
                    int nextCachedEntryIndex = ourCachedEntryIndex - 1;

                    // The dataservice will not retrieve entries that have not been published.  So, if we 
                    // grab an id of an entry that hasn't been published yet, we need to go further up the list
                    // to get the next link.  If there are no next posts that have been published, there's
                    // no need to render the next link control.
                    do
                    {
                        nextEntry = cachedEntryCollection[nextCachedEntryIndex];
                        --nextCachedEntryIndex;
                    } while (nextEntry == null &&
                        nextCachedEntryIndex >= 0);

                    if (nextEntry != null)
                    {
                        HyperLink nextLink = new HyperLink();
                        nextLink.Text = TruncateString(nextEntry.Title, maxLength) + nextNavigationText;
                        if (nextEntry.Link != null)
                        {
                            nextLink.NavigateUrl = nextEntry.Link;
                        }
                        else
                        {
                            nextLink.NavigateUrl = SiteUtilities.GetPermaLinkUrl(nextEntry);
                        }
                        return nextLink;
                    }
                }
            }
            return new LiteralControl("");
        }


        #region Date/time macros
        /// <summary>
        /// The DateTime (formatted like DateTime.Now) the entry was created.
        /// Corresponds to the macro: &lt;%when%&gt;
        /// </summary>
        /// <remarks>
        /// The output is plain text.
        /// </remarks>
        public virtual Control When
        {
            get
            {
                if (requestPage.SiteConfig.AdjustDisplayTimeZone)
                {
                    return new LiteralControl(requestPage.SiteConfig.GetConfiguredTimeZone().FormatAdjustedUniversalTime(entry.CreatedUtc));
                }
                else
                {
                    return new LiteralControl(entry.CreatedUtc.ToString("U") + " UTC");
                }
            }
        }

        /// <summary>
        /// The DateTime (formatted like DateTime.Now) the entry was modified.
        /// Corresponds to the macro: &lt;%modified%&gt;
        /// </summary>
        /// <remarks>
        /// The output is plain text.
        /// </remarks>
        public virtual Control Modified
        {
            get
            {
                if (DateTime.Compare(entry.CreatedUtc, entry.ModifiedUtc) == 0)
                {
                    return new LiteralControl();
                }

                if (requestPage.SiteConfig.AdjustDisplayTimeZone)
                {
                    return
                        new LiteralControl(requestPage.SiteConfig.GetConfiguredTimeZone().FormatAdjustedUniversalTime(entry.ModifiedUtc));
                }
                else
                {
                    return new LiteralControl(entry.ModifiedUtc.ToString("U") + " UTC");
                }
            }
        }

        /// <summary>
        /// This macro injects a bit of JavaScript into the output stream that will render
        /// the entry's time in the user's browser using his/her local timezone.
        /// </summary>
        public virtual Control ReaderWhen
        {
            get
            {
                string controlString = String.Format("<script language=\"javascript\">fmtDate = new Date();fmtDate.setTime(Date.parse('{0}'));" + "document.write( fmtDate.toLocaleString() )</script>", entry.CreatedUtc.ToString("r", CultureInfo.InvariantCulture));
                return new LiteralControl(controlString);
            }
        }

        /// <summary>
        /// Returns a formatted DateTime as specified by the format parameter.
        /// Corresponds to the macro: &lt;%formattedWhen(...)%&gta and &lt;%formattedModified(...)%&gt;
        /// </summary>
        /// <remarks>
        /// The output is plain text.
        /// </remarks>
        private Control FormattedDate(string format, bool addTimezone, DateTime date)
        {
            DateTime dateValue;
            string name;

            if (requestPage.SiteConfig.AdjustDisplayTimeZone)
            {
                WindowsTimeZone tz = requestPage.SiteConfig.GetConfiguredTimeZone();
                dateValue = tz.ToLocalTime(date);
                if (tz.IsDaylightSavingTime(dateValue))
                {
                    name = tz.DaylightName;
                }
                else
                {
                    name = tz.StandardName;
                }
            }
            else
            {
                dateValue = date;
                name = "UTC";
            }
            if (addTimezone)
            {
                return new LiteralControl(string.Format("{0} {1}", dateValue.ToString(format), name));
            }
            else
            {
                return new LiteralControl(string.Format("{0}", dateValue.ToString(format)));
            }
        }

        public virtual Control FormattedWhen(string format)
        {
            return FormattedDate(format, true, entry.CreatedUtc);
        }

        public virtual Control FormattedWhenBare(string format)
        {
            return FormattedDate(format, false, entry.CreatedUtc);
        }

        public virtual Control FormattedModified(string format)
        {
            if (DateTime.Compare(entry.CreatedUtc, entry.ModifiedUtc) == 0)
            {
                return new LiteralControl();
            }

            return FormattedDate(format, true, entry.ModifiedUtc);
        }

        public virtual Control FormattedModifiedBare(string format)
        {
            if (DateTime.Compare(entry.CreatedUtc, entry.ModifiedUtc) == 0)
            {
                return new LiteralControl();
            }

            return FormattedDate(format, false, entry.ModifiedUtc);
        }

        public virtual Control IfModified(string expression)
        {
            if (DateTime.Compare(entry.CreatedUtc, entry.ModifiedUtc) == 0)
            {
                return new LiteralControl();
            }

            return LocalString(expression);
        }
        #endregion

        /// <summary>
        /// A placeholder for rendering an HTML hyperlink for each single weblog entry.
        /// Corresponds to the macro: &lt;%permalink%&gt;
        /// </summary>
        /// <remarks>
        /// The output is HTML.
        /// </remarks>
        public virtual Control Permalink
        {
            get
            {
                PlaceHolder placeHolder = new PlaceHolder();
                HyperLink hl = new HyperLink();
                hl.CssClass = "permalinkStyle";
                Image img = new Image();
                img.ImageUrl = new Uri(new Uri(SiteUtilities.GetBaseUrl(requestPage.SiteConfig)), requestPage.GetThemedImageUrl("itemLink")).ToString();
                img.CssClass = "permalinkImageStyle";
                img.ToolTip = requestPage.CoreStringTables.GetString("text_permalink");
                img.AlternateText = "#";
                hl.Controls.Add(img);
                hl.NavigateUrl = SiteUtilities.GetPermaLinkUrl(entry);
                hl.Attributes.Add("rel", "bookmark");
                placeHolder.Controls.Add(hl);
                return placeHolder;
            }
        }

        /// <summary>
        /// A placeholder for rendering a formatted, possibly title-based permalink for each single weblog entry.
        /// Corresponds to the macro: &lt;%permalinkUrl%&gt;
        /// </summary>
        /// <remarks>
        /// The output is raw text.
        /// </remarks>
        public virtual Control PermalinkUrl
        {
            get { return new LiteralControl(SiteUtilities.GetPermaLinkUrl(entry)); }
        }

        /// <summary>
        /// A placeholder for rendering a raw permalink for each single weblog entry.
        /// Corresponds to the macro: &lt;%permalinkUrlRaw%&gt;
        /// </summary>
        /// <remarks>
        /// The output is raw text, url encoded
        /// </remarks>
        public virtual Control PermalinkUrlRaw
        {
            get { return new LiteralControl(SiteUtilities.GetPermaLinkUrl(entry)); }
        }


        /// <summary>
        /// The unique guid of the item.
        /// Corresponds to the macro: &lt;%itemGuid%&gt;
        /// </summary>
        /// <remarks>
        /// The output is raw text.
        /// </remarks>
        public virtual Control ItemGuid
        {
            get { return new LiteralControl(entry.EntryId); }
        }

        /// <summary>
        /// An HTML hyperlink to view comments on weblog entries.
        /// Corresponds to the macro: &lt;%commentlink%&gt;
        /// </summary>
        /// <remarks>
        /// The output is HTML.
        /// <P/>
        /// A counter for counting comments can be de/activated through
        /// &lt;ShowCommentCount&gt; in the SiteConfig file of the Web application.
        /// </remarks>
        public virtual Control Commentlink
        {
            get
            {
                if (requestPage.SiteConfig.EnableComments)
                {
                    if (requestPage.SiteConfig.ShowCommentCount)
                    {
                        bool allComments = SiteSecurity.IsValidContributor();
                        CommentCollection comments = requestPage.DataService.GetCommentsFor(entry.EntryId, allComments);
                        HyperLink hl = new HyperLink();
                        hl.CssClass = "commentLinkStyle";
                        hl.NavigateUrl = SiteUtilities.GetCommentViewUrl(entry.EntryId) + "#commentstart";
                        hl.Text = requestPage.CoreStringTables.GetString("text_comments") + " [" + comments.Count.ToString() + "]";
                        return hl;
                    }
                    else
                    {
                        HyperLink hl = new HyperLink();
                        hl.CssClass = "commentLinkStyle";
                        hl.NavigateUrl = SiteUtilities.GetCommentViewUrl(entry.EntryId) + "#commentstart";
                        hl.Text = requestPage.CoreStringTables.GetString("text_comments");
                        return hl;
                    }
                }
                else
                {
                    return new PlaceHolder();
                }
            }
        }

        /// <summary>
        /// An HTML hyperlink.
        /// Corresponds to the macro: &lt;%trackbackLink%&gt;
        /// </summary>
        /// <remarks>
        /// The output is HTML hyperlink.
        /// </remarks>
        public virtual Control Trackbacklink
        {
            get
            {
                if (requestPage.SiteConfig.EnableTrackbackService)
                {
                    HyperLink hl = new HyperLink();
                    hl.CssClass = "trackbackLinkStyle";
                    hl.NavigateUrl = SiteUtilities.GetTrackbackUrl(entry.EntryId);
                    hl.ToolTip = requestPage.CoreStringTables.GetString("tooltip_trackback");
                    hl.Text = requestPage.CoreStringTables.GetString("text_trackback");
                    return hl;
                }
                return new LiteralControl(String.Empty);
            }
        }

        /// <summary>
        /// A placeholder to render a list of hyperlinks.
        /// Corresponds to the macro: &lt;%trackbackList%&gt;
        /// </summary>
        /// <remarks>
        /// The output is HTML (hyperlinks). Does not include Referrals. To render Referrals see <see cref="ReferralList"/>
        /// </remarks>
        public virtual Control TrackbackList
        {
            get
            {
                PlaceHolder placeHolder = new PlaceHolder();

                if (requestPage.ShowTrackingDetail)
                {
                    TrackingCollection trackings = requestPage.DataService.GetTrackingsFor(entry.EntryId);

                    if (trackings.Count > 0)
                    {
                        HtmlGenericControl trkSpan = new HtmlGenericControl("span");

                        placeHolder.Controls.Add(trkSpan);
                        trkSpan.Controls.Add(new LiteralControl(requestPage.CoreStringTables.GetString("text_trackings") + "<br />"));

                        foreach (Tracking trk in trackings)
                        {
                            if (trk.TrackingType == TrackingType.Pingback | trk.TrackingType == TrackingType.Trackback)
                            {
                                HyperLink hl = new HyperLink();
                                hl.CssClass = "trackbackLinkStyle";
                                hl.NavigateUrl = trk.PermaLink;
                                if (trk.RefererTitle != null && trk.RefererTitle.Length > 0 && trk.RefererBlogName != null && trk.RefererBlogName.Length > 0)
                                {
                                    hl.Text = String.Format("\"{0}\" ({1})", trk.RefererTitle, trk.RefererBlogName);
                                }
                                else if (trk.RefererBlogName != null && trk.RefererBlogName.Length > 0)
                                {
                                    hl.Text = trk.RefererBlogName;
                                }
                                else if (trk.RefererTitle != null && trk.RefererTitle.Length > 0)
                                {
                                    hl.Text = trk.RefererTitle;
                                }
                                else
                                {
                                    hl.Text = trk.PermaLink;
                                }

                                // Referral links can get very long, so clip them.
                                hl.Text = SiteUtilities.ClipString(hl.Text, 80);

                                trkSpan.Controls.Add(hl);

                                // allow the user to delete the trackback
                                if (!requestPage.HideAdminTools && SiteSecurity.IsInRole("admin"))
                                {
                                    // need to url encode the url
                                    string url = trk.PermaLink.Replace("&", "%26");

                                    hl = new HyperLink();
                                    hl.CssClass = "deleteLinkStyle";
                                    Image img = new Image();
                                    img.CssClass = "deleteLinkImageStyle";
                                    img.ImageUrl = new Uri(new Uri(SiteUtilities.GetBaseUrl(requestPage.SiteConfig)), requestPage.GetThemedImageUrl("deletebutton")).ToString();
                                    img.BorderWidth = 0;
                                    hl.Controls.Add(img);
                                    hl.NavigateUrl = String.Format("javascript:deleteReferral('{0}', '{1}', '{2}')",
                                        entry.EntryId,
                                        url,
                                        trk.TrackingType == TrackingType.Trackback ? ("Trackback") : ("Pingback"));
                                    trkSpan.Controls.Add(hl);
                                }

                                trkSpan.Controls.Add(new LiteralControl("&nbsp;[" + trk.TrackingType.ToString() + "]<br />"));
                            }
                        }

                        if (trkSpan.Controls.Count > 1) return placeHolder;
                        else return null;
                    }
                }

                return null;
            }
        }

        /// <summary>
        /// Returns the list of trackbacks embedded in a div element.
        /// </summary>
        public virtual Control TrackbackListDiv(string divClass)
        {
            Control trackbacks = this.TrackbackList;
            if (trackbacks == null)
            {
                return new LiteralControl();
            }

            HtmlGenericControl div = new HtmlGenericControl("div");
            div.Controls.Add(trackbacks);

            if (divClass != null && divClass.Length > 0)
            {
                div.Attributes.Add("class", divClass);
            }

            return div;
        }

        /// <summary>
        /// A placeholder to render a list of hyperlinks.
        /// Corresponds to the macro: &lt;%trackbackList%&gt;
        /// </summary>
        /// <remarks>
        /// The output is HTML (hyperlinks).
        /// </remarks>
        public virtual Control ReferralList
        {
            get
            {
                return ReferralListFiltered(Int32.MaxValue, false, false);
            }
        }

        public virtual Control ReferralListFiltered(int maxReferrals, bool excludeOnlineAggregators, bool excludeSearchEngines)
        {
            PlaceHolder placeHolder = new PlaceHolder();

            if (requestPage.ShowTrackingDetail && requestPage.SiteConfig.EnableEntryReferrals)
            {
                TrackingCollection trackings = requestPage.DataService.GetTrackingsFor(entry.EntryId);

                if (trackings.Count > 0)
                {
                    placeHolder.Controls.Add(new LiteralControl("<br />"));
                    HtmlGenericControl referralSpan = new HtmlGenericControl("span");
                    HtmlGenericControl referralSpanHidden = new HtmlGenericControl("span");
                    referralSpanHidden.Style.Add("display", "none");
                    referralSpanHidden.ID = "referralSpanHidden";

                    HtmlAnchor anchor = new HtmlAnchor();
                    anchor.HRef = "referralList";

                    placeHolder.Controls.Add(referralSpan);
                    referralSpan.Controls.Add(anchor);
                    referralSpan.Controls.Add(new LiteralControl("Referred by:<br />"));
                    referralSpan.Controls.Add(referralSpanHidden);

                    int count = 0;
                    foreach (Tracking trk in trackings)
                    {
                        if (trk.TrackingType == TrackingType.Referral | trk.TrackingType == TrackingType.Unknown)
                        {
                            // do not render a referral if it's in the filtered list
                            if (excludeOnlineAggregators && trk.PermaLink.IndexOf("bloglines.com") > -1 | trk.PermaLink.IndexOf("newsgator.com") > -1)
                            {
                                continue;
                            }
                            if (excludeSearchEngines && trk.PermaLink.IndexOf("q=") > -1 | trk.PermaLink.IndexOf("p=") > -1)
                            {
                                continue;
                            }

                            // if this is a referral from a search engine, make it PrettyPrint
                            HyperLink hl = SiteUtilities.ParseSearchString(trk.PermaLink);

                            if (hl == null)
                            {
                                hl = new HyperLink();
                                hl.NavigateUrl = trk.PermaLink;
                                if (trk.RefererTitle != null && trk.RefererTitle.Length > 0 && trk.RefererBlogName != null && trk.RefererBlogName.Length > 0)
                                {
                                    hl.Text = String.Format("\"{0}\" ({1})", trk.RefererTitle, trk.RefererBlogName);
                                }
                                else if (trk.RefererBlogName != null && trk.RefererBlogName.Length > 0)
                                {
                                    hl.Text = trk.RefererBlogName;
                                }
                                else if (trk.RefererTitle != null && trk.RefererTitle.Length > 0)
                                {
                                    hl.Text = trk.RefererTitle;
                                }
                                else
                                {
                                    hl.Text = trk.PermaLink;
                                }

                                // Referral links can get very long, so clip them.
                                hl.Text = SiteUtilities.ClipString(hl.Text, 60);
                            }

                            hl.CssClass = "trackbackLinkStyle";
                            hl.Attributes.Add("rel", "nofollow");

                            // allow the user to delete the referral
                            HyperLink deleteHyperlink = null;
                            if (!requestPage.HideAdminTools && SiteSecurity.IsInRole("admin"))
                            {
                                // need to url encode the url
                                string url = HttpUtility.UrlEncode(trk.PermaLink);
                                deleteHyperlink = new HyperLink();
                                deleteHyperlink.CssClass = "deleteLinkStyle";
                                Image img = new Image();
                                img.CssClass = "deleteLinkImageStyle";
                                img.ImageUrl = new Uri(new Uri(SiteUtilities.GetBaseUrl(requestPage.SiteConfig)), requestPage.GetThemedImageUrl("deletebutton")).ToString();
                                img.BorderWidth = 0;
                                deleteHyperlink.Controls.Add(img);
                                deleteHyperlink.NavigateUrl = String.Format("javascript:deleteReferral('{0}', '{1}', 'Referral')", entry.EntryId, url);
                            }

                            if (count > maxReferrals)
                            {
                                referralSpanHidden.Controls.Add(hl);
                                if (deleteHyperlink != null) referralSpanHidden.Controls.Add(deleteHyperlink);
                                referralSpanHidden.Controls.Add(new LiteralControl("&nbsp;[" + trk.TrackingType.ToString() + "]<br />"));
                            }
                            else
                            {
                                referralSpan.Controls.Add(hl);
                                if (deleteHyperlink != null) referralSpan.Controls.Add(deleteHyperlink);
                                referralSpan.Controls.Add(new LiteralControl("&nbsp;[" + trk.TrackingType.ToString() + "]<br />"));
                                count++;
                            }
                        }
                    }

                    if (count > maxReferrals)
                    {
                        //SDH: 2005-01-20. This is going to be a problem, since ASP.NET hasn't realized yet, 
                        // due to the late-bound nature of our Macros, that the span and hyperlink are going
                        // to be within a container control in CommentView. We may want
                        // to change the getElementById() call to search for elements that ENDWITH what we are
                        // looking for. Until then this code below won't work in CommentView.aspx.
                        //OmarS: 2005-01-30. I fixed this by looping through the elements
                        referralSpan.Controls.Add(new LiteralControl("<br />"));
                        HyperLink hl = new HyperLink();
                        referralSpan.Controls.Add(hl);
                        hl.Text = requestPage.CoreStringTables.GetString("text_referrallist_more");
                        hl.ID = "referralMore";
                        //string script = "document.getElementById('"+ referralSpanHidden.ClientID + "').style.display='inline';";
                        //script += "document.getElementById('" + hl.ClientID + "').style.display='none';";

                        hl.NavigateUrl = "javascript:showReferral()";
                        //hl.Attributes.Add("onclick", script);
                    }
                }
            }

            return placeHolder;
        }

        /// <summary>
        /// A placeholder to render a list of hyperlinks.
        /// Corresponds to the macro: &lt;%relatedPostList%&gt;
        /// </summary>
        /// <remarks>
        /// The output is HTML (hyperlinks).
        /// </remarks>
        public virtual Control RelatedPostList
        {
            get
            {
                PlaceHolder placeHolder = new PlaceHolder();

                if (requestPage.ShowTrackingDetail)
                {
                    string[] splitCategories = entry.GetSplitCategories();
                    IBlogDataService dataService = requestPage.DataService;
                    CategoryCacheEntryCollection allCategories = dataService.GetCategories();
                    EntryCollection entries = new EntryCollection();
                    List<string> entryIds = new List<string>();
                    entryIds.Add(entry.EntryId);

                    foreach (string cat in splitCategories)
                    {
                        string displayName = cat;
                        CategoryCacheEntry categoryEntry;
                        if ((categoryEntry = allCategories[cat]) != null)
                        {
                            displayName = categoryEntry.DisplayName;
                        }

                        entries.AddRange(dataService.GetEntriesForCategory(displayName, null));
                    }

                    if (entries.Count > 0)
                    {
                        HtmlGenericControl relatedPosts = new HtmlGenericControl("span");

                        placeHolder.Controls.Add(relatedPosts);
                        relatedPosts.Controls.Add(new LiteralControl(requestPage.CoreStringTables.GetString("text_related_posts") + "<br />"));

                        int count = 0;
                        entries.Sort(new EntrySorter());
                        foreach (Entry relatedPost in entries)
                        {
                            if (entryIds.Contains(relatedPost.EntryId))
                            {
                                continue;
                            }
                            if (count++ > 5)
                            {
                                break;
                            }

                            entryIds.Add(relatedPost.EntryId);

                            HyperLink link = new HyperLink();
                            link.CssClass = "relatedPostLinkStyle";
                            link.Text = relatedPost.Title;
                            if (relatedPost.Link != null)
                            {
                                link.NavigateUrl = relatedPost.Link;
                            }
                            else
                            {
                                link.NavigateUrl = SiteUtilities.GetPermaLinkUrl(relatedPost);
                            }
                            relatedPosts.Controls.Add(link);
                            relatedPosts.Controls.Add(new LiteralControl("<br />"));
                        }

                        return placeHolder;
                    }
                }

                return null;
            }

        }

        /// <summary>
        /// Returns the list of related posts embedded in a div element.
        /// </summary>
        public virtual Control RelatedPostListDiv(string divClass)
        {
            Control related = this.RelatedPostList;
            if (related == null)
            {
                return new LiteralControl();
            }

            HtmlGenericControl div = new HtmlGenericControl("div");
            div.Controls.Add(related);

            if (divClass != null && divClass.Length > 0)
            {
                div.Attributes.Add("class", divClass);
            }

            return div;
        }


        protected virtual Control RenderCategoryLinks(bool asTechnoratiTags)
        {
            PlaceHolder categories = new PlaceHolder();
            string[] splitCategories = entry.GetSplitCategories();
            IBlogDataService dataService = requestPage.DataService;
            CategoryCacheEntryCollection allCategories = dataService.GetCategories();

            foreach (string cat in splitCategories)
            {
                string displayName = cat;
                CategoryCacheEntry categoryEntry;
                if ((categoryEntry = allCategories[cat]) != null)
                {
                    displayName = categoryEntry.DisplayName;
                }

                if (categories.Controls.Count > 0)
                {
                    categories.Controls.Add(new LiteralControl("&nbsp;|&nbsp;"));
                }
                HyperLink category = new HyperLink();
                category.CssClass = "categoryLinkStyle";
                category.Text = displayName;
                if (asTechnoratiTags)
                {
                    category.Attributes.Add("rel", "tag"); //For folks who care like Technorati
                    category.NavigateUrl = "http://www.technorati.com/tag/" + HttpContext.Current.Server.UrlEncode(displayName);
                }
                else
                {
                    category.NavigateUrl = SiteUtilities.GetCategoryViewUrl(cat);
                }
                categories.Controls.Add(category);
            }
            return categories;
        }

        /// <summary>
        /// Renders a list of user-definable hyperlinks.
        /// Corresponds to the macro: &lt;%categoryLinks%&gt;
        /// </summary>
        /// <remarks>
        /// The output is HTML.
        /// <P/>
        /// The list is created depending on the existing weblog entries.
        /// Entries are parsed and categories are generated on-the-fly.
        /// In case there is no entry for a specific category it 'disappears'.
        /// </remarks>
        public virtual Control CategoryLinks
        {
            get
            {
                return RenderCategoryLinks(false);
            }
        }


        /// <summary>
        /// Renders a list of hyperlinks that turn Categories into TechnoratiTags.
        /// Corresponds to the macro: &lt;%TechnoratiTags%&gt;
        /// </summary>
        /// <remarks>
        /// The output is HTML.
        /// <P/>
        /// The list is created depending on the existing weblog entries.
        /// Entries are parsed and categories are generated on-the-fly.
        /// In case there is no entry for a specific category it 'disappears'.
        /// </remarks>
        public virtual Control TechnoratiTags
        {
            get
            {
                return RenderCategoryLinks(true);
            }
        }


        /// <summary>
        /// A placeholder for rendering HTML hyperlinks for each single weblog entry.
        /// Corresponds to the macro: &lt;%editButton%&gt;
        /// </summary>
        /// <remarks>
        /// The output is HTML.
        /// <P/>
        /// If the user IsInRole("admin") then he should be able to edit or delete entries.
        /// In this case there are appropriate hyperlinked images rendered to the page.
        /// </remarks>
        public virtual Control EditButton
        {
            get
            {
                PlaceHolder placeHolder = new PlaceHolder();
                if (!requestPage.HideAdminTools &&
                    // enable edit and delete buttons under the following conditions:
                    // the user is an administrator, or
                    // the user is a contributor and they wrote the entry
                    // (contributors should not have access to entries they didn't write!!)
                    (SiteSecurity.IsInRole("admin") ||
                         (SiteSecurity.IsInRole("contributor") &&
                          HttpContext.Current.User.Identity.Name == entry.Author)))
                {
                    HyperLink hl = new HyperLink();
                    hl.CssClass = "editLinkStyle";
                    Image img = new Image();
                    img.CssClass = "editLinkImageStyle";
                    img.ImageUrl = new Uri(new Uri(SiteUtilities.GetBaseUrl(requestPage.SiteConfig)), requestPage.GetThemedImageUrl("editbutton")).ToString();
                    img.BorderWidth = 0;
                    img.AlternateText = HttpUtility.HtmlEncode("Edit " + entry.Title);
                    hl.Controls.Add(img);
                    hl.NavigateUrl = SiteUtilities.GetEditViewUrl(entry.EntryId);
                    hl.ToolTip = HttpUtility.HtmlEncode("Edit " + entry.Title);

                    placeHolder.Controls.Add(hl);
                    hl = new HyperLink();
                    hl.ToolTip = HttpUtility.HtmlEncode("Delete " + entry.Title);
                    hl.CssClass = "deleteLinkStyle";
                    img = new Image();
                    img.CssClass = "deleteLinkImageStyle";
                    img.ImageUrl = new Uri(new Uri(SiteUtilities.GetBaseUrl(requestPage.SiteConfig)), requestPage.GetThemedImageUrl("deletebutton")).ToString();
                    img.BorderWidth = 0;
                    img.AlternateText = HttpUtility.HtmlEncode("Delete " + entry.Title);
                    hl.Controls.Add(img);
                    hl.NavigateUrl = String.Format("javascript:deleteEntry(\"{0}\", \"{1}\")", entry.EntryId, entry.Title.Replace("\"", "\\\""));
                    placeHolder.Controls.Add(hl);
                }
                return placeHolder;
            }
        }

        /// <summary>
        /// An HTML hyperlink pointing to the disclaimer file in siteConfig/disclaimer.format.html
        /// Corresponds to the macro: &lt;%disclaimerLink%&gt;
        /// </summary>
        /// <remarks>
        /// The output is HTML hyperlink.
        /// </remarks>
        public virtual Control DisclaimerLink
        {
            get
            {
                HyperLink hl = new HyperLink();
                hl.ToolTip = requestPage.CoreStringTables.GetString("tooltip_disclaimer");
                hl.Text = requestPage.CoreStringTables.GetString("text_disclaimer");
                hl.CssClass = "disclaimerLinkStyle";
                hl.NavigateUrl = String.Format("FormatPage.aspx?path={0}", "siteConfig/disclaimer.format.html");

                return hl;
            }
        }

        /// <summary>
        /// Ignored in this version.
        /// </summary>
        public virtual Control Enclosure
        {
            get
            {
                if (entry.Enclosure != null)
                {

                    string linkText = String.Format("<a href=\"{0}\">{1} ({2:#.##} {3})</a>",
                                                    SiteUtilities.GetEnclosureLinkUrl(entry.EntryId, entry.Enclosure),
                                                    entry.Enclosure.Name,
                                                    (entry.Enclosure.Length > 1024 * 1024) ? (((double)entry.Enclosure.Length) / (1024.0 * 1024.0)) : (((double)entry.Enclosure.Length) / 1024.0),
                                                    (entry.Enclosure.Length > 1024 * 1024) ? "MB" : "KB");

                    return new LiteralControl(linkText);
                }
                else
                {
                    return new LiteralControl("");
                }
            }
        }

        public virtual Control FeedFlare()
        {
            const string scriptTag = "<script src=\"http://feeds.feedburner.com/~s/{0}?i={1}\" type=\"text/javascript\"></script>";

            string feedburnerName = requestPage.SiteConfig.FeedBurnerName;
            if (feedburnerName != null && feedburnerName.Length > 0)
            {
                string script = string.Format(scriptTag, feedburnerName, HttpUtility.HtmlEncode(SiteUtilities.GetPermaLinkUrl(requestPage.SiteConfig, (ITitledEntry)entry)));
                return new LiteralControl(script);
            }
            return null;
        }

        public virtual Control PagePostCount()
        {
            return new LiteralControl(this.requestPage.WeblogEntries.Count.ToString());
        }

    }

    /// <summary>
    /// Like Macros and DayMacros and ItemMacros, EditMacros always return controls.
    /// </summary>
    public class EditMacros : Macros
    {

        /// <summary>
        /// EditMacros' constructor
        /// </summary>
        /// <param name="page">The page we are rendering</param>
        public EditMacros(SharedBasePage page)
            : base(page)
        {
        }

        public override RadioProperties InitRadioProperties()
        {
            return MacrosFactory.CreateEditRadioPropertiesInstance(requestPage);
        }

        /// <summary>
        /// Placeholder for rendering the edit boxes to edit/create weblog entries.
        /// Corresponds to the macro: &lt;%bodytext%&gt;
        /// </summary>
        /// <remarks>
        /// The output is HTML.
        /// <P/>
        /// Class EditMacros overrides the functionality of base class Macros.
        /// </remarks>
        public override Control Bodytext
        {
            get
            {
                PlaceHolder bodyPlaceHolder = new PlaceHolder();
                bodyPlaceHolder.Controls.Add(requestPage.LoadControl("EditEntryBox.ascx"));
                return bodyPlaceHolder;
            }
        }
    }
}
