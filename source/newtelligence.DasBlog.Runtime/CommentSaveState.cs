using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace newtelligence.DasBlog.Runtime
{
	public enum CommentSaveState
	{
		Added,

		Deleted,

		Approved,

		NotFound,

		Failed,

		PostCommentsDisabled,

		SiteCommentsDisabled
	}
}
