﻿using DasBlog.Web.Models.BlogViewModels;
using DasBlog.Web.TagHelpers.Layout;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.AspNetCore.Routing;
using System.Threading.Tasks;

namespace DasBlog.Web.TagHelpers.Post
{
public class PostDeleteLinkTagHelper : TagHelper
{
private readonly IAntiforgery antiforgery;
private readonly LinkGenerator linkGenerator;

public PostDeleteLinkTagHelper(IAntiforgery antiforgery, LinkGenerator linkGenerator)
{
this.antiforgery = antiforgery;
this.linkGenerator = linkGenerator;
}

public PostViewModel Post { get; set; }

public string BlogPostId { get; set; }

public string BlogTitle { get; set; }

[ViewContext]
public ViewContext ViewContext { get; set; }

public override void Process(TagHelperContext context, TagHelperOutput output)
{
if (Post != null)
{
BlogPostId = Post.EntryId;
BlogTitle = Post.Title;
}

var modalId = $"deleteModal_{BlogPostId?.Replace("-", "")}";

output.TagName = "a";
output.TagMode = TagMode.StartTagAndEndTag;
output.Attributes.SetAttribute("href", "#");
output.Attributes.SetAttribute("data-bs-toggle", "modal");
output.Attributes.SetAttribute("data-bs-target", $"#{modalId}");
output.Attributes.SetAttribute("role", "button");
output.Content.SetHtmlContent("Delete this post");

// Emit the matching modal markup alongside the link so custom
// themes don't have to remember to include <delete-post-modal />.
// If the modal has already been emitted for this post in this
// request (e.g. a theme calls <delete-post-modal /> explicitly),
// this is a no-op so we don't produce a duplicate-id modal.
var httpContext = ViewContext?.HttpContext;
if (Post != null
&& !string.IsNullOrEmpty(Post.EntryId)
&& antiforgery != null
&& linkGenerator != null
&& httpContext != null
&& !DeletePostModalRenderer.HasBeenRendered(httpContext, Post.EntryId))
{
var modalHtml = DeletePostModalRenderer.BuildModalHtml(Post, httpContext, antiforgery, linkGenerator);
DeletePostModalRenderer.MarkRendered(httpContext, Post.EntryId);
output.PostElement.AppendHtml(modalHtml);
}
}

public override Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
{
return Task.Run(() => Process(context, output));
}
}
}