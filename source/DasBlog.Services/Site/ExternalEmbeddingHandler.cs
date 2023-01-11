using System;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Serialization;
using DasBlog.Services.ConfigFile;
using HtmlAgilityPack;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace DasBlog.Services.Site
{
	public class ExternalEmbeddingHandler : IExternalEmbeddingHandler
	{
		const string CACHEPREFIX_WEBMANIFEST = "webmanifest_";
		const string CACHEPREFIX_OGEMBEDDINGS = "ogembeddings_";
		const string CACHEPREFIX_OEMBEMBEDDINGS = "oembembeddings_";
		readonly TimeSpan positiveExpiration = TimeSpan.FromMinutes(60); // expiration for extries we have found
		readonly TimeSpan negativeExpiration = TimeSpan.FromMinutes(240); // expiration for entries we haven't found
		private readonly IDasBlogSettings dasBlogSettings;
		private readonly IMemoryCache memoryCache;
		private readonly ILogger logger;
		private static XmlSerializer xmlOEmbedSerializer = new XmlSerializer(typeof(OEmbed));

		HttpClient client;
		public ExternalEmbeddingHandler(IDasBlogSettings dasBlogSettings, IMemoryCache memoryCache, ILogger<IExternalEmbeddingHandler> logger)
		{
			client = new HttpClient();
			this.dasBlogSettings = dasBlogSettings;
			this.memoryCache = memoryCache;
			this.logger = logger;
		}

#pragma warning disable CS1998
		public async Task<string> InjectCategoryLinksAsync(string content)
#pragma warning restore CS1998
		{
			if (dasBlogSettings.SiteConfiguration.EnableRewritingHashtagsToCategoryLinks)
			{
				return Regex.Replace(content, @"(?<!=['""])#(\w+)", $"<a href=\"{dasBlogSettings.RelativeToRoot("category/")}$1\">#$1 </a>");
			}
			else
			{
				return content;
			}
		}

		public static bool IsWildCardUrlMatch(string pattern, string text)
		{
			pattern = pattern.Replace(".", @"\.").Replace("*", ".*");
			return Regex.IsMatch(text, pattern);
		}

		protected async Task<string> GetOpenGraphAndTwitterCardEmbeddingAsync(string url)
		{
			string cacheKey = CACHEPREFIX_OGEMBEDDINGS + url;
			string embedding;

			if (!memoryCache.TryGetValue(cacheKey, out embedding))
			{
				try
				{
					// Use HttpClient to send a GET request to the page
					HttpResponseMessage response = await client.GetAsync(url);
					if (response.IsSuccessStatusCode && response.Content.Headers.GetValues("Content-Type").FirstOrDefault().StartsWith("text/html"))
					{
						string html = await response.Content.ReadAsStringAsync();

						// Use HtmlAgilityPack to parse the HTML content
						HtmlDocument doc = new HtmlDocument();
						doc.LoadHtml(html);

						// create the site title
						string siteTitle = response.RequestMessage.RequestUri.Host;
						string[] parts = siteTitle.Split('.');
						if (parts.Length > 2)
						{
							siteTitle = string.Join(".", parts.Skip(parts.Length - 2));
						}

						// get the web site manifest
						WebManifest webManifest = null;
						string manifestUrl = doc.DocumentNode.SelectSingleNode("//link[@rel='manifest']")?.Attributes["href"]?.Value;
						if (manifestUrl != null)
						{
							Uri manifestUri = new Uri(response.RequestMessage.RequestUri, manifestUrl);
							webManifest = await GetWebManifestAsync(manifestUri);
						}

						if (webManifest != null && !string.IsNullOrEmpty(webManifest.Name))
						{
							siteTitle = webManifest.Name;
						}

						if (webManifest?.Icons?.Count() > 0)
						{
							ManifestImageResource icon = webManifest.Icons?.FirstOrDefault();
							if (icon != null && !string.IsNullOrEmpty(icon.Src))
							{
								siteTitle = $"<img class=\"opengraph-preview-site-icon\" src=\"{new Uri(response.RequestMessage.RequestUri, icon.Src).AbsoluteUri}\"></img> {siteTitle}";
							}
						}
						else
						{

							string icon = doc.DocumentNode.SelectSingleNode("//link[translate(@rel, 'ABCDEFGHIJKLMNOPQRSTUVWXYZ', 'abcdefghijklmnopqrstuvwxyz')='icon']")?.Attributes["href"]?.Value;
							if (string.IsNullOrEmpty(icon))
							{
								icon = doc.DocumentNode.SelectSingleNode("//link[translate(@rel, 'ABCDEFGHIJKLMNOPQRSTUVWXYZ', 'abcdefghijklmnopqrstuvwxyz')='alternate icon']")?.Attributes["href"]?.Value;
							}
							if (string.IsNullOrEmpty(icon))
							{
								icon = doc.DocumentNode.SelectSingleNode("//link[translate(@rel, 'ABCDEFGHIJKLMNOPQRSTUVWXYZ', 'abcdefghijklmnopqrstuvwxyz')='shortcut icon']")?.Attributes["href"]?.Value;
							}
							if (!string.IsNullOrEmpty(icon))
							{
								siteTitle = $"<img class=\"opengraph-preview-site-icon\" src=\"{new Uri(response.RequestMessage.RequestUri, icon).AbsoluteUri}\"></img> {siteTitle}";
							}
						}

						// Extract the values of the og:title, og:description, and og:image <meta> tags
						string title = doc.DocumentNode.SelectSingleNode("//meta[@property='og:title']")?.Attributes["content"]?.Value;
						string description = doc.DocumentNode.SelectSingleNode("//meta[@property='og:description']")?.Attributes["content"]?.Value;
						string imageUrl = doc.DocumentNode.SelectSingleNode("//meta[@property='og:image']")?.Attributes["content"]?.Value;
						if (string.IsNullOrEmpty(title))
						{
							title = doc.DocumentNode.SelectSingleNode("//meta[@property='twitter:title']")?.Attributes["content"]?.Value;
						}
						if (string.IsNullOrEmpty(description))
						{
							title = doc.DocumentNode.SelectSingleNode("//meta[@property='twitter:description']")?.Attributes["content"]?.Value;
						}
						if (string.IsNullOrEmpty(imageUrl))
						{
							title = doc.DocumentNode.SelectSingleNode("//meta[@property='twitter:imageUrl']")?.Attributes["content"]?.Value;
						}

						if (string.IsNullOrEmpty(title) && string.IsNullOrEmpty(description) && string.IsNullOrEmpty(imageUrl))
						{
							memoryCache.Set(cacheKey, null as object, negativeExpiration);
							return null;
						}

						// Build the HTML code for the OpenGraph preview
						string htmlResult = $"<a class=\"opengraph-preview-link\" href=\"{url}\"><div class=\"opengraph-preview\">";
						if (!string.IsNullOrEmpty(siteTitle))
						{
							htmlResult += $"<div class=\"opengraph-preview-site\">{siteTitle}</div>";
						}
						if (!string.IsNullOrEmpty(imageUrl))
						{
							htmlResult += $"<img class=\"opengraph-preview-image\" src=\"{imageUrl}\"/>";
						}
						htmlResult += "<div class=\"opengraph-preview-info\">";
						if (!string.IsNullOrEmpty(title))
						{
							htmlResult += $"<h3 class=\"opengraph-preview-title\">{title}</h3>";
						}
						if (!string.IsNullOrEmpty(description))
						{
							htmlResult += $"<p class=\"opengraph-preview-description\">{description}</p>";
						}
						htmlResult += "</div></div></a>";
						memoryCache.Set(cacheKey, htmlResult, positiveExpiration);
						return htmlResult;
					}
				}
				catch (Exception ex)
				{
					logger.LogError(ex, $"error requesting web page from {url}");
				}
				memoryCache.Set(cacheKey, null as object, negativeExpiration);
				return null;
			}
			else
			{
				return embedding;
			}
		}

		private async Task<WebManifest> GetWebManifestAsync(Uri manifestUri)
		{
			WebManifest webManifest;
			var cacheKey = CACHEPREFIX_WEBMANIFEST + manifestUri.AbsoluteUri;
			if (!memoryCache.TryGetValue(cacheKey, out webManifest))
			{
				try
				{
					var manifestResult = await client.GetAsync(manifestUri);
					if (manifestResult != null && manifestResult.IsSuccessStatusCode &&
						(manifestResult.Content.Headers.ContentType.MediaType.Contains("+json") ||
						 manifestResult.Content.Headers.ContentType.MediaType.Contains("/json")))
					{
						webManifest = WebManifest.FromJson(await manifestResult.Content.ReadAsStringAsync());
					}
					if (webManifest != null)
					{
						// fix up icon URIs that are relative to the manifest
						foreach (var item in webManifest.Icons)
						{
							item.Src = new Uri(manifestUri, item.Src).AbsoluteUri;
						}
					}
					// we are also adding the manifest if it's NULL so that we don't do this request again
					memoryCache.Set(cacheKey, webManifest, TimeSpan.FromDays(1));
				}
				catch (Exception ex)
				{
					logger.LogError(ex, $"error requesting web manifest from {manifestUri}");
				}
			}
			return webManifest;
		}

		public async Task<string> InjectDynamicEmbeddingsAsync(string content)
		{
			if ( !dasBlogSettings.SiteConfiguration.EnableRewritingBareLinksToEmbeddings )
			{ 
			
				return content;
			}
			/*
			 * I'm finding embeddings iteratively with a do/while loop. I could 
			 * find all the matches in one go, fan out acquisition of the embeddings for paralellization  
			 * and then process the the results in sequence, but that would probably be too much voodoo
			 * for the moment.
			 */
			var linkPattern = @"(?:(<a\s+([^>]+)?href=['""](?'url'(?:http(s)?://)\S+)['""]([^>]+)?>\k'url'<\/a>)|(?<![\+&])(?<!=['""]?)(?<!<[aA][^>]+>)(?'url'(?:http(s)?://)[^<\s]*))";
			Match match = Regex.Match(content, linkPattern);
			int stringOffset = 0;
			if (match.Success)
			{
				do
				{
					bool linkReplaced = false;
					string rest = content.Substring(stringOffset + match.Index + match.Length);
					var link = match.Groups["url"].Value;
					var matchOffset = match.Length;
					OEmbed embedding;

					var cacheKey = CACHEPREFIX_OEMBEMBEDDINGS + link;
					if (!memoryCache.TryGetValue(cacheKey, out embedding))
					{
						OEmbedEndpoint endpoint;
						OEmbedProvider provider;

						if (FindOEmbedEndpoint(link, out endpoint, out provider) && endpoint.Url != null)
						{
							try
							{
								// wait? didn't we check the cache just above? Yes, but that was before checking all the providers.
								// we now found the provider that matches and just blocked on it and 
								// will now look whether a parallel thread managed to already fetch the data
								// before us
								if (!memoryCache.TryGetValue(cacheKey, out embedding))
								{
									var endpointUrl = endpoint.Url.Replace("{format}", "json");
									var result = await client.GetAsync(new Uri($"{endpointUrl}?url={link}"));
									if (result.StatusCode == System.Net.HttpStatusCode.OK)
									{
										if (result.Content.Headers.GetValues("Content-Type").FirstOrDefault().StartsWith("application/json"))
										{
											embedding = JsonConvert.DeserializeObject<OEmbed>(await result.Content.ReadAsStringAsync());
										}
										else if (result.Content.Headers.GetValues("Content-Type").FirstOrDefault().StartsWith("text/xml"))
										{
											embedding = (OEmbed)xmlOEmbedSerializer.Deserialize(await result.Content.ReadAsStreamAsync());
										}
									}
									else if (result.StatusCode == System.Net.HttpStatusCode.NotFound ||
												result.StatusCode == System.Net.HttpStatusCode.Forbidden)
									{
										// remove the endpoint for this instance
										endpoint.Url = null;
									}
								}
							}
							catch (Exception ex)
							{
								logger.LogError(ex, $"error handling oEmbed endpoint {endpoint.Url}");
								endpoint.Url = null;
							}
						}
						memoryCache.Set(cacheKey, embedding, positiveExpiration);
					}

					if (embedding != null)
					{
						content = content.Substring(0, stringOffset + match.Index) + embedding.Html + rest;
						matchOffset = embedding.Html.Length;
						linkReplaced = true;
					}

					if (!linkReplaced)
					{
						var html = await GetOpenGraphAndTwitterCardEmbeddingAsync(link);
						if (!string.IsNullOrEmpty(html))
						{
							content = content.Substring(0, stringOffset + match.Index) + html + rest;
							matchOffset = html.Length;
						}
					}

					stringOffset = stringOffset + match.Index + matchOffset;
					match = Regex.Match(rest, linkPattern);
				} while (match.Success);
			}
			return content;
		}

		private bool FindOEmbedEndpoint(string link, out OEmbedEndpoint endpoint, out OEmbedProvider provider)
		{
			endpoint = null;
			provider = null;
			var prvs = dasBlogSettings.OEmbedProviders.Providers.ToList();
			// sort reversely by the tracked UsageCount in the provider object
			prvs.Sort();
			foreach (var prv in prvs)
			{
				if (prv.Endpoints != null) foreach (var ep in prv.Endpoints)
					{
						if (ep.Schemes != null) foreach (var scheme in ep.Schemes)
							{
								if (IsWildCardUrlMatch(scheme, link))
								{
									provider = prv;
									endpoint = ep;
									provider.UsageCount++;
									return true;
								}
							}
					}
			}
			return false;
		}

#pragma warning disable CS1998
		public async Task<string> InjectIconsForBareLinksAsync(string content)
#pragma warning restore CS1998
		{
			if (!dasBlogSettings.SiteConfiguration.EnableRewritingBareLinksToIcons)
			{

				return content;
			}

			// This replaces the link with a link icon if the href and the inner text are identical
			content = Regex.Replace(content, @"(<a\s+([^>]+)?href=['""](\S+)['""]([^>]+)?>\3<\/a>)", "<a $2 href='$3' $4>&#x1F517;</a>");
			// This replaces "naked" links outside of HTML attributes and as text of <a> tags
			content = Regex.Replace(content, @"(?<![\+&])(?<!=['""]?)(?<!/)(?<!\?)(?<!<[aA][^>]+>)((?:http(s)?://)[^<\s]*)", @"<a href=""$1"">&#x1F517;</a>");
			return content;
		}


	}
}
