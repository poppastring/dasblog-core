using NodaTime;
using NUnit.Framework;
using System;

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

			entries = blogService.GetEntriesForDay(DateTime.MaxValue.AddDays(-2), DateTimeZone.Utc, string.Empty, int.MaxValue, int.MaxValue, string.Empty);

			Assert.IsNotNull(entries);
		}
		#endregion Tests
	}
}
