using System;
using DasBlog.Tests.FunctionalTests.Common;
using DasBlog.Tests.Support.Common;
using Xunit;
using Xunit.Abstractions;

namespace DasBlog.Tests.FunctionalTests.TestInfrastructureTests
{
	[Collection(Constants.TestInfrastructureUsersCollection)]
	public class XPathTests2 : IClassFixture<InfrastructureTestPlatform>, IDisposable
	{
		private InfrastructureTestPlatform platform;
		public XPathTests2(ITestOutputHelper testOutputHelper, InfrastructureTestPlatform platform)
		{
			this.platform = platform;
			platform.CompleteSetup(testOutputHelper);
		}
		[Fact]
		[Trait(Constants.CategoryTraitType, Constants.TestInfrastructureTestTraitValue)]
		public void SetDayExtraValue_WithReplacementlValue_IsReflectedInFile()
		{
			using (var sandbox = platform.CreateSandbox(Constants.CommentsEnvironment))
			{
				var testDataProcessor = platform.CreateTestDataProcessor(sandbox); 
				testDataProcessor.SetDayExtraValue(new DateTime(2018, 2, 24), "5d8c292c-ebd8-46fc-95ed-64ca5912c3fc",
					"IsPublic", "xxx");
				var value = testDataProcessor.GetDayExtraValue(new DateTime(2018, 2, 24),
					"5d8c292c-ebd8-46fc-95ed-64ca5912c3fc", "IsPublic");
				Assert.Equal("xxx", value.value);
			}
		}

		public void Dispose()
		{
			platform?.Dispose();
		}
	}
}
