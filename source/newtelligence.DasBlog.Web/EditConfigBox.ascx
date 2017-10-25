<%@ Control Language="c#" AutoEventWireup="True" Codebehind="EditConfigBox.ascx.cs" Inherits="newtelligence.DasBlog.Web.EditConfigBox" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>

<div class="bodyContentStyle">
	<div class="pageTopic">
		<asp:Label ID="Label12" runat="server" Text='<%# resmgr.GetString("text_config_title") %>'>
		</asp:Label></div>
	<asp:ValidationSummary ID="validationSummary" runat="server" HeaderText='<%# resmgr.GetString("text_config_errors_title") %>'>
	</asp:ValidationSummary>
	<div class="configSectionStyle">
		<div class="configSectionTitleStyle">
			<asp:Label ID="lBasicSettings" runat="server" Text='<%# resmgr.GetString("text_config_basic_settings") %>'>
			</asp:Label></div>
		<table cellspacing="0" cellpadding="2" border="0">
			<tr>
				<td class="configLabelColumnStyle" nowrap="nowrap">
					<asp:Label ID="labelTitle" runat="server" Text='<%# resmgr.GetString("text_title") %>' CssClass="configLabelStyle">
					</asp:Label><asp:RequiredFieldValidator ID="validatorRFTitle" runat="server" ErrorMessage="'Title' is a required field and cannot be empty."
						Display="Dynamic" ControlToValidate="textTitle">*</asp:RequiredFieldValidator></td>
				<td>
					<asp:TextBox ID="textTitle" AccessKey="T" runat="server" CssClass="configControlStyle" MaxLength="120"
						Columns="40"></asp:TextBox></td>
			</tr>
			<tr>
				<td class="configLabelColumnStyle" nowrap="nowrap">
					<asp:Label ID="labelSubtitle" runat="server" Text='<%# resmgr.GetString("text_subtitle") %>' CssClass="configLabelStyle">
					</asp:Label></td>
				<td>
					<asp:TextBox ID="textSubtitle" runat="server" CssClass="configControlStyle" MaxLength="120" Columns="40"></asp:TextBox></td>
			</tr>
			<tr>
				<td class="configLabelColumnStyle" nowrap="nowrap">
					<asp:Label ID="labelContact" runat="server" Text='<%# resmgr.GetString("text_contact_email") %>' CssClass="configLabelStyle">
					</asp:Label><asp:RequiredFieldValidator ID="validatorRFContact" runat="server" ErrorMessage="'Contact Email' is a required field and cannot be empty."
						Display="Dynamic" ControlToValidate="textContact">*</asp:RequiredFieldValidator><asp:RegularExpressionValidator ID="validatorREContact" runat="server" ErrorMessage="The given Email address does not appear to be valid."
						Display="Dynamic" ControlToValidate="textContact" ValidationExpression="\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*">*</asp:RegularExpressionValidator></td>
				<td>
					<asp:TextBox ID="textContact" runat="server" CssClass="configControlStyle" MaxLength="120" Columns="40"></asp:TextBox></td>
			</tr>
			<tr>
				<td class="configLabelColumnStyle" nowrap="nowrap">
					<asp:Label ID="labelRoot" runat="server" Text='<%# resmgr.GetString("text_external_base_url") %>' CssClass="configLabelStyle">
					</asp:Label><asp:RequiredFieldValidator ID="validatorRFRoot" runat="server" ErrorMessage="'External Base Url' is a required field and cannot be empty."
						Display="Dynamic" ControlToValidate="textRoot">*</asp:RequiredFieldValidator><asp:RegularExpressionValidator ID="validatorRERoot" runat="server" ErrorMessage="The base Url is not in a valid format. It must be a valid http Url like 'http://server/blogdir'"
						Display="Dynamic" ControlToValidate="textRoot" ValidationExpression="https?://([-\w\.]+)+(:\d+)?(/([-\w/_\.]*(\?\S+)?)?)?">*</asp:RegularExpressionValidator></td>
				<td>
					<asp:TextBox ID="textRoot" AccessKey="U" runat="server" CssClass="configControlStyle" MaxLength="120"
						Columns="40"></asp:TextBox></td>
			</tr>
			<tr>
				<td class="configLabelColumnStyle" nowrap="nowrap">
					<asp:Label ID="labelCopyright" runat="server" Text='<%# resmgr.GetString("text_copyright_owner") %>' CssClass="configLabelStyle">
					</asp:Label><asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ErrorMessage="'Copyright Owner' is a required field and cannot be empty."
						Display="Dynamic" ControlToValidate="textCopyright">*</asp:RequiredFieldValidator></td>
				<td>
					<asp:TextBox ID="textCopyright" AccessKey="Y" runat="server" CssClass="configControlStyle" MaxLength="120"
						Columns="40"></asp:TextBox></td>
			</tr>
			<tr>
				<td class="configLabelColumnStyle" nowrap="nowrap">
					<asp:Label ID="labelTextPassword" runat="server" Text='<%# resmgr.GetString("text_password")%>' CssClass="configLabelStyle">
					</asp:Label><asp:CompareValidator ID="ComparevalidatorPassword" runat="server" ErrorMessage="The passwords do not match"
						Display="Dynamic" ControlToValidate="textConfirmPassword" ControlToCompare="textPassword">*</asp:CompareValidator></td>
				<td>
					<asp:TextBox ID="textPassword" AccessKey="Y" runat="server" CssClass="configControlStyle" MaxLength="40"
						Columns="10" TextMode="Password"></asp:TextBox>&nbsp;
					<asp:Label ID="labelTextConfirmPassword" runat="server" Text='<%# resmgr.GetString("text_password_repeat")%>' CssClass="configLabelStyle">
					</asp:Label><asp:TextBox ID="textConfirmPassword" runat="server" CssClass="configControlStyle" MaxLength="40"
						Columns="10" TextMode="Password"></asp:TextBox></td>
			</tr>
			<tr>
				<td>
				</td>
				<td class="configLabelColumnStyle" nowrap="nowrap">
					<asp:CheckBox ID="checkEnableEncryptLoginPassword" runat="server" Text='<%# resmgr.GetString("text_config_encrypt_passwords")%>' CssClass="configLabelStyle">
					</asp:CheckBox></td>
			</tr>
		</table>
	</div>
	<div class="configSectionStyle">
		<div class="configSectionTitleStyle">
			<asp:Label ID="lServices" runat="server" Text='<%# resmgr.GetString("text_config_services") %>'>
			</asp:Label></div>
		<table id="Table2" cellspacing="0" cellpadding="2" border="0">
			<tr>
				<td class="configLabelColumnStyle" nowrap="nowrap">
					<asp:CheckBox ID="checkEnableAutoPingback" runat="server" Text='<%# resmgr.GetString("text_config_auto_pingback") %>' CssClass="configControlStyle">
					</asp:CheckBox></td>
				<td class="configLabelColumnStyle" nowrap="nowrap">
					<asp:CheckBox ID="checkEnableEditService" runat="server" Text='<%# resmgr.GetString("text_config_edit_web_service") %>' CssClass="configControlStyle">
					</asp:CheckBox></td>
			</tr>
			<tr>
				<td class="configLabelColumnStyle" nowrap="nowrap">
					<asp:CheckBox ID="checkEnableClickThrough" runat="server" Text='<%# resmgr.GetString("text_config_enable_clickthrough") %>' CssClass="configControlStyle">
					</asp:CheckBox></td>
				<td class="configLabelColumnStyle" nowrap="nowrap">
					<asp:CheckBox ID="checkEnableCommentApi" runat="server" Text='<%# resmgr.GetString("text_config_comment_api") %>' CssClass="configControlStyle">
					</asp:CheckBox></td>
			</tr>
			<tr>
				<td class="configLabelColumnStyle" nowrap="nowrap">
					<asp:CheckBox ID="checkEnableCrosspost" runat="server" Text='<%# resmgr.GetString("text_config_enable_crosspost") %>' CssClass="configControlStyle">
					</asp:CheckBox></td>
				<td class="configLabelColumnStyle" nowrap="nowrap">
					<asp:CheckBox ID="checkEnableConfigEditService" runat="server" Text='<%# resmgr.GetString("text_config_config_web_service") %>' CssClass="configControlStyle">
					</asp:CheckBox></td>
		    </tr>
			<tr>
				<td class="configLabelColumnStyle" nowrap="nowrap">
					<asp:CheckBox ID="checkEnableAggregatorBugging" runat="server" Text='<%# resmgr.GetString("text_config_enable_aggregator_bugging") %>' CssClass="configControlStyle">
					</asp:CheckBox></td>
				<td class="configLabelColumnStyle" nowrap="nowrap">
					<asp:CheckBox ID="checkEnableTrackbackService" runat="server" Text='<%# resmgr.GetString("text_config_trackback_service") %>' CssClass="configControlStyle">
					</asp:CheckBox></td>
			</tr>
			<tr>
				<td class="configLabelColumnStyle" nowrap="nowrap">
					<asp:CheckBox ID="checkEnableBloggerApi" runat="server" Text='<%# resmgr.GetString("text_config_blogger_api") %>' CssClass="configControlStyle">
					</asp:CheckBox></td>
				<td class="configLabelColumnStyle" nowrap="nowrap">
					<asp:CheckBox ID="checkEnablePingbackService" runat="server" Text='<%# resmgr.GetString("text_config_pingback_service") %>' CssClass="configControlStyle">
					</asp:CheckBox></td>
			</tr>
			<tr>
				<td class="configLabelColumnStyle" nowrap="nowrap">
					<asp:CheckBox ID="checkEnableAutoSave" runat="server" Text='<%# resmgr.GetString("text_config_enable_auto_save") %>' CssClass="configControlStyle">
					</asp:CheckBox></td>
				<td class="configLabelColumnStyle" nowrap="nowrap">
					<asp:CheckBox ID="checkEnableEntryReferral" runat="server" Text='<%# resmgr.GetString("text_config_enable_entry_referral") %>' CssClass="configControlStyle">
					</asp:CheckBox></td>
			</tr>
		</table>
	</div>
	<div class="configSectionStyle">
		<div class="configSectionTitleStyle">
			<asp:Label ID="labelCommentSettings" runat="server" Text='<%# resmgr.GetString("text_config_comment_settings") %>'>
			</asp:Label></div>
		<table cellspacing="0" cellpadding="2" border="0">
			<tr>
				<td class="configLabelColumnStyle" style="HEIGHT: 21px" nowrap="nowrap">
					<asp:CheckBox ID="checkEnableComments" runat="server" Text='<%# resmgr.GetString("text_config_comments") %>' CssClass="configControlStyle">
					</asp:CheckBox></td>
				<td class="configLabelColumnStyle" style="HEIGHT: 21px" nowrap="nowrap">
					<asp:CheckBox ID="checkCommentsRequireApproval" runat="server" Text='<%# resmgr.GetString("text_config_require_comment_approval") %>' CssClass="configControlStyle">
					</asp:CheckBox></td>
			</tr>
			<tr>
				<td colspan="2" class="configLabelColumnStyle" style="HEIGHT: 21px" nowrap="nowrap">
					<asp:CheckBox ID="checkEnableCoComment" runat="server" Text='<%# resmgr.GetString("text_config_enablecocomment") %>' CssClass="configControlStyle">
					</asp:CheckBox></td>
			</tr>
			<tr>
				<td class="configLabelColumnStyle" colspan="2">
					<fieldset>
						<legend>
							<%# resmgr.GetString("text_days_expire_field")%>
						</legend>
						<table>
							<tr>
								<td class="configLabelColumnStyle" style="HEIGHT: 21px" nowrap="nowrap">
									<asp:CheckBox ID="checkEnableCommentDays" runat="server" Text='<%# resmgr.GetString("text_config_enable_commentdayslimit") %>' CssClass="configControlStyle">
									</asp:CheckBox>&nbsp;<asp:TextBox ID="textDaysCommentsAllowed" runat="server" CssClass="configControlStyle" Enabled="False"
										MaxLength="6" Width="72px"></asp:TextBox>&nbsp;<asp:Label ID="labelDaysCommentsAllowed" runat="server" Text='<%# resmgr.GetString("text_days_comments_allowed")%>' CssClass="configLabelStyle">
									</asp:Label></td>
							</tr>
						</table>
					</fieldset>
				</td>
			</tr>
			<tr>
				<td class="configLabelColumnStyle" colspan="2">
					<fieldset>
						<legend>
							<%# resmgr.GetString("text_gravatar_field")%>
						</legend>
						<table>
							<tr>
								<td class="configLabelColumnStyle" style="HEIGHT: 21px" nowrap="nowrap" colspan="2">
									<asp:CheckBox ID="checkEnableGravatar" runat="server" Text='<%# resmgr.GetString("text_config_enable_gravatar") %>' CssClass="configControlStyle">
									</asp:CheckBox></td>
							</tr>
							<tr>
								<td class="configLabelColumnStyle" style="HEIGHT: 21px" nowrap="nowrap">
									<asp:Label ID="lblGravatarSize" runat="server" Text='<%# resmgr.GetString("text_config_gravatar_size")%>' CssClass="configLabelStyle">
									</asp:Label></td>
								<td>
									<asp:TextBox ID="textGravatarSize" runat="server" CssClass="configControlStyle" Enabled="False"></asp:TextBox></td>
							</tr>
							<tr>
								<td class="configLabelColumnStyle" style="HEIGHT: 21px" nowrap="nowrap">
									<asp:Label ID="lblGravatarBorderColor" runat="server" Text='<%# resmgr.GetString("text_config_gravatar_border")%>' CssClass="configLabelStyle">
									</asp:Label></td>
								<td>
									<asp:TextBox ID="textGravatarBorderColor" runat="server" CssClass="configControlStyle" Enabled="False"></asp:TextBox></td>
							</tr>
							<tr>
								<td class="configLabelColumnStyle" style="HEIGHT: 21px" nowrap="nowrap">
									<asp:Label ID="lblNoGravatar" runat="server" Text='<%# resmgr.GetString("text_config_no_gravatar_img")%>' CssClass="configLabelStyle">
									</asp:Label></td>
								<td>
									<asp:TextBox ID="textNoGravatarPath" runat="server" CssClass="configControlStyle" Enabled="False"></asp:TextBox></td>
							</tr>
							<tr>
								<td class="configLabelColumnStyle" style="HEIGHT: 21px" nowrap="nowrap">
									<asp:Label ID="lblGravatarRating" runat="server" Text='<%# resmgr.GetString("text_config_gravatar_rating")%>' CssClass="configLabelStyle">
									</asp:Label></td>
								<td>
									<asp:DropDownList ID="dropGravatarRating" runat="server" Enabled="False">
										<asp:ListItem Value="G">G</asp:ListItem>
										<asp:ListItem Value="PG">PG</asp:ListItem>
										<asp:ListItem Value="R" Selected="True">R</asp:ListItem>
										<asp:ListItem Value="X">X</asp:ListItem>
									</asp:DropDownList></td>
							</tr>
						</table>
					</fieldset>
				</td>
			</tr>
			<tr>
				<td class="configLabelColumnStyle" colspan="2">
					<fieldset>
						<legend>
							<%# resmgr.GetString("text_config_georss")%>
						</legend>
						<table>
							<tr>
								<td class="configLabelColumnStyle" style="HEIGHT: 21px" nowrap="nowrap" colspan="2">
									<asp:CheckBox ID="checkEnableGeoRss" runat="server" Text='<%# resmgr.GetString("text_config_enable_georss") %>' CssClass="configControlStyle">
									</asp:CheckBox></td>
							</tr>
							<tr>
							    <td class="configLabelColumnStyle" colspan="2">
									<asp:CheckBox ID="checkEnableDefaultLatLongForNonGeoCodedPosts" runat="server" Text='<%# resmgr.GetString("text_config_enable_defaultlatlongfornongeocodedposts") %>' CssClass="configControlStyle">
									</asp:CheckBox>
								</td>
							</tr>
							<tr>
								<td class="configLabelColumnStyle" style="HEIGHT: 21px" nowrap="nowrap">
									<asp:Label ID="lblDefaultLat" runat="server" Text='<%# resmgr.GetString("text_config_default_lat") %>' CssClass="configLabelStyle"></asp:Label></td>
								<td>
									<asp:TextBox ID="textDefaultLatitude" runat="server" CssClass="configControlStyle" Enabled="true"></asp:TextBox><asp:CustomValidator
										ID="cvDefaultLatitude" runat="server" ControlToValidate="textDefaultLatitude" Display="Dynamic" ErrorMessage='<%# resmgr.GetString("text_error_invalid_latitude") %>'
										OnServerValidate="cvDefaultLatitude_ServerValidate" ValidateEmptyText="True">*</asp:CustomValidator></td>
							</tr>
							<tr>
								<td class="configLabelColumnStyle" style="HEIGHT: 21px" nowrap="nowrap">
									<asp:Label ID="lblDefaultLong" runat="server" Text='<%# resmgr.GetString("text_config_default_long")%>' CssClass="configLabelStyle">
									</asp:Label></td>
								<td>
									<asp:TextBox ID="textDefaultLongitude" runat="server" CssClass="configControlStyle" Enabled="true"></asp:TextBox><asp:CustomValidator
										ID="cvDefaultLongitude" runat="server" ControlToValidate="textDefaultLongitude" Display="Dynamic" ErrorMessage='<%# resmgr.GetString("text_error_invalid_longitude") %>'
										OnServerValidate="cvDefaultLongitude_ServerValidate" ValidateEmptyText="True">*</asp:CustomValidator></td>
							</tr>
							<tr>
							    <td class="configLabelColumnStyle" colspan="4">&nbsp;</td>
							</tr>
							<tr>
								<td class="configLabelColumnStyle" style="HEIGHT: 21px" nowrap="nowrap" colspan="2">
									<asp:CheckBox ID="checkEnableGoogleMaps" runat="server" Text='<%# resmgr.GetString("text_config_enable_google_maps") %>' CssClass="configControlStyle">
									</asp:CheckBox></td>
							</tr>
							<tr>
								<td class="configLabelColumnStyle" style="HEIGHT: 21px" nowrap="nowrap">
									<asp:Label ID="lblGoogleMapsApi" runat="server" Text='<%# resmgr.GetString("text_config_google_maps_api")%>' CssClass="configLabelStyle">
									</asp:Label></td>
								<td>
									<asp:TextBox ID="textGoogleMapsApi" runat="server" CssClass="configControlStyle" Enabled="true" Columns="60"></asp:TextBox>
									<asp:CustomValidator ID="cvGoogleMapsApi" runat="server" ControlToValidate="textGoogleMapsApi" Display="Dynamic" ErrorMessage='<%# resmgr.GetString("text_error_google_maps_api_key") %>'
										OnServerValidate="cvGoogleMapsApi_ServerValidate" ValidateEmptyText="True">*</asp:CustomValidator></td>
							</tr>
						</table>
					</fieldset>
				</td>
			</tr>
			<tr>
				<td class="configLabelColumnStyle" colspan="2">
					<fieldset>
						<legend>
							<%# resmgr.GetString("text_comments_html_field")%>
						</legend>
						<table>
							<tr>
								<td class="configLabelColumnStyle" style="HEIGHT: 21px" nowrap="nowrap">
									<asp:CheckBox ID="checkAllowHtml" runat="server" Text='<%# resmgr.GetString("text_config_comments_allow_html") %>' CssClass="configControlStyle">
									</asp:CheckBox></td>
							</tr>
							<tr>
								<td class="configLabelColumnStyle" nowrap="nowrap">
									<asp:Label ID="lblAllowedTags" runat="server" Text='<%# resmgr.GetString("text_config_allowed_tags")%>' CssClass="configLabelStyle">
									</asp:Label>
									<asp:CheckBoxList ID="checkBoxListAllowedTags" runat="server" RepeatColumns="5" RepeatDirection="Horizontal"
										Enabled="False" />
								</td>
							</tr>
						</table>
					</fieldset>
				</td>
			</tr>
		</table>
	</div>
	<div class="configSectionStyle">
		<div class="configSectionTitleStyle">
			<asp:Label ID="ServicesToPing" runat="server" Text='<%# resmgr.GetString("text_config_services_to_ping") %>'>
			</asp:Label></div>
		<div>
			<asp:Label ID="labelServicesToPing" runat="server" Text='<%# resmgr.GetString("text_config_services_to_ping_description") %>' CssClass="configLabelStyle">
			</asp:Label><asp:CheckBoxList ID="checkBoxListPingServices" runat="server" RepeatDirection="Horizontal" RepeatColumns="3"></asp:CheckBoxList></div>
	</div>
	<div class="configSectionStyle">
		<div class="configSectionTitleStyle">
			<asp:Label ID="lAppearance" runat="server" Text='<%# resmgr.GetString("text_config_appearance") %>'>
			</asp:Label></div>
		<table cellspacing="0" cellpadding="2" border="0">
			<tr>
				<td class="configLabelColumnStyle" nowrap="nowrap">
					<asp:Label ID="labelTheme"  AssociatedControlID="listThemes" AccessKey="H" runat="server" Text='<%# resmgr.GetString("text_config_default_theme") %>' CssClass="configLabelStyle">
					</asp:Label></td>
				<td colspan="2">
					<asp:DropDownList ID="listThemes"  runat="server" CssClass="configControlStyle"></asp:DropDownList></td>
			</tr>
			<tr>
				<td class="configLabelColumnStyle" nowrap="nowrap">
					<asp:Label ID="labelFrontPageCategory" runat="server" Text='<%# resmgr.GetString("text_config_category_filter") %>' CssClass="configLabelStyle">
					</asp:Label></td>
				<td colspan="2">
					<asp:TextBox ID="textFrontPageCategory" AccessKey="A" runat="server" CssClass="configControlStyle"
						MaxLength="120" Columns="25"></asp:TextBox></td>
			</tr>
			<tr>
				<td class="configLabelColumnStyle" nowrap="nowrap">
					<asp:Label ID="labelFrontPageDayCount" runat="server" Text='<%# resmgr.GetString("text_config_max_days_main_page") %>' CssClass="configLabelStyle">
					</asp:Label><asp:RequiredFieldValidator ID="validatorRFFrontPageDayCount" runat="server" ErrorMessage="'Max. Days on Main  Page' is a required field and cannot be empty."
						Display="Dynamic" ControlToValidate="textFrontPageDayCount">*</asp:RequiredFieldValidator><asp:RangeValidator ID="validatorRGFrontPageDayCount" runat="server" ErrorMessage="The number of days cannot be less than 1"
						Display="Dynamic" ControlToValidate="textFrontPageDayCount" MaximumValue="100000" Type="Integer" MinimumValue="1">*</asp:RangeValidator></td>
				<td colspan="2">
					<asp:TextBox ID="textFrontPageDayCount" AccessKey="X" runat="server" CssClass="configControlStyle"
						MaxLength="4" Columns="3"></asp:TextBox></td>
			</tr>
			<tr>
				<td class="configLabelColumnStyle" nowrap="nowrap">
					<asp:Label ID="labelContentLookaheadDays" runat="server" Text='<%# resmgr.GetString("text_config_content_lookahead_days") %>' CssClass="configLabelStyle">
					</asp:Label><asp:RangeValidator ID="validatorRGContentLookaheadDays" runat="server" ErrorMessage='<%# resmgr.GetString("text_error_content_lookeahead_days_range") %>' Display="Dynamic" ControlToValidate="textContentLookaheadDays" MaximumValue="100000" Type="Integer" MinimumValue="0">*</asp:RangeValidator></td>
				<td colspan="2">
					<asp:TextBox ID="textContentLookaheadDays" AccessKey="X" runat="server" CssClass="configControlStyle"
						MaxLength="4" Columns="3"></asp:TextBox></td>
			</tr>
			<tr>
				<td class="configLabelColumnStyle" nowrap="nowrap">
					<asp:Label ID="labelFrontPageEntryCount" runat="server" Text='<%# resmgr.GetString("text_config_max_entries_main_page") %>' CssClass="configLabelStyle">
					</asp:Label><asp:RequiredFieldValidator ID="validatorRFFrontPageEntryCount" runat="server" ErrorMessage="'Max. Entries on Main  Page' is a required field and cannot be empty."
						Display="Dynamic" ControlToValidate="textFrontPageEntryCount">*</asp:RequiredFieldValidator><asp:RangeValidator ID="validatorRGFrontPageEntryCount" runat="server" ErrorMessage="The number of entries cannot be less than 1"
						Display="Dynamic" ControlToValidate="textFrontPageEntryCount" MaximumValue="100000" Type="Integer" MinimumValue="1">*</asp:RangeValidator></td>
				<td colspan="2">
					<asp:TextBox ID="textFrontPageEntryCount" AccessKey="X" runat="server" CssClass="configControlStyle"
						MaxLength="4" Columns="3"></asp:TextBox></td>
			</tr>
			<tr>
				<td class="configLabelColumnStyle"> <!-- should wrap -->
					<asp:Label ID="labelEntriesPerPage" runat="server" Text='<%# resmgr.GetString("text_config_entries_per_category_page") %>' CssClass="configLabelStyle">
					</asp:Label><asp:RequiredFieldValidator ID="Requiredfieldvalidator3" runat="server" ErrorMessage="Enter a value between 1 and 100"
						Display="Dynamic" ControlToValidate="textEntriesPerPage">*</asp:RequiredFieldValidator><asp:RangeValidator ID="Rangevalidator3" runat="server" ErrorMessage="The number of entries cannot be less than 1"
						Display="Dynamic" ControlToValidate="textEntriesPerPage" MaximumValue="100" Type="Integer" MinimumValue="1">*</asp:RangeValidator></td>
				<td colspan="2">
					<asp:TextBox ID="textEntriesPerPage" AccessKey="X" runat="server" CssClass="configControlStyle"
						MaxLength="4" Columns="3"></asp:TextBox></td>
			</tr>
			<tr>
				<td class="configLabelColumnStyle" nowrap="nowrap">
				</td>
				<td class="configLabelColumnStyle" nowrap="nowrap" colspan="2">
					<asp:CheckBox ID="checkCategoryAllEntries" runat="server" Text='<%# resmgr.GetString("text_config_category_all_entries") %>' CssClass="configControlStyle">
					</asp:CheckBox></td>
			</tr>
			<tr>
				<td class="configLabelColumnStyle" nowrap="nowrap">
					<asp:Label ID="labelEntryEditControl" runat="server" Text='<%# resmgr.GetString("text_config_entry_edit_control") %>' CssClass="configLabelStyle">
					</asp:Label></td>
				<td colspan="2">
					<asp:DropDownList ID="drpEntryEditControl" runat="server" CssClass="configControlStyle"></asp:DropDownList></td>
			</tr>
			<tr>
				<td class="configLabelColumnStyle" nowrap="nowrap">
					<asp:Label ID="Label11" runat="server" Text='<%# resmgr.GetString("text_config_time_zone") %>' CssClass="configLabelStyle">
					</asp:Label></td>
				<td colspan="2">
					<asp:DropDownList ID="listTimeZones" runat="server" CssClass="configControlStyle"></asp:DropDownList><asp:CheckBox ID="checkUseUTC" runat="server" Text='<%# resmgr.GetString("text_config_always_utc") %>' CssClass="configControlStyle"></asp:CheckBox></td>
			</tr>
			<tr>
				<td class="configLabelColumnStyle" nowrap="nowrap">
					<asp:Label ID="labelOtherAppearanceSettings" runat="server" Text='<%# resmgr.GetString("text_config_other_appearance") %>' CssClass="configLabelStyle">
					</asp:Label></td>
				<td>
					<asp:CheckBox ID="checkShowCommentCounters" AccessKey="W" runat="server" Text='<%# resmgr.GetString("text_config_show_comment_counters") %>' CssClass="configControlStyle">
					</asp:CheckBox></td>
			</tr>
			<tr>
				<td class="configLabelColumnStyle" nowrap="nowrap">
				</td>
				<td>
					<asp:CheckBox ID="checkShowItemDescriptionInAggregatedViews" runat="server" Text='<%# resmgr.GetString("text_config_description_in_aggregated_views") %>' CssClass="configControlStyle">
					</asp:CheckBox></td>
			</tr>
			<tr>
				<td class="configLabelColumnStyle" nowrap="nowrap">
				</td>
				<td>
					<asp:CheckBox ID="checkEnableStartPageCaching" runat="server" Text='<%# resmgr.GetString("text_config_enable_output_caching") %>' CssClass="configControlStyle">
					</asp:CheckBox></td>
			</tr>
			<tr>
				<td class="configLabelColumnStyle" nowrap="nowrap">
				</td>
				<td>
					<asp:CheckBox ID="checkEnableBlogrollDescription" runat="server" Text='<%# resmgr.GetString("text_config_enable_blogroll_description") %>' CssClass="configControlStyle">
					</asp:CheckBox></td>
			</tr>
			<tr>
				<td class="configLabelColumnStyle" nowrap="nowrap">
				</td>
				<td>
					<asp:CheckBox ID="checkEnableSearchHighlight" runat="server" Text='<%# resmgr.GetString("text_config_enable_search_highlighting") %>' CssClass="configControlStyle">
					</asp:CheckBox></td>
			</tr>
			<tr>
				<td class="configLabelColumnStyle" nowrap="nowrap">
				</td>
				<td>
					<asp:CheckBox ID="checkEntryTitleAsLink" runat="server" Text='<%# resmgr.GetString("text_config_entrytitle_as_link") %>' CssClass="configControlStyle">
					</asp:CheckBox></td>
			</tr>
			<tr>
				<td class="configLabelColumnStyle" nowrap="nowrap">
				</td>
				<td>
					<asp:CheckBox ID="checkEnableUrlRewriting" Text='<%# resmgr.GetString("text_config_enable_url_rewriting") %>' CssClass="configControlStyle" runat="server">
					</asp:CheckBox></td>
			</tr>
			<tr>
				<td class="configLabelColumnStyle" nowrap="nowrap">
				</td>
				<td>
					<table id="Table3" cellspacing="0" cellpadding="0" border="0">
						<tr>
							<td colspan="2">
								<p>
									<asp:CheckBox ID="checkEnableTitlePermaLink" Text='<%# resmgr.GetString("text_config_enable_title_permalink") %>' CssClass="configControlStyle" runat="server">
									</asp:CheckBox></p>
							</td>
						</tr>
						<tr>
							<td style="WIDTH: 20px">
								&nbsp;</td>
							<td>
								<asp:CheckBox ID="checkEnableTitlePermaLinkSpaces" Text='<%# resmgr.GetString("text_config_enable_title_permalink_spaces") %>' CssClass="configControlStyle" runat="server">
								</asp:CheckBox><asp:DropDownList ID="dropDownTitlePermalinkReplacementCharacter" runat="server"/></td>
						</tr>
						<tr>
							<td style="WIDTH: 20px">
								&nbsp;</td>
							<td>
								<asp:CheckBox ID="checkEnableTitlePermaLinkUnique" Text='<%# resmgr.GetString("text_config_enable_title_permalink_unique") %>' CssClass="configControlStyle" runat="server">
								</asp:CheckBox></td>
						</tr>
						<tr>
							<td style="WIDTH: 20px">
								&nbsp;</td>
							<td>
								<asp:CheckBox ID="checkShowCommentsWhenViewingEntry" runat="server" Text='<%# resmgr.GetString("text_config_show_comments_with_entry") %>' CssClass="configControlStyle">
								</asp:CheckBox></td>
						</tr>
					</table>
				</td>
			</tr>
		</table>
	</div>
	<div class="configSectionStyle">
		<div class="configSectionTitleStyle">
			<asp:Label ID="lNotifications" runat="server" Text='<%# resmgr.GetString("text_config_notifications") %>'>
			</asp:Label></div>
		<table cellspacing="0" cellpadding="2" border="0">
			<tr>
				<td class="configLabelColumnStyle" nowrap="nowrap">
					<asp:Label ID="labelSmtpServer" runat="server" Text='<%# resmgr.GetString("text_config_smtp_server")%>' CssClass="configLabelStyle">
					</asp:Label><asp:CustomValidator ID="validatorSmtpAddress" runat="server" ErrorMessage="The SMTP address does not appear to be valid."
						Display="Dynamic" ControlToValidate="textSmtpServer" EnableClientScript="False">*</asp:CustomValidator></td>
				<td>
					<asp:TextBox ID="textSmtpServer" runat="server" CssClass="configControlStyle" MaxLength="120"
						Columns="40"></asp:TextBox></td>
			</tr>
			<tr>
				<td class="configLabelColumnStyle" nowrap="nowrap">
					<asp:Label ID="labelNotificationEmailAddress" runat="server" Text='<%# resmgr.GetString("text_config_notification_email")%>' CssClass="configLabelStyle">
					</asp:Label><asp:RegularExpressionValidator ID="validationRENotification" runat="server" ErrorMessage="The given Notification Email address does not appear to be valid."
						Display="Dynamic" ControlToValidate="textNotificationEmailAddress" ValidationExpression="^\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*(\s*;\s*\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*)*$">*</asp:RegularExpressionValidator></td>
				<td>
					<asp:TextBox ID="textNotificationEmailAddress" runat="server" CssClass="configControlStyle" MaxLength="120"
						Columns="40"></asp:TextBox></td>
			</tr>
			<tr>
				<td colspan="2">
					<asp:CheckBox ID="checkEnableSmtpAuthentication" Text='<%# resmgr.GetString("text_config_enable_smtp_auth") %>' CssClass="configControlStyle" runat="server">
					</asp:CheckBox></td>
			</tr>
			<tr>
				<td class="configLabelColumnStyle" nowrap="nowrap">
					<asp:Label ID="labelSmtpUsername" CssClass="configLabelStyle" runat="server" Text='<%# resmgr.GetString("text_config_smtp_username") %>'>
					</asp:Label></td>
				<td>
					<asp:TextBox ID="textSmtpUsername" CssClass="configControlStyle" MaxLength="120" Columns="40"
						runat="server"></asp:TextBox></td>
			</tr>
			<tr>
				<td class="configLabelColumnStyle" nowrap="nowrap">
					<asp:Label ID="labelSmtpPassword" CssClass="configLabelStyle" runat="server" Text='<%# resmgr.GetString("text_config_smtp_password") %>'>
					</asp:Label></td>
				<td>
					<asp:TextBox ID="textSmtpPassword" CssClass="configControlStyle" MaxLength="120" Columns="40"
						TextMode="Password" runat="server"></asp:TextBox></td>
			</tr>
            <asp:PlaceHolder runat="server" ID="phSmtpTrustWarning">
			<tr>
			    <td class="configLabelColumnStyle" colspan="2">
			        <asp:Label runat="server" ID="labelSmtpPortTrustWarning" ForeColor="red" Text='<%# resmgr.GetString("text_smtp_trust_warning") %>' />
			    </td>
			</tr></asp:PlaceHolder>
			<tr>
				<td class="configLabelColumnStyle" nowrap="nowrap">
					<asp:Label ID="labelSmtpPort" CssClass="configLabelStyle" runat="server" Text='<%# resmgr.GetString("text_config_smtp_port") %>'>
					</asp:Label></td>
				<td>
					<asp:TextBox ID="textSmtpPort" CssClass="configControlStyle" MaxLength="5" Columns="5" runat="server"></asp:TextBox><asp:RangeValidator ID="validatorRGSmtpPort" runat="server" ErrorMessage="The SMTP Port is invalid"
						Display="Dynamic" ControlToValidate="textSmtpPort" MaximumValue="65536" Type="Integer" MinimumValue="1">*</asp:RangeValidator>
				</td>
			</tr>
			<tr>
				<td colspan="2">
					<asp:CheckBox ID="checkUseSSLForSMTP" Text='<%# resmgr.GetString("text_config_ssl_smtp") %>' Runat="server" CssClass="configControlStyle">
					</asp:CheckBox>
				</td>
			</tr>

			<tr>
				<td colspan="2" style="HEIGHT: 64px">
					<p>
						<asp:CheckBox ID="checkComments" runat="server" Text='<%# resmgr.GetString("text_config_notify_comments")%>' CssClass="configControlStyle">
						</asp:CheckBox><asp:CheckBox ID="checkPosts" runat="server" Text='<%# resmgr.GetString("text_config_notify_posts")%>' CssClass="configControlStyle"></asp:CheckBox><asp:CheckBox ID="checkTrackbacks" runat="server" Text='<%# resmgr.GetString("text_config_notify_trackbacks")%>' CssClass="configControlStyle"></asp:CheckBox><asp:CheckBox ID="checkPingbacks" runat="server" Text='<%# resmgr.GetString("text_config_notify_pingbacks")%>' CssClass="configControlStyle"></asp:CheckBox><asp:CheckBox ID="checkReferrals" runat="server" Text='<%# resmgr.GetString("text_config_notify_referrals")%>' CssClass="configControlStyle"></asp:CheckBox></p>
					<p>
						<asp:CheckBox id="checkDailyReport" runat="server" CssClass="configControlStyle"></asp:CheckBox></p>
				</td>
			</tr>
			<tr>
				<td align="right" colspan="2">
					<asp:Button ID="buttonTestSMTP" runat="server" Text='<%# resmgr.GetString("text_test_smtp") %>' onclick="buttonTestSMTP_Click">
					</asp:Button></td>
			</tr>
		</table>
	</div>
	<div class="configSectionStyle">
		<div class="configSectionTitleStyle">
			<asp:Label ID="lSyndication" runat="server" Text='<%# resmgr.GetString("text_config_syndication") %>'>
			</asp:Label></div>
		<table id="Table1" cellspacing="0" cellpadding="2" border="0">
			<tr>
				<td class="configLabelColumnStyle" nowrap="nowrap">
					<asp:Label ID="labelRssMainDayCount" runat="server" Text='<%# resmgr.GetString("text_config_main_max_days_rss")%>' CssClass="configLabelStyle">
					</asp:Label><asp:RangeValidator ID="validatorRGMainMaxDaysInRss" runat="server" ErrorMessage="The number of days cannot be less than 1"
						Display="Dynamic" ControlToValidate="textMainMaxDaysInRss" MaximumValue="100000" Type="Integer" MinimumValue="1">*</asp:RangeValidator><asp:RequiredFieldValidator ID="validatorRFMainMaxDaysInRss" runat="server" ErrorMessage="'Max. Days in Main RSS Feed' is a required field and cannot be empty."
						Display="Dynamic" ControlToValidate="textMainMaxDaysInRss">*</asp:RequiredFieldValidator></td>
				<td>
					<asp:TextBox ID="textMainMaxDaysInRss" AccessKey="M" runat="server" CssClass="configControlStyle"
						MaxLength="4" Columns="3"></asp:TextBox></td>
			</tr>
			<tr>
				<td class="configLabelColumnStyle" nowrap="nowrap">
					<asp:Label ID="labelRssMainEntryCount" runat="server" Text='<%# resmgr.GetString("text_config_main_max_entries_rss")%>' CssClass="configLabelStyle">
					</asp:Label><asp:RangeValidator ID="validatorRGMainMaxEntriesInRss" runat="server" ErrorMessage="The number of entries cannot be less than 1"
						Display="Dynamic" ControlToValidate="textMainMaxEntriesInRss" MaximumValue="100000" Type="Integer" MinimumValue="1">*</asp:RangeValidator><asp:RequiredFieldValidator ID="validatorRFMainMaxEntriesInRss" runat="server" ErrorMessage="'Max. Entries in Main RSS Feed' is a required field and cannot be empty."
						Display="Dynamic" ControlToValidate="textMainMaxEntriesInRss">*</asp:RequiredFieldValidator></td>
				<td>
					<asp:TextBox ID="textMainMaxEntriesInRss" runat="server" CssClass="configControlStyle" MaxLength="4"
						Columns="3"></asp:TextBox></td>
			</tr>
			<tr>
				<td class="configLabelColumnStyle" nowrap="nowrap">
					<asp:Label ID="labelOtherMaxEntriesInRss" runat="server" Text='<%# resmgr.GetString("text_config_other_max_entries_rss")%>' CssClass="configLabelStyle">
					</asp:Label><asp:RangeValidator ID="validatorRGOtherMaxEntriesInRss" runat="server" ErrorMessage="The number of days cannot be less than 1"
						Display="Dynamic" ControlToValidate="textOtherMaxEntriesInRss" MaximumValue="100000" Type="Integer" MinimumValue="1">*</asp:RangeValidator><asp:RequiredFieldValidator ID="validatorRFOtherMaxEntriesInRss" runat="server" ErrorMessage="'Max. Entries in Category/Comment RSS Feeds' is a required field and cannot be empty."
						Display="Dynamic" ControlToValidate="textOtherMaxEntriesInRss">*</asp:RequiredFieldValidator></td>
				<td>
					<asp:TextBox ID="textOtherMaxEntriesInRss" AccessKey="I" runat="server" CssClass="configControlStyle"
						MaxLength="4" Columns="3"></asp:TextBox></td>
			</tr>
			<tr>
				<td class="configLabelColumnStyle" nowrap="nowrap">
					<asp:Label ID="labelRssLanguage" runat="server" Text='<%# resmgr.GetString("text_config_main_rss_language")%>' CssClass="configLabelStyle">
					</asp:Label>
				</td>
				<td>
					<asp:TextBox ID="textRssLanguage" runat="server" CssClass="configControlStyle" MaxLength="5"
						Columns="3"></asp:TextBox></td>
			</tr>
			<tr>
				<td class="configLabelColumnStyle" nowrap="nowrap" colspan="2">
					<asp:Label ID="labelRSSChannelImage" runat="server" Text='<%# resmgr.GetString("text_config_channelimage")%>' CssClass="configLabelStyle">
					</asp:Label></td>
			</tr>
			<tr>
				<td class="configLabelColumnStyle" nowrap="nowrap" colspan="2">
					<asp:TextBox ID="textRSSChannelImage" runat="server" CssClass="configControlStyle" MaxLength="64"
						Columns="60"></asp:TextBox></td>
			</tr>
			<tr>
				<td class="configLabelColumnStyle" nowrap="nowrap" colspan="2">
					<asp:CheckBox ID="checkAlwaysIncludeContentInRSS" runat="server" Text='<%# resmgr.GetString("text_config_always_comments_in_rss") %>' CssClass="configControlStyle">
					</asp:CheckBox></td>
			</tr>
			<tr>
				<td class="configLabelColumnStyle" nowrap="nowrap" colspan="2">
					<asp:CheckBox ID="checkAttemptToHtmlTidyContent" runat="server" Text='<%# resmgr.GetString("text_config_try_to_xhtmltidy-content") %>' CssClass="configControlStyle">
					</asp:CheckBox></td>
			</tr>
			<tr>
				<td class="configLabelColumnStyle" nowrap="nowrap" colspan="2">
					<asp:CheckBox ID="checkEnableRSSItemFooter" runat="server" Text='<%# resmgr.GetString("text_config_enable_rss_footer") %>' CssClass="configControlStyle">
					</asp:CheckBox><br />
					<asp:TextBox ID="textRSSItemFooter" runat="server" CssClass="configControlStyle" Columns="60"
						TextMode="MultiLine" Rows="5" ReadOnly="false"></asp:TextBox></td>
			</tr>
			<tr>
				<td class="configLabelColumnStyle" nowrap="nowrap">
					<asp:Label ID="labelFeedBurnerName" runat="server" Text='<%# resmgr.GetString("text_config_feedburner_name")%>' CssClass="configLabelStyle">
					</asp:Label></td>
				<td>
					<asp:TextBox ID="textFeedBurnerName" runat="server" CssClass="configControlStyle" MaxLength="64"
						Columns="9"></asp:TextBox></td>
			</tr>
			<tr>
				<td class="configLabelColumnStyle" nowrap="nowrap" colspan="2">
					<asp:CheckBox ID="checkUseFeedScheme" Text='<%# resmgr.GetString("text_config_use_feed_scheme") %>' CssClass="configControlStyle" runat="server">
					</asp:CheckBox></td>
			</tr>
			<tr>
				<td class="configLabelColumnStyle" nowrap="nowrap">
					<asp:Label ID="lblRSSEndPointRewrite" runat="server" Text='<%# resmgr.GetString("text_RSSEndPointRewrite")%>' CssClass="configLabelStyle"></asp:Label>
                </td>
				<td nowrap="nowrap" colspan="3">
					<asp:TextBox ID="txtRSSEndPointRewrite" runat="server" CssClass="configControlStyle" MaxLength="120" Columns="40"></asp:TextBox>
				</td>
			</tr>
		</table>
	</div>
	<div class="configSectionStyle">
		<div class="configSectionTitleStyle">
			<asp:Label ID="lMailToWeblog" runat="server" Text='<%# resmgr.GetString("text_config_mail_to_weblog") %>'>
			</asp:Label></div>
		<table cellspacing="0" cellpadding="2" border="0">
		
            <asp:PlaceHolder runat="server" ID="phPop3TrustWarning">
			<tr>
			    <td class="configLabelColumnStyle" colspan="2">
    				<asp:Label runat="server" ID="labelPop3TrustWarning" ForeColor="Red"  Text='<%# resmgr.GetString("text_pop3_trust_warning") %>' />
			    </td>
			</tr></asp:PlaceHolder>
			<tr>
				<td class="configLabelColumnStyle" nowrap="nowrap">
				</td>
				<td nowrap="nowrap" colspan="3">
					<asp:CheckBox ID="checkPop3Enabled" runat="server" Text='<%# resmgr.GetString("text_config_enable_mail_to_weblog")%>' CssClass="configControlStyle">
					</asp:CheckBox></td>
			</tr>
			<tr>
				<td class="configLabelColumnStyle" nowrap="nowrap">
					<asp:Label ID="labelPop3Server" runat="server" Text='<%# resmgr.GetString("text_config_pop3_server")%>' CssClass="configLabelStyle">
					</asp:Label><asp:CustomValidator ID="validatorPop3Server" runat="server" ErrorMessage="The POP3 address does not appear to be valid."
						Display="Dynamic" EnableClientScript="False">*</asp:CustomValidator></td>
				<td nowrap="nowrap" colspan="3">
					<asp:TextBox ID="textPop3Server" runat="server" CssClass="configControlStyle" MaxLength="120"
						Columns="40"></asp:TextBox></td>
			</tr>
			<tr>
				<td class="configLabelColumnStyle" nowrap="nowrap">
					<asp:Label ID="labelPop3Username" runat="server" Text='<%# resmgr.GetString("text_username")%>' CssClass="configLabelStyle">
					</asp:Label></td>
				<td nowrap="nowrap" colspan="3">
					<asp:TextBox ID="textPop3Username" runat="server" CssClass="configControlStyle" MaxLength="120"
						Columns="40"></asp:TextBox></td>
			</tr>
			<tr>
				<td class="configLabelColumnStyle" nowrap="nowrap">
					<asp:Label ID="Label9" runat="server" Text='<%# resmgr.GetString("text_password")%>' CssClass="configLabelStyle">
					</asp:Label><asp:CompareValidator ID="validatorCPPop3Password" runat="server" ErrorMessage="The passwords do not match"
						Display="Dynamic" ControlToValidate="textPop3PasswordRepeat" ControlToCompare="textPop3Password">*</asp:CompareValidator></td>
				<td class="configLabelColumnStyle" nowrap="nowrap" colspan="3">
					<asp:TextBox ID="textPop3Password" runat="server" CssClass="configControlStyle" MaxLength="40"
						Columns="10" TextMode="Password"></asp:TextBox>&nbsp;
					<asp:Label ID="labelPop3PasswordRepeat" runat="server" Text='<%# resmgr.GetString("text_password_repeat")%>' CssClass="configLabelStyle">
					</asp:Label>&nbsp;
					<asp:TextBox ID="textPop3PasswordRepeat" runat="server" CssClass="configControlStyle" MaxLength="40"
						Columns="10" TextMode="Password"></asp:TextBox></td>
			</tr>
			<tr>
				<td class="configLabelColumnStyle" nowrap="nowrap">
					<asp:Label ID="Label1" runat="server" Text='<%# resmgr.GetString("text_config_subject_prefix")%>' CssClass="configLabelStyle">
					</asp:Label></td>
				<td class="configLabelColumnStyle" nowrap="nowrap" colspan="3">
					<asp:TextBox ID="textPop3SubjectPrefix" runat="server" CssClass="configControlStyle" MaxLength="120"
						Columns="30"></asp:TextBox></td>
			</tr>
			<tr>
				<td class="configLabelColumnStyle" nowrap="nowrap">
					<asp:Label ID="labelPop3Interval" runat="server" Text='<%# resmgr.GetString("text_config_poll_interval")%>' CssClass="configLabelStyle">
					</asp:Label><asp:RangeValidator ID="validatorRGPop3Interval" runat="server" ErrorMessage="The POP3 interval cannot be less than 30 seconds"
						Display="Dynamic" ControlToValidate="textPop3Interval" MaximumValue="100000" Type="Integer" MinimumValue="30">*</asp:RangeValidator></td>
				<td class="configLabelColumnStyle" nowrap="nowrap" colspan="3">
					<asp:TextBox ID="textPop3Interval" AccessKey="I" runat="server" CssClass="configControlStyle"
						MaxLength="6" Columns="4"></asp:TextBox></td>
			</tr>
			<tr>
				<td class="configLabelColumnStyle" nowrap="nowrap">
					<asp:CheckBox ID="checkPop3InlineAttachedPictures" runat="server" Text='<%# resmgr.GetString("text_config_inline_attached_pics")%>' CssClass="configControlStyle">
					</asp:CheckBox></td>
				<td class="configLabelColumnStyle" nowrap="nowrap" colspan="3">
					&nbsp;&nbsp;&nbsp;
					<asp:Label ID="Label8" runat="server" Text='<%# resmgr.GetString("text_config_max_thumb_height") %>' CssClass="configLabelStyle">
					</asp:Label><asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ErrorMessage="Thumbnail value must be set. Set to 0 to disable thumbnailing."
						ControlToValidate="textPop3AttachedPicturesPictureThumbnailHeight">*</asp:RequiredFieldValidator><asp:RangeValidator ID="RangeValidator2" runat="server" ErrorMessage="Must be a value between 0 and 1024"
						ControlToValidate="textPop3AttachedPicturesPictureThumbnailHeight" MaximumValue="1024" Type="Integer" MinimumValue="0">*</asp:RangeValidator><asp:TextBox ID="textPop3AttachedPicturesPictureThumbnailHeight" runat="server" CssClass="configControlStyle"
						Columns="4"></asp:TextBox></td>
			</tr>
			<tr>
				<td class="configLabelColumnStyle" nowrap="nowrap" colspan="4">
					<asp:Label ID="Label14" runat="server" Text='<%# resmgr.GetString("text_config_mail_deletion") %>' CssClass="configLabelStyle">
					</asp:Label></td>
			</tr>
			<tr>
				<td class="configLabelColumnStyle" nowrap="nowrap">
					<asp:RadioButton ID="mailDeletionAll" runat="server" Text='<%# resmgr.GetString("text_config_delete_all_messages")%>' CssClass="configControlStyle" GroupName="mailDeletion">
					</asp:RadioButton></td>
				<td class="configLabelColumnStyle" nowrap="nowrap" colspan="3">
					<asp:RadioButton ID="mailDeletionProcessed" runat="server" Text='<%# resmgr.GetString("text_config_delete_processed_messages")%>' CssClass="configControlStyle" GroupName="mailDeletion">
					</asp:RadioButton></td>
			</tr>
			<tr>
				<td class="configLabelColumnStyle" nowrap="nowrap" colspan="4">
					<asp:CheckBox ID="logIgnoredEmails" runat="server" Text='<%# resmgr.GetString("text_config_log_ignored_emails")%>' CssClass="configControlStyle">
					</asp:CheckBox></td>
			</tr>
		</table>
	</div>
	<div class="configSectionStyle">
		<div class="configSectionTitleStyle">
			<asp:Label ID="lXSS" runat="server" Text='<%# resmgr.GetString("text_config_xss") %>'>
			</asp:Label></div>
		<table cellspacing="0" cellpadding="2" border="0">
			<tr>
				<td class="configLabelColumnStyle" nowrap="nowrap">
				</td>
				<td>
					<asp:CheckBox ID="checkXssEnabled" Text='<%# resmgr.GetString("text_config_xss_enable") %>' CssClass="configControlStyle" runat="server">
					</asp:CheckBox></td>
			</tr>
			<tr>
				<td class="configLabelColumnStyle" nowrap="nowrap">
					<asp:Label ID="Label2" runat="server" Text='<%# resmgr.GetString("text_config_endpoint") %>' CssClass="configLabelStyle">
					</asp:Label><asp:CustomValidator ID="Customvalidator1" runat="server" ErrorMessage="The xss address does not appear to be valid."
						Display="Dynamic" EnableClientScript="False">*</asp:CustomValidator></td>
				<td>
					<asp:TextBox ID="textXssEndpoint" runat="server" CssClass="configControlStyle" MaxLength="120"
						Columns="40"></asp:TextBox></td>
			</tr>
			<tr>
				<td nowrap="nowrap">
					<asp:Label ID="Label3" runat="server" Text='<%# resmgr.GetString("text_username") %>' CssClass="configLabelStyle">
					</asp:Label></td>
				<td>
					<asp:TextBox ID="textXssUsername" runat="server" CssClass="configControlStyle" MaxLength="120"
						Columns="40"></asp:TextBox></td>
			</tr>
			<tr>
				<td class="configLabelColumnStyle" nowrap="nowrap">
					<asp:Label ID="Label10" runat="server" Text='<%# resmgr.GetString("text_password") %>' CssClass="configLabelStyle">
					</asp:Label><asp:CompareValidator ID="Comparevalidator1" runat="server" ErrorMessage="The passwords do not match"
						Display="Dynamic" ControlToValidate="textXssPasswordRepeat" ControlToCompare="textXssPassword">*</asp:CompareValidator></td>
				<td>
					<asp:TextBox ID="textXssPassword" runat="server" CssClass="configControlStyle" MaxLength="40"
						Columns="10" TextMode="Password"></asp:TextBox>&nbsp;
					<asp:Label ID="Label4" runat="server" Text='<%# resmgr.GetString("text_password_repeat") %>' CssClass="configLabelStyle">
					</asp:Label><asp:TextBox ID="textXssPasswordRepeat" runat="server" CssClass="configControlStyle" MaxLength="40"
						Columns="10" TextMode="Password"></asp:TextBox></td>
			</tr>
			<tr>
				<td nowrap="nowrap">
					<asp:Label ID="Label6" runat="server" Text='<%# resmgr.GetString("text_config_update_interval") %>' CssClass="configLabelStyle">
					</asp:Label><asp:RangeValidator ID="Rangevalidator1" runat="server" ErrorMessage="The XSS interval cannot be less than 30 seconds"
						Display="Dynamic" ControlToValidate="textXssInterval" MaximumValue="100000" Type="Integer" MinimumValue="30">*</asp:RangeValidator></td>
				<td>
					<asp:TextBox ID="textXssInterval" AccessKey="I" runat="server" CssClass="configControlStyle"
						MaxLength="6" Columns="5"></asp:TextBox></td>
			</tr>
			<tr>
				<td nowrap="nowrap">
					<asp:Label ID="Label5" runat="server" Text='<%# resmgr.GetString("text_config_rss_file_name") %>' CssClass="configLabelStyle">
					</asp:Label></td>
				<td>
					<asp:TextBox ID="textXssRssFilename" runat="server" CssClass="configControlStyle" MaxLength="40"
						Columns="15"></asp:TextBox></td>
			</tr>
		</table>
	</div>
	<div class="configSectionStyle">
	    <div class="configSectionTitleStyle">
	        <asp:Label id="lblOpenId" runat="server" Text='<%# resmgr.GetString("text_config_OpenIdSettings") %>' />
	    </div>
        <table cellspacing="0" cellpadding="2" border="0">
			<tr>
				<td class="configLabelColumnStyle" nowrap="nowrap">
					<asp:CheckBox ID="chkAllowOpenIdCommenter" runat="server" Text='<%# resmgr.GetString("text_config_AllowOpenIdCommenters") %>' CssClass="configControlStyle">
					</asp:CheckBox>
					</td>
			</tr>	

			<tr>
				<td class="configLabelColumnStyle" nowrap="nowrap">
					<asp:CheckBox ID="chkBypassSpamOpenIdCommenter" runat="server" Text='<%# resmgr.GetString("text_config_OpenIdCommentsBypassSpamCheck") %>' CssClass="configControlStyle">
					</asp:CheckBox>
					</td>
			</tr>	

			<tr>
				<td class="configLabelColumnStyle" nowrap="nowrap">
					<asp:CheckBox ID="chkAllowOpenIdAdmin" runat="server" Text='<%# resmgr.GetString("text_config_AllowOpenIdAdmin") %>' CssClass="configControlStyle">
					</asp:CheckBox>
					</td>
			</tr>	
        </table>
	
	</div>
	
	<div class="configSectionStyle">
		<div class="configSectionTitleStyle">
			<asp:Label ID="lBlackList" runat="server" Text='<%# resmgr.GetString("text_config_blacklist") %>'>
			</asp:Label></div>
		<table cellspacing="0" cellpadding="2" border="0">
			<tr>
				<td class="configLabelColumnStyle" nowrap="nowrap" colspan="2">
					<asp:CheckBox ID="checkReferralUrlBlacklist" runat="server" Text='<%# resmgr.GetString("text_config_enable_referral_url_blacklist")%>' CssClass="configControlStyle">
					</asp:CheckBox></td>
			</tr>
			<tr>
				<td class="configLabelColumnStyle" nowrap="nowrap">
					<asp:Label ID="Label13" runat="server" Text='<%# resmgr.GetString("text_config_referralblacklist") %>' CssClass="configLabelStyle">
					</asp:Label></td>
				<td>
					<asp:TextBox ID="textReferralBlacklist" runat="server" CssClass="configControlStyle" MaxLength="2048"
						Columns="40"></asp:TextBox></td>
			</tr>
			<tr>
				<td class="configLabelColumnStyle" nowrap="nowrap" colspan="2">
					<asp:CheckBox ID="checkReferralBlacklist404s" runat="server" Text='<%# resmgr.GetString("text_config_referralblacklist404s")%>' CssClass="configControlStyle">
					</asp:CheckBox></td>
			</tr>
			<tr>
				<td class="configLabelColumnStyle" nowrap="nowrap" colspan="2">
					<asp:CheckBox ID="checkLogBlockedReferrals" runat="server" Text='<%# resmgr.GetString("text_config_logBlockedReferrals")%>' CssClass="configControlStyle">
					</asp:CheckBox></td>
			</tr>
			<tr>
				<td class="configLabelColumnStyle" nowrap="nowrap" colspan="2">
					<asp:CheckBox ID="checkCaptchaEnabled" runat="server" Text='<%# resmgr.GetString("text_config_enablecaptcha")%>' CssClass="configControlStyle">
					</asp:CheckBox></td>
			</tr>
			<tr>
				<td class="configLabelColumnStyle" nowrap="nowrap" colspan="2">
					<asp:CheckBox ID="checkResolveCommenterIP" runat="server" Text='<%# resmgr.GetString("text_config_resolve_commenter_ip")%>' CssClass="configControlStyle">
					</asp:CheckBox></td>
			</tr>
			<tr>
				<td class="configLabelColumnStyle" nowrap="nowrap">
					<asp:CheckBox ID="checkSpamBlockingEnabled" runat="server" Text='<%# resmgr.GetString("text_config_enablespamblocking")%>' CssClass="configControlStyle">
					</asp:CheckBox>
					<asp:RadioButtonList ID="optionSpamHandling" Runat="server"></asp:RadioButtonList></td>
				<td>
					<asp:Label ID="Label7" runat="server" Text='<%# resmgr.GetString("text_config_spamblockingapikey") %>' CssClass="configLabelStyle">
					</asp:Label><br />
					<asp:TextBox ID="textSpamBlockingApiKey" runat="server" CssClass="configControlStyle" MaxLength="2048"
						Columns="40"></asp:TextBox></td>
			</tr>
			<tr>
				<td class="configLabelColumnStyle" nowrap="nowrap" colspan="2">
					<asp:CheckBox ID="checkDisableEmailDisplay" runat="server" Text='<%# resmgr.GetString("text_config_disableemaildisplay")%>' CssClass="configControlStyle">
					</asp:CheckBox></td>
			</tr>
		</table>
	</div>
	<div class="configSectionStyle">
		<div class="configSectionTitleStyle">
			<asp:Label ID="lblSEOSocialSettings" runat="server" Text='<%# resmgr.GetString("text_SEOSocialSettings") %>'></asp:Label>
		</div>
		<table cellspacing="0" cellpadding="2" border="0">
			<tr>
				<td class="configLabelColumnStyle" nowrap="nowrap">
					<asp:Label ID="lblMetaDescription" runat="server" Text='<%# resmgr.GetString("text_MetaDescription") %>' CssClass="configLabelStyle"></asp:Label>
				</td>
				<td>
					<asp:TextBox ID="txtMetaDescription" runat="server" CssClass="configControlStyle" MaxLength="120" Columns="40"></asp:TextBox>
				</td>
			</tr>
			<tr>
				<td nowrap="nowrap">
					<asp:Label ID="lblMetaKeywords" runat="server" Text='<%# resmgr.GetString("text_MetaKeywords") %>' CssClass="configLabelStyle"></asp:Label>
				</td>
				<td>
					<asp:TextBox ID="txtMetaKeywords" runat="server" CssClass="configControlStyle" MaxLength="120" Columns="40"></asp:TextBox>
				</td>
			</tr>
			<tr>
				<td nowrap="nowrap">
					<asp:Label ID="lblTwitterCard" runat="server" Text='<%# resmgr.GetString("text_TwitterCard") %>' CssClass="configLabelStyle"></asp:Label>
				</td>
				<td>
					<asp:TextBox ID="txtTwitterCard" runat="server" CssClass="configControlStyle" MaxLength="120" Columns="40"></asp:TextBox>
				</td>
			</tr>
			<tr>
				<td nowrap="nowrap">
					<asp:Label ID="lblTwitterSite" runat="server" Text='<%# resmgr.GetString("text_TwitterSite") %>' CssClass="configLabelStyle"></asp:Label>
				</td>
				<td>
					<asp:TextBox ID="txtTwitterSite" runat="server" CssClass="configControlStyle" MaxLength="120" Columns="40"></asp:TextBox>
				</td>
			</tr>
			<tr>
				<td nowrap="nowrap">
					<asp:Label ID="lblTwitterCreator" runat="server" Text='<%# resmgr.GetString("text_TwitterCreator") %>' CssClass="configLabelStyle"></asp:Label>
				</td>
				<td>
					<asp:TextBox ID="txtTwitterCreator" runat="server" CssClass="configControlStyle" MaxLength="120" Columns="40"></asp:TextBox>
				</td>
			</tr>
			<tr>
				<td nowrap="nowrap">
					<asp:Label ID="lblTwitterImage" runat="server" Text='<%# resmgr.GetString("text_TwitterImage") %>' CssClass="configLabelStyle"></asp:Label>
				</td>
				<td>
					<asp:TextBox ID="txtTwitterImage" runat="server" CssClass="configControlStyle" MaxLength="120" Columns="40"></asp:TextBox>
				</td>
			</tr>
			<tr>
				<td nowrap="nowrap">
					<asp:Label ID="lblFaceBookAdmins" runat="server" Text='<%# resmgr.GetString("text_FaceBookAdmins") %>' CssClass="configLabelStyle"></asp:Label>
				</td>
				<td>
					<asp:TextBox ID="txtFaceBookAdmins" runat="server" CssClass="configControlStyle" MaxLength="120" Columns="40"></asp:TextBox>
				</td>
			</tr>
			<tr>
				<td nowrap="nowrap">
					<asp:Label ID="lblFaceBookAppID" runat="server" Text='<%# resmgr.GetString("text_FaceBookAppID") %>' CssClass="configLabelStyle"></asp:Label>
				</td>
				<td>
					<asp:TextBox ID="txtFaceBookAppID" runat="server" CssClass="configControlStyle" MaxLength="120" Columns="40"></asp:TextBox>
				</td>
			</tr>
		</table>
	</div>

    	<div class="configSectionStyle">
		<div class="configSectionTitleStyle">
			<asp:Label ID="Label15" runat="server" Text='<%# resmgr.GetString("text_AMPSettings") %>'></asp:Label>
		</div>
		<table cellspacing="0" cellpadding="2" border="0">
			<tr>
				<td class="configLabelColumnStyle" nowrap="nowrap" colspan="2">
					<asp:CheckBox ID="checkAmpEnabled" runat="server" Text='<%# resmgr.GetString("text_AMPPagesEnabled")%>' CssClass="configControlStyle">
					</asp:CheckBox></td>
			</tr>
		</table>
	</div>

	<div style="CLEAR: both">
		<asp:Button ID="buttonSave" runat="server" Text='<%# resmgr.GetString("text_save_settings") %>' onclick="buttonSave_Click">
		</asp:Button></div>
</div>
