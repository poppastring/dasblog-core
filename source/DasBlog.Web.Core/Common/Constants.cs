/*
 * Note that this file is shared between DasBlog.Core and the test routines
 * If a change is made in one project then the other project may need
 * to be rebuilt otherwise a razor compilation failure may occur.
 * This applies to element ids, page titles etc.
 * Although the raxor page is recompiled on the fly the constants which are embedded in the DasBlog.Core
 * assembly's data will not have been updated
 * TODO - fix this gotcha
 */

using System.Data;

namespace DasBlog.Core.Common
{
	public static class Constants // has to be public for razor
	{
		public const string BlogPostCreateAction = "Create";
		public const string BlogPostCancelAction = "Cancel";
		public const string BlogPostAddCategoryAction = "Add";
		public const string BlogPostEditAction = "Edit";
		public const string UploadImageAction = "Upload Image";
		public const string UsersCreateMode = "Create";
		public const string UsersEditMode = "Edit";
		public const string UsersDeleteMode = "Delete";
		public const string UsersViewMode = "View";
		public const string CreateUserSubView = "CreateUser";
		public const string EditUserSubView = "EditUser";
		public const string DeleteUserSubView = "DeleteUser";
		public const string ViewUserSubView = "ViewUser";
		public const string UsersErrorAction = "ErrorUser";
		public const string SaveAction = "Save";
		public const string CancelAction = "Cancel";
		public const string DeleteAction = "Delete";

		// ViewData
		public const string ShowPageControl = "show-page-control";
		public const string PageNumber = "page-number";
		public const string PostCount = "post-count";
		//
		public const string TinyMceEditor = "tinymce";
		public const string NicEditEditor = "nicedit";
		public const string TextAreaEditor = "textarea";
		public const string EventFieldSeparator = " :: ";
		public const string UrlEventFielD = "{eventurl_6473}";
		public const string CodeEventFielD = "{eventurl_6474}";
		public const string LogDirectory = "logs";
	
		// element ids
		public const string PasswordValidationId = "passwordValidation";
		// ****** NOTE *******
		// asp-for and its friends cannot handle const. so a field is required
		// BUT razor uses the FIELD NAME not the field value as the cshtml value
		// This may well change in future
		public static readonly string Email = "Email";
		public static readonly string Password = "Password";
		public const string Name = "Name";
		public const string Content = "Content";
		public const string SaveContentButtonId = "SaveContentButton";
		public const string NavBarId = "navbar";
		public const string CategoryPageTitle = "Category";
		public const string CommentsStartId = "comments-start";
		public const string CommentOnThisPostId = "comment-on-this-post";
		public const string NextPageId = "next-page";
		public const string PreviousPageId = "previous-page";
	
		public const string SearcherRouteName = "searcher";
		public const string ArchivePageTitle = "Archive";
		public const string HomePageTitle = "Blog";
		// navbar link ids
		public const string LoginButtonId = "loginButton";
		public const string CategoryNavId = "categoryNavBarItemId";
		public const string ArchiveNavId = "archiveNavBarItemId";
		public const string HomeNavId = "homeNavBarItemId";
		// page test ids - used by automated browser tests
		public const string PageTestIdClass = "page-test-id";
		public const string LoginPageTestId = "login-page";
		public const string HomePageTestId = "home-page";
		public const string CategoryPageTestId = "category-page";
		public const string ArchivePageTestId = "archive-page";
		public const string SubscribePageTestId = "subscribe-page";
		public const string PostMaintenancePageTestId = "post-maintenance-page";
		public const string UsersPageTestId = "users-page";
		public const string ActivityPageTestId = "activity-page";
		//
		public const string DasBlogDataRoot = "DAS_BLOG_DATA_ROOT";
				// e.g. C:/alt/projects3/dasblog-core/source/DasBlog.Tests/Resources/Environments/Vanilla
		public const string DasBlogOverrideRootUrl = "DAS_BLOG_OVERRIDE_ROOT_URL";
				// set = 1 to override.  Absence of any other value will not override
		public const string AspNetCoreUrls = "ASPNETCORE_URLS";		// set by VS or dotnet cli if luanchsettings.json is presnet
				// typically "http://localhost:50432/"

	}

}
