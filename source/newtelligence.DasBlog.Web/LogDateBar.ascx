<%@ Control Language="c#" AutoEventWireup="True" Codebehind="LogDateBar.ascx.cs" Inherits="newtelligence.DasBlog.Web.LogDateBar" TargetSchema="http://schemas.microsoft.com/intellisense/ie5"%>
<a id="linkPrevious" runat="server" title='<%# resmgr.GetString("tooltip_previous")%>'>&lt;</a>
<asp:DropDownList id="DropDownListMonth" runat="server"></asp:DropDownList>
<asp:DropDownList id="DropDownListDay" runat="server"></asp:DropDownList>
<asp:DropDownList id="DropDownListYear" runat="server"></asp:DropDownList>
<asp:Button id="buttonGo" runat="server" Text='<%# resmgr.GetString("text_go")%>'></asp:Button>
<a id="linkNext" runat="server" title='<%# resmgr.GetString("tooltip_next")%>'>&gt;</a>
<a id="linkToday" runat="server" title='<%# resmgr.GetString("tooltip_today")%>'>&gt;|</a>
