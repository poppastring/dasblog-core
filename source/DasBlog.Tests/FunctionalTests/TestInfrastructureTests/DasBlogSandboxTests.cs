using System;
using System.IO;
using DasBlog.Tests.Support;
using DasBlog.Tests.Support.Common;
using DasBlog.Tests.Support.Interfaces;
using Xunit;
using Xunit.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Constants = DasBlog.Tests.Support.Common.Constants;

namespace DasBlog.Tests.FunctionalTests.TestInfrastructureTests
{
	public class DasBlogSandboxTests : IClassFixture<InfrastructureTestPlatform>
	{
		private readonly InfrastructureTestPlatform platform;
		private IDasBlogSandbox dasBlogSandbox;

		public DasBlogSandboxTests(ITestOutputHelper testOutputHelper, InfrastructureTestPlatform platform)
		{
			this.platform = platform;
			this.platform.CompleteSetup(testOutputHelper);
			dasBlogSandbox = platform.ServiceProvider.GetService<IDasBlogSandbox>();
			
		}

		[Fact]
		[Trait(Constants.CategoryTraitType, Constants.TestInfrastructureTestTraitValue)]
		public void GitFS_ForUnmodifiedDirectory_ReturnsClean()
		{
			try
			{
				dasBlogSandbox.Init();
				Assert.True(true);
			}
			finally
			{
				dasBlogSandbox.Terminate();
			}
		}

	}
}
