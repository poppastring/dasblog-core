#### Browser Based Tests

##### Usage
change directory to the project directory (containing .git) usually called dasblog-core and do
```
dotnet test source/DasBlog.Tests/FunctionalTests --logger trx;LogfileName=browsser_based_test_results.xml --results-directory=./test_results --filter Category=BrowserBasedTest
``` 
For failing tests the logs will be printed along with the results.  If you need to inspect the logs for passing tests
the the location  is `source/DasBlog.Tests/FunctioalTests/test_results/browser_based_test_results.xml`.  You will
need some sort of tool to format them as the results are fairly unreadable in their raw form.

[DasBlog.Tests/FunctionalTests/BrowserBasedTests/appsettings.json](appsettings.json) can be used to set log levels.  Note that this
overrides the settings in [DasBlog.Tests/FunctionalTests/appsettings.json](../appsettings.json).

The following code fragment shows a couple of typical component tests in a typical test class.



##### Anatomy of a Component Test
```
```

##### Detail
Automated browser based tests exercise the web app end-to-end just as interactive
manual use of the web app. does.

The test mechanism employs [Selenium](../../../SeleniumPlusDasBlogCoreInACoupleOfPages.md).

All browser based tests must use the same test data environment as they rely on the web app running continuously
in a separate process.