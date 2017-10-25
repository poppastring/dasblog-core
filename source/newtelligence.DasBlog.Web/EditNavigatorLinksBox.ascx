<%@ Control Language="c#" AutoEventWireup="True" Codebehind="EditNavigatorLinksBox.ascx.cs" Inherits="newtelligence.DasBlog.Web.EditNavigatorLinksBox" TargetSchema="http://schemas.microsoft.com/intellisense/ie5"%>
<%@ Register TagPrefix="cc1" Namespace="newtelligence.DasBlog.Web.Core.WebControls" Assembly="newtelligence.DasBlog.Web.Core" %>
<div class="bodyContentStyle">
	<div class="pageTopic">
		<asp:Label id=labelTitle runat="server" Text='<%# resmgr.GetString("text_navigatorlinks_title") %>'>
		</asp:Label>
	</div>
	<div class="configSectionStyle">
		<cc1:ShadowBox id="shadowBox" style="MARGIN-TOP:10px" Width="100%" runat="server" ShadowDepth="6">
<asp:datagrid id=navigatorLinksGrid runat="server" Width="100%" AutoGenerateColumns="False" PageSize="20" AllowPaging="True" BorderColor="#999999" BorderStyle="Solid" BorderWidth="1px" CellPadding="4" GridLines="None">
<SelectedItemStyle Font-Bold="True" ForeColor="White" BackColor="#000099">
</SelectedItemStyle>

<AlternatingItemStyle BackColor="WhiteSmoke">
</AlternatingItemStyle>

<HeaderStyle Font-Bold="True" CssClass="sidetitle">
</HeaderStyle>

<Columns>
<asp:TemplateColumn HeaderText='Navigator Links'>
<HeaderStyle Width="98%">
</HeaderStyle>

<ItemTemplate>
<P>
<asp:HyperLink id=hlList Text='<%# DataBinder.Eval(Container, "DataItem.Name") %>' runat="server" NavigateUrl='<%# DataBinder.Eval(Container, "DataItem.Url") %>'>
								</asp:HyperLink>&nbsp; </P>
</ItemTemplate>

<EditItemTemplate>
<table id=Table1 cellspacing=0 cellpadding=2 width="100%" border=0>
<tr>
<td style="HEIGHT: 29px" noWrap width="1%"><asp:Label runat="server" Text='<%# resmgr.GetString("text_title") %>' /></td>
<td style="HEIGHT: 29px">
<asp:TextBox id=textname Text='<%# DataBinder.Eval(Container, "DataItem.Name") %>' runat="server" Width="99%"></asp:TextBox></td></tr>
<tr>
<td style="white-space: nowrap; width:1%"><asp:Label ID="Label1" runat="server"  text='<%# resmgr.GetString("text_html_url") %>' /></td>
<td>
<asp:TextBox id=texturl Text='<%# DataBinder.Eval(Container, "DataItem.Url") %>' runat="server" Width="99%"></asp:TextBox></td></tr></table>
</EditItemTemplate>
</asp:TemplateColumn>
<asp:TemplateColumn>
<HeaderStyle Width="2%">
</HeaderStyle>

<HeaderTemplate>
							<div style="white-space: nowrap;">
								<asp:ImageButton id=addButton runat="server" ImageUrl='<%# ((newtelligence.DasBlog.Web.Core.SharedBasePage)Page).GetThemedImageUrl("addbutton-list") %>' CommandName="AddItem" ToolTip='<%# resmgr.GetString("tooltip_item_add") %>'>
								</asp:ImageButton></div>

</HeaderTemplate>

<ItemTemplate>
								<div style="white-space: nowrap;">
								<asp:ImageButton id=ImageButton1 runat="server" ImageUrl='<%# ((newtelligence.DasBlog.Web.Core.SharedBasePage)Page).GetThemedImageUrl("editbutton-list") %>' CommandName="Edit" AlternateText="Edit" CausesValidation="False" ToolTip='<%# resmgr.GetString("tooltip_item_edit") %>'>
								</asp:ImageButton>&nbsp;
								<asp:ImageButton id=ImageButton2 runat="server" ImageUrl='<%# ((newtelligence.DasBlog.Web.Core.SharedBasePage)Page).GetThemedImageUrl("deletebutton-list") %>' CommandName="Delete" AlternateText="Delete" CausesValidation="False" ToolTip='<%# resmgr.GetString("tooltip_item_delete") %>'>
								</asp:ImageButton>&nbsp;
								<asp:ImageButton id=ImageButton5 runat="server" ToolTip='<%# resmgr.GetString("tooltip_item_moveup") %>' CommandName="MoveUp" ImageUrl='<%# ((newtelligence.DasBlog.Web.Core.SharedBasePage)Page).GetThemedImageUrl("moveup-list") %>' AlternateText="Move Up">
								</asp:ImageButton>&nbsp;
								<asp:ImageButton id=ImageButton6 runat="server" ToolTip='<%# resmgr.GetString("tooltip_item_movedown") %>' CommandName="MoveDown" ImageUrl='<%# ((newtelligence.DasBlog.Web.Core.SharedBasePage)Page).GetThemedImageUrl("movedown-list") %>' AlternateText="Move Down">
								</asp:ImageButton></div>

</ItemTemplate>

<EditItemTemplate>
								<div style="white-space: nowrap;">
								<asp:ImageButton id=ImageButton3 runat="server" ImageUrl='<%# ((newtelligence.DasBlog.Web.Core.SharedBasePage)Page).GetThemedImageUrl("okbutton-list") %>' CommandName="Update" AlternateText="Ok" CausesValidation="False" ToolTip='<%# resmgr.GetString("tooltip_item_ok") %>'>
								</asp:ImageButton>&nbsp;
								<asp:ImageButton id=ImageButton4 runat="server" ImageUrl='<%# ((newtelligence.DasBlog.Web.Core.SharedBasePage)Page).GetThemedImageUrl("undobutton-list") %>' CommandName="Cancel" AlternateText="Cancel" CausesValidation="False" ToolTip='<%# resmgr.GetString("tooltip_item_cancel") %>'>
								</asp:ImageButton>&nbsp;
								<asp:ImageButton id=ImageButton7 runat="server" ToolTip='<%# resmgr.GetString("tooltip_item_moveup") %>' CommandName="MoveUp" ImageUrl='<%# ((newtelligence.DasBlog.Web.Core.SharedBasePage)Page).GetThemedImageUrl("moveup-list") %>' AlternateText="Move Up">
								</asp:ImageButton>&nbsp;
								<asp:ImageButton id=ImageButton8 runat="server" ToolTip='<%# resmgr.GetString("tooltip_item_movedown") %>' CommandName="MoveDown" ImageUrl='<%# ((newtelligence.DasBlog.Web.Core.SharedBasePage)Page).GetThemedImageUrl("movedown-list") %>' AlternateText="Move Down">
								</asp:ImageButton></div>

</EditItemTemplate>
</asp:TemplateColumn>
</Columns>

<PagerStyle HorizontalAlign="Center" Mode="NumericPages">
</PagerStyle>
</asp:datagrid>
		</cc1:ShadowBox>
	</div>
</div>
