<%@ Control Language="c#" AutoEventWireup="false" Codebehind="LoginBox.ascx.cs" Inherits="newtelligence.DasBlog.Web.LoginBox" TargetSchema="http://schemas.microsoft.com/intellisense/ie5"%>
<%@ Register Assembly="DotNetOpenAuth" Namespace="DotNetOpenAuth.OpenId.RelyingParty" TagPrefix="cc1" %>
<div class="signInContainerStyle" id="login">
	<table class="signInTableStyle">
		<tr>
			<td class="signInLabelCellStyle"><asp:label id=Label1 Text='<%# resmgr.GetString("text_username") %>' runat="server">
				</asp:label></td>
			<td class="signInEditCellStyle"><asp:textbox id="username" runat="server" CssClass="signInUsernameTextBoxStyle"></asp:textbox><input type="hidden" id="challenge" runat="server"></td>
		</tr>
		<tr>
			<td class="signInLabelCellStyle"><asp:label id=Label2 Text='<%# resmgr.GetString("text_password") %>' runat="server">
				</asp:label></td>
			<td class="signInEditCellStyle"><asp:textbox id="password" runat="server" CssClass="signInPasswordTextBoxStyle" TextMode="Password"></asp:textbox></td>
		</tr>
		<tr>
			<td class="signInEditCellStyle" colSpan="2"><asp:checkbox id=rememberCheckbox Text='<%# resmgr.GetString("text_remember_login") %>' runat="server" CssClass="signRememberCheckBoxStyle">
				</asp:checkbox>&nbsp;&nbsp;<asp:Button id=doSignIn Text='<%# resmgr.GetString("text_log_on") %>' runat="server" CssClass="signInButtonStyle">
				</asp:Button>
			</td>
		</tr>
	</table>
	<hr />
		<cc1:OpenIdLogin ID="OpenIdLogin1" runat="server" CssClass="openidLogin"
		RequestEmail="Require" RequestNickname="Request" RegisterVisible="false" 
		RememberMeVisible="True" PolicyUrl="~/PrivacyPolicy.aspx" TabIndex="1"
		/>
</div>
