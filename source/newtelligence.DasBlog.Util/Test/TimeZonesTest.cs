using System;
using System.Globalization;
using newtelligence.DasBlog.Util;
using NUnit.Framework;

namespace UnitTests.Subtext.Framework.Util
{
	[TestFixture]
	public class TimeZonesTest
	{
		[Test]
		public void CanEnumerateTimezones()
		{
			Console.WriteLine(WindowsTimeZone.TimeZones.Count + " timezones.");
			Assert.IsTrue(WindowsTimeZone.TimeZones.Count > 10, "Expected more than ten.");
		}
		
		[Test]
		public void SimplePstConversionTest()
		{
			WindowsTimeZone pst = WindowsTimeZone.TimeZones.GetByZoneIndex(4); //PST
			Console.WriteLine(pst.ToLocalTime(DateTime.UtcNow));

			Console.WriteLine(TimeZone.CurrentTimeZone.ToLocalTime(DateTime.UtcNow));
		}
		
		[Test]
		public void ToLocalTimeReturnsProperTime()
		{
			IFormatProvider culture = new CultureInfo("en-US", true);
			DateTime utcDate = DateTime.Parse("12/30/2006 19:30", culture, DateTimeStyles.AllowWhiteSpaces);
			utcDate = utcDate.ToLocalTime().ToUniversalTime();
			Assert.AreEqual("12/30/2006 19:30", utcDate.ToString("MM/dd/yyyy HH:mm", culture), "An assumption about round tripping the UTC date was wrong.");

			WindowsTimeZone pst = WindowsTimeZone.TimeZones.GetByZoneIndex(4); //PST

			DateTime pstDate = pst.ToLocalTime(utcDate);

			string formattedPstDate = pstDate.ToString("MM/dd/yyyy HH:mm", culture);
			Assert.AreEqual("12/30/2006 11:30", formattedPstDate);
		}
		
		[Test]
		public void ToLocalTimeReturnsProperTimeDuringDaylightSavings()
		{
			IFormatProvider culture = new CultureInfo("en-US", true);
			DateTime utcDate = DateTime.Parse("10/01/2006 19:30", culture, DateTimeStyles.AllowWhiteSpaces);
			utcDate = utcDate.ToLocalTime().ToUniversalTime();
			Assert.AreEqual("10/01/2006 19:30", utcDate.ToString("MM/dd/yyyy HH:mm", culture), "An assumption about round tripping the UTC date was wrong.");
						
			WindowsTimeZone pst = WindowsTimeZone.TimeZones.GetByZoneIndex(4); //PST

			DateTime pstDate = pst.ToLocalTime(utcDate);
			
			string formattedPstDate = pstDate.ToString("MM/dd/yyyy HH:mm", culture);
			Assert.AreEqual("10/01/2006 12:30", formattedPstDate);
		}
		
		[Test]
		public void ToUniversalTimeReturnsProperTime()
		{
			IFormatProvider culture = new CultureInfo("en-US", true);
			DateTime localDate = DateTime.Parse("10/01/2006 12:30", culture, DateTimeStyles.AllowWhiteSpaces);
			
			WindowsTimeZone pst = WindowsTimeZone.TimeZones.GetByZoneIndex(4); //PST

			DateTime utcDate = pst.ToUniversalTime(localDate);
			
			string formattedPstDate = utcDate.ToString("MM/dd/yyyy HH:mm", culture);
			Assert.AreEqual("10/01/2006 19:30", formattedPstDate);
		}

		[Test]
		public void GetDaylightChangesReturnsProperDaylightSavingsInfo()
		{
			WindowsTimeZone pst = WindowsTimeZone.TimeZones.GetByZoneIndex(4); //PST
			DaylightTime daylightChanges = pst.GetDaylightChanges(2006);
			DateTime start = DateTime.Parse("4/2/2006 2:00:00 AM");
			DateTime end = DateTime.Parse("10/29/2006 2:00:00 AM");
			Assert.AreEqual(start, daylightChanges.Start);
			Assert.AreEqual(end, daylightChanges.End);
		}
		
		[Test]
		public void GetZoneByIndexReturnsProperPSTInfo()
		{
			WindowsTimeZone pst = WindowsTimeZone.TimeZones.GetByZoneIndex(4); //PST
			Assert.AreEqual("Pacific Daylight Time", pst.DaylightName);
			Assert.AreEqual("Pacific Daylight Time", pst.DaylightZoneName);
			Assert.AreEqual(new TimeSpan(1, 0, 0), pst.DaylightBias, "Expected a one hour bias");

		}
	}
}
