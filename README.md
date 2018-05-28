
# Dasblog
The [DasBlog Blogging Engine](https://msdn.microsoft.com/en-us/library/aa480016.aspx) reintroduced with ASP.NET Core

[![Build status](https://ci.appveyor.com/api/projects/status/github/poppastring/dasblog-core?branch=master&svg=true)](https://ci.appveyor.com/project/poppastring/dasblog-core)

## Core Tenets - "This we believe"

- We want to pragmatically reuse core DasBlog projects where feasible (includes reusing existing config files).
- Do not be afraid to abandon features/projects that are easily solved with a NuGet package (Pop3, CAPTCHA, Noda Time, etc.)
- Let's use Razor for "Themes"
- Port the custom logging solution to use .NET Core
- DI all the things (avoid massive static objects)
- WISH: Build on Linux (clone/build)
- WISH: Docker version with XML files outside in volume mount


## Installing the development tools

- Install [.NET Core SDK 2.1.3-RC1](https://www.microsoft.com/net/download/dotnet-core/sdk-2.1.300-rc1)
- Install [Visual Studio Community 2017 (15.7.1 or newer)](https://www.visualstudio.com/thank-you-downloading-visual-studio/?sku=Community&rel=15)

You can find more specific installation steps for Windows development [here](https://www.microsoft.com/net/core#windowscmd)


## Building

In order to build the DasBlog Core, ensure that you have [Git](https://git-scm.com/downloads) installed.

Clone or fork a copy of the repo:

```bash
git clone https://github.com/poppastring/dasblog-core
```

Change to the "source" directory and open the *DasBlog All.sln* and perform a build.

## What we have completed so far...
- Upgraded all existing projects to 4.6.X (for .NET Standard 2 support)
- Created a new ASP.NET Web Core 2 project that shows the blogs home page with posts
- Integrated ViewLocationExpanders to support "Themes"
- AddIISUrlRewrite middleware to support 301 redirects for static ".aspx" (archives.aspx, monthview.aspx, etc.)
- Integrate existing dasBlog config (meta, security, site, etc)
- Integrate with original DasBlog layer via Manager classes
- Support for RSS and RSS by category
- Support for Sitemap
- Support for paging e.g. page/1, page/2, etc.
- Add/Edit/Delete blog posts
