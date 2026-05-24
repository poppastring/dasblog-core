# Copilot instructions for dasblog-core

DasBlog Core is an ASP.NET Core blogging engine that preserves the spirit of the
original DasBlog while running cross-platform on modern .NET. Posts and
configuration are stored as XML files on disk (no database), and the runtime is
the descendant of the original `newtelligence.DasBlog.Runtime`.

When helping in this repo, follow the rules below. Prefer linking to the
[wiki](https://github.com/poppastring/dasblog-core/wiki),
[FAQ](../FAQ.md), and [CONTRIBUTING](../CONTRIBUTING.md) over restating their
contents here.

## Stack and tooling

- **Target framework:** .NET 10 (all projects). Do not downgrade TFMs.
- **IDE:** Visual Studio 2026 (17.14+). The solution is `source/DasBlog All.sln`.
- **Package management:** Central — see `source/Directory.Packages.props`. Add
  or bump versions there, not in individual `.csproj` files.
- **Build:** `dotnet build "source/DasBlog All.sln"`
- **Run the web app:** `dotnet run --project source/DasBlog.Web`
- **Tests:** `dotnet test source/DasBlog.Tests/UnitTests/DasBlog.Tests.UnitTests.csproj`

## Repository layout

Projects under `source/`:

| Project | Purpose |
|---------|---------|
| `DasBlog.Web` | ASP.NET Core MVC host, controllers, views, themes, admin UI |
| `DasBlog.Web.Core` / `DasBlog.Web.UI` / `DasBlog.Web.Editor` / `DasBlog.Web.Repositories` | Web-layer building blocks shared by the host |
| `DasBlog.Core` | Cross-cutting framework abstractions |
| `DasBlog.Managers` | Application services that sit between controllers and the runtime |
| `DasBlog.Services` | Configuration, site security, activity, scheduling, etc. |
| `newtelligence.DasBlog.Runtime` | The classic XML-backed blog runtime — change with care |
| `Subtext.Akismet` | Spam-filtering integration |
| `NetEscapades.Extensions.Logging.RollingFile` | Vendored rolling-file logger |
| `DasBlog.CLI` | Command-line config/management tool |
| `DasBlog.Template.csproj` + `.template.config` / `template-staging` | `dotnet new dasblog` template packaging |
| `DasBlog.Tests/UnitTests` | xUnit unit tests |
| `DasBlog.Tests/DasBlog.Test.Integration` | Integration tests |

## Coding conventions

- Honor `source/.editorconfig`. The important rules:
  - **C# files use tabs** for indentation, UTF-8 with BOM, final newline required.
  - `csproj`, `props`, `json`, etc. use 2-space indent.
  - `using` directives sorted with `System.*` first; avoid `this.` qualification;
	prefer language keywords (`string`, `int`) over framework names.
- Prefer constructor injection; services are registered in
  `DasBlogServiceCollectionExtensions` and the pipeline is wired in
  `DasBlogApplicationBuilderExtensions` / `Program.cs`.
- Configuration lives in `source/DasBlog.Web/Config/` (site config, metaconfig,
  themes). Treat the XML schemas as a public contract — additive changes only
  unless a migration is included.
- Logging goes through the rolling-file logger; do not introduce a new logging
  framework.
- Keep controllers thin; put logic in `DasBlog.Managers` or `DasBlog.Services`.
- Don't introduce a database, ORM, or external state store. Persistence is XML
  on disk by design.

## Don't-touch zones

- `source/**/bin/`, `source/**/obj/`, `nupkg/`, `nupk/`, `source/DasBlog.Web/logs/`
- Generated files inside `template-staging/`
- Vendored code in `Subtext.Akismet` and `NetEscapades.Extensions.Logging.RollingFile`
  unless the change is the explicit task
- `newtelligence.DasBlog.Runtime` — changes here can break existing blogs;
  require integration test coverage and a clear migration note

## Doing work

- Before editing, build the affected project to capture the baseline.
- After editing, build that project (or the full solution for cross-cutting
  changes) and run unit tests. Run integration tests if you touched the runtime,
  managers, or persistence.
- Keep changes surgical. Unrelated formatting or refactors belong in a separate PR.
- If a change affects user-visible behavior, configuration, or install steps,
  update the matching page(s) under the
  [wiki](https://github.com/poppastring/dasblog-core/wiki), `README.md`, or
  `FAQ.md` in the same PR.

## Pull requests

Link the issue, describe the change and the test you ran, and call out any doc updates (or explicitly
note that none are needed).
