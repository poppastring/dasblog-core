<%@ Control Language="c#" AutoEventWireup="True" Codebehind="Logout.ascx.cs" Inherits="newtelligence.DasBlog.Web.Logout" TargetSchema="http://schemas.microsoft.com/intellisense/ie5"%>
<div  class="signInContainerStyle">
    <table class="signInTableStyle">
		<tr>
			<td class="signInLabelCellStyle">
	    <asp:LinkButton id=linkButtonLogout Text='<%# resmgr.GetString("text_admin_logout")%>' Runat="server" CausesValidation="false" onclick="linkButtonLogout_Click" />
	        </td>
	    </tr><tr>
			<td class="signInLabelCellStyle">
	    
	    <asp:Label id=labelLoggedOnAs Text='<%# resmgr.GetString("text_admin_loggedonas")%>' Runat="server">
	    </asp:Label>
	    <asp:Label id="securityLogon" runat="server"></asp:Label>
    </td></tr></table>
</div>
