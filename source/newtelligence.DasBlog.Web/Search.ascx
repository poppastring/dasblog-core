<%@ Control Language="c#" AutoEventWireup="True" Codebehind="Search.ascx.cs" Inherits="newtelligence.DasBlog.Web.Search" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
<script type="text/javascript">
<!--
function doSearch(searchString)
{
	// Trim string.
	searchString = searchString.replace(/^\s+|\s+$/g, "");
	if (searchString.length > 0)
	{
		location.href = "<%# requestPage.SiteConfig.Root %>SearchView.aspx?q=" + encodeURIComponent(searchString);
	}

	return false;
}
-->
</script>
<div class="searchContainerStyle">
	<input id="searchString" onkeypress="javascript:if (event.keyCode == 13) { doSearch(searchString.value); return false; }" type="text" class="searchTextBoxStyle" />
	<input id="searchButton" runat="server" type="button" value='<%# resmgr.GetString("text_search") %>' name='<%# resmgr.GetString("text_search") %>' onclick="doSearch(searchString.value);" class="searchButtonStyle" />
</div>
