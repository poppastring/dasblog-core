#### Functional Test Infrastructure
The infrastructure for the functional tests is implemented in [TestSupportPlatform](Common/TestSupportPlatform.cs) which
draws on the helper classes in [Support](../Support).  It's principal role is to provide dependency injection and
orchestrate logging.

Unfortunately the implementation of the `TestSupportPlatform` requires discussion. You will see from the API docs for
the `TestSupportPlatform` constructor that there is a little dance involving the derived test platform such as
`ComponentTestPlatform` or `BrowserBasedTestPlatform` and the test's constructor.  This arises because XUnit is difficult about its logging, requiring
(for good reasons) the user to build a logger around an XUnit component called `ITestOutputHelper` which it provides
only (?) to the test's constructor.  The upshot is that implementors of test classes must remember to **_call TestPlatform.CompleteSetup_**
in the constructor.

##### See Also
- [Test Infrastructure](../Support/TestInfrastructure.md)

