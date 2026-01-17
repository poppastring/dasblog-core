using DasBlog.Managers.Interfaces;
using DasBlog.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace DasBlog.Web.TagHelpers.User
{
	public class UserAvatarTagHelper : TagHelper
	{
		private readonly IDasBlogSettings dasBlogSettings;
		private readonly IHttpContextAccessor httpContextAccessor;
		private readonly ISiteSecurityManager securityManager;
		private const string gravatarLink = "//www.gravatar.com/avatar/{0}?rating={1}&size={2}&default={3}";

		public string Css { get; set; }

		public int? Size { get; set; }

		public UserAvatarTagHelper(IDasBlogSettings dasBlogSettings, IHttpContextAccessor httpContextAccessor, ISiteSecurityManager securityManager)
		{
			this.dasBlogSettings = dasBlogSettings;
			this.httpContextAccessor = httpContextAccessor;
			this.securityManager = securityManager;
		}

		public override void Process(TagHelperContext context, TagHelperOutput output)
		{
			var user = httpContextAccessor.HttpContext.User;

			output.TagName = "img";
			output.TagMode = TagMode.SelfClosing;

			if (user?.Identity?.IsAuthenticated == true)
			{
				// Get the username from the authenticated identity
				var username = user.Identity.Name;

				// Retrieve the user's email from the security configuration
				var userDetails = securityManager.GetUserByDisplayName(username);

				string email = null;
				if (userDetails != null && !string.IsNullOrEmpty(userDetails.EmailAddress))
				{
					email = userDetails.EmailAddress;
				}
				else
				{
					// Fallback: In DasBlog, username is typically the email
					email = username;
				}

				var hash = GetMd5Hash(email);
				var avatarSize = Size;

				output.Attributes.SetAttribute("src", string.Format(gravatarLink, 
					hash,
					dasBlogSettings.SiteConfiguration.CommentsGravatarRating,
					avatarSize,
					dasBlogSettings.SiteConfiguration.CommentsGravatarNoImgPath));

				var displayName = userDetails?.DisplayName ?? username;
				output.Attributes.SetAttribute("alt", $"{displayName} avatar");
			}
			else
			{
				// Fallback: User icon SVG data URI
				output.Attributes.SetAttribute("src", "data:image/svg+xml,%3Csvg xmlns='http://www.w3.org/2000/svg' fill='%23666' viewBox='0 0 24 24'%3E%3Cpath d='M12 12c2.21 0 4-1.79 4-4s-1.79-4-4-4-4 1.79-4 4 1.79 4 4 4zm0 2c-2.67 0-8 1.34-8 4v2h16v-2c0-2.66-5.33-4-8-4z'/%3E%3C/svg%3E");
				output.Attributes.SetAttribute("alt", "Default avatar");
			}

			// Set default CSS classes
			var cssClasses = "user-avatar rounded-circle";
			if (!string.IsNullOrEmpty(Css))
			{
				cssClasses = $"{cssClasses} {Css}";
			}
			output.Attributes.SetAttribute("class", cssClasses);

			// Set size attributes
			var size = Size ?? 32;
			output.Attributes.SetAttribute("width", size.ToString());
			output.Attributes.SetAttribute("height", size.ToString());
		}

		public override Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
		{
			return Task.Run(() => Process(context, output));
		}

		private string GetMd5Hash(string input)
		{
			if (string.IsNullOrEmpty(input))
			{
				return string.Empty;
			}

			using var md5 = MD5.Create();
			var inputBytes = Encoding.ASCII.GetBytes(input.Trim().ToLowerInvariant());
			var hashBytes = md5.ComputeHash(inputBytes);
			return BitConverter.ToString(hashBytes).Replace("-", "").ToLowerInvariant();
		}
	}
}
