using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using newtelligence.DasBlog.Runtime;
using NodaTime;
using Xunit;

namespace DasBlog.Test.Integration
{
	public class ArchiveMonthTimeZoneTests
	{
		[Fact]
		public void GetEntriesForMonth_IncludesPostsOnLocalMonthBoundaries()
		{
			var cases = new[]
			{
				new ArchiveCase(
					"Eastern first-of-month post",
					new DateTime(2026, 10, 1),
					DateTimeZoneProviders.Tzdb["America/New_York"],
					new DateTime(2026, 10, 1, 18, 12, 4, DateTimeKind.Utc)),
				new ArchiveCase(
					"UTC+10 first-of-month post",
					new DateTime(2026, 8, 1),
					DateTimeZone.ForOffset(Offset.FromHours(10)),
					new DateTime(2026, 7, 31, 16, 0, 0, DateTimeKind.Utc)),
				new ArchiveCase(
					"UTC-10 first-of-month post",
					new DateTime(2026, 10, 1),
					DateTimeZone.ForOffset(Offset.FromHours(-10)),
					new DateTime(2026, 10, 1, 11, 30, 0, DateTimeKind.Utc)),
				new ArchiveCase(
					"UTC-10 last-of-month post",
					new DateTime(2026, 10, 1),
					DateTimeZone.ForOffset(Offset.FromHours(-10)),
					new DateTime(2026, 11, 1, 9, 30, 0, DateTimeKind.Utc)),
				new ArchiveCase(
					"UTC+14 year-rollover post",
					new DateTime(2027, 1, 1),
					DateTimeZone.ForOffset(Offset.FromHours(14)),
					new DateTime(2026, 12, 31, 12, 30, 0, DateTimeKind.Utc))
			};

			using var content = new TemporaryContent();
			content.WriteEntries(cases);
			var service = BlogDataServiceFactory.GetService(content.DirectoryPath, null);

			try
			{
				foreach (var archiveCase in cases)
				{
					var matchesInMonth = service.GetEntriesForMonth(
						archiveCase.Month, archiveCase.TimeZone, string.Empty)
						.Where(entry => entry.Title == archiveCase.Title);
					Assert.Single(matchesInMonth);

					var previousMonth = service.GetEntriesForMonth(
						archiveCase.Month.AddMonths(-1), archiveCase.TimeZone, string.Empty);
					Assert.DoesNotContain(previousMonth, entry => entry.Title == archiveCase.Title);

					var nextMonth = service.GetEntriesForMonth(
						archiveCase.Month.AddMonths(1), archiveCase.TimeZone, string.Empty);
					Assert.DoesNotContain(nextMonth, entry => entry.Title == archiveCase.Title);
				}
			}
			finally
			{
				BlogDataServiceFactory.RemoveService(content.DirectoryPath);
			}
		}

		private sealed record ArchiveCase(string Title, DateTime Month, DateTimeZone TimeZone, DateTime CreatedUtc);

		private sealed class TemporaryContent : IDisposable
		{
			public string DirectoryPath { get; } = Path.Combine(
				Path.GetTempPath(), "dasblog-archive-month-tests-" + Guid.NewGuid().ToString("N"));

			public TemporaryContent() => Directory.CreateDirectory(DirectoryPath);

			public void WriteEntries(IEnumerable<ArchiveCase> cases)
			{
				var entriesByUtcDate = new Dictionary<DateTime, EntryCollection>();
				foreach (var archiveCase in cases)
				{
					var utcDate = archiveCase.CreatedUtc.Date;
					if (!entriesByUtcDate.TryGetValue(utcDate, out var entries))
					{
						entries = new EntryCollection();
						entriesByUtcDate.Add(utcDate, entries);
					}

					var entry = new Entry();
					entry.Initialize();
					entry.Title = archiveCase.Title;
					entry.CreatedUtc = archiveCase.CreatedUtc;
					entries.Add(entry);
				}

				var serializer = new XmlSerializer(typeof(DayEntry), "urn:newtelligence-com:dasblog:runtime:data");
				foreach (var (utcDate, entries) in entriesByUtcDate)
				{
					var dayEntry = new DayEntry { DateUtc = utcDate, Entries = entries };
					var filePath = Path.Combine(DirectoryPath,
						utcDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture) + ".dayentry.xml");
					using var file = File.Create(filePath);
					serializer.Serialize(file, dayEntry);
				}
			}

			public void Dispose() => Directory.Delete(DirectoryPath, true);
		}
	}
}
