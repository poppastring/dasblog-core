<%@ Register TagPrefix="cc1" Namespace="WebControlCaptcha" Assembly="WebControlCaptcha" %>
<%@ Control Language="c#" AutoEventWireup="True" Codebehind="EmailBox.ascx.cs" Inherits="newtelligence.DasBlog.Web.EmailBox" TargetSchema="http://schemas.microsoft.com/intellisense/ie5"%>
<div class="bodyContentStyle" id="content" runat="server">
	<div class="commentViewBoxStyle" id="commentViewEntry">
		<h3><%# resmgr.GetString("text_send_an_email") %></h3>
		<table class="commentViewTableStyle" id="commentViewTable" cellspacing="1" cellpadding="1"
			width="100%" border="0">
			<tr>
				<td noWrap><asp:label id=Label1 Text='<%# resmgr.GetString("text_person_name") %>' runat="server" CssClass="commentViewLabelStyle">
					</asp:label><asp:requiredfieldvalidator id=validatorRFName runat="server" ControlToValidate="name" Display="Dynamic" ErrorMessage='<%# resmgr.GetString("text_error_person_name_rf")%>'>*</asp:requiredfieldvalidator></td>
				<td width="100%"><asp:textbox id="name" runat="server" Width="99%" CssClass="commentViewControlStyle" Columns="40"
						MaxLength="32"></asp:textbox></td>
			</tr>
			<tr>
				<td noWrap><asp:label id=Label2 Text='<%# resmgr.GetString("text_person_email") %>' runat="server" CssClass="commentViewLabelStyle"></asp:label><asp:regularexpressionvalidator id=validatorREEmail runat="server" ControlToValidate="email" Display="Dynamic" ErrorMessage='<%# resmgr.GetString("text_error_person_email_re")%>' ValidationExpression="\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*">*</asp:regularexpressionvalidator></td>
				<td noWrap><asp:textbox id="email" runat="server" Width="99%" CssClass="commentViewControlStyle" Columns="60"
						MaxLength="40"></asp:textbox></td>
			</tr>
			<tr>
				<td noWrap colSpan="2"></td>
			</tr>
			<tr>
				<td></td>
				<td>
					<P><asp:CheckBox id="rememberMe" runat="server" Text='<%# resmgr.GetString("text_person_remember") %>' Checked="True" CssClass="commentViewControlStyle"></asp:CheckBox></P>
				</td>
			</tr>
			<tr>
				<td colSpan="2" height="24" noWrap><asp:Label id="Label4" runat="server" Text='<%# resmgr.GetString("text_comment_content") %>' CssClass="commentViewLabelStyle"></asp:Label>
					<asp:RequiredFieldValidator id="validatorRFComment" runat="server" ErrorMessage='<%# resmgr.GetString("text_comment_content_rf")%>'
						ControlToValidate="comment">*</asp:RequiredFieldValidator></td>
			</tr>
			<tr>
				<td colSpan="2" noWrap>
					<asp:TextBox Rows="12" Columns="60" id="comment" runat="server" TextMode="MultiLine" CssClass="commentViewControlStyle"
						MaxLength='500' Width="99%"></asp:TextBox>
					<P></P>
					<cc1:CaptchaControl CaptchaFontWarping="4" id=CaptchaControl1 runat="server" Text='<%# resmgr.GetString("text_captcha")%>' ShowSubmitButton="false" CaptchaTimeout="300" CaptchaFont="Verdana">
					</cc1:CaptchaControl>
					<P></P>
				</td>
			</tr>
			<tr>
				<td colSpan="2" noWrap>
					<div style="WIDTH:50%;TEXT-ALIGN:left"><asp:ValidationSummary id="ValidationSummary1" runat="server"></asp:ValidationSummary></div>
					<asp:Button id="mailSend" runat="server" Text='<%# resmgr.GetString("text_person_email") %>' onclick="mailSend_Click"></asp:Button>
				</td>
			</tr>
		</table>
	</div>
</div>
