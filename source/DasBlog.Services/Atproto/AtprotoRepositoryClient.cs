using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace DasBlog.Services.Atproto
{
	public sealed class AtprotoRepositoryClient : IAtprotoRepositoryClient
	{
		private const string PublicationCollection = "site.standard.publication";
		private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);
		private readonly IHttpClientFactory httpClientFactory;

		public AtprotoRepositoryClient(IHttpClientFactory httpClientFactory)
		{
			this.httpClientFactory = httpClientFactory;
		}

		public async Task<AtprotoSession> CreateSessionAsync(string pdsUrl, string handle, string appPassword, CancellationToken cancellationToken = default)
		{
			using var request = new HttpRequestMessage(HttpMethod.Post, CreateXrpcUri(pdsUrl, "com.atproto.server.createSession"));
			request.Content = CreateJsonContent(new { identifier = handle, password = appPassword });

			using var response = await httpClientFactory.CreateClient().SendAsync(request, cancellationToken);
			await EnsureSuccessAsync(response, cancellationToken);

			var session = await JsonSerializer.DeserializeAsync<AtprotoSession>(await response.Content.ReadAsStreamAsync(cancellationToken), JsonOptions, cancellationToken);
			if (string.IsNullOrWhiteSpace(session?.Did) || string.IsNullOrWhiteSpace(session.AccessJwt))
			{
				throw new InvalidOperationException("ATProto did not return a valid session.");
			}

			return new AtprotoSession { Did = session.Did, AccessJwt = session.AccessJwt, PdsUrl = pdsUrl };
		}

		public async Task PutPublicationAsync(AtprotoSession session, string rkey, AtprotoPublication publication, CancellationToken cancellationToken = default)
		{
			using var request = CreateAuthenticatedRequest(session, "com.atproto.repo.putRecord");
			request.Content = CreateJsonContent(new
			{
				repo = session.Did,
				collection = PublicationCollection,
				rkey,
				record = new Dictionary<string, object>
				{
					["$type"] = PublicationCollection,
					["url"] = publication.Url,
					["name"] = publication.Name,
					["description"] = publication.Description
				}
			});

			using var response = await httpClientFactory.CreateClient().SendAsync(request, cancellationToken);
			await EnsureSuccessAsync(response, cancellationToken);
		}

		public async Task DeletePublicationAsync(AtprotoSession session, string rkey, CancellationToken cancellationToken = default)
		{
			using var request = CreateAuthenticatedRequest(session, "com.atproto.repo.deleteRecord");
			request.Content = CreateJsonContent(new { repo = session.Did, collection = PublicationCollection, rkey });

			using var response = await httpClientFactory.CreateClient().SendAsync(request, cancellationToken);
			await EnsureSuccessAsync(response, cancellationToken);
		}

		private static Uri CreateXrpcUri(string pdsUrl, string method)
		{
			if (!Uri.TryCreate(pdsUrl, UriKind.Absolute, out var pdsUri) || pdsUri.Scheme != Uri.UriSchemeHttps)
			{
				throw new ArgumentException("The ATProto PDS URL must be an absolute HTTPS URL.", nameof(pdsUrl));
			}

			return new Uri(pdsUri, $"xrpc/{method}");
		}

		private HttpRequestMessage CreateAuthenticatedRequest(AtprotoSession session, string method)
		{
			var request = new HttpRequestMessage(HttpMethod.Post, CreateXrpcUri(session.PdsUrl, method));
			request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", session.AccessJwt);
			return request;
		}

		private static StringContent CreateJsonContent(object value)
		{
			return new StringContent(JsonSerializer.Serialize(value, JsonOptions), Encoding.UTF8, "application/json");
		}

		private static async Task EnsureSuccessAsync(HttpResponseMessage response, CancellationToken cancellationToken)
		{
			if (!response.IsSuccessStatusCode)
			{
				var responseBody = await response.Content.ReadAsStringAsync(cancellationToken);
				throw new HttpRequestException($"ATProto request failed with status {(int)response.StatusCode}: {responseBody}");
			}
		}
	}
}
