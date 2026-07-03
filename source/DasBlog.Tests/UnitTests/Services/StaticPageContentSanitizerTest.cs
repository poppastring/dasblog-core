using DasBlog.Web.Services;
using Xunit;

namespace DasBlog.Tests.UnitTests.Services
{
	public class StaticPageContentSanitizerTest
	{
		[Theory]
		[Trait("Category", "UnitTest")]
		[InlineData(null)]
		[InlineData("")]
		[InlineData("   ")]
		public void Sanitize_EmptyContent_ReturnsEmptyString(string content)
		{
			var sanitizer = new StaticPageContentSanitizer();

			var result = sanitizer.Sanitize(content);

			Assert.Equal(string.Empty, result);
		}

		[Fact]
		[Trait("Category", "UnitTest")]
		public void Sanitize_MaliciousHtml_RemovesScriptAndEventHandlers()
		{
			var sanitizer = new StaticPageContentSanitizer();
			var malicious = "<p>Hello</p><script>alert('xss')</script><a href=\"javascript:alert('xss')\" onclick=\"alert('xss')\">click</a><img src=\"x\" onerror=\"alert('xss')\" />";

			var result = sanitizer.Sanitize(malicious);

			Assert.Contains("Hello", result);
			Assert.Contains("click", result);
			Assert.DoesNotContain("<script", result, System.StringComparison.OrdinalIgnoreCase);
			Assert.DoesNotContain("onclick", result, System.StringComparison.OrdinalIgnoreCase);
			Assert.DoesNotContain("onerror", result, System.StringComparison.OrdinalIgnoreCase);
			Assert.DoesNotContain("javascript:", result, System.StringComparison.OrdinalIgnoreCase);
		}

		[Fact]
		[Trait("Category", "UnitTest")]
		public void Sanitize_SafeHtml_PreservesBasicFormatting()
		{
			var sanitizer = new StaticPageContentSanitizer();
			var html = "<p>Hello <strong>world</strong> <a href=\"https://example.com\">link</a></p>";

			var result = sanitizer.Sanitize(html);

			Assert.Contains("<p", result, System.StringComparison.OrdinalIgnoreCase);
			Assert.Contains("<strong", result, System.StringComparison.OrdinalIgnoreCase);
			Assert.Contains("<a", result, System.StringComparison.OrdinalIgnoreCase);
			Assert.Contains("https://example.com", result, System.StringComparison.OrdinalIgnoreCase);
			Assert.Contains("Hello", result);
			Assert.Contains("world", result);
			Assert.Contains("link", result);
		}

		[Fact]
		[Trait("Category", "UnitTest")]
		public void Sanitize_ActivityLogHtml_RemovesMaliciousContent()
		{
			var sanitizer = new StaticPageContentSanitizer();
			var html = "<span><strong>Audit:</strong> Login from <a href=\"https://example.com\">example</a></span><script>alert('xss')</script><img src=\"x\" onerror=\"alert('xss')\" />";

			var result = sanitizer.Sanitize(html);

			Assert.Contains("Audit:", result);
			Assert.Contains("<span", result, System.StringComparison.OrdinalIgnoreCase);
			Assert.Contains("<strong", result, System.StringComparison.OrdinalIgnoreCase);
			Assert.Contains("<a", result, System.StringComparison.OrdinalIgnoreCase);
			Assert.DoesNotContain("<script", result, System.StringComparison.OrdinalIgnoreCase);
			Assert.DoesNotContain("onerror", result, System.StringComparison.OrdinalIgnoreCase);
		}
	}
}
