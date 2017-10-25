<%@ Register TagPrefix="cc1" Namespace="newtelligence.DasBlog.Web.Core.WebControls" Assembly="newtelligence.DasBlog.Web.Core" %>
<%@ Register TagPrefix="ftb" Namespace="FreeTextBoxControls" Assembly="FreeTextBox" %>
<%@ Control Language="c#" AutoEventWireup="True" Codebehind="EditContentFiltersBox.ascx.cs" Inherits="newtelligence.DasBlog.Web.EditContentFiltersBox" TargetSchema="http://schemas.microsoft.com/intellisense/ie5"%>
<div class="bodyContentStyle">
	<div class="pageTopic">
		<asp:Label id=labelTitle runat="server" Text='<%# resmgr.GetString("text_filters_title") %>'>
		</asp:Label>
	</div>
	<div class="configSectionStyle">
		<asp:CheckBox id=checkFilterHtml Text='<%# resmgr.GetString("text_filters_apply_web") %>' runat="server">
		</asp:CheckBox>&nbsp;
		<asp:CheckBox id=checkFilterRSS Text='<%# resmgr.GetString("text_filters_apply_xml") %>' runat="server">
		</asp:CheckBox>
		<hr />
		<asp:Label id=Label3 Text='<%# resmgr.GetString("text_filters_warning") %>' runat="server" Font-Size="Smaller">
		</asp:Label><br />
		<asp:Label id="postProcessorNotice" Text='<%# resmgr.GetString("text_filters_postProcessors") %>' runat="server" Font-Size="Smaller">
		</asp:Label><br />
		<asp:Label id=changesAlert Text='<%# resmgr.GetString("text_filters_not_saved") %>' runat="server" ForeColor="Red" Visible="False">
		</asp:Label><br />
		<cc1:ShadowBox id="shadowBox" style="MARGIN-TOP:10px" Width="100%" runat="server" ShadowDepth="6">
			<asp:datagrid id="contentFiltersGrid" runat="server" Width="100%" GridLines="None" CellPadding="4"
				BorderWidth="1px" BorderStyle="Solid" BorderColor="#999999" AllowPaging="True" PageSize="8"
				AutoGenerateColumns="False">
				<SelectedItemStyle Font-Bold="True" ForeColor="White" BackColor="#000099"></SelectedItemStyle>
				<AlternatingItemStyle BackColor="WhiteSmoke"></AlternatingItemStyle>
				<HeaderStyle Font-Bold="True" CssClass="sidetitle"></HeaderStyle>
				<Columns>
					<asp:TemplateColumn HeaderText="Find">
						<HeaderStyle Wrap="False"></HeaderStyle>
						<ItemTemplate>
							<asp:Label id=Label1 runat="server" Width="100%" Text='<%# System.Web.HttpUtility.HtmlEncode(TruncateString(DataBinder.Eval(Container, "DataItem.Expression").ToString(),50)) %>' ToolTip='<%# DataBinder.Eval(Container, "DataItem.Expression").ToString()%>'>
							</asp:Label>
							<br />
							<asp:CheckBox id=CheckBox1 runat="server" Enabled="False" Checked='<%# DataBinder.Eval(Container, "DataItem.IsRegEx") %>'  Text='<%# resmgr.GetString("text_filters_col_isregex") %>'>
							</asp:CheckBox>
						</ItemTemplate>
						<EditItemTemplate>
							<asp:TextBox id=textExpression runat="server" Width="95%"  Text='<%# DataBinder.Eval(Container, "DataItem.Expression") %>'>
							</asp:TextBox>
							<br />
							<asp:CheckBox id=checkRegEx runat="server" Checked='<%# DataBinder.Eval(Container, "DataItem.IsRegEx") %>' Text='<%# resmgr.GetString("text_filters_col_isregex") %>'>
							</asp:CheckBox>
						</EditItemTemplate>
					</asp:TemplateColumn>
					<asp:TemplateColumn HeaderText="Replace with">
						<HeaderStyle Wrap="False"></HeaderStyle>
						<ItemTemplate>
							<asp:Label id=Label2 runat="server" Width="100%" Text='<%# System.Web.HttpUtility.HtmlEncode(TruncateString(DataBinder.Eval(Container, "DataItem.MapTo").ToString(),60)) %>' ToolTip='<%# DataBinder.Eval(Container, "DataItem.MapTo").ToString() %>'>
							</asp:Label>
							<br />
							&nbsp;
						</ItemTemplate>
						<EditItemTemplate>
							<asp:TextBox id=textMapTo runat="server" Width="95%" Text='<%# DataBinder.Eval(Container, "DataItem.MapTo") %>'>
							</asp:TextBox>
							<br />
							&nbsp;
						</EditItemTemplate>
					</asp:TemplateColumn>
					<asp:TemplateColumn>
						<ItemTemplate>
						</ItemTemplate>
						<EditItemTemplate>
						</EditItemTemplate>
					</asp:TemplateColumn>
					<asp:TemplateColumn>
						<HeaderStyle Width="2%"></HeaderStyle>
						<HeaderTemplate>
							<div style="white-space: nowrap;">
								<asp:ImageButton id=addButton runat="server" ToolTip='<%# resmgr.GetString("tooltip_item_add") %>' CommandName="AddItem" ImageUrl='<%# ((newtelligence.DasBlog.Web.Core.SharedBasePage)Page).GetThemedImageUrl("addbutton-list") %>' AlternateText="Add">
								</asp:ImageButton></div>
						</HeaderTemplate>
						<ItemTemplate>
							<div style="white-space: nowrap;">
								<asp:ImageButton id=ImageButton1 runat="server" ToolTip='<%# resmgr.GetString("tooltip_item_edit") %>' CommandName="Edit" ImageUrl='<%# ((newtelligence.DasBlog.Web.Core.SharedBasePage)Page).GetThemedImageUrl("editbutton-list") %>' CausesValidation="False" AlternateText="Edit">
								</asp:ImageButton>&nbsp;
								<asp:ImageButton id=ImageButton2 runat="server" ToolTip='<%# resmgr.GetString("tooltip_item_delete") %>' CommandName="Delete" ImageUrl='<%# ((newtelligence.DasBlog.Web.Core.SharedBasePage)Page).GetThemedImageUrl("deletebutton-list") %>' CausesValidation="False" AlternateText="Delete">
								</asp:ImageButton>&nbsp;
								<asp:ImageButton id=ImageButton5 runat="server" ToolTip='<%# resmgr.GetString("tooltip_item_moveup") %>' CommandName="MoveUp" ImageUrl='<%# ((newtelligence.DasBlog.Web.Core.SharedBasePage)Page).GetThemedImageUrl("moveup-list") %>' AlternateText="Move Up">
								</asp:ImageButton>&nbsp;
								<asp:ImageButton id=ImageButton6 runat="server" ToolTip='<%# resmgr.GetString("tooltip_item_movedown") %>' CommandName="MoveDown" ImageUrl='<%# ((newtelligence.DasBlog.Web.Core.SharedBasePage)Page).GetThemedImageUrl("movedown-list") %>' AlternateText="Move Down">
								</asp:ImageButton></div>
						</ItemTemplate>
						<EditItemTemplate>
							<div style="white-space: nowrap;">
								<asp:ImageButton id=ImageButton3 runat="server" ToolTip='<%# resmgr.GetString("tooltip_item_ok") %>' CommandName="Update" ImageUrl='<%# ((newtelligence.DasBlog.Web.Core.SharedBasePage)Page).GetThemedImageUrl("okbutton-list") %>' CausesValidation="False" AlternateText="Ok">
								</asp:ImageButton>&nbsp;
								<asp:ImageButton id=ImageButton4 runat="server" ToolTip='<%# resmgr.GetString("tooltip_item_cancel") %>' CommandName="Cancel" ImageUrl='<%# ((newtelligence.DasBlog.Web.Core.SharedBasePage)Page).GetThemedImageUrl("undobutton-list") %>' CausesValidation="False" AlternateText="Cancel">
								</asp:ImageButton>&nbsp;
								<asp:ImageButton id=ImageButton7 runat="server" ToolTip='<%# resmgr.GetString("tooltip_item_moveup") %>' CommandName="MoveUp" ImageUrl='<%# ((newtelligence.DasBlog.Web.Core.SharedBasePage)Page).GetThemedImageUrl("moveup-list") %>' AlternateText="Move Up">
								</asp:ImageButton>&nbsp;
								<asp:ImageButton id=ImageButton8 runat="server" ToolTip='<%# resmgr.GetString("tooltip_item_movedown") %>' CommandName="MoveDown" ImageUrl='<%# ((newtelligence.DasBlog.Web.Core.SharedBasePage)Page).GetThemedImageUrl("movedown-list") %>' AlternateText="Move Down">
								</asp:ImageButton></div>
						</EditItemTemplate>
					</asp:TemplateColumn>
				</Columns>
				<PagerStyle HorizontalAlign="Center" Mode="NumericPages"></PagerStyle>
			</asp:datagrid>
		</cc1:ShadowBox>
		<div class="FreeTextboxStyle"><asp:PlaceHolder ID="editControlHolder" Runat="server"></asp:PlaceHolder></div>
		<cc1:ShadowBox id="sampleContentBox" Width="100%" runat="server" ShadowDepth="5" BorderWidth="1px"
			BorderStyle="Dotted">
			<div align="right">
				<asp:Panel id="previewPanel" runat="server" Width="100%" BorderWidth="1px" BorderStyle="Dotted"
					HorizontalAlign="Left"></asp:Panel>
				<asp:Button id=buttonFilterResults Text='<%# resmgr.GetString("text_filters_view_results") %>' runat="server" onclick="buttonFilterResults_Click">
				</asp:Button></div>
		</cc1:ShadowBox>
		<p>
			<asp:Button id="buttonSaveChanges" runat="server" Text='<%# resmgr.GetString("text_save_changes") %>' onclick="buttonSaveChanges_Click">
			</asp:Button></p>
	</div>
</div>
