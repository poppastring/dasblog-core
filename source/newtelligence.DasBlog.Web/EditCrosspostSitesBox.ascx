<%@ Register TagPrefix="cc1" Namespace="newtelligence.DasBlog.Web.Core.WebControls" Assembly="newtelligence.DasBlog.Web.Core" %>
<%@ Control Language="c#" AutoEventWireup="True" Codebehind="EditCrosspostSitesBox.ascx.cs" Inherits="newtelligence.DasBlog.Web.EditCrosspostSitesBox" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" debug="True"%>
<div class="bodyContentStyle">
	<div class="pageTopic">
		<asp:Label id=labelTitle runat="server" Text='<%# resmgr.GetString("text_crosspost_title") %>'>
		</asp:Label>
	</div>
	<div class="configSectionStyle">
		<asp:Label id=changesAlert Text='<%# resmgr.GetString("text_sites_not_saved") %>' runat="server" Visible="False" ForeColor="Red">
		</asp:Label>
		<cc1:ShadowBox id="shadowBox" style="MARGIN-TOP:10px" Width="100%" runat="server" ShadowDepth="6">
			<asp:datagrid id="crosspostSitesGrid" runat="server" Width="100%" GridLines="None" CellPadding="4"
				BorderWidth="1px" BorderStyle="Solid" BorderColor="#999999" PageSize="8" AutoGenerateColumns="False" onload="crosspostSitesGrid_Load">
				<SelectedItemStyle Font-Bold="True" ForeColor="White" BackColor="#000099"></SelectedItemStyle>
				<AlternatingItemStyle BackColor="WhiteSmoke"></AlternatingItemStyle>
				<HeaderStyle Font-Bold="True" CssClass="sidetitle"></HeaderStyle>
				<Columns>
					<asp:TemplateColumn HeaderText="Profile">
						<HeaderStyle Wrap="False" Width="85%"></HeaderStyle>
						<ItemTemplate>
							<asp:Label id=labelProfile Text='<%# System.Web.HttpUtility.HtmlEncode(TruncateString(DataBinder.Eval(Container, "DataItem.ProfileName").ToString(),30)) %>' runat="server" Width="100%" ToolTip='<%# DataBinder.Eval(Container, "DataItem.ProfileName").ToString()%>'>
							</asp:Label></P>
						</ItemTemplate>
						<EditItemTemplate>
							<table id="Table4" cellspacing="0" cellpadding="3" width="100%" border="0">
								<tr>
									<td valign="top" noWrap>
										<asp:Label id=labelBlogURL Text='<%# resmgr.GetString("text_blog_url") %>' runat="server">
										</asp:Label></td>
									<td>
										<P>
											<asp:TextBox id="textBoxBlogURL" runat="server" Width="99%"></asp:TextBox><br />
											example: http://dasblog.info/</P>
									</td>
								</tr>
								<tr>
									<td noWrap>
										<asp:Label id=labelServerSoftware Text='<%# resmgr.GetString("text_blog_software") %>' runat="server">
										</asp:Label></td>
									<td width="100%">
										<asp:DropDownList id=blogSoftware runat="server" DataValueField="Name" DataTextField="Name" DataSource="<%# GetServerInfo() %>">
										</asp:DropDownList>
										<asp:Button id="buttonAutoFill" Text=<%# resmgr.GetString("text_auto_fill") %> runat="server" CommandName="autoFill" CausesValidation="False"></asp:Button></td>
								</tr>
							</table>
							<hr>
							<table id="Table1" cellspacing="0" cellpadding="3" width="100%" border="0" runat="server">
								<tr>
									<td noWrap>
										<asp:Label id=labelProfileName Text='<%# resmgr.GetString("text_profile_name") %>' runat="server">
										</asp:Label></td>
									<td noWrap>
										<asp:TextBox id=textProfileName Text='<%# DataBinder.Eval(Container, "DataItem.ProfileName") %>' runat="server">
										</asp:TextBox>
										<asp:RequiredFieldValidator id="RequiredFieldValidator1" runat="server" ErrorMessage="The profile name is a required field"
											ControlToValidate="textProfileName">*</asp:RequiredFieldValidator></td>
									<td></td>
									<td noWrap></td>
								</tr>
								<tr>
									<td>
										<asp:Label id=labelHostName Text='<%# resmgr.GetString("text_hostname") %>' runat="server">
										</asp:Label></td>
									<td noWrap>
										<asp:TextBox id=textHostName Text='<%# DataBinder.Eval(Container, "DataItem.HostName") %>' runat="server">
										</asp:TextBox>
										<asp:RequiredFieldValidator id="RequiredFieldValidator2" runat="server" ErrorMessage="The host name is a required field"
											ControlToValidate="textHostName">*</asp:RequiredFieldValidator></td>
									<td>
										<asp:Label id=labelPort Text='<%# resmgr.GetString("text_port") %>' runat="server">
										</asp:Label></td>
									<td noWrap>
										<asp:TextBox id=textPort Text='<%# DataBinder.Eval(Container, "DataItem.Port") %>' runat="server" Width="50px">
										</asp:TextBox>
										<asp:RequiredFieldValidator id="RequiredFieldValidator3" runat="server" ErrorMessage="The port number is a required field"
											ControlToValidate="textPort">*</asp:RequiredFieldValidator>
										<asp:RangeValidator id="RangeValidator1" runat="server" ErrorMessage="The port number must be between 1 and 65535"
											ControlToValidate="textPort" MaximumValue="65535" MinimumValue="0" Type="Integer">*</asp:RangeValidator></td>
								</tr>
								<tr>
									<td>
										<asp:Label id=labelUsername Text='<%# resmgr.GetString("text_username") %>' runat="server">
										</asp:Label></td>
									<td noWrap>
										<asp:TextBox id=textUsername Text='<%# DataBinder.Eval(Container, "DataItem.Username") %>' runat="server">
										</asp:TextBox></td>
									<td>
										<asp:Label id=labelPasswordSet runat="server" Visible='<%# ((string)DataBinder.Eval(Container, "DataItem.Password")) != null &amp;&amp; ((string)DataBinder.Eval(Container, "DataItem.Password")).Length>0%>'>(set)</asp:Label></td>
									<td noWrap></td>
								</tr>
								<tr>
									<td>
										<asp:Label id=labelPassword Text='<%# resmgr.GetString("text_password") %>' runat="server">
										</asp:Label></td>
									<td noWrap>
										<asp:TextBox id=textPassword Text='<%# DataBinder.Eval(Container, "DataItem.Password") %>' runat="server" TextMode="Password">
										</asp:TextBox></td>
									<td>
										<asp:Label id=labelPasswordRepeat Text='<%# resmgr.GetString("text_password_repeat") %>' runat="server">
										</asp:Label></td>
									<td noWrap>
										<asp:TextBox id="textPasswordRepeat" runat="server" TextMode="Password"></asp:TextBox>
										<asp:CompareValidator id="CompareValidator1" runat="server" ErrorMessage="Passwords don't match" ControlToValidate="textPassword"
											ControlToCompare="textPasswordRepeat">*</asp:CompareValidator></td>
								</tr>
								<tr>
									<td>
										<asp:Label id=labelEndpoint Text='<%# resmgr.GetString("text_endpoint") %>' runat="server">
										</asp:Label></td>
									<td>
										<asp:TextBox id=textEndpoint Text='<%# DataBinder.Eval(Container, "DataItem.Endpoint") %>' runat="server">
										</asp:TextBox>
										<asp:RequiredFieldValidator id="RequiredFieldValidator4" runat="server" ErrorMessage="The endpoint is a required field"
											ControlToValidate="textEndpoint">*</asp:RequiredFieldValidator></td>
									<td>
										<asp:Label id=labelApiType Text='<%# resmgr.GetString("text_apitype") %>' runat="server">
										</asp:Label></td>
									<td>
										<asp:DropDownList id=listApiType runat="server" SelectedValue='<%# DataBinder.Eval(Container, "DataItem.ApiType") %>'>
											<asp:ListItem Value="blogger">Blogger</asp:ListItem>
											<asp:ListItem Value="metaweblog">MetaWeblog</asp:ListItem>
										</asp:DropDownList></td>
								</tr>
							</table>
							<hr>
							<table id="Table2" cellspacing="0" cellpadding="3" width="100%" border="0">
								<tr>
									<td width="10%">
										<asp:Button id=buttonTest Text='<%# resmgr.GetString("text_load_blogs") %>' runat="server" CommandName="testConnection">
										</asp:Button>
									</td>
									<%--
									luke@jurasource.co.uk 24-MAR-04
									Allow users to choose what blog to crosspost to, instead of just the first one in the list (previous behaviour)
									--%>
									<td>
										<asp:Label id=labelBlog Text='<%# resmgr.GetString("text_select_blog") %>' runat="server">
										</asp:Label>
									</td>
									<td>
										<asp:DropDownList ID="listAllBlogNames" runat="server" EnableViewState="True" Enabled="True" DataSource='<%# DataBinder.Eval(Container, "DataItem.AllBlogNames") %>'>
											<asp:ListItem Value=""></asp:ListItem>
										</asp:DropDownList>
									</td>
									<td>
										<asp:TextBox id=textBlogName Text='<%# DataBinder.Eval(Container, "DataItem.BlogName") %>' runat="server" Enabled="False">
										</asp:TextBox>
									</td>
									<td>
										<asp:TextBox id="textBlogId" Text='<%# DataBinder.Eval(Container, "DataItem.BlogId") %>' runat="server" Enabled="False">
										</asp:TextBox>
									</td>
								</tr>
							</table>
							<asp:Label id="labelTestError" runat="server" ForeColor="Red"></asp:Label>
							<asp:ValidationSummary id="ValidationSummary1" runat="server" DisplayMode="List"></asp:ValidationSummary>
						</EditItemTemplate>
					</asp:TemplateColumn>
					<asp:TemplateColumn>
						<HeaderStyle Width="15%"></HeaderStyle>
						<HeaderTemplate>
							<div style="white-space: nowrap;">
								<asp:ImageButton id=addButton runat="server" ToolTip='<%# resmgr.GetString("tooltip_item_add") %>' CommandName="AddItem" CausesValidation="False" AlternateText="Add" ImageUrl='<%# ((newtelligence.DasBlog.Web.Core.SharedBasePage)Page).GetThemedImageUrl("addbutton-list") %>'>
								</asp:ImageButton></div>
						</HeaderTemplate>
						<ItemTemplate>
							<div style="white-space: nowrap;">
								<asp:ImageButton id=ImageButton1 runat="server" ToolTip='<%# resmgr.GetString("tooltip_item_edit") %>' CommandName="Edit" CausesValidation="False" AlternateText="Edit" ImageUrl='<%# ((newtelligence.DasBlog.Web.Core.SharedBasePage)Page).GetThemedImageUrl("editbutton-list") %>'>
								</asp:ImageButton>&nbsp;
								<asp:ImageButton id=ImageButton2 runat="server" ToolTip='<%# resmgr.GetString("tooltip_item_delete") %>' CommandName="Delete" CausesValidation="False" AlternateText="Delete" ImageUrl='<%# ((newtelligence.DasBlog.Web.Core.SharedBasePage)Page).GetThemedImageUrl("deletebutton-list") %>'>
								</asp:ImageButton>&nbsp;&nbsp;&nbsp;
							</div>
						</ItemTemplate>
						<EditItemTemplate>
							<div style="white-space: nowrap;">
								<asp:ImageButton id=ImageButton3 runat="server" ToolTip='<%# resmgr.GetString("tooltip_item_ok") %>' CommandName="Update" AlternateText="Ok" ImageUrl='<%# ((newtelligence.DasBlog.Web.Core.SharedBasePage)Page).GetThemedImageUrl("okbutton-list") %>'>
								</asp:ImageButton>&nbsp;
								<asp:ImageButton id=ImageButton4 runat="server" ToolTip='<%# resmgr.GetString("tooltip_item_cancel") %>' CommandName="Cancel" CausesValidation="False" AlternateText="Cancel" ImageUrl='<%# ((newtelligence.DasBlog.Web.Core.SharedBasePage)Page).GetThemedImageUrl("undobutton-list") %>'>
								</asp:ImageButton>&nbsp;&nbsp;&nbsp;
							</div>
						</EditItemTemplate>
					</asp:TemplateColumn>
				</Columns>
				<PagerStyle HorizontalAlign="Center" Mode="NumericPages"></PagerStyle>
			</asp:datagrid>
		</cc1:ShadowBox>
		<P>
			<asp:Button id="buttonSaveChanges" runat="server" Text='<%# resmgr.GetString("text_save_changes") %>' onclick="buttonSaveChanges_Click">
			</asp:Button></P>
	</div>
</div>
