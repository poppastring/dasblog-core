<%@ Control Language="c#" AutoEventWireup="True" Codebehind="AdminNavBar.ascx.cs" Inherits="newtelligence.DasBlog.Web.AdminNavBar" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
<div class="adminNavbarStyle" id="DIV1" runat="server">
	<ul>		
		<li id="editEntry" class="first" runat="server">
			<asp:HyperLink runat="server" NavigateUrl="EditEntry.aspx" id="hyperLinkEditEntry" Text='<%# resmgr.GetString("text_admin_add_entry")%>'>
			</asp:HyperLink>
		</li>
		<li id="editUser"  runat="server">
			<asp:HyperLink runat="server" NavigateUrl="EditUser.aspx" id="hyperLinkEditUser" Text='<%# resmgr.GetString("text_admin_edit_user")%>'>
			</asp:HyperLink>
		</li>
		<li id="activity" runat="server">
			<asp:HyperLink runat="server" NavigateUrl="Referrers.aspx" id="hyperLinkActivity" Text='<%# resmgr.GetString("text_admin_activity")%>'>
			</asp:HyperLink>
		</li>
		<li id="editConfig" runat="server">
			<asp:HyperLink runat="server" NavigateUrl="EditConfig.aspx" id="hyperLinkEditConfig" Text='<%# resmgr.GetString("text_admin_config")%>'>
			</asp:HyperLink>
		</li>
		<li id="editContentFilters" runat="server">
			<asp:HyperLink runat="server" NavigateUrl="EditContentFilters.aspx" id="hyperLinkEditContentFilters" Text='<%# resmgr.GetString("text_admin_content_filters")%>' >
			</asp:HyperLink>
		</li>
		<li id="editBlogRoll" runat="server">
			<asp:HyperLink runat="server" NavigateUrl="EditBlogRoll.aspx" id="hyperLinkEditBlogRoll" Text='<%# resmgr.GetString("text_admin_blogroll")%>'>
			</asp:HyperLink>
		</li>
		<li id="editNavigatorLinks" runat="server">
			<asp:HyperLink runat="server" NavigateUrl="EditNavigatorLinks.aspx" id="hyperLinkEditNavigatorLinks" Text='<%# resmgr.GetString("text_admin_navlinks")%>'>
			</asp:HyperLink>
		</li>
		<%if (siteConfig.EnableCrossposts) {%>
		<li id="editCrossPostSites" runat="server">
			<asp:HyperLink runat="server" NavigateUrl="EditCrosspostSites.aspx" id="hyperLinkEditCrossPostSites" Text='<%# resmgr.GetString("text_admin_crosspostsites")%>'>
			</asp:HyperLink>			
		</li>
		<%}%>
	</ul>
</div>
