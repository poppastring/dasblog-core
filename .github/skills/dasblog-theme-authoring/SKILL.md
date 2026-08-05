---
name: dasblog-theme-authoring
description: Creates, modernizes, visually compares, and diagnoses DasBlog Core themes. Use for theme folders, Razor theme templates, legacy theme migration, theme assets, fonts, responsive design, and rendering differences.
---

# DasBlog theme authoring

Use this workflow when creating, importing, modernizing, or troubleshooting a
DasBlog Core theme.

## Authoritative references

1. Read the
   [Designing a theme wiki page](https://github.com/poppastring/dasblog-core/wiki/4.-Designing-a-theme).
2. Read the
   [Tag Helpers and Partial Views page](https://github.com/poppastring/dasblog-core/wiki/5.-Tag-Helpers-&-Partial-Views).
3. Inspect current themes under `source/DasBlog.Web/Themes/`.
4. Inspect `source/DasBlog.Web/Views/Themes/Edit.cshtml` for current theme-file
   descriptions.
5. Treat current source code as authoritative if documentation differs.

## Theme contract

Themes belong under:

```text
source/DasBlog.Web/Themes/<theme-name>/
```

Use the primary templates according to their intended roles:

| Template | Role |
|---|---|
| `_Layout.cshtml` | HTML shell, metadata, navigation, footer, styles, and scripts |
| `_BlogPage.cshtml` | Full-post container that normally renders `_BlogItems` |
| `_BlogItem.cshtml` | Full post template used by individual and full aggregate views |
| `_BlogPageSummary.cshtml` | Aggregate summary container that normally renders `_BlogItemsSummary` |
| `_BlogItemSummary.cshtml` | Summary template for one post in an aggregate view |
| `custom.css` | Theme-specific styles |

`_BlogPage.cshtml` and `_BlogPageSummary.cshtml` receive a
`ListPostsViewModel`. `_BlogItem.cshtml` and `_BlogItemSummary.cshtml` receive a
`PostViewModel`.

Do not add route-based conditions to `_BlogItem.cshtml` to distinguish the home
page from individual posts. Use the summary templates and
`ShowItemSummaryInAggregatedViews`.

Static pages are separate from the blog page templates and are rendered by
`source/DasBlog.Web/Views/BlogPost/LoadStaticPage.cshtml`.

## Current components

Prefer current tag helpers and view components:

- `<site-head-meta />`
- `<site-rss-link />`
- `<site-atom-link />`
- `<twitter-card />`
- `<open-graph />`
- `<blog-posting-schema />`
- `<theme-stylesheets />`
- `<vc:cookie-consent />`
- `<vc:comment-block comments="@Model.Comments" />`
- `<vc:collapse-comment-block post="@Model" />`

Do not introduce obsolete compatibility partials into new themes.

Use the bundled Bootstrap and Font Awesome versions. Do not add CDN
dependencies when equivalent local assets already exist.

## Assets

Theme-specific assets belong inside the theme folder:

- CSS
- JavaScript
- Fonts
- Images
- Favicon

Blog-post content assets belong under the configured binary-content directory,
normally `source/DasBlog.Web/content/binary`. Do not move post content into a
theme merely to make a page render.

The content directory is normally ignored by Git. Verify whether required
content assets must be deployed separately.

JavaScript files are not loaded automatically. Reference them explicitly from
`_Layout.cshtml`.

Use `/theme/<theme-name>/...` for public theme asset URLs. Do not confuse that
URL with the repository path `Themes/<theme-name>/`.

## Legacy theme modernization

When importing an older theme:

1. Inventory the exported files, assets, and external dependencies.
2. Identify the Bootstrap, Font Awesome, jQuery, and third-party framework
   versions.
3. Capture the original site before editing.
4. Preserve its visual identity, content links, typography, spacing, and
   responsive behavior.
5. Replace obsolete framework markup and dependencies with current DasBlog and
   Bootstrap equivalents.
6. Recreate only the third-party CSS rules required by the design.
7. Do not copy large unused vendor bundles.
8. Preserve locally hosted fonts when their license permits redistribution.
9. Separate missing post-content assets from missing theme assets.
10. Keep site-specific content out of reusable templates unless preserving that
    content is explicitly required.

## Visual comparison

When matching an existing site:

1. Capture the reference and local pages at identical viewport sizes.
2. Allow images, fonts, animations, and embeds time to settle.
3. Compare navigation, container width, typography, spacing, cards, footer, and
   responsive behavior.
4. Check the home page and at least one individual post.
5. Check both summary and full aggregate modes.
6. Diagnose external embeds independently before changing theme code.

A blank third-party iframe does not necessarily indicate a theme defect. Verify
the iframe markup, source URL, provider catalog availability, content security
policy, and browser behavior.

## Implementation rules

- Build the affected web project before editing when practical.
- Make surgical changes and preserve unrelated working-tree changes.
- Use current DasBlog tag helpers instead of reproducing application logic.
- Keep controllers and application services unchanged unless the task proves
  the issue is not confined to the theme.
- Do not modify `site.config` or `site.Development.config` merely to make a
  theme preview convenient without preserving the user's existing settings.
- Do not hardcode the active theme name in shared stylesheet links. Prefer
  `<theme-stylesheets />`.
- Ensure individual posts retain their visible title unless the requested
  design explicitly says otherwise.
- Use `_BlogItemSummary.cshtml` for distinct home-page or listing presentation.

## Validation

After editing:

1. Build:

   ```powershell
   dotnet build source/DasBlog.Web/DasBlog.Web.csproj
   ```

2. Check:
   - Home page with summary mode enabled
   - Home page with summary mode disabled
   - Individual post title and content
   - Archive and category pages
   - Authenticated administrative controls
   - Desktop and mobile layouts
   - Theme fonts and images
   - Post-content images
   - External embeds

3. If matching an existing site, compare screenshots at the same viewport.
4. Check whether the theme change requires a wiki update.
5. Do not commit, push, publish, or submit changes unless explicitly requested.
