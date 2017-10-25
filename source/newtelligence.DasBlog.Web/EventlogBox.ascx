<%@ Register TagPrefix="uc1" TagName="LogDateBar" Src="LogDateBar.ascx" %>
<%@ Control language="c#" Codebehind="EventlogBox.ascx.cs" AutoEventWireup="True" Inherits="newtelligence.DasBlog.Web.EventlogBox" %>
<%@ Register TagPrefix="uc1" TagName="ActivityBox" Src="ActivityBox.ascx" %>
<uc1:ActivityBox id="ActivityBox1" runat="server"></uc1:ActivityBox>
<div class="bodyContentStyle">
    <div class="pageTopic">
        <asp:Label id=labelTitle runat="server" Text='<%# resmgr.GetString("text_eventlog_title") %>'>
        </asp:Label>
        <uc1:LogDateBar id="LogDateBar1" runat="server"></uc1:LogDateBar>
    </div>
    <div>
        <style type="text/css">
        .statsTableRowStyleError { BACKGROUND-COLOR: lightpink }
        .statsTableRowStyleBlocked { BACKGROUND-COLOR: lightgoldenrodyellow }
        .statsTableRowStyleSecurityFailure { BACKGROUND-COLOR: lightsalmon }
        </style>
        <asp:placeholder id="contentPlaceHolder" runat="server"></asp:placeholder>
    </div>
</div>
