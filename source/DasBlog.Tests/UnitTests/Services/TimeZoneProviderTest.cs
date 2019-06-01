using System;
using System.Collections.Generic;
using DasBlog.Core.Services;
using DasBlog.Services.Site;
using Microsoft.Extensions.DependencyInjection;
using NodaTime;
using Xunit;

namespace DasBlog.Tests.UnitTests.Services
{
	public class TimeZoneProviderTest
	{
		[Theory]
		[Trait("Category", "UnitTest")]
		[MemberData(nameof(TimeZoneTestData))]
		public void GetConfiguredTimeZone_Respects_ConfigSettings(
		  bool adjustDisplayTimeZone,decimal displayTimeZoneIndex, DateTimeZone expected )
		{
			ITimeZoneProvider tzp = CreateTimeZoneProvider(adjustDisplayTimeZone, displayTimeZoneIndex);
			var tz = tzp.GetConfiguredTimeZone();
			Assert.Equal(expected, tz);
		}

		private ITimeZoneProvider CreateTimeZoneProvider(bool adjustDisplayTimeZone, decimal displayTimeZoneIndex)
		{
			IServiceCollection services = new ServiceCollection();
			services.Configure<TimeZoneProviderOptions>(opt =>
			{
				opt.AdjustDisplayTimeZone = adjustDisplayTimeZone;
				opt.DisplayTimeZoneIndex = displayTimeZoneIndex;
			});
			services.AddSingleton<ITimeZoneProvider, TimeZoneProvider>();
			IServiceProvider sp = services.BuildServiceProvider();
			var tzp = sp.GetService<ITimeZoneProvider>();
			return tzp;
		}

		public static IEnumerable<Object[]> TimeZoneTestData
			=> new List<object[]>
			{
				{new object[] {false, 0, DateTimeZone.ForOffset(Offset.FromHoursAndMinutes(0, 0))}},
				{new object[] {false, 4, DateTimeZone.ForOffset(Offset.FromHoursAndMinutes(0, 0))}},
				{new object[] {true, 0, DateTimeZone.ForOffset(Offset.FromHoursAndMinutes(0, 0))}},
				{new object[] {true, 2, DateTimeZone.ForOffset(Offset.FromHoursAndMinutes(2, 0))}},
				{new object[] {true, 2.5m, DateTimeZone.ForOffset(Offset.FromHoursAndMinutes(2, 30))}},
			};

	}
}
