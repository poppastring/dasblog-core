using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Resources;
using System.Web.UI;
using System.Web.UI.WebControls;
using newtelligence.DasBlog.Runtime;
using newtelligence.DasBlog.Web.Core;
using newtelligence.DasBlog.Web.TextEditors;

namespace newtelligence.DasBlog.Web
{
    public partial class EditUserBox : UserControl
    {
        private const string passwordPlaceHolder = "########";
        protected ResourceManager resmgr;
        private Core.EditControlAdapter editControl;

        protected void Page_Load(object sender, EventArgs e)
        {
            SharedBasePage requestPage = this.Page as SharedBasePage;

            if (!SiteSecurity.IsValidContributor())
            {
                Response.Redirect("~/FormatPage.aspx?path=SiteConfig/accessdenied.format.html");
            }

            this.ID = "EditUserBox";

            editControl.Text = GetProfileContent();
            editControl.Width = Unit.Percentage(99d);
            editControl.Height = Unit.Pixel(400);
            editControl.SetLanguage(CultureInfo.CurrentUICulture.Name);
            editControl.SetTextDirection(requestPage.ReadingDirection);

            if (!IsPostBack)
            {
                SiteConfig siteConfig = requestPage.SiteConfig;
                User currentUser = SiteSecurity.GetUser(requestPage.User.Identity.Name);

                textEMail.Text = currentUser.EmailAddress;
                textDisplayName.Text = currentUser.DisplayName;
                checkboxNewPost.Checked = currentUser.NotifyOnNewPost;
                checkboxAllComment.Checked = currentUser.NotifyOnAllComment;
                checkboxOwnComment.Checked = currentUser.NotifyOnOwnComment;
                textPassword.Text = passwordPlaceHolder;
                textConfirmPassword.Text = passwordPlaceHolder;
                textOpenIdIdentifier.Text = currentUser.OpenIDUrl;

                DataBind();
            }
        }

        protected override void OnInit(EventArgs e)
        {
            InitializeComponent();
            base.OnInit(e);

            this.editControl = EditControlProvider.CreateEditControl<TinyMCEAdapter>();
            this.editControlHolder.Controls.Add(editControl.Control);
            this.editControl.Initialize();
        }

        /// <summary>
        ///		Required method for Designer support - do not modify
        ///		the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.Init += new System.EventHandler(this.EditUserBox_Init);
        }

        string GetProfilePath()
        {
            SharedBasePage requestPage = this.Page as SharedBasePage;
            string profileFilename = string.Format("{0}.format.html", requestPage.User.Identity.Name);
            return SiteUtilities.MapPath(Path.Combine(requestPage.SiteConfig.ProfilesDir, profileFilename));
        }

        string GetProfileContent()
        {
            string profileContent = string.Empty;

            try
            {
                FileInfo profileInfo = new FileInfo(GetProfilePath());
                if (!profileInfo.Exists)
                {
                    DirectoryInfo profileDirectory = new DirectoryInfo(profileInfo.DirectoryName);
                    if (!profileDirectory.Exists)
                    {
                        profileDirectory.Create();
                    }

                    using (profileInfo.Create())
                    {
                        // using statement will enforce the dispose of the FileStream
                    };
                }
                else
                {
                    using (StreamReader reader = new StreamReader(profileInfo.FullName))
                    {
                        profileContent = reader.ReadToEnd();
                    }
                }
            }
            catch (Exception exc)
            {
                ErrorTrace.Trace(TraceLevel.Error, exc);
            }

            return profileContent;
        }

        void SetProfileContent(string content)
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(GetProfilePath()))
                {
                    writer.Write(content);
                }
            }
            catch (Exception exc)
            {
                ErrorTrace.Trace(TraceLevel.Error, exc);
            }
        }

        protected void buttonSave_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                SharedBasePage requestPage = Page as SharedBasePage;
                SiteConfig siteConfig = requestPage.SiteConfig;

                string userName = requestPage.User.Identity.Name;

                User user = SiteSecurity.GetUser(userName);

                // failed to retrieve the user
                if (user != null)
                {
                    if (textPassword.Text.Length > 0 && textPassword.Text != passwordPlaceHolder)
                    {
                        user.Password = textPassword.Text;
                    }

                    user.EmailAddress = textEMail.Text;
                    user.NotifyOnNewPost = checkboxNewPost.Checked;
                    user.NotifyOnAllComment = checkboxAllComment.Checked;
                    user.NotifyOnOwnComment = checkboxOwnComment.Checked;
                    user.DisplayName = textDisplayName.Text;
                    user.OpenIDUrl = textOpenIdIdentifier.Text;

                    SiteSecurity.UpdateUser(user);
                }

                SetProfileContent(editControl.Text);
                requestPage.Redirect(Page.Request.Url.AbsoluteUri);
            }
        }

        protected void EditUserBox_Init(object sender, EventArgs e)
        {
            resmgr = ApplicationResourceTable.Get();
        }
    }
}