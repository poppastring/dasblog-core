#### Component Tests

##### Usage
change directory to the project directory (containing .git) usually called dasblog-core and do
```
dotnet test source/DasBlog.Tests/FunctionalTests --logger trx;LogfileName=component_test_results.xml --results-directory ./test_results --filter Category=ComponentTest
``` 
For failing tests the logs will be printed along with the results.  If you need to inspect the logs for passing tests
the the location  is `source/DasBlog.Tests/FunctioalTests/test_results/component_test_results.xml`.  You will
need some sort of tool to format them as the results are fairly unreadable in their raw form.

[DasBlog.Tests/FunctionalTests/ComponentTests/appsettings.json](appsettings.json) can be used to set log levels.  Note that this
overrides the settings in [DasBlog.Tests/FunctionalTests/appsettings.json](../appsettings.json).

The following code fragment shows a couple of typical component tests in a typical test class.



##### Anatomy of a Component Test
```
using System;
using Constants = DasBlog.Tests.Support.Common.Constants;
using Xunit;
using Xunit.Abstractions;
using newtelligence.DasBlog.Runtime;

namespace DasBlog.Tests.FunctionalTests.ComponentTests	// (1)
{
	[Collection(Constants.TestInfrastructureUsersCollection)]  // (2)
	public class BlogManagerTests : IClassFixture<ComponentTestPlatform>, IDisposable // (3)
	{

		private ComponentTestPlatform platform;
		public BlogManagerTests(ITestOutputHelper testOutputHelper, ComponentTestPlatform componentTestPlatform)  // (4)
		{
			componentTestPlatform.CompleteSetup(testOutputHelper);  // (5)
			this.platform = componentTestPlatform;
		}

		[Fact]	// (6)
		[Trait(Constants.CategoryTraitType, Constants.ComponentTestTraitValue)]		// (7)
		public void SearchingBlog_WithUnmatchableData_ReturnsNull()		// (8)
		{
			using (var sandbox = platform.CreateSandbox(Constants.VanillaEnvironment))  // (9)
			{
				var blogManager = platform.CreateBlogManager(sandbox);		// (10)
				EntryCollection entries = blogManager.SearchEntries("this cannot be found", null);  // (11)
				Assert.Empty(entries);		// (12)
			}
		}
		[Fact]
		[Trait(Constants.CategoryTraitType, Constants.ComponentTestTraitValue)]
		public void AddComment_ForFirstComent_CreatesDayFeedbackFile()
		{
			using (var sandbox = platform.CreateSandbox(Constants.CommentsEnvironment))
			{
				var testDataProcessor = platform.CreateTestDataProcessor(sandbox);
				testDataProcessor.SetSiteConfigValue("DaysCommentsAllowed", "9999");		// (13)
				var blogManager = platform.CreateBlogManager(sandbox);
				var comment = MakeMinimalComment("entry-id-2018-02-25-0001");
				blogManager.AddComment("entry-id-2018-02-25-0001", comment);
				Assert.True(DayFeedbackFileExists(Path.Combine(sandbox.TestEnvironmentPath, Constants.ContentDirectory)
				  ,new DateTime(2018, 2, 25) ));
			}
		}
		public void Dispose()		// (14)
		{
		}
	}
}
```
The points below refer to the respective lines above annotated with point numbers in parentheses.

1. Namespace name is unimportant but the documentation relies on the file system hierarchy remaining unchanged.  Note
that `Constants` in the `using` section refers to the test assembly's constants.  Where the web app's constants are included in test files
they are aliased as `WebAppConstants`.  Inevitably legacy newtelligence namespaces are referenced but currently we
avoid ASP.NET dependencies.  This is just code hygiene,  The author has no reason to believe there would be a problem
introducing such a dependency.
2. The `[Collection]` attribute has the effect of preventing classes annotated with it from executing in parallel.  It should be applied
to any test class which uses the [test infrastructure](../../Support/TestInfrastructure.md) - in particular the versioned file system.  It is not clear to the
author what is preventing browser based tests executing in parallel with component tests - perhaps they do.
3. The component tests need the [component test platform](ComponentTestInfrastructure.md) to provide the SUT's API via DI
and a file system on which to operate.
4. An implementation of ITestOutputHelper is provided by XUnit to the test's constructor and is required for use by the XUnitLogger.
5. Unfortunately it is not possible to acquire an instance of ITestOutputHelper other than in the test constructor.
Therefore **_we need complete the setup_** of the `ComponentTestPlatform` in particular adding in the log provider.
6. `[Fact]` attribute is standard XUnit.
7. The "ComponentTest" category trait is used to identify this a a component test as opposed to browser based or integration.
8. Use the Unit-of-work/State-under-test/Expected-behaviour naming convention for tests.
9. A sandbox is created comprising content and configuration files in some pristine state.  There are a number of
environments to choose from including "Vanilla" and "EmptyContent".  They are located at
 [source/DasBlog.Tests/Resources/Environments](../../Resources/Environments)
10. This example happens to be a blog manager test.  The blog manager is created here.  Be aware that DataService on which it
relies does not get unloaded from the process so a step in BlogManager creation is to clear the data service caches.
11. Some operation is executed, typically on a service API.
12. Standard XUnit assertion
13. There is an instance of the [TestDataProcessor](../Common/ITestDataProcessor.cs) class to allow for the SUT's data files to be manipulated before and
inspected after tests.
14. XUnit's way is to use `IDisposable.Dispose()` to tear down resources allocated in the constructor which nned to be de-allocated.