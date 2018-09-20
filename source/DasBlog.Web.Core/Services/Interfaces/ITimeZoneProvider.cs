using Microsoft.Extensions.Options;
using NodaTime;

namespace DasBlog.Core.Services.Interfaces
{
	public interface ITimeZoneProvider
	{
		DateTimeZone GetConfiguredTimeZone();
	}
}
