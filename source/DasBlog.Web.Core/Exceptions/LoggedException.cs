using System;

namespace DasBlog.Core.Exceptions
{
	/// <summary>
	/// LoggedExcption should be used where the exception has been recorded in the log.  Avoids being recorded multiple times
	/// </mmary>
	public class LoggedException : Exception
	{
		public LoggedException()
		{
			
		}
		public LoggedException(string message) : base(message)
		{
		}

		public LoggedException(string message, Exception innerException) : base(message, innerException)
		{
		}
	}
}
