<%@ Register TagPrefix="cc1" Namespace="WebControlCaptcha" Assembly="WebControlCaptcha" %>
<%@ Control Language="c#" CodeBehind="CommentViewBox.ascx.cs" AutoEventWireup="True" Inherits="newtelligence.DasBlog.Web.CommentViewBox" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
<div id="content" runat="server" class="bodyContentStyle">

    <div runat="server" id="commentViewContent">
    </div>
    <div id="commentViewEntry" class="commentViewBoxStyle">
        <span id="commentsClosed" style="text-align: center" runat="server">
            <asp:Label ID="labelCommentsClosed" runat="server" CssClass="commentViewLabelStyle" Text='<%# resmgr.GetString("text_comments_closed") %>'>
            </asp:Label>
        </span><span id="commentsModerated" style="color: red; text-align: center" runat="server">
            <asp:Label ID="labelCommentsModerated" runat="server" CssClass="commentViewLabelStyle" Text='<%# resmgr.GetString("text_comments_require_approval") %>' />
        </span>
        <table class="commentViewTableStyle" id="openIdTable" cellspacing="1" cellpadding="1" border="0" runat="server" width="100%">
            <tr>
                <td style="white-space: nowrap">
                    <asp:Label ID="Label4" runat="server" CssClass="commentViewLabelStyle" Text='<%# resmgr.GetString("text_openid_name") %>'></asp:Label>
                    <asp:RequiredFieldValidator ValidationGroup="OpenId" ID="RequiredFieldValidator1" runat="server" ErrorMessage='<%# resmgr.GetString("text_error_openid_name_rf")%>' Display="Dynamic" ControlToValidate="openid_identifier">*</asp:RequiredFieldValidator>
                </td>
                <td width="100%">
                    <asp:TextBox CssClass="openidtextbox" ValidationGroup="OpenId" ID="openid_identifier" MaxLength="96" runat="server" Columns="40"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td colspan="2">
                    <%=resmgr.GetString("text_openid_instructions") %>
                </td>
            </tr>
        </table>
        <table class="commentViewTableStyle" id="commentViewTable" cellspacing="1" cellpadding="1" border="0" runat="server" width="100%">
            <tr>
                <td style="white-space: nowrap">
                    <asp:Label ID="Label1" runat="server" CssClass="commentViewLabelStyle" Text='<%# resmgr.GetString("text_person_name") %>'></asp:Label>
                    <asp:RequiredFieldValidator ID="validatorRFName" ValidationGroup="Classic" runat="server" ErrorMessage='<%# resmgr.GetString("text_error_person_name_rf")%>' Display="Dynamic" ControlToValidate="name">*</asp:RequiredFieldValidator>
                </td>
                <td width="100%">
                    <asp:TextBox ID="name" MaxLength="32" runat="server" ValidationGroup="Classic" Columns="40" CssClass="commentViewControlStyle" Width="99%"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td style="white-space: nowrap">
                    <asp:Label ID="Label2" runat="server" CssClass="commentViewLabelStyle" Text='<%# resmgr.GetString("text_person_email") %>'></asp:Label>
                    <asp:RegularExpressionValidator ID="validatorREEmail" ValidationGroup="Classic" runat="server" ErrorMessage='<%# resmgr.GetString("text_error_person_email_re")%>' Display="Dynamic" ControlToValidate="email" ValidationExpression="\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*">*</asp:RegularExpressionValidator>
                </td>
                <td style="white-space: nowrap">
                    <asp:TextBox MaxLength="40" ID="email" runat="server" ValidationGroup="Normal" Columns="60" CssClass="commentViewControlStyle" Width="99%"></asp:TextBox>
                    <span id="commentsGravatarEnabled" class="gravatarLabel" runat="server">
                        <br/>
                        <asp:Label ID="labelGravatarEnabled" runat="server" CssClass="commentViewLabelStyle" Text='<%# resmgr.GetString("text_comment_gravatarable") %>'>
                        </asp:Label>
                    </span>
                </td>
            </tr>
            <tr>
                <td style="white-space: nowrap">
                    <asp:Label ID="Label3" runat="server" CssClass="commentViewLabelStyle" Text='<%# resmgr.GetString("text_person_homepage") %>'></asp:Label>
                </td>
                <td>
                    <asp:TextBox MaxLength="64" ID="homepage" runat="server" Columns="60" CssClass="commentViewControlStyle" Width="99%"></asp:TextBox>
                </td>
            </tr>
             <tr id="trCheesySpam" runat="server">
                <td style="white-space: nowrap">
                    <asp:Label ID="lblCheesySpamQ" runat="server" CssClass="commentViewLabelStyle" Text='2+2=?'></asp:Label>
                </td>
                <td>
                    <asp:TextBox MaxLength="64" ID="txtCheesySpamA" runat="server" Columns="60" CssClass="commentViewControlStyle" Width="99%"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td style="white-space: nowrap" colspan="2">
                </td>
            </tr>
            <tr>
                <td>
                </td>
                <td>
                    <p>
                        <asp:CheckBox ID="rememberMe" runat="server" Text='<%# resmgr.GetString("text_person_remember") %>' Checked="True" CssClass="commentViewControlStyle"></asp:CheckBox></p>
                </td>
            </tr>
            <tr>
                <td colspan="2" height="24">
                    <asp:Label ID="labelComment" runat="server" Text='<%# resmgr.GetString("text_comment_content") %>' CssClass="commentViewLabelStyle"></asp:Label>
                    <asp:Label ID="labelCommentHtml" runat="server" CssClass="commentViewLabelStyle"></asp:Label>
                    <asp:RequiredFieldValidator ID="validatorRFComment" runat="server" ErrorMessage='<%# resmgr.GetString("text_comment_content_rf")%>' ValidationGroup="Base" ControlToValidate="comment">*</asp:RequiredFieldValidator>
                </td>
            </tr>
            <tr>
                <td colspan="2" style="white-space: nowrap">
                    <asp:TextBox Rows="12" Columns="60" ID="comment" runat="server" TextMode="MultiLine" CssClass="commentViewControlStyle livepreview" Width="99%"></asp:TextBox>
                    <p>
                    </p>
                    <cc1:CaptchaControl CaptchaFontWarping="Extreme" id="CaptchaControl1" runat="server" Text='<%# resmgr.GetString("text_captcha")%>' ShowSubmitButton="False" CaptchaTimeout="300" CaptchaFont="Verdana">
                    </cc1:CaptchaControl>
                    <asp:CustomValidator ValidationGroup="Base" ID="cvCaptcha" runat="server" Display="None" ErrorMessage='<%# resmgr.GetString("text_captcha_error")%>'>*</asp:CustomValidator>
                </td>
            </tr>
            <tr>
                <td colspan="2" height="24">
                    <asp:Label ID="lblCommentPreview" runat="server" Text='<%# resmgr.GetString("text_comment_livepreview") %>' CssClass="commentViewLabelStyle"></asp:Label>
                </td>
            </tr>
            <tr>
                <td colspan="2">
                    <div class="commentBodyStyle livepreview">
                    </div>
                </td>
            </tr>
            <tr>
                <td colspan="2" style="white-space: nowrap">
                    <div style="width: 50%; text-align: left">
                        <%-- Hack: but works for now --%>
                        <asp:ValidationSummary ID="ValidationSummaryBase" runat="server" ValidationGroup="Base" />
                        <asp:ValidationSummary ID="ValidationSummaryClassic" runat="server" ValidationGroup="Classic" />
                        <asp:ValidationSummary ID="ValidationSummaryOpenId" runat="server" ValidationGroup="OpenId" />
                    </div>
                    <asp:Button ID="add" OnClick="add_Click" runat="server" Text='<%# resmgr.GetString("text_save_comment") %>' CssClass="commentViewControlStyle"></asp:Button>
                </td>
            </tr>
        </table>
    </div>
</div>
