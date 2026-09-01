---
name: dasblog-theme-authoring
description: Creates, modifies, modernizes, visually compares, and diagnoses DasBlog Core themes. Use for greenfield themes, deployed or source themes, Razor templates, legacy migration, local previews, theme assets, responsive design, and deployment preparation.
---

# DasBlog theme authoring

Use this workflow when creating, modifying, importing, modernizing, or
troubleshooting a DasBlog Core theme.

## Authoritative references

1. Read the
   [Designing a theme wiki page](https://github.com/poppastring/dasblog-core/wiki/4.-Designing-a-theme).
2. Read the
   [Tag Helpers and Partial Views page](https://github.com/poppastring/dasblog-core/wiki/5.-Tag-Helpers-&-Partial-Views).
3. Inspect current themes under `source/DasBlog.Web/Themes/`.
4. Inspect `source/DasBlog.Web/Views/Themes/Edit.cshtml` for current theme-file
   descriptions.
5. Treat current source code as authoritative if documentation differs.

## Choose the workflow

Determine the starting point before editing:

| Mode | Starting point | Goal |
|---|---|---|
| Greenfield | A current built-in theme or new design requirements | Create a new theme using the current DasBlog theme contract |
| Brownfield | An existing source theme, published installation, or legacy site | Preserve identity and behavior while modifying or modernizing the theme |

For either mode, keep the original theme intact. Work under a new theme name
unless the user explicitly requests in-place maintenance.

### Greenfield

1. Gather the visual identity, layout, typography, content, accessibility, and
   responsive requirements.
2. Select the closest current built-in theme as a technical baseline.
3. Copy all required theme templates into a new, clearly named theme folder.
4. Replace site-specific branding and assets without removing required DasBlog
   components or authenticated controls.
5. Create representative content for testing without putting sample content in
   reusable theme templates.
6. Validate every rendering path before packaging the theme.

### Brownfield

1. Identify whether the starting point is source code or a published
   installation, and identify the active production theme.
2. Inventory theme files, content files, assets, fonts, external dependencies,
   established routes, and production configuration.
3. Copy the active theme to a new name.
4. Make coherent, incremental changes.
5. Track theme changes separately from content and configuration changes.
6. Preserve published routes, titles, metadata, and production settings unless
   a requested change requires otherwise.

## Source and published locations

In a source checkout, themes belong under:

```text
source/DasBlog.Web/Themes/<theme-name>/
```

In a published installation, the equivalent runtime location is:

```text
Themes/<theme-name>/
```

Do not assume a published site has the source repository layout. Inspect the
installation before choosing paths or build commands.

## Theme contract

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

## Brownfield and legacy modernization

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

## Isolated local preview

Do not change production configuration merely to preview a theme.

For a source checkout, use an appropriate local environment configuration and
run `source/DasBlog.Web`. For a published installation, prefer an isolated copy:

1. Copy the published installation to a temporary preview location.
2. Create environment-specific Preview configuration from the production
   configuration.
3. Set only the Preview root URL and active theme as needed.
4. Set `ASPNETCORE_ENVIRONMENT=Preview` and bind to a loopback URL.
5. Copy or link the working theme into the preview.
6. Copy changed content files into the preview when content is not shared.
7. Confirm that production configuration and the original theme are unchanged.

DasBlog caches XML content. Restart the preview after changing content XML.
Theme CSS and other linked assets may update without a restart.

## Content and links

Theme work can expose site-specific content changes. Keep their ownership clear:

- Razor, CSS, fonts, JavaScript, and theme images belong in the theme folder.
- Post and static-page content belongs in the configured `content` directory.
- Post content assets normally belong under `content/binary`.
- DasBlog content files can store HTML as encoded text inside `.dayentry.xml`.
  Preserve the encoding and parse every changed XML file before previewing or
  deploying it.
- Use root-relative URLs such as `/contact-us` for internal site links so they
  work under production and local hosts.
- Keep URLs absolute when the protocol and host are part of the contract,
  including external services, email links, canonical redirects, production
  roots, and social metadata images.
- Content changes can require separate deployment even when they were made to
  support a visual redesign.

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
- Preserve the original visual identity in brownfield work unless the user
  explicitly approves a new direction.
- Do not mix unrelated content rewrites into a reusable theme.

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
   - Internal links under the local preview host
   - XML parsing for every changed content file

3. If matching an existing site, compare screenshots at the same viewport.
4. Confirm that production configuration and the original theme remain
   unchanged.
5. Produce a deployment manifest that separates:
   - The complete new or changed theme folder
   - Changed content files and content assets
   - Intentional configuration changes
   - The theme activation step
6. Check whether the theme change requires a wiki update.
7. Do not commit, push, publish, or submit changes unless explicitly requested.
