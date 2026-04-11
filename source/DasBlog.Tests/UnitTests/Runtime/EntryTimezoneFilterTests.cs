using System;
using Xunit;
using newtelligence.DasBlog.Runtime;
using NodaTime;

namespace DasBlog.Tests.UnitTests.Runtime
{
	public class EntryTimezoneFilterTests
	{
		private readonly DateTimeZone utcPlus5 = DateTimeZone.ForOffset(Offset.FromHours(5));

		[Fact]
		public void OccursBetween_ConvertsUtcToLocalBeforeComparing()
		{
			// Entry created at 22:00 UTC = 03:00 next day in UTC+5
			var entry = new Entry { CreatedUtc = new DateTime(2024, 1, 15, 22, 0, 0, DateTimeKind.Utc) };
			var startLocal = new DateTime(2024, 1, 16, 0, 0, 0);
			var endLocal = new DateTime(2024, 1, 16, 23, 59, 59);

			// In UTC+5, 22:00 UTC = 03:00 Jan 16 local, so it should be within Jan 16 local range
			Assert.True(Entry.OccursBetween(entry, utcPlus5, startLocal, endLocal));
		}

		[Fact]
		public void OccursBetween_ExcludesEntryOutsideLocalRange()
		{
			// Entry created at 10:00 UTC = 15:00 in UTC+5
			var entry = new Entry { CreatedUtc = new DateTime(2024, 1, 15, 10, 0, 0, DateTimeKind.Utc) };
			var startLocal = new DateTime(2024, 1, 16, 0, 0, 0);
			var endLocal = new DateTime(2024, 1, 16, 23, 59, 59);

			// In UTC+5, 10:00 UTC Jan 15 = 15:00 Jan 15 local, outside Jan 16 range
			Assert.False(Entry.OccursBetween(entry, utcPlus5, startLocal, endLocal));
		}

		[Fact]
		public void OccursBefore_ConvertsUtcToLocal()
		{
			// Entry created at 20:00 UTC Jan 15 = 01:00 Jan 16 in UTC+5
			var entry = new Entry { CreatedUtc = new DateTime(2024, 1, 15, 20, 0, 0, DateTimeKind.Utc) };

			// In UTC+5, this is Jan 16 01:00, which is NOT before midnight Jan 16
			Assert.False(Entry.OccursBefore(entry, utcPlus5, new DateTime(2024, 1, 16, 0, 0, 0)));

			// But it IS before Jan 16 02:00
			Assert.True(Entry.OccursBefore(entry, utcPlus5, new DateTime(2024, 1, 16, 2, 0, 0)));
		}

		[Fact]
		public void OccursInMonth_UsesTimezone()
		{
			// Entry created at 23:30 UTC Dec 31 = 04:30 Jan 1 in UTC+5
			var entry = new Entry { CreatedUtc = new DateTime(2023, 12, 31, 23, 30, 0, DateTimeKind.Utc) };

			// In UTC+5, this is January 1 — should be in January
			Assert.True(Entry.OccursInMonth(entry, utcPlus5, new DateTime(2024, 1, 1)));

			// Should NOT be in December (local time is Jan 1)
			Assert.False(Entry.OccursInMonth(entry, utcPlus5, new DateTime(2023, 12, 1)));
		}
	}

	public class DayEntryTimezoneFilterTests
	{
		private readonly DateTimeZone utcPlus5 = DateTimeZone.ForOffset(Offset.FromHours(5));

		[Fact]
		public void OccursBetween_ConvertsUtcDateToLocal()
		{
			// Day entry for Jan 15 UTC midnight = Jan 15 05:00 in UTC+5
			var dayEntry = new DayEntry();
			dayEntry.DateUtc = new DateTime(2024, 1, 15, 0, 0, 0, DateTimeKind.Utc);

			var startLocal = new DateTime(2024, 1, 15, 0, 0, 0);
			var endLocal = new DateTime(2024, 1, 15, 23, 59, 59);

			// In UTC+5, midnight UTC = 05:00 local, which is within Jan 15 local range
			Assert.True(DayEntry.OccursBetween(dayEntry, utcPlus5, startLocal, endLocal));
		}

		[Fact]
		public void OccursBetween_ExcludesDayOutsideLocalRange()
		{
			var utcMinus5 = DateTimeZone.ForOffset(Offset.FromHours(-5));
			// Day entry for Jan 16 UTC midnight = Jan 15 19:00 in UTC-5
			var dayEntry = new DayEntry();
			dayEntry.DateUtc = new DateTime(2024, 1, 16, 0, 0, 0, DateTimeKind.Utc);

			var startLocal = new DateTime(2024, 1, 16, 0, 0, 0);
			var endLocal = new DateTime(2024, 1, 16, 23, 59, 59);

			// In UTC-5, midnight Jan 16 UTC = 19:00 Jan 15 local, outside Jan 16 range
			Assert.False(DayEntry.OccursBetween(dayEntry, utcMinus5, startLocal, endLocal));
		}

		[Fact]
		public void OccursInMonth_UsesTimezone()
		{
			// Day entry for Dec 31 UTC = Dec 31 05:00 in UTC+5
			var dayEntry = new DayEntry();
			dayEntry.DateUtc = new DateTime(2023, 12, 31, 0, 0, 0, DateTimeKind.Utc);

			// In UTC+5, Dec 31 midnight UTC = Dec 31 05:00 local — still December
			Assert.True(DayEntry.OccursInMonth(dayEntry, utcPlus5, new DateTime(2023, 12, 1)));
		}
	}
}
