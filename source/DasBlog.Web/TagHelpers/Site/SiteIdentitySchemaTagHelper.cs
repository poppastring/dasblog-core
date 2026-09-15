using DasBlog.Services;
using DasBlog.Services.ConfigFile.Interfaces;
using DasBlog.Web.Models.AdminViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Collections.Generic;
using System.Text.Json;

namespace DasBlog.Web.TagHelpers.Site
{
	[HtmlTargetElement("site-identity-schema", TagStructure = TagStructure.WithoutEndTag)]
	public class SiteIdentitySchemaTagHelper : TagHelper
	{
		private readonly IDasBlogSettings dasBlogSettings;
		private readonly IHttpContextAccessor httpContextAccessor;

		public SiteIdentitySchemaTagHelper(IDasBlogSettings dasBlogSettings, IHttpContextAccessor httpContextAccessor)
		{
			this.dasBlogSettings = dasBlogSettings;
			this.httpContextAccessor = httpContextAccessor;
		}

		public override void Process(TagHelperContext context, TagHelperOutput output)
		{
			var request = httpContextAccessor.HttpContext?.Request;
			if (request == null || request.Path != "/")
			{
				output.SuppressOutput();
				return;
			}

			var config = dasBlogSettings.SiteConfiguration;
			var metadata = dasBlogSettings.MetaTags;
			var publisher = new Dictionary<string, object>
			{
				["@type"] = MetaViewModel.NormalizePublisherType(metadata.PublisherType),
				["name"] = config.Title,
				["url"] = dasBlogSettings.GetBaseUrl()
			};
			if (!string.IsNullOrWhiteSpace(metadata.TwitterImage))
			{
				publisher["image"] = dasBlogSettings.RelativeToRoot(metadata.TwitterImage);
			}
			var sameAs = GetSameAs(metadata);
			if (sameAs.Count > 0)
			{
				publisher["sameAs"] = sameAs;
			}

			var website = new Dictionary<string, object>
			{
				["@context"] = "https://schema.org",
				["@type"] = "WebSite",
				["name"] = config.Title,
				["url"] = dasBlogSettings.GetBaseUrl(),
				["publisher"] = publisher
			};

			output.TagName = "script";
			output.TagMode = TagMode.StartTagAndEndTag;
			output.Attributes.SetAttribute("type", "application/ld+json");
			output.Content.SetHtmlContent(JsonSerializer.Serialize(website));
		}

		private static IReadOnlyList<string> GetSameAs(IMetaTags metadata)
		{
			var sameAs = new List<string>();
			var twitterProfileUrl = GetTwitterProfileUrl(metadata.TwitterSite);
			if (!string.IsNullOrEmpty(twitterProfileUrl))
			{
				sameAs.Add(twitterProfileUrl);
			}

			var mastodonProfileUrl = GetMastodonProfileUrl(metadata.MastodonServerUrl, metadata.MastodonAccount);
			if (!string.IsNullOrEmpty(mastodonProfileUrl))
			{
				sameAs.Add(mastodonProfileUrl);
			}

			return sameAs;
		}

		private static string GetTwitterProfileUrl(string twitterSite)
		{
			var handle = twitterSite?.Trim().TrimStart('@');
			return string.IsNullOrWhiteSpace(handle) ? null : $"https://x.com/{handle}";
		}

		private static string GetMastodonProfileUrl(string mastodonServerUrl, string mastodonAccount)
		{
			var serverUrl = mastodonServerUrl?.Trim().TrimEnd('/');
			var account = mastodonAccount?.Trim().TrimStart('@');
			if (string.IsNullOrWhiteSpace(serverUrl) || string.IsNullOrWhiteSpace(account))
			{
				return null;
			}

			return $"{serverUrl}/@{account}";
		}
	}
}
