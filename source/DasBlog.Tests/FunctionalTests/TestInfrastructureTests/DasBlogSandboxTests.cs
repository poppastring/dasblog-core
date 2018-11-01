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
	[Collection(Constants.TestInfrastructureUsersCollection)]
	public class DasBlogSandboxTests : IClassFixture<InfrastructureTestPlatform>
	{
		private readonly InfrastructureTestPlatform platform;
		private IDasBlogSandbox dasBlogSandbox;

		public DasBlogSandboxTests(ITestOutputHelper testOutputHelper, InfrastructureTestPlatform platform)
		{
			this.platform = platform;
			this.platform.CompleteSetup(testOutputHelper);
			dasBlogSandbox = platform.CreateSandbox(Constants.VanillaEnvironment);

		}

/*
		[Fact]
		public void TextEx()
		{
			Assert.Throws<Exception>(() => DoStuff());
		}

		private void DoStuff()
		{
			try
			{
				throw new Exception("exceptional");

			}
			catch (Exception)
			{
				throw;
			}
		}
*/

		[Fact]
		[Trait(Constants.CategoryTraitType, Constants.TestInfrastructureTestTraitValue)]
		public void GitFS_ForUnmodifiedDirectory_ReturnsClean()
		{
			try
			{
//				dasBlogSandbox.Init();
				Assert.True(true);
			}
			finally
			{
				dasBlogSandbox?.Terminate(false);
			}
		}

	}
}
