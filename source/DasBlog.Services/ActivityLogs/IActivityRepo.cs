using System;
using System.Collections.Generic;

namespace DasBlog.Services.ActivityLogs
{
	public interface IActivityRepo : IDisposable
	{
		IEnumerable<String> GetEventLines(DateTime date);
	}
}
