<%@ Page Language="C#" ValidateRequest="false" Trace="false" %>
<%@ Register TagPrefix="FTB" Namespace="FreeTextBoxControls" Assembly="FreeTextBox" %>
<script runat="server">
protected void Page_Load(Object Src, EventArgs E) {
	
	if (!newtelligence.DasBlog.Web.SiteSecurity.IsValidContributor()) 
    {
		Response.Redirect("~/FormatPage.aspx?path=SiteConfig/accessdenied.format.html");
    }
    
	// *** remove this return statement to use the following code ***
	return;

	string currentFolder = ImageGallery1.CurrentImagesFolder;
	
	// modify the directories allowed
	if (currentFolder == "~/images") {

		// these are the default directories FTB:ImageGallery will find
		string[] defaultDirectories = System.IO.Directory.GetDirectories(Server.MapPath(currentFolder),"*");
		
		// user defined custom directories
		string[] customDirectories = new string[] {"folder1","folder2"};
		
		// the gallery will use these images in this instance
		ImageGallery1.CurrentDirectories = customDirectories;
	}
	
	
	// modify the images allowed
	if (currentFolder == "~/images") {

		System.IO.DirectoryInfo directoryInfo = new System.IO.DirectoryInfo(Server.MapPath(currentFolder));

		// these are the default images FTB:ImageGallery will find
		System.IO.FileInfo[] defaultImages = directoryInfo.GetFiles("*");
		
		// user defined custom images (here, we're just allowing the first two)
		System.IO.FileInfo[] customImages = new System.IO.FileInfo[2] {defaultImages[0], defaultImages[1]};
		
		// the gallery will use these images in this instance
		ImageGallery1.CurrentImages = customImages;
	}	
	
}
</script>
<html>
<head>
	<title>Image Gallery</title>
</head>
<body>
    <form id="Form1" runat="server" enctype="multipart/form-data">  
		<FTB:ImageGallery UtilityImagesLocation="ExternalFile" JavaScriptLocation="ExternalFile" SupportFolder="../ftb/" AllowImageDelete=false AllowImageUpload=true AllowDirectoryCreate=false AllowDirectoryDelete=false id="ImageGallery1" runat="Server" />
	</form>
</body>
</html>
