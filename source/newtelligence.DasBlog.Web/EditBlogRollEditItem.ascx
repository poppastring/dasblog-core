<%@ Control Language="c#" AutoEventWireup="True" Codebehind="EditBlogRollEditItem.ascx.cs" Inherits="newtelligence.DasBlog.Web.EditBlogRollEditItem" TargetSchema="http://schemas.microsoft.com/intellisense/ie5"%>
<%@ Register TagPrefix="blogcore" Namespace="newtelligence.DasBlog.Web.Core" Assembly="newtelligence.DasBlog.Web.Core" %>
<%@ Register TagPrefix="uc1" TagName="EditBlogRollItem" Src="EditBlogRollItem.ascx" %>
<table id="itemEditTable" cellspacing="0" cellpadding="2" width="100%" border="0" style="MARGIN-BOTTOM: 20px">
	<tr>
		<td style="FONT-SIZE: smaller" nowrap="nowrap" width="1%">
			<asp:Label id=labelTitle Text='<%# resmgr.GetString("text_title") %>' runat="server">
			</asp:Label></td>
		<td>
			<asp:TextBox id="textTitle" runat="server" Font-Size="Smaller" Text="" Width="100%"></asp:TextBox></td>
		<td width="55" rowspan="4" valign="bottom" align="right"><asp:ImageButton id="buttonAddItem" runat="server" ImageUrl='<%# ((newtelligence.DasBlog.Web.Core.SharedBasePage)Page).GetThemedImageUrl("addbutton-list") %>' CommandName="AddSubitem"
				AlternateText="Add Entry" ToolTip='<%# resmgr.GetString("tooltip_item_add") %>'></asp:ImageButton></td>
	</tr>
	<tr>
		<td style="FONT-SIZE: smaller">
			<asp:Label id=labelDescription Text='<%# resmgr.GetString("text_description") %>' runat="server">
			</asp:Label></td>
		<td>
			<asp:TextBox id="textDescription" runat="server" Font-Size="Smaller" Text="" Width="100%"></asp:TextBox></td>
	</tr>
	<tr>
		<td style="FONT-SIZE: smaller" nowrap="nowrap" width="1%">
			<asp:Label id=labelHtmlUrl Text='<%# resmgr.GetString("text_html_url") %>' runat="server">
			</asp:Label></td>
		<td style="HEIGHT: 27px">
			<asp:TextBox id="textHtmlUrl" runat="server" Font-Size="Smaller" Text="" Width="100%"></asp:TextBox></td>
	</tr>
	<tr>
		<td style="FONT-SIZE: smaller">
			<asp:Label id=labelXmlUrl Text='<%# resmgr.GetString("text_xml_url") %>' runat="server">
			</asp:Label></td>
		<td>
			<asp:TextBox id="textXmlUrl" runat="server" Font-Size="Smaller" Text="" Width="100%"></asp:TextBox></td>
	</tr>
</table>
<asp:panel id="multiItemPanel" runat="server" Width="100%">
	<asp:datagrid id="blogRollGrid" runat="server" Width="100%" AutoGenerateColumns="False" AllowPaging="True"
		BorderColor="#999999" BorderStyle="Solid" BorderWidth="0px" CellPadding="4" GridLines="None"
		ShowHeader="false">
		<SelectedItemStyle Font-Bold="True" ForeColor="White" BackColor="#000099"></SelectedItemStyle>
		<AlternatingItemStyle BackColor="WhiteSmoke"></AlternatingItemStyle>
		<HeaderStyle Font-Bold="True" BackColor="WhiteSmoke"></HeaderStyle>
		<Columns>
			<asp:TemplateColumn HeaderText="Blogroll Feeds">
				<HeaderStyle Width="98%"></HeaderStyle>
				<HeaderTemplate>
				</HeaderTemplate>
				<ItemTemplate>
					<uc1:EditBlogRollItem id=editBlogRollItem title='<%# DataBinder.Eval(Container,"DataItem.title")%>' runat="server" HtmlUrl='<%# DataBinder.Eval(Container,"DataItem.htmlUrl")%>' XmlUrl='<%# DataBinder.Eval(Container,"DataItem.xmlUrl")%>' Description='<%# DataBinder.Eval(Container,"DataItem.description")%>' Outline='<%# DataBinder.Eval(Container,"DataItem.outline")%>'>
					</uc1:EditBlogRollItem>
				</ItemTemplate>
				<EditItemTemplate>
					<blogcore:NamingPanel id="nestedEdit" runat="server"></blogcore:NamingPanel>
				</EditItemTemplate>
			</asp:TemplateColumn>
			<asp:TemplateColumn>
				<HeaderStyle Width="55px"></HeaderStyle>
				<ItemStyle VerticalAlign="Top"></ItemStyle>
				<ItemTemplate>
					<div align="right">
						<asp:ImageButton id=ImageButton1 runat="server" ToolTip='<%# resmgr.GetString("tooltip_item_edit") %>' AlternateText="Edit" CommandName="Edit" ImageUrl='<%# ((newtelligence.DasBlog.Web.Core.SharedBasePage)Page).GetThemedImageUrl("editbutton-list") %>' CausesValidation="False">
						</asp:ImageButton>&nbsp;
						<asp:ImageButton id=ImageButton2 runat="server" ToolTip='<%# resmgr.GetString("tooltip_item_delete") %>' AlternateText="Delete" CommandName="Delete" ImageUrl='<%# ((newtelligence.DasBlog.Web.Core.SharedBasePage)Page).GetThemedImageUrl("deletebutton-list") %>' CausesValidation="False">
						</asp:ImageButton></div>
				</ItemTemplate>
				<EditItemTemplate>
					<div align="right">
						<asp:ImageButton id=ImageButton3 runat="server" ToolTip='<%# resmgr.GetString("tooltip_item_ok") %>' AlternateText="Ok" CommandName="Update" ImageUrl='<%# ((newtelligence.DasBlog.Web.Core.SharedBasePage)Page).GetThemedImageUrl("okbutton-list") %>' CausesValidation="False">
						</asp:ImageButton>&nbsp;
						<asp:ImageButton id=ImageButton4 runat="server" ToolTip='<%# resmgr.GetString("tooltip_item_cancel") %>' AlternateText="Cancel" CommandName="Cancel" ImageUrl='<%# ((newtelligence.DasBlog.Web.Core.SharedBasePage)Page).GetThemedImageUrl("undobutton-list") %>' CausesValidation="False">
						</asp:ImageButton></div>
				</EditItemTemplate>
			</asp:TemplateColumn>
		</Columns>
		<PagerStyle HorizontalAlign="Center" Mode="NumericPages"></PagerStyle>
	</asp:datagrid>
</asp:panel>
