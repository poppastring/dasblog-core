using System.IO;
using DasBlog.Tests.FunctionalTests.Common;
using DasBlog.Tests.Support.Common;
using DasBlog.Tests.Support.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace DasBlog.Tests.FunctionalTests.TestInfrastructureTests
{
	public class InfrastructureTestPlatform : TestSupportPlatform
	{
		protected override void InjectDependencies(IServiceCollection services)
		{
		}
		public IDasBlogSandbox CreateSandbox(string environment)
		{
			return ServiceProvider.GetService<IDasBlogSandboxFactory>().CreateSandbox(ServiceProvider, environment);
		}

		protected override void CompleteSetupLocal()
		{
			// nothing to do
		}

		public ITestDataProcessor CreateTestDataProcessor(IDasBlogSandbox sandbox)
		{
			return ServiceProvider.GetService<ITestDataProcesorFactory>().CreateTestDataProcessor(sandbox);
		}

		protected override string AppSettingsPathRelativeToProject { get; set; } =
			Constants.TestInfrastructureTestsRelativeToProject;
	}
}
