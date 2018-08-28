using System;
using System.IO;
using newtelligence.DasBlog.Runtime;
using newtelligence.DasBlog.Util;
using NodaTime;
using NUnit.Framework;

namespace newtelligence.DasBlog.Runtime.Test
{
	/// <summary>
	/// Summary description for TrackbackTests.
	/// </summary>
	[TestFixture]
	public class TrackbackTests : TestBaseLocal
	{
		#region Tests
		[Test]
		public void TestTrackbackCreation()
		{
			EntryCollection entries = new EntryCollection();

			entries = blogService.GetEntriesForDay(DateTime.MaxValue.AddDays(-2), DateTimeZone.Utc, String.Empty, int.MaxValue, int.MaxValue, String.Empty);

			int numberOfTrackings = 3;

			for (int i = 0; i < numberOfTrackings; i++)
			{
				Tracking t = new Tracking();
				t.PermaLink = "http://www.foo.com/" + i;
				t.RefererBlogName = "Trackback " + i;
				t.RefererExcerpt = "";
				t.RefererTitle = "Trackback " + i;
				t.TargetEntryId = entries[0].EntryId;
				t.TargetTitle = entries[0].Title;
				t.TrackingType = TrackingType.Trackback;

				blogService.AddTracking( t );
			}

			System.Threading.Thread.Sleep(2000);

			TrackingCollection trackingCollection  = blogService.GetTrackingsFor(entries[0].EntryId);

			Assert.IsTrue(trackingCollection.Count == numberOfTrackings);
		}
		#endregion Tests
	}
}
