

# Dasblog
The [DasBlog Blogging Engine](https://msdn.microsoft.com/en-us/library/aa480016.aspx) reintroduced with ASP.NET Core

[![Build Status](https://dev.azure.com/poppastring/dasblog-core/_apis/build/status/poppastring.dasblog-core?branchName=master)](https://dev.azure.com/poppastring/dasblog-core/_build/latest?definitionId=2&branchName=master)


## Core Tenets - "This we believe"

- We want to pragmatically reuse core DasBlog projects where feasible (includes reusing existing config files).
- Do not be afraid to abandon features/projects that are easily solved with a NuGet package (Pop3, CAPTCHA, Noda Time, etc.)
- Let's use Razor for "Themes"
- Port the custom logging solution to use .NET Core
- DI all the things (avoid massive static objects)
- WISH: Build on Linux (clone/build)
- WISH: Docker version with XML files outside in volume mount


## Installing the development tools

- Install [.NET Core SDK 2.1.300](https://www.microsoft.com/net/download/thank-you/dotnet-sdk-2.1.300-windows-x64-installer)
- Install [Visual Studio Community 2017 (15.7.1 or newer)](https://www.visualstudio.com/thank-you-downloading-visual-studio/?sku=Community&rel=15)

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
- netcoreapp2.1
- Self -Contained
- win-x64

So far deployment to Azure and to a .NET based hosting service provider worked fine, via Web Deploy and FTP respectively. If you have additional questions or concerns please [submit an issue](https://github.com/poppastring/dasblog-core/issues).


## What we have completed so far...
- Upgraded all existing projects to 4.7.2 (for .NET Standard 2 support)
- Created a new ASP.NET Web Core 2 project that shows the blogs home page with posts
- Integrated ViewLocationExpanders to support "Themes"
- AddIISUrlRewrite middleware to support 301 redirects for static ".aspx" (archives.aspx, monthview.aspx, etc.)
- Integrate existing dasBlog config (meta, security, site, etc)
- Integrate with original DasBlog layer via Manager classes
- Support for RSS and RSS by category
- Support for Sitemap
- Support for paging e.g. page/1, page/2, etc.
- Add/Edit/Delete blog posts
- Add Comments
- Support for Live Writer
- Security and User Management
- Selenium integration

## Documentation
- [Functonal Tests Guide](source/DasBlog.Tests/FunctionalTests/FunctionalTests.md)
- [Test Documentation](source/TestDocumentationIndex.md)
- [Cross-platform Guide](source/CrossPlatform.md)
