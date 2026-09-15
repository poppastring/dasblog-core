# Frequently Asked Questions


#### Q: What is this?
A: DasBlog Core  is an open source blogging engine based heavily on the  work produced by a community of volunteers and contributors who helped make [the original DasBlog a very popular blogging option](https://github.com/shanselman/dasblog).


#### Q: Does this replace the original DasBlog?
A: We think so. The original DasBlog relies on Web Forms and will not be supported going forward.


#### Q: Does it work on a Mac?  On Linux?
A: Yes! Since .NET runs on Mac OS and Linux, DasBlog Core runs there also. Any original code that relied on Windows-specific APIs have been removed.


#### Q: I found a bug, what should I do?
A: Add to an existing issue or create a [new issue via GitHub](https://github.com/poppastring/dasblog-core/issues). Before creating a new issue, make sure that the issue does not already exist and feel free to add to the issue.


#### Q: How can I get involved?
A: Please jump right in!  We love contributions and we are happy to offer advice via Issues or PRs.


#### Q: Does DasBlog Core  support themes?
A: Yes. Themes are developed using Razor! More details can be found [here](https://github.com/poppastring/dasblog-core/wiki/4.-Designing-a-theme).


#### Q: Is there a public Theme gallery?
A: DasBlog Core ships with several built-in themes and includes a [theme editor](https://github.com/poppastring/dasblog-core/wiki/4.-Designing-a-theme#theme-editor) at `/admin/themes` for browsing, switching, and customizing themes directly from the browser.


#### Q: How do I modify an existing theme safely?
A: Copy the theme under a new name and keep the active production theme unchanged while you work. The [theme design guide](https://github.com/poppastring/dasblog-core/wiki/4.-Designing-a-theme) covers greenfield creation and brownfield modification.


#### Q: How do I test a theme locally without changing my live site?
A: Use a source checkout or an isolated copy of the published site with environment-specific Preview configuration, a loopback root URL, and the new theme selected only in Preview. Do not replace production configuration merely to make local testing convenient.


#### Q: Why did a content change not appear in my theme preview?
A: DasBlog caches XML content. Restart the preview after changing a `.dayentry.xml` file. Theme CSS and other linked assets may update without restarting.


#### Q: Which files do I deploy after changing a theme?
A: Deploy the complete custom theme folder and any content files or content assets changed separately during the work. Activate the new theme only after those files are present on the site.


#### Q: How does DasBlog control search engine indexing?
A: `robots.txt` controls crawling and references the sitemap, but it does not guarantee that a discovered URL is removed from search results. Administrative, account, setup, and error routes therefore also send `X-Robots-Tag: noindex, nofollow`. Public posts, the homepage, static pages, categories, and archives remain indexable.


#### Q: What changed with Google analytics and captcha settings?
A: DasBlog Core now treats this as a breaking change and no longer supports Google-specific analytics or captcha keys.

For existing blogs:
1. Remove `GoogleAnalyticsID`, `AnalyticsTrackingId`, `AnalyticsProvider`, `EnableCaptcha`, `RecaptchaSiteKey`, `RecaptchaSecretKey`, `RecaptchaMinimumScore`, `CaptchaSiteKey`, `CaptchaSecretKey`, and `CaptchaMinimumScore` from your config files.
2. For spam protection, use Akismet moderation (`EnableSpamModeration` + `AkismetAPIKey`) and/or spam question validation (`CheesySpamQ` + `CheesySpamA`).

#### Q: I am not developer, how can I support this effort?
A: Here are a few ways you can support the DasBlog Core effort:
 * Use the product
 * If you like it, tell your friends
 * If you find an bug or have suggestions please create an [issue](https://github.com/poppastring/dasblog-core/issues).
