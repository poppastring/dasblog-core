using System;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace DasBlog.Tests.FunctionalTests.Common
{
	/// <summary>
	/// There is nothing worse than using logging to provide diagnostics only to find
	/// that it does not work.  This class is intended to mitigate that.
	/// usage:
	/// 	insert "LopValidator.Validate(loggerFactory : myfactory);" in some location where logging is expected.
	///     An exception will be thrown if things are not hunkydory. e.g. no logger factory has been created.
	///     If you want an exception to be thrown showing non-error observations then call
	/// 	"LogValidator.Validate(loggerFactory: myfactory, alwaysThrow: true);",
	/// </summary>
	// TDDO more analysis of logger factory (reflection required)  
	public static class LoggerValidator
	{
		public static void Validate(IServiceCollection services, bool alwaysThrow = false)
		{
			List<string> observations = new List<string>();
			Validate(serviceProvider: services.BuildServiceProvider(), alwaysThrow: alwaysThrow, observations: observations);
		}

		public static void Validate(IServiceProvider serviceProvider, bool alwaysThrow = false, IList<string> observations = null)
		{
			ILoggerFactory loggerFactory = serviceProvider.GetService<ILoggerFactory>();
			Validate(loggerFactory, alwaysThrow, observations);
		}

		public static void Validate(ILogger logger, bool alwaysThrow = false)
		{
			throw new NotImplementedException("Pass ILoggerFactory instead");
				// TODO: use reflection to implement this.
		}

		public static void Validate(ILoggerFactory loggerFactory, bool alwaysThrow = false, IList<string> observations = null)
		{
			observations = observations ?? new List<string>();
			bool error = false;
			if (loggerFactory == null)
			{
				observations.Add(
				  "logger factory is null - please make sure you have called platform.CompleteSetup()" 
				    + Environment.NewLine + "from your text fixture's constructor, for an example see"
					+ Environment.NewLine + "BlogManagerTests(ITestOutputHelper testOutputHelper, ComponentTestPlatform componentTestPlatform)" 
				    + Environment.NewLine + "in BlogManagerTests.cs");
				error = true;
			}

			if (observations.Count == 0)
			{
				observations.Add("The logger factory appears to be valid");
			}
			if (error || alwaysThrow)
			{
				throw new Exception(string.Join(Environment.NewLine, observations));
			}
		}
	}
}
