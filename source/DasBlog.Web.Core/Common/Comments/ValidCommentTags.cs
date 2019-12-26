using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DasBlog.Core.Common.Comments
{
	public class ValidCommentTags
	{
		public List<Tag> Tag { get; set; }

		public bool IsValidTag(string tagName)
		{
			if (Tag.Count(s => s.Name == tagName) == 0) return false;

			return Tag.Single(s => s.Name == tagName) != null && Tag.Single(s => s.Name == tagName).Allowed;
		}
	}
}
