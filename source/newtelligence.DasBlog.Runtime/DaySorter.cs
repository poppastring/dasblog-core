using System.Collections.Generic;

namespace newtelligence.DasBlog.Runtime
{
	/// <summary>
	/// Summary description for DaySorter.
	/// </summary>
	public class DaySorter : IComparer<DayEntry>
	{
        public int Compare(DayEntry left, DayEntry right)
		{
			return right.DateUtc.CompareTo(left.DateUtc);
		}
	}
}
