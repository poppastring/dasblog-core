# DasBlog Core

A blogging engine built with ASP.NET Core. This template creates a ready-to-run DasBlog Core blog.

## Quick start

```bash
# Install the template
dotnet new install DasBlog.Template

# Create a new blog
dotnet new dasblog -n myblog

# Run it
cd myblog
.\DasBlog.Web.exe
```

Navigate to `https://localhost:5001` and log in with the default credentials:

- **Email:** myemail@myemail.com
- **Password:** admin

**Change these immediately** at `/admin/authors`.

## Template options

| Option | Description | Default |
|--------|-------------|---------|
| `--blogTitle` | Your blog title | My DasBlog! |
| `--blogUrl` | Root URL of your blog | *(empty)* |
| `--adminEmail` | Admin email address | admin@example.com |

Example:

```bash
dotnet new dasblog -n myblog --blogTitle "My Blog" --blogUrl "https://myblog.example.com"
```

## Next steps

- [Configure your blog](https://github.com/poppastring/dasblog-core/wiki/2.-Configure-your-blog) — settings, themes, CDN
- [Create a blog post](https://github.com/poppastring/dasblog-core/wiki/3.-Create-a-blog-post) — browser editor or Open Live Writer
- [Design a theme](https://github.com/poppastring/dasblog-core/wiki/4.-Designing-a-theme) — built-in theme editor at `/admin/themes`
- [Deploy to Azure](https://github.com/poppastring/dasblog-core/wiki/1.-Deployment) — App Service, Linux, or Windows

## Learn more

- [GitHub](https://github.com/poppastring/dasblog-core)
- [Wiki](https://github.com/poppastring/dasblog-core/wiki)
