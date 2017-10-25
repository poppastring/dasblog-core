<%@ Register TagPrefix="uc1" TagName="ActivityBox" Src="ActivityBox.ascx" %>
<%@ Control language="c#" Codebehind="UserClickThroughsBox.ascx.cs" AutoEventWireup="True" Inherits="newtelligence.DasBlog.Web.UserClickThroughsBox" %>
<%@ Register TagPrefix="uc1" TagName="LogDateBar" Src="LogDateBar.ascx" %>
<uc1:ActivityBox id="ActivityBox1" runat="server"></uc1:ActivityBox>
<div class="bodyContentStyle">
	<div class="pageTopic">
		<asp:Label id=Label1 runat="server" Text='<%# newtelligence.DasBlog.Web.ApplicationResourceTable.Get().GetString("text_userclickthrough_title") %>'>
		</asp:Label>
		<uc1:LogDateBar id="LogDateBar1" runat="server"></uc1:LogDateBar></div>
	<asp:placeholder id="contentPlaceHolder" runat="server"></asp:placeholder>
</div>
