<p align="center">
  <a href="https://github.com/poppastring/dasblog-core">
    <img src="https://github.com/poppastring/dasblog-core/blob/main/images/dasblog.jpg" alt="DasBlog" />
  </a>
</p>
<p align="center">
	<a href="https://github.com/poppastring/dasblog-core/wiki">Wiki</a> |
	<a href="https://github.com/poppastring/dasblog-core/blob/main/FAQ.md">FAQ</a> |
	<a href="https://www.poppastring.com/blog/category/dasblog-core">Blog</a> |
	<a href="https://github.com/poppastring/dasblog-core/blob/main/CONTRIBUTING.md">Contributing</a>
	<br /><br />
	<a href="https://github.com/poppastring/dasblog-core/releases/">
		<img src="https://img.shields.io/github/v/release/poppastring/dasblog-core.svg" alt="Latest release" />
	</a>
	<a href="https://poppastring.visualstudio.com/dasblog-core/_build/latest?definitionId=2&branchName=main">
		<img src="https://poppastring.visualstudio.com/dasblog-core/_apis/build/status/poppastring.dasblog-core?branchName=master&jobName=Job&configuration=Job%20windows" alt="Windows Build status" />
	</a>
	<a href="https://poppastring.visualstudio.com/dasblog-core/_build/latest?definitionId=2&branchName=main">
		<img src="https://poppastring.visualstudio.com/dasblog-core/_apis/build/status/poppastring.dasblog-core?branchName=master&jobName=Job&configuration=Job%20linux" alt="Linux Build status" />
	</a>
</p>

# DasBlog

A blogging engine built with ASP.NET Core that preserves the essence of the original [DasBlog](https://msdn.microsoft.com/en-us/library/aa480016.aspx) while taking advantage of the modern cross-platform goodness of .NET.

## Quick start

```bash
dotnet new install DasBlog.Template
dotnet new dasblog -n myblog
cd myblog
.\DasBlog.Web.exe
```

Navigate to `http://localhost:5000`. The first request redirects you to `/admin/setup` to create your admin user — pick an email, display name, and a strong password, then you'll be sent to the login page.

## Features

* **Browser editor** for creating and publishing posts with image uploads, categories, and scheduled publishing
* **[Open Live Writer](https://openlivewriter.com/)** and MetaWeblog API support for desktop publishing
* **[Theme management](https://github.com/poppastring/dasblog-core/wiki/4.-Designing-a-theme)** with a built-in editor for creating, customizing, and switching Razor themes
* **Static pages** for content outside the blog timeline
* **Comment moderation** with spam filtering
* **RSS/Atom feeds** and categories
* **[ActivityPub](https://github.com/poppastring/dasblog-core/wiki/2.-Configure-your-blog#6-activitypub-optional)** for Fediverse discovery
* **CDN support** for serving media from a content delivery network
* **CLI tools** for [configuration and management](https://github.com/poppastring/dasblog-core/wiki/CLI-Reference)

## Install

| Option | Description |
|--------|-------------|
| [**.NET template**](https://github.com/poppastring/dasblog-core/wiki/1.-Install#net-template) | `dotnet new dasblog` — fastest way to get running |
| [**Deploy to Azure Button**](https://github.com/poppastring/dasblog-core/wiki/1.-Install#azure--deploy-button) | One-click Azure deployment |
| [**Azure App Services**](https://github.com/poppastring/dasblog-core/wiki/Azure-App-Services) | Step-by-step for Linux, Windows, or sub-folder |
| [**Cloud hosting**](https://github.com/poppastring/dasblog-core/wiki/1.-Install#cloud-hosting) | Any host that supports .NET 10 |
| [**Local**](https://github.com/poppastring/dasblog-core/wiki/1.-Install#local-install) | Run on your own machine |

## Documentation

Check out the [wiki](https://github.com/poppastring/dasblog-core/wiki) for deployment guides, configuration, theme design, and architecture.

Migrating from classic DasBlog? See the [migration guide](https://github.com/poppastring/dasblog-core/wiki/7.-Migrating-from-DasBlog).

## Contributing

We welcome contributions! See [CONTRIBUTING.md](https://github.com/poppastring/dasblog-core/blob/main/CONTRIBUTING.md) for setup instructions and guidelines.

Please [submit an issue](https://github.com/poppastring/dasblog-core/issues) if you encounter any problems.
