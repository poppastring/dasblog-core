---
applyTo: "source/DasBlog.Tests/**"
---

# Test conventions

- **Framework:** xUnit. Do not add MSTest or NUnit.
- **Naming:** `MethodUnderTest_State_ExpectedBehavior` (e.g.
  `GetEntry_MissingId_ReturnsNull`).
- **Layout:** Mirror the production namespace under
  `DasBlog.Tests/UnitTests/<Area>`. Integration tests go in
  `DasBlog.Tests/DasBlog.Test.Integration`.
- **Fixtures:** Reuse `DasBlogSettingsMock`, `TestEntry`, and helpers in
  `Common/`, `TestContent/`, and `UnitTestsConstants.cs` rather than rolling
  new ones.
- **Isolation:** Unit tests must not touch the real file system, network, or a
  developer's `Config/` folder. Use the `TestContent/` fixtures.
- **Integration tests:** May spin up the web host but must clean up any files
  they create under temp directories.
- **Run before pushing:**
  `dotnet test source/DasBlog.Tests/UnitTests/DasBlog.Tests.UnitTests.csproj`
