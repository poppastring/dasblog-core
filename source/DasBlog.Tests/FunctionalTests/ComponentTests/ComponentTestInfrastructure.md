#### Component Test Infrastructure
Component test infrastructure currently comprises the the [ComponentTestPlatform](../FunctionalTestDataInfrastructure.md), a thin
layer over [TestSupportPlatform](../Common/TestSupportPlatform.cs) and [test data processor](../Common/ITestDataProcessor.cs).

The component test platform is responsible for creating the test data processor and the various components of the SUT (
the dasblog-core web app).  Currently the only component created is `BlogManager`.

In addition the component test platform allows the user to decide which sandbox (file system) to run each test against.

The test data processor can be used make changes to the sandbox file system before tests are run and to inspect directories
and files after completion to verify modified state.
