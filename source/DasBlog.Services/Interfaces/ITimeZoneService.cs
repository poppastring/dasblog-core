using System;
using NodaTime;

namespace DasBlog.Services
{
	/// <summary>
	/// Timezone configuration and time conversion for display and storage.
	/// </summary>
	public interface ITimeZoneService
	{
		DateTimeZone GetConfiguredTimeZone();
		DateTime GetContentLookAhead();
		DateTime GetDisplayTime(DateTime datetime);
		DateTime GetCreateTime(DateTime datetime);
	}
}
