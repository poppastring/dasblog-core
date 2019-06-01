using NodaTime;

namespace DasBlog.Services.Site
{
	public interface ITimeZoneProvider
	{
		DateTimeZone GetConfiguredTimeZone();
	}
}
