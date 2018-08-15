/*
 * Note that this file is shared between DasBlog.Core and the test routines
 * If a change is made in one project then the other project may need
 * to be rebuilt otherwise a razor compilation failure may occur.
 * This applies to element ids, page titles etc.
 * Although the raxor page is recompiled on the fly the constants which are embedded in the DasBlog.Core
 * assembly's data will not have been updated
 * TODO - fix this gotcha
 */

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

		public const string TinyMceEditor = "tinymce";
		public const string NicEditEditor = "nicedit";
		public const string TextAreaEditor = "textarea";
		public const string EventFieldSeparator = " :: ";
		public const string UrlEventFielD = "{eventurl_6473}";
		public const string CodeEventFielD = "{eventurl_6474}";
		public const string LogDirectory = "logs";
	
		// element ids
		public const string PasswordValidationId = "passwordValidation";
		public const string NavBarId = "navbar";
		public const string CategoryPageTitle = "Category";
	
		public const string SearcherRouteName = "searcher";
		public const string ArchivePageTitle = "Archive";
		public const string HomePageTitle = "Blog";
		// navbar link ids
		public const string LoginButtonId = "loginButton";
		public const string CategoryNavId = "categoryNavBarItemId";
		public const string ArchiveNavId = "archiveNavBarItemId";
		public const string HomeNavId = "homeNavBarItemId";
		//
		public const string DasBlogDataRoot = "DAS_BLOG_DATA_ROOT";
	}

}
