using System;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace DasBlog.Services.ActivityPub.Helper
{
	public class ActorService(string privatePem, string keyId, ILogger? logger = null)
	{
		private readonly string _privatePem = privatePem;
		private readonly string _keyId = keyId;
		public ILogger? Logger { get; set; } = logger;

		public static readonly JsonSerializerOptions SerializerOptions = new()
		{
			PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
		};

		public static async Task<Actor> FetchActorInformationAsync(string actorUrl)
		{
			using (var httpClient = new HttpClient())
			{
				httpClient.DefaultRequestHeaders.Add("Accept", "application/activity+json");

				var response = await httpClient.GetAsync(actorUrl);

				if (response.IsSuccessStatusCode)
				{
					var jsonContent = await response.Content.ReadAsStringAsync();
					return JsonSerializer.Deserialize<Actor>(jsonContent, ActorService.SerializerOptions)!;
				}

				throw new Exception($"Failed to fetch information from {actorUrl}");
			}
		}

		static string CreateHashSha256(string input)
		{
			using (var sha256 = SHA256.Create())
			{
				byte[] hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(input));
				return Convert.ToBase64String(hashBytes);
			}
		}

		public async Task SendSignedRequest(string document, Uri url)
		{
			// Get current UTC date in HTTP format
			var date = DateTime.UtcNow.ToString("r");

			// Load RSA private key from file
			using (var rsa = RSA.Create())
			{
				rsa.ImportFromPem(this._privatePem);

				var digest = $"SHA-256={CreateHashSha256(document)}";

				// Build the to-be-signed string
				// string signedString = $"(request-target): post {url.AbsolutePath}\nhost: {url.Host}\ndate: {date}";
				var signedString = $"(request-target): post {url.AbsolutePath}\nhost: {url.Host}\ndate: {date}\ndigest: {digest}";

				// Sign the to-be-signed string

				var signatureBytes = rsa.SignData(Encoding.UTF8.GetBytes(signedString), HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);

				// Base64 encode the signature
				var signature = Convert.ToBase64String(signatureBytes);

				Logger?.LogInformation($"Using key: {this._keyId}");

				// Build the HTTP signature header
				var header = $"keyId=\"{this._keyId}\",headers=\"(request-target) host date digest\",signature=\"{signature}\",algorithm=\"rsa-sha256\"";

				// Create HTTP client
				using (var client = new HttpClient())
				{
					// Set request headers
					client.DefaultRequestHeaders.Add("Host", url.Host);
					client.DefaultRequestHeaders.Add("Date", date);
					client.DefaultRequestHeaders.Add("Signature", header);
					client.DefaultRequestHeaders.Add("Digest", digest);

					Logger?.LogInformation(document);

					// Make the POST request
					var response = await client.PostAsync(url, new StringContent(document, Encoding.UTF8, "application/activity+json"));

					// Print the response
					var responseString = await response.Content.ReadAsStringAsync();

					Logger?.LogInformation($"Response {response.StatusCode} - {responseString}");
				}
			}
		}
	}

}
