<%@ Control Language="c#" AutoEventWireup="True" Codebehind="ThemeBar.ascx.cs" Inherits="newtelligence.DasBlog.Web.ThemeBar" TargetSchema="http://schemas.microsoft.com/intellisense/ie5"%>
<asp:Label id=Label1 runat="server" Text='<%# newtelligence.DasBlog.Web.ApplicationResourceTable.Get().GetString("text_pick_theme") %>'>
</asp:Label>
<asp:PlaceHolder id="content" runat="server"></asp:PlaceHolder>
