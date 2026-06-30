﻿using DasBlog.Web.TagHelpers.Post;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Routing;
using System;

namespace DasBlog.Web.TagHelpers
{
[Obsolete]
public class DeletePostTagHelper : PostDeleteLinkTagHelper
{
public DeletePostTagHelper(IAntiforgery antiforgery, LinkGenerator linkGenerator)
: base(antiforgery, linkGenerator)
{
}
}
}