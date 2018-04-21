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
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Resources;
using System.Text;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using newtelligence.DasBlog.Runtime;
using newtelligence.DasBlog.Web.Core;
using newtelligence.DasBlog.Web.TextEditors;

namespace newtelligence.DasBlog.Web
{
    /// <summary>
    ///		Summary description for EditEntryBox.
    /// </summary>
    public partial class EditEntryBox : SharedBaseControl
    {
        public Entry CurrentEntry
        {
            get
            {
                if (ViewState["Entry"] != null)
                {
                    return (Entry)ViewState["Entry"];
                }
                else
                {
                    return null;
                }
            }
            set
            {
                ViewState["Entry"] = value;
            }
        }

        protected bool isDHTMLEdit = false;
        protected ResourceManager resmgr;
        protected SiteConfig siteConfig;

        private Core.EditControlAdapter editControl;

        public EditEntryBox()
        {
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            SharedBasePage requestPage = this.Page as SharedBasePage;
            siteConfig = SiteConfig.GetSiteConfig();

            resmgr = ApplicationResourceTable.Get();

            imageUpload.Accept = "image/jpeg,image/gif,image/png";
            editControl.Width = Unit.Percentage(99d);
            editControl.Height = Unit.Pixel(400);
            editControl.Text = "<p></p>";

            // TODO: OmarS need to get rid of this
            isDHTMLEdit = true;

            editControl.SetLanguage(CultureInfo.CurrentUICulture.Name);
            editControl.SetTextDirection(requestPage.ReadingDirection);

            if (!requestPage.SiteConfig.EnableCrossposts)
            {
                gridCrossposts.Visible = false;
                labelCrosspost.Visible = false;
            }

            if (!SiteSecurity.IsValidContributor())
            {
                Response.Redirect("~/FormatPage.aspx?path=SiteConfig/accessdenied.format.html");
            }

            CrosspostInfoCollection crosspostSiteInfo = new CrosspostInfoCollection();

            if (!IsPostBack)
            {
                foreach (CrosspostSite site in requestPage.SiteConfig.CrosspostSites)
                {
                    CrosspostInfo ci = new CrosspostInfo(site);
                    ci.TrackingUrlBase = SiteUtilities.GetCrosspostTrackingUrlBase(requestPage.SiteConfig);
                    crosspostSiteInfo.Add(ci);
                }

                // set up categories
                foreach (CategoryCacheEntry category in requestPage.DataService.GetCategories())
                {
                    this.categoryList.Items.Add(category.Name);
                }


                // get the cultures
                CultureInfo[] cultures = CultureInfo.GetCultures(CultureTypes.AllCultures);

                // setup temp store for listitem items, for sorting
                List<ListItem> cultureList = new List<ListItem>(cultures.Length);

                foreach (CultureInfo ci in cultures)
                {
                    string langName = (ci.NativeName != ci.EnglishName) ? ci.NativeName + " / " + ci.EnglishName : ci.NativeName;

                    if (langName.Length > 55)
                    {
                        langName = langName.Substring(0, 55) + "...";
                    }

                    cultureList.Add(new ListItem(langName, ci.Name));
                }

                // setup the sort culture
                string rssCulture = requestPage.SiteConfig.RssLanguage;

                CultureInfo sortCulture;

                try
                {
                    sortCulture = (rssCulture != null && rssCulture.Length > 0 ? new CultureInfo(rssCulture) : CultureInfo.CurrentCulture);
                }
                catch (ArgumentException)
                {
                    // default to the culture of the server
                    sortCulture = CultureInfo.CurrentCulture;
                }

                // sort the list
                cultureList.Sort(delegate(ListItem x, ListItem y)
                {
                    // actual comparison
                    return String.Compare(x.Text, y.Text, true, sortCulture);
                });

                // add to the languages listbox
                ListItem[] cultureListItems = cultureList.ToArray();

                listLanguages.Items.AddRange(cultureListItems);

                listLanguages.SelectedValue = "";

                if (requestPage != null && requestPage.WeblogEntryId != "")
                {
                    Session["newtelligence.DasBlog.Web.EditEntryBox.OriginalReferrer"] = Request.UrlReferrer;
                    Entry entry = requestPage.DataService.GetEntryForEdit(requestPage.WeblogEntryId);

                    if (entry != null)
                    {
                        CurrentEntry = entry;
                        entryTitle.Text = entry.Title;
                        entryAbstract.Text = entry.Description;

                        textDate.SelectedDate = entry.CreatedLocalTime;

                        if (isDHTMLEdit)
                        {
                            editControl.Text = entry.Content;
                        }

                        foreach (string s in entry.GetSplitCategories())
                        {
                            categoryList.Items.FindByText(s).Selected = true;
                        }

                        this.checkBoxAllowComments.Checked = entry.AllowComments;
                        this.checkBoxPublish.Checked = entry.IsPublic;
                        this.checkBoxSyndicated.Checked = entry.Syndicated;

                        // GeoRSS.
                        this.txtLat.Text = String.Format(CultureInfo.InvariantCulture, "{0}", entry.Latitude);
                        this.txtLong.Text = String.Format(CultureInfo.InvariantCulture, "{0}", entry.Longitude);

                        if (entry.Attachments.Count > 0)
                        {
                            foreach (Attachment enclosure in entry.Attachments)
                            {
                                enclosure.Url = SiteUtilities.GetEnclosureLinkUrl(requestPage.SiteConfig, entry.EntryId, enclosure);
                            }

                            this.enclosureUpload.Visible = false;
                            this.buttonRemove.Visible = true;
                            this.labelEnclosureName.Visible = true;
                            this.labelEnclosureName.Text = entry.Attachments[0].Name;
                        }

                        listLanguages.SelectedValue = entry.Language == null ? "" : entry.Language;

                        // merge the crosspost config with the crosspost data
                        foreach (CrosspostInfo cpi in crosspostSiteInfo)
                        {
                            foreach (Crosspost cp in entry.Crossposts)
                            {
                                if (cp.ProfileName == cpi.Site.ProfileName)
                                {
                                    cpi.IsAlreadyPosted = true;
                                    cpi.TargetEntryId = cp.TargetEntryId;
                                    cpi.Categories = cp.Categories;
                                    break;
                                }
                            }
                        }
                        // if the entry is not public yet but opened for editing, then we can setup autosave.
                        // (If the entry was already published publically and then autosave was used, the 
                        // entry's status would change to non-public and then no longer be accessible!)
                        if (requestPage.SiteConfig.EnableAutoSave && !entry.IsPublic)
                        {
                            SetupAutoSave();
                        }

                        if (requestPage.SiteConfig.EnableGoogleMaps)
                        {
                            AddGoogleMapsApi();
                        }

                    }
                }
                else // This is a brand new entry, so setup the AutoSave script if it's enabled.
                {
                    if (requestPage.SiteConfig.EnableAutoSave)
                    {
                        SetupAutoSave();
                    }

                    if (requestPage.SiteConfig.EnableGoogleMaps)
                    {
                        AddGoogleMapsApi();
                    }

                    txtLat.Text = String.Format(CultureInfo.InvariantCulture, "{0}", siteConfig.DefaultLatitude);
                    txtLong.Text = String.Format(CultureInfo.InvariantCulture, "{0}", siteConfig.DefaultLongitude);
                }

                gridCrossposts.DataSource = crosspostSiteInfo;
                DataBind();
            }
        }

        private void AddGoogleMapsApi()
        {
            if (!Page.ClientScript.IsClientScriptBlockRegistered(this.GetType(), "GoogleMapsApi"))
            {
                System.Text.StringBuilder sb = new StringBuilder();
                sb.Append("<script src=\"http://maps.google.com/maps?file=api&v=2&key=");
                sb.Append(SiteConfig.GetSiteConfig().GoogleMapsApiKey);
                sb.Append("\" type=\"text/javascript\"></script>");
                Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "GoogleMapsApi", sb.ToString());
            }
        }

        private void SetupAutoSave()
        {
            if (!Page.ClientScript.IsClientScriptBlockRegistered(this.GetType(), "AutoSaveScript"))
            {
                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                sb.Append("<script language='javascript' type='text/javascript' runat='server'> " + "\n");

                sb.Append("function StartInterval() " + "\n");
                sb.Append("{ " + "\n");
                sb.Append("\tinterval = window.setInterval('AutoSaveTimer()',60000);" + "\n");
                sb.Append("} " + "\n");

                sb.Append("function StopInterval() " + "\n");
                sb.Append("{ " + "\n");
                sb.Append("	window.clearInterval(interval);" + "\n");
                sb.Append("} " + "\n");

                string autoSaveText = resmgr.GetString("text_autosave");

                //To do create method using the old 
                //function(result) to set the autosave
                //time and success message on return.
                // add the javascript to allow deletion of the entry
                sb.Append("function AutoSaveTimer()\n");
                sb.Append("{\n");
                //sb.Append("alert('before from ajax'); \n");
                sb.Append("		var url = 'autoSaveEntry.ashx';");//?entryId=" +  CurrentEntry.EntryId + "';");

                // Justice Gray: To properly save this entry we need to create the data required to post to our
                // handler.  Both the entry ID and the text inside the edit box need to be passed.

                sb.AppendFormat("var editTextBox = document.getElementById('{0}');", this.editControl.Control.ClientID);
                sb.AppendFormat("var titleBox = document.getElementById('{0}');", this.entryTitle.ClientID);
                // If our entry does not exist yet, create a new one and use that for the autosave.
                if (CurrentEntry == null)
                {
                    Entry ourAutosavedEntry = new Entry();
                    ourAutosavedEntry.Initialize();
                    CurrentEntry = ourAutosavedEntry;
                }
                sb.AppendFormat("var ajax = new AjaxDelegate(url, AutoSaveResult, '{0}', titleBox.value, '{1}', editTextBox.value);",
                                    CurrentEntry.EntryId, this.Context.User.Identity.Name);

                //				sb.Append("		var ajax = new AjaxDelegate(url, AutoSaveResult); \n");
                sb.Append("		ajax.Fetch(); \n");
                sb.Append("}\n");

                sb.Append("function AutoSaveResult(url, response)  " + "\n");
                sb.Append("{ \n");
                //sb.Append("		var result = eval(response); \n");
                sb.Append("		var dt = new Date(); \n");
                sb.Append("		document.getElementById('" + this.autoSaveLabel.ClientID + "').innerHTML = '" + autoSaveText + " ' + dt.toLocaleString(); \n");
                sb.Append("} \n");
                sb.Append("</script> \n");

                sb.Append("<script> " + "\n");
                sb.Append("StartInterval(); " + "\n");
                //sb.Append("var entryId;" + "\n");
                sb.Append("</script> " + "\n");


                Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "AutoSaveScript", sb.ToString());
            }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            this.editControl = EditControlProvider.CreateEditControl<TinyMCEAdapter>();
            this.editControlHolder.Controls.Add(editControl.Control);
            this.editControl.Initialize();
        }


        protected void save_Click(object sender, EventArgs e)
        {
            SharedBasePage requestPage = this.Page as SharedBasePage;

            if (SiteSecurity.IsValidContributor())
            {
                //Catch empty posts!
                if (!editControl.HasText())
                {
                    return;
                }

                CrosspostInfoCollection crosspostList = new CrosspostInfoCollection();
                Entry entry;

                if (CurrentEntry == null)
                {
                    entry = new Entry();
                    entry.Initialize();
                }
                else
                {
                    entry = CurrentEntry;
                }

                //Try a culture specific parse...
                // TODO: Come up with a shiny javascript datetime picker

                if (textDate.SelectedDateFormatted.Length > 0)
                {
                    try
                    {
                        DateTime createdLocalTime = new DateTime(textDate.SelectedDate.Year,
                            textDate.SelectedDate.Month,
                            textDate.SelectedDate.Day,
                            entry.CreatedLocalTime.Hour,
                            entry.CreatedLocalTime.Minute,
                            entry.CreatedLocalTime.Second,
                            entry.CreatedLocalTime.Millisecond);

                        entry.CreatedLocalTime = createdLocalTime;
                    }
                    catch (FormatException fex)
                    {
                        Trace.Write("Bad DateTime string creating new Entry: " + fex.ToString());
                    }
                }

                // see if we need to delete any old Enclosures
                if (entry.Enclosure != null)
                {
                    if (this.enclosureUpload.Visible == true && this.buttonRemove.Visible == false)
                    {
                        DeleteEnclosures();
                    }
                }

                // upload the attachment
                if (enclosureUpload.Value != null && enclosureUpload.Value != String.Empty)
                {
                    try
                    {
                        long numBytes;
                        string type;

                        string baseFileName;
                        string fileUrl = HandleUpload(enclosureUpload, entry.EntryId, out type, out numBytes, out baseFileName);

                        entry.Attachments.Add(new Attachment(baseFileName, type, numBytes, AttachmentType.Enclosure));
                    }
                    catch (Exception exc)
                    {
                        ErrorTrace.Trace(TraceLevel.Error, exc);
                    }
                }

                entry.Language = listLanguages.SelectedValue == "" ? null : listLanguages.SelectedValue;
                entry.Title = entryTitle.Text;
                entry.Description = entryAbstract.Text;
                entry.Author = requestPage.User.Identity.Name;
                entry.AllowComments = checkBoxAllowComments.Checked;
                entry.IsPublic = checkBoxPublish.Checked;
                entry.Syndicated = checkBoxSyndicated.Checked;

                // GeoRSS.
                if (siteConfig.EnableGeoRss)
                {
                    double latitude, longitude;
                    if (double.TryParse(txtLat.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out latitude))
                    {
                        entry.Latitude = latitude;
                    }
                    else
                    {
                        entry.Latitude = null;
                    }

                    if (double.TryParse(txtLong.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out longitude))
                    {
                        entry.Longitude = longitude;
                    }
                    else
                    {
                        entry.Longitude = null;
                    }
                }

                if (isDHTMLEdit)
                {
                    entry.Content = editControl.Text;
                }

                // handle categories
                string categories = "";

                StringBuilder sb = new StringBuilder();
                bool needSemi = false;

                foreach (ListItem listItem in categoryList.Items)
                {
                    if (listItem.Selected)
                    {
                        if (needSemi) sb.Append(";");
                        sb.Append(listItem.Text);
                        needSemi = true;
                    }
                }

                categories = sb.ToString();
                entry.Categories = categories;

                // handle crosspostSiteInfo
                CrosspostInfoCollection crosspostSiteInfo = new CrosspostInfoCollection();

                // we need to reload the crosspostinfo as it contains sensitive data like password
                foreach (CrosspostSite site in requestPage.SiteConfig.CrosspostSites)
                {
                    CrosspostInfo ci = new CrosspostInfo(site);
                    ci.TrackingUrlBase = SiteUtilities.GetCrosspostTrackingUrlBase(requestPage.SiteConfig);
                    crosspostSiteInfo.Add(ci);
                }

                // merge the crosspost config with the crosspost data
                foreach (CrosspostInfo cpi in crosspostSiteInfo)
                {
                    foreach (Crosspost cp in entry.Crossposts)
                    {
                        if (cp.ProfileName == cpi.Site.ProfileName)
                        {
                            cpi.IsAlreadyPosted = true;
                            cpi.TargetEntryId = cp.TargetEntryId;
                            cpi.Categories = cp.Categories;
                            break;
                        }
                    }
                }

                foreach (DataGridItem item in gridCrossposts.Items)
                {
                    CheckBox checkSite = item.FindControl("checkSite") as CheckBox;
                    if (checkSite.Checked)
                    {
                        TextBox textSiteCategory = item.FindControl("textSiteCategory") as TextBox;
                        foreach (CrosspostInfo cpi in crosspostSiteInfo)
                        {
                            if (cpi.Site.ProfileName == checkSite.Text)
                            {
                                cpi.Categories = textSiteCategory.Text;
                                crosspostList.Add(cpi);
                                break;
                            }
                        }
                    }
                }

                try
                {
                    // prevent SaveEntry from happenning twice
                    if (crosspostList.Count == 0) crosspostList = null;

                    if (CurrentEntry == null) // new entry
                    {
                        SiteUtilities.SaveEntry(entry, this.textTrackback.Text, crosspostList, requestPage.SiteConfig, requestPage.LoggingService, requestPage.DataService);
                    }
                    else // existing entry
                    {
                        SiteUtilities.UpdateEntry(entry, this.textTrackback.Text, crosspostList, requestPage.SiteConfig, requestPage.LoggingService, requestPage.DataService);
                    }
                }
                catch (Exception ex)
                {
                    //SDH: Changed to ex.ToString as the InnerException is often null, which causes another error in this catch!
                    StackTrace st = new StackTrace();
                    requestPage.LoggingService.AddEvent(
                        new EventDataItem(EventCodes.Error, ex.ToString() + Environment.NewLine + st.ToString(), SiteUtilities.GetPermaLinkUrl(entry)));

                    // if we created a new entry, and there was an error, delete the enclosure folder
                    DeleteEnclosures();

                    requestPage.Redirect("FormatPage.aspx?path=SiteConfig/pageerror.format.html");
                }


                entryTitle.Text = "";
                entryAbstract.Text = "";
                categoryList.Items.Clear();

                if (Session["newtelligence.DasBlog.Web.EditEntryBox.OriginalReferrer"] != null)
                {
                    Uri originalReferrer = Session["newtelligence.DasBlog.Web.EditEntryBox.OriginalReferrer"] as Uri;
                    Session.Remove("newtelligence.DasBlog.Web.EditEntryBox.OriginalReferrer");
                    Redirect(originalReferrer.AbsoluteUri);
                }
                else
                {
                    Redirect(SiteUtilities.GetAdminPageUrl(requestPage.SiteConfig));
                }
            }
        }

        //FIX: this should be in the IBlogDataService somewhere, not in this class
        private void DeleteEnclosures()
        {
            try
            {
                SharedBasePage requestPage = this.Page as SharedBasePage;

                if (CurrentEntry != null)
                {
                    // TODO: when we add support for more than just enclosures, we can't delete the entire folder
                    DirectoryInfo enclosuresPath = new DirectoryInfo(SiteUtilities.MapPath(Path.Combine(requestPage.SiteConfig.BinariesDir, CurrentEntry.EntryId)));
                    if (enclosuresPath.Exists) { enclosuresPath.Delete(true); }

                    CurrentEntry.Attachments.Remove(CurrentEntry.Enclosure);
                }
            }
            catch (Exception ex)
            {
                ErrorTrace.Trace(TraceLevel.Error, ex);
            }
        }

        private string HandleUpload(HtmlInputFile fileInput, out string type, out long numBytes, out string fileName)
        {
            return HandleUpload(fileInput, null, out type, out numBytes, out fileName);
        }

        private string HandleUpload(HtmlInputFile fileInput, string entryId, out string type, out long numBytes, out string savedFileName)
        {
            type = fileInput.PostedFile.ContentType;
            numBytes = fileInput.PostedFile.InputStream.Length;

            SharedBasePage requestPage = this.Page as SharedBasePage;

            string postedFileName = Path.GetFileName(fileInput.PostedFile.FileName);

            string filename = Path.Combine(entryId ?? "", postedFileName);

            string absUrl = requestPage.BinaryDataService.SaveFile(fileInput.PostedFile.InputStream, ref filename);
            savedFileName = Path.GetFileName(filename);

            return absUrl;
        }

        protected void categoryAddButton_Click(object sender, EventArgs e)
        {
            string newCategory = textBoxNewCategory.Text.Trim();
            if (newCategory != String.Empty)
            {
                ListItem listItem = new ListItem(textBoxNewCategory.Text);
                listItem.Selected = true;
                categoryList.Items.Add(listItem);
            }
            textBoxNewCategory.Text = "";
        }

        protected void buttonUpload_Click(object sender, System.EventArgs e)
        {
            try
            {
                long numBytes;
                string uploadType;
                SharedBasePage requestPage = this.Page as SharedBasePage;

                string baseFileName;
                string absUrl = HandleUpload(imageUpload, out uploadType, out numBytes, out baseFileName);
                string linkText = String.Format("<img border=\"0\" src=\"{0}\">",
                    absUrl);

                editControl.Text += linkText;
            }
            catch (Exception exc)
            {
                ErrorTrace.Trace(System.Diagnostics.TraceLevel.Error, exc);
            }
        }

        protected void buttonRemove_Click(object sender, System.EventArgs e)
        {
            this.enclosureUpload.Visible = true;
            this.buttonRemove.Visible = false;
            this.labelEnclosureName.Visible = false;
            this.labelEnclosureName.Text = String.Empty;
        }

        protected void buttonUploadAttachment_Click(object sender, System.EventArgs e)
        {
            try
            {
                long numBytes;
                string uploadType;
                SharedBasePage requestPage = this.Page as SharedBasePage;
                string baseFileName;
                string absUrl = HandleUpload(attachmentUpload, out uploadType, out numBytes, out baseFileName);
                string linkText = String.Format("<a href=\"{0}\">{1} ({2:#.##} {3})</a>",
                    absUrl,
                    baseFileName,
                    (numBytes > 1024 * 1024) ? (((double)numBytes) / (1024.0 * 1024.0)) : (((double)numBytes) / 1024.0),
                    (numBytes > 1024 * 1024) ? "MB" : "KB");


                editControl.Text += linkText;
            }
            catch (Exception exc)
            {
                ErrorTrace.Trace(System.Diagnostics.TraceLevel.Error, exc);
            }
        }
    }
}
