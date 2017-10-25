using System;
using NUnit.Framework;
using newtelligence.DasBlog.Runtime;

namespace newtelligence.DasBlog.Runtime.Test
{
	/// <summary>
	/// Summary description for AppTest.
	/// </summary>
	[TestFixture]
	public class AppTest : TestBaseLocal
	{
		#region Tests
		[Test]
		public void CreateApp()
		{
			EntryCollection entries = new EntryCollection();
			DayEntryCollection days = new DayEntryCollection();

			entries = blogService.GetEntriesForDay(DateTime.MaxValue.AddDays(-2), TimeZone.CurrentTimeZone, String.Empty, int.MaxValue, int.MaxValue, String.Empty);

			Assert.IsNotNull(entries);
		}
		#endregion Tests
	}
}
