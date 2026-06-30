using DasBlog.Web.Models.BlogViewModels;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.AspNetCore.Routing;
using System;

namespace DasBlog.Web.TagHelpers.Layout
{
	[Obsolete("<post-delete-link> now renders its own confirmation modal. <delete-post-modal> is retained for backward compatibility with existing themes and is a no-op when the link has already emitted the modal.")]
	[HtmlTargetElement("delete-post-modal", TagStructure = TagStructure.WithoutEndTag)]
	public class DeletePostModalTagHelper : TagHelper
	{
private readonly IAntiforgery antiforgery;
private readonly LinkGenerator linkGenerator;

public DeletePostModalTagHelper(IAntiforgery antiforgery, LinkGenerator linkGenerator)
{
this.antiforgery = antiforgery;
this.linkGenerator = linkGenerator;
}

public PostViewModel Post { get; set; }

[ViewContext]
public ViewContext ViewContext { get; set; }

public override void Process(TagHelperContext context, TagHelperOutput output)
{
output.TagName = null;
output.TagMode = TagMode.StartTagAndEndTag;

if (Post == null || string.IsNullOrEmpty(Post.EntryId))
{
output.SuppressOutput();
return;
}

var httpContext = ViewContext?.HttpContext;

if (DeletePostModalRenderer.HasBeenRendered(httpContext, Post.EntryId))
{
output.SuppressOutput();
return;
}

var html = DeletePostModalRenderer.BuildModalHtml(Post, httpContext, antiforgery, linkGenerator);
DeletePostModalRenderer.MarkRendered(httpContext, Post.EntryId);

output.Content.SetHtmlContent(html);
}
}
}
