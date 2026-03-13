using Microsoft.Extensions.Options;
using NodaTime;

namespace DasBlog.Services.Site
{
	public class TimeZoneProvider : ITimeZoneProvider
	{
		private readonly IOptionsMonitor<TimeZoneProviderOptions> optionsMonitor;
		public TimeZoneProvider(IOptionsMonitor<TimeZoneProviderOptions> optionsMonitor)
		{
			this.optionsMonitor = optionsMonitor;
		}
		public DateTimeZone GetConfiguredTimeZone()
		{
			var options = optionsMonitor.CurrentValue;
			if (options.AdjustDisplayTimeZone)
			{
				return DateTimeZone.ForOffset(Offset.FromHoursAndMinutes((int)options.DisplayTimeZoneIndex, 
						(int)(options.DisplayTimeZoneIndex % 1m * 60)));
			}
			else
			{
				return DateTimeZone.Utc;
			}
		}
	}

	public class TimeZoneProviderOptions
	{
		public bool AdjustDisplayTimeZone { get; set; }
		public decimal DisplayTimeZoneIndex { get; set; }
	}
}
