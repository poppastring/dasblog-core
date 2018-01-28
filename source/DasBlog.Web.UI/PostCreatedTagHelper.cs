using System.Threading.Tasks;
using DasBlog.Web.UI.Models.BlogViewModels;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace DasBlog.Web.UI.Views
{
    public class PostCreatedTagHelper : TagHelper
    {
        public PostViewModel Post { get; set; }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            output.TagMode = TagMode.StartTagAndEndTag;
            output.TagName = "p";

            var content = await output.GetChildContentAsync();
            string format = content.GetContent();
            output.Content.SetHtmlContent(Post.CreatedDateTime.ToString(format));
        }
    }
}