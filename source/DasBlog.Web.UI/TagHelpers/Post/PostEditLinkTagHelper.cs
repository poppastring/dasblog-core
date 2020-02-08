using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DasBlog.Services;

namespace DasBlog.Web.TagHelpers.Post
{
	public class PostEditLinkTagHelper : EditPostTagHelper
	{
		public PostEditLinkTagHelper(IDasBlogSettings dasBlogSettings) : base(dasBlogSettings)
		{
			
		}
	}
}
