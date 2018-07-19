using System;
using System.Collections.Generic;

namespace DasBlog.Core.Services.Interfaces
{
	public interface IActivityRepo : IDisposable
	{
		IEnumerable<String> GetEventLines(DateTime date);
	}
}