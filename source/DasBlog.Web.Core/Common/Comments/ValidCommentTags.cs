using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DasBlog.Core.Common.Comments
{
	public class ValidCommentTags
	{
		public List<Tag> Tags { get; set; }

		public bool IsValidTag(string tagName)
		{
			if (Tags.Count(s => s.Name == tagName) == 0) return false;

			return Tags.Single(s => s.Name == tagName) != null && Tags.Single(s => s.Name == tagName).Allowed;
		}
	}
}
