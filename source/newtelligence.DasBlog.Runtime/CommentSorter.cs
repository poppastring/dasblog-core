using System.Collections.Generic;

namespace newtelligence.DasBlog.Runtime
{
	/// <summary>IComparer implementation used for sorting comments by date.</summary>
    
    public class CommentSorter : IComparer<Comment>
    {
        public int Compare( Comment left, Comment right )
        {
            return right.CreatedUtc.CompareTo(left.CreatedUtc);
        }
    }
}
