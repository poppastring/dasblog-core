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

Navigate to `http://localhost:5000`. The first request redirects to **`/account/setup`**, where you create the admin user (email, display name, and a strong password). Once you submit the form you'll be sent to the login page.

Your password is stored as a salted PBKDF2 hash; the plaintext is never written to disk. Change credentials at any time from `/admin/authors`.

## Template options

| Option | Description | Default |
|--------|-------------|---------|
| `--blogTitle` | Your blog title | My DasBlog! |
| `--adminEmail` | Placeholder admin email written into the initial config. You'll be asked to pick your real email during first-run setup. | myemail@myemail.com |

Example:

```bash
dotnet new dasblog -n myblog --blogTitle "My Blog"
```

## Next steps

- [Configure your blog](https://github.com/poppastring/dasblog-core/wiki/2.-Configure-your-blog) — settings, themes, CDN
- [Create a blog post](https://github.com/poppastring/dasblog-core/wiki/3.-Create-a-blog-post) — browser editor or Open Live Writer
- [Design a theme](https://github.com/poppastring/dasblog-core/wiki/4.-Designing-a-theme) — built-in theme editor at `/admin/themes`
- [Deploy to Azure](https://github.com/poppastring/dasblog-core/wiki/1.-Deployment) — App Service, Linux, or Windows

## Learn more

- [GitHub](https://github.com/poppastring/dasblog-core)
- [Wiki](https://github.com/poppastring/dasblog-core/wiki)
