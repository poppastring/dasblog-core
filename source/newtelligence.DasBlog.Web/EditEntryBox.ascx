<%@ Register TagPrefix="bdp" Namespace="BasicFrame.WebControls" Assembly="BasicFrame.WebControls.BasicDatePicker" %>
<%@ Control Language="c#" AutoEventWireup="True" Codebehind="EditEntryBox.ascx.cs" Inherits="newtelligence.DasBlog.Web.EditEntryBox" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
<%@ Register TagPrefix="ftb" Namespace="FreeTextBoxControls" Assembly="FreeTextBox" %>
<script src="scripts/AjaxDelegate.js" type="text/javascript"></script>


<% if (siteConfig.EnableGoogleMaps)
   { %>
<script type="text/javascript">
    function showHideMap() 
    {
      var state = document.getElementById("showhidemapbutton").value;
      if (state.toLowerCase() == "<%# resmgr.GetString("text_show_map").ToLower() %>")
      {
          if (GBrowserIsCompatible()) 
          {
            document.getElementById("map").style.display = "";
            var map = new GMap2(document.getElementById("map"));
            map.addControl(new GSmallMapControl());
            map.addControl(new GMapTypeControl());
            var lat = document.getElementById("<%=txtLat.ClientID%>").value;
            var lon = document.getElementById("<%=txtLong.ClientID%>").value;
            map.setCenter(new GLatLng(lat, lon), 5);
            
            document.getElementById("showhidemapbutton").value = "<%# resmgr.GetString("text_hide_map") %>";
            
            GEvent.addListener(map, "click", function(overlay, point) 
            {
                map.clearOverlays();
                var loc = new GMarker(point);
                map.addOverlay(loc);
                var latlong = loc.getPoint().toString();
	            var ll_len = latlong.length;
	            var latlong_split = latlong.substring(1, ll_len -1).split(",");
                document.getElementById("<%=txtLat.ClientID%>").value = latlong_split[0];
                document.getElementById("<%=txtLong.ClientID%>").value = latlong_split[1];
             });  
          }
       }
       else
       {
          GUnload();
          document.getElementById("map").style.display = "none";
          document.getElementById("showhidemapbutton").value = "<%# resmgr.GetString("text_show_map") %>";
       }
    }
</script>
<% } %>

<div class="bodyContentStyle">
	<table id="tableContent" cellspacing="2" cellpadding="2" width="100%" border="0">
		<tr>
			<td>
				<table id="Table1" cellspacing="2" cellpadding="2" width="100%" border="0">
					<tr>
						<td><asp:label id=labelTextTitle Text='<%# resmgr.GetString("text_title") %>' runat="server">
							</asp:label></td>
						<td width="95%">
							<asp:textbox id="entryTitle" runat="server" Columns="40" Width="99%"></asp:textbox></td>
						<td><asp:label id="labelDate" runat="server" Text='<%# resmgr.GetString("text_date") %>'></asp:label></td>
						<td>
							<BDP:BDPLite ResourcePath="~/DatePicker/" DateFormat="ShortDate" id="textDate" runat="server"></BDP:BDPLite></td>
					</tr>
				</table>
			</td>
		</tr>
		<tr>
			<td>
				<div class="FreeTextboxStyle"><asp:PlaceHolder ID="editControlHolder" Runat="server"></asp:PlaceHolder></div>
			</td>
		</tr>
		<tr>
			<td><asp:label id="autoSaveLabel" runat="server" Text=" "></asp:label></td>
		</tr>
	</table>
	<table id="tableAdvancedOptions" cellspacing="2" cellpadding="2" width="100%" border="0">
		<tr>
			<td valign="top"><asp:Label id="labelImageName" runat="server" Text='<%# resmgr.GetString("text_add_image") %>' Visible="True">[labelImageName]</asp:Label></td>
			<td>
				<INPUT id="imageUpload" type="file" size="60" name="imageUpload" runat="server">&nbsp;
				<asp:Button id="buttonUpload" runat="server" Text='<%# resmgr.GetString("text_upload_image") %>' Visible="True" onclick="buttonUpload_Click">
				</asp:Button>
			</td>
		</tr>
		<tr>
			<td valign="top"><asp:Label id="labelAttachmentUpload" runat="server" Text='<%# resmgr.GetString("text_add_attachment") %>' Visible="True">[labelAttachmentName]</asp:Label></td>
			<td>
				<INPUT id="attachmentUpload" type="file" size="60" name="attachmentUpload" runat="server">&nbsp;
				<asp:Button id="buttonUploadAttachment" runat="server" Text='<%# resmgr.GetString("text_upload") %>' Visible="True" onclick="buttonUploadAttachment_Click">
				</asp:Button>
			</td>
		</tr>
		<tr>
			<td valign="top"><asp:label id=labelEnclosureUpload Text='<%# resmgr.GetString("text_enclosure") %>' runat="server">
				</asp:label></td>
			<td>
				<INPUT id="enclosureUpload" type="file" size="60" name="enclosureUpload" runat="server">
				<asp:Label id="labelEnclosureName" runat="server" Text='Enclosure' Visible="False">[labelEnclosureName]</asp:Label>&nbsp;
				<asp:Button id="buttonRemove" runat="server" Text='<%# resmgr.GetString("text_remove") %>' Visible="False" onclick="buttonRemove_Click">
				</asp:Button>
			</td>
		</tr>
		<tr>
			<td valign="top">
				<asp:label id=labelTextDescription Text='<%# resmgr.GetString("text_description") %>' runat="server">
				</asp:label></td>
			<td><asp:textbox id="entryAbstract" runat="server" Width="99%" Columns="60" TextMode="MultiLine"></asp:textbox></td>
		</tr>
		<tr>
			<td>
				<asp:label id=labelLanguage Text='<%# resmgr.GetString("text_language") %>' runat="server" ToolTip='<%# resmgr.GetString("tooltip_language") %>'>
				</asp:label><asp:label id=labelTooltip runat="server" Font-Size="8pt" ToolTip='<%# resmgr.GetString("tooltip_language") %>' BackColor="Blue" ForeColor="White" Font-Bold="True">&nbsp;?&nbsp; </asp:label></td>
			<td><asp:dropdownlist id="listLanguages" runat="server"></asp:dropdownlist></td>
		</tr>
<% if (siteConfig.EnableGeoRss)
   {
%>
        <tr>
            <td>
                <div id="locationlabel">
                <asp:Label ID="lblLocation" runat="server" Text='<%# resmgr.GetString("text_location") %>'></asp:Label>
                </div>
            </td>    
            <td>       
                <div id="locationdata">
         
<% 
                if (siteConfig.EnableGoogleMaps)
                {
%>            
                <div id="map" style="display: none; height: 300px; width: 400px;"></div>
                &nbsp;
<%
                }
%>
                <asp:Label ID="lblLat" runat="server" Text='<%# resmgr.GetString("text_latitude") %>'></asp:Label>
                <asp:TextBox ID="txtLat" runat="server" Columns="80" Width="136px"></asp:TextBox>
                &nbsp;
                <asp:Label ID="lblLong" runat="server" Text='<%# resmgr.GetString("text_longitude") %>'></asp:Label>
                <asp:TextBox ID="txtLong" runat="server" Columns="80" Width="136px"></asp:TextBox>
                &nbsp;
<%
                if (siteConfig.EnableGoogleMaps)
                {
%>
                <input id="showhidemapbutton" onclick="showHideMap()" type="button" value="<%# resmgr.GetString("text_show_map") %>" /><br />
<%
                }
%>
                </div>
            </td>
        </tr>
<% 
   }
%>
		<tr>
			<td><asp:label id=labelCategories Text='<%# resmgr.GetString("text_categories") %>' runat="server">
				</asp:label></td>
			<td><asp:textbox id="textBoxNewCategory" runat="server" Width="136px" Columns="80"></asp:textbox><asp:button id="categoryAddButton" Text="Add" runat="server" onclick="categoryAddButton_Click"></asp:button><asp:checkboxlist id="categoryList" runat="server" RepeatColumns="4" RepeatDirection="Horizontal"></asp:checkboxlist></td>
		</tr>
		<tr>
			<td><asp:label id="labelOptions" Text='<%# resmgr.GetString("text_options") %>' runat="server"></asp:label></td>
			<td>
				<asp:CheckBox id="checkBoxAllowComments" runat="server" Text='<%# resmgr.GetString("text_allow_comments") %>' Checked="True">
				</asp:CheckBox>
				|
				<asp:CheckBox id="checkBoxPublish" runat="server" Text='<%# resmgr.GetString("text_publish") %>' Checked="True">
				</asp:CheckBox>
				|
				<asp:CheckBox id=checkBoxSyndicated runat="server" Text='<%# resmgr.GetString("text_syndicate") %>' Checked="True">
				</asp:CheckBox>
			</td>
		</tr>
		<tr>
			<td><asp:label id=labelTrackback Text='<%# resmgr.GetString("text_trackback_to") %>' runat="server">
				</asp:label></td>
			<td><asp:textbox id="textTrackback" runat="server" Width="99%" Columns="80"></asp:textbox></td>
		</tr>
		<tr>
			<td><asp:label id=labelCrosspost Text='<%# resmgr.GetString("text_crosspost") %>' runat="server"></asp:label></td>
			<td><asp:datagrid id="gridCrossposts" runat="server" Width="100%" AutoGenerateColumns="False">
					<Columns>
						<asp:TemplateColumn HeaderText="Site">
							<HeaderStyle Width="30%"></HeaderStyle>
							<ItemTemplate>
								<asp:CheckBox id=checkSite runat="server" Text='<%# DataBinder.Eval(Container, "DataItem.Site.ProfileName") %>' Checked='<%# DataBinder.Eval(Container, "DataItem.IsAlreadyPosted") %>'>
								</asp:CheckBox>
							</ItemTemplate>
						</asp:TemplateColumn>
						<asp:TemplateColumn HeaderText="Category List">
							<HeaderStyle Width="70%"></HeaderStyle>
							<ItemTemplate>
								<asp:TextBox id="textSiteCategory" runat="server" Width="98%" Enabled='<%# ((string)DataBinder.Eval(Container, "DataItem.Site.ApiType") != "blogger") %>' Text='<%# ((string)DataBinder.Eval(Container, "DataItem.Site.ApiType") == "blogger")?"unsupported":DataBinder.Eval(Container, "DataItem.Categories") %>'>
								</asp:TextBox>
							</ItemTemplate>
						</asp:TemplateColumn>
					</Columns>
				</asp:datagrid></td>
		</tr>
		<tr>
			<td></td>
			<td align="right"><asp:button id=buttonSave Text='<%# resmgr.GetString("text_post_to_weblog") %>' runat="server" onclick="save_Click"></asp:button></td>
		</tr>
	</table>
</div>
