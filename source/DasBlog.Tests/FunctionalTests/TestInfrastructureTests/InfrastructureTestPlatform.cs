using DasBlog.Tests.FunctionalTests.Common;
using DasBlog.Tests.Support.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Xunit.Abstractions;

namespace DasBlog.Tests.FunctionalTests.TestInfrastructureTests
{
	public class InfrastructureTestPlatform : TestSupportPlatform
	{
		protected override void InjectDependencies(IServiceCollection services)
		{
			// nothing to do
		}
		public IDasBlogSandbox CreateSandbox(string environment)
		{
			return ServiceProvider.GetService<IDasBlogSandboxFactory>().CreateSandbox(ServiceProvider, environment);
		}

		protected override void CompleteSetupLocal()
		{
			// nothing to do
		}
	}
}
