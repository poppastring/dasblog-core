using System;

namespace DasBlog.SmokeTest.Logging
{
	public static class SmokeTestLoggerExtensions
	{
		public static void LogInformation<T>(this ILogger<T> logger, string message)
		{
			logger.Log(message);
		}
		public static void LogError<T>(this ILogger<T> logger, string message)
		{
			logger.Log(message);
		}
	}

	public interface ILogger<T>
	{
		void Log(string message);
	}
	class SmokeTestLogger<T> : ILogger<T>
	{
		public void Log(string message)
		{
			Console.WriteLine("info: " + message);
		}
	}
}
