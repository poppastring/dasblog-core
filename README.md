# Dasblog Core
One of the primary goals of this project is to preserve the essence of the original [DasBlog Blogging Engine](https://msdn.microsoft.com/en-us/library/aa480016.aspx) while taking advantage of the modern cross platform goodness of ASP.NET Core.

Check out the [wiki](https://github.com/poppastring/dasblog-core/wiki) for additional information on DasBlog Fundamentals. 

|Windows|Linux|macOS|
|-------|-----|-----|
|[![Build Status](https://poppastring.visualstudio.com/dasblog-core/_apis/build/status/poppastring.dasblog-core?branchName=master&jobName=Job&configuration=Job%20windows)](https://poppastring.visualstudio.com/dasblog-core/_build/latest?definitionId=2&branchName=master)|[![Build Status](https://poppastring.visualstudio.com/dasblog-core/_apis/build/status/poppastring.dasblog-core?branchName=master&jobName=Job&configuration=Job%20linux)](https://poppastring.visualstudio.com/dasblog-core/_build/latest?definitionId=2&branchName=master)|[![Build Status](https://poppastring.visualstudio.com/dasblog-core/_apis/build/status/poppastring.dasblog-core?branchName=master&jobName=Job&configuration=Job%20mac)](https://poppastring.visualstudio.com/dasblog-core/_build/latest?definitionId=2&branchName=master)|


## Our guiding principles

In developing this project we attempted to follow some [fundamental principles](https://www.poppastring.com/blog/one-hard-thing-in-software-engineering):
- Pragmatically reuse core DasBlog projects where feasible (includes reusing existing config files).
- Do not be afraid to abandon features/projects that are easily solved with a NuGet package (Pop3, CAPTCHA, Noda Time, etc.)
- Let's use Razor for "Themes"
- Port the custom logging solution to use .NET Core
- DI all the things (avoid massive static objects)


## Installing the development tools

- Install [.NET Core SDK 3.0.100](https://dotnet.microsoft.com/download/thank-you/dotnet-sdk-3.0.100-windows-x64-installer)
- Install [Visual Studio Community 2019 (16.3.0 or newer)](https://visualstudio.microsoft.com/thank-you-downloading-visual-studio/?sku=Community&rel=16)

You can find more specific installation steps for Windows development [here](https://www.microsoft.com/net/core#windowscmd)


## Building

In order to build the DasBlog Core, ensure that you have [Git](https://git-scm.com/downloads) installed.

Fork the repo:

```bash
git clone https://github.com/[YOUR_GITHUB_ID]/dasblog-core
```

Change to the "source" directory and open the *DasBlog All.sln* and perform a build.

## Deployment

Currently the most efficient way to deploy dasblog-core is by following the [Overview of deployment in Visual Studio](https://docs.microsoft.com/en-us/visualstudio/deployment/deploying-applications-services-and-components-resources?view=vs-2017).

Configuration settings are as follows:
- Configuration: Release
- netcoreapp3.0
- Self -Contained
- win-x64

So far deployment to Azure and to a .NET based hosting service provider worked fine, via Web Deploy and FTP respectively. If you have additional questions or concerns please [submit an issue](https://github.com/poppastring/dasblog-core/issues).

## Design
DasBlog Core uses a templating system based on the [Razor Engine](https://docs.microsoft.com/en-us/aspnet/web-pages/overview/getting-started/introducing-razor-syntax-c) to define the layouts of each theme. 

Check out the [Theme Design wiki here](https://github.com/poppastring/dasblog-core/wiki/5.-Theme-Design).

## Documentation
- [DasBlog Core Wiki](https://github.com/poppastring/dasblog-core/wiki)
- [Site Configuration](https://github.com/poppastring/dasblog-core/wiki/1.-Site-Configuration)
- [Site Security Configuration](https://github.com/poppastring/dasblog-core/wiki/2.-Site-Security-Configuration)
- [Meta Configuration](https://github.com/poppastring/dasblog-core/wiki/3.-Meta-Configuration)
- [Functonal Tests Guide](source/DasBlog.Tests/FunctionalTests/FunctionalTests.md)
- [Test Documentation](source/TestDocumentationIndex.md)
- [Cross-platform Guide](source/CrossPlatform.md)
