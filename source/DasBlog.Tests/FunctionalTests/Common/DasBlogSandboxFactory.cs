using System;
using DasBlog.Tests.Support;
using DasBlog.Tests.Support.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.DependencyInjection;

namespace DasBlog.Tests.FunctionalTests.Common
{
	public class DasBlogSandboxFactory : IDasBlogSandboxFactory
	{
		public IDasBlogSandbox CreateSandbox(IServiceProvider serviceProvider, string environment)
		{
			ILogger<DasBlogSandbox>
				sandboxLogger = serviceProvider.GetService<ILoggerFactory>().CreateLogger<DasBlogSandbox>();
			DasBlogISandboxOptions opts = new DasBlogISandboxOptions {Environment = environment};
			IOptions<DasBlogISandboxOptions> optionsAccessor = new OptionsAccessor<DasBlogISandboxOptions> {Value = opts};
			var fs = serviceProvider.GetService<IVersionedFileService>();
			var sandbox = new DasBlogSandbox(sandboxLogger, fs, optionsAccessor);
			sandbox.Init();
			return sandbox;
		}
		
	}

	public interface IDasBlogSandboxFactory
	{
		IDasBlogSandbox CreateSandbox(IServiceProvider serviceProvider, string environment);
	}
}
