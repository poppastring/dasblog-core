# Dasblog
The [DasBlog Blogging Engine](https://msdn.microsoft.com/en-us/library/aa480016.aspx) reintroduced with ASP.NET Core

[![Build status](https://ci.appveyor.com/api/projects/status/github/poppastring/dasblog-core?branch=master&svg=true)](https://ci.appveyor.com/project/poppastring/dasblog-core)
[![FOSSA Status](https://app.fossa.io/api/projects/git%2Bgithub.com%2Fpoppastring%2Fdasblog-core.svg?type=shield)](https://app.fossa.io/projects/git%2Bgithub.com%2Fpoppastring%2Fdasblog-core?ref=badge_shield)

## Core Tenets - "This we believe"

- We want to pragmatically reuse core DasBlog projects where feasible (includes reusing existing config files).
- Do not be afraid to abandon features/projects that are easily solved with a NuGet package (Pop3, CAPTCHA, Noda Time, etc.)
- Let's use Razor for "Themes"
- Port the custom logging solution to use .NET Core
- DI all the things (avoid massive static objects)
- WISH: Build on Linux (clone/build)
- WISH: Docker version with XML files outside in volume mount


## Installing the development tools

- Install [.NET Core SDK 2.0](https://aka.ms/dotnet-sdk-2.0.0-win-gs-x64)
- Install [Visual Studio Community 2017](https://www.visualstudio.com/thank-you-downloading-visual-studio/?sku=Community&rel=15)

You can find more specific installation steps for Windows development [here](https://www.microsoft.com/net/core#windowscmd)


## Building

In order to build the DasBlog Core, ensure that you have [Git](https://git-scm.com/downloads) installed.

Clone a copy of the repo:

```bash
git clone https://github.com/poppastring/dasblog-core
```

Change to the "source" directory and open the *DasBlog All.sln* and perform a build.

## The story so far...
- Upgraded all existing projects to 4.6.X (for .NET Standard 2 support)
- Created a new ASP.NET Web Core 2 project that shows the blogs home page with posts
- Integrated ViewLocationExpanders to support "Themes"
- AddIISUrlRewrite middleware to support 301 redirects for static ".aspx" (archives.aspx, monthview.aspx, etc.)
- Integrate existing dasBlog config (meta, security, site, etc)
- Integrate with original DasBlog layer via repository pattern
- Support for RSS and RSS by category
- Support for Sitemap
- Support for paging e.g. page/1, page/2, etc.


## License
[![FOSSA Status](https://app.fossa.io/api/projects/git%2Bgithub.com%2Fpoppastring%2Fdasblog-core.svg?type=large)](https://app.fossa.io/projects/git%2Bgithub.com%2Fpoppastring%2Fdasblog-core?ref=badge_large)