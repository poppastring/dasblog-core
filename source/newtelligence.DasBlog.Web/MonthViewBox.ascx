<%@ Control language="c#" Codebehind="MonthViewBox.ascx.cs" AutoEventWireup="True" Inherits="newtelligence.DasBlog.Web.MonthViewBox" targetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
<%@ OutputCache Duration="900" VaryByParam="*" %>
<div class="bodyContentStyle">
	<% Response.Write("Rendered: " + System.DateTime.Now); %><br />
	<asp:placeholder id="contentPlaceHolder" runat="server"></asp:placeholder>
</div>
