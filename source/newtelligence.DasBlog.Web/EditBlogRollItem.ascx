<%@ Control Language="c#" AutoEventWireup="True" Codebehind="EditBlogRollItem.ascx.cs" Inherits="newtelligence.DasBlog.Web.EditBlogRollItem" TargetSchema="http://schemas.microsoft.com/intellisense/ie5"%>
<p>
	<asp:HyperLink id=rssLink runat="server" ImageUrl='<%# (outlineCollection != null &amp;&amp; outlineCollection.Count > 0 ) ? ((newtelligence.DasBlog.Web.Core.SharedBasePage)Page).GetThemedImageUrl("outlinearrow") : ((newtelligence.DasBlog.Web.Core.SharedBasePage)Page).GetThemedImageUrl("xmlButton")%>' NavigateUrl='<%# XmlUrl %>'>
	</asp:HyperLink>
	<asp:HyperLink id=htmlLink runat="server" Text='<%# Title %>' NavigateUrl='<%# HtmlUrl %>'>
	</asp:HyperLink>&nbsp;
	<asp:Label id=description runat="server" Text='<%# Description %>'>
	</asp:Label>
</p>
