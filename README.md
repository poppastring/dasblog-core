# Dasblog
The DasBlog Blogging Engine reintroduced with ASP.NET Core


## Core Tenants - "This we believe"

- We want to pragmatically reuse core DasBlog projects where feasible.
- Do not be afraid to abandon features/projects that are easily solved with a NuGet package (Pop3, CAPTCHA, Noda Time, etc.)
- Let's use Razor for "Themes"
- Port the custom logging solution to use .NET Core
- DI all the things (avoid massive static objects)
- WISH: Build on Linux (clone/build)



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
