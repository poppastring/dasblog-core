using DasBlog.Web.Services.Interfaces;
using Ganss.Xss;

namespace DasBlog.Web.Services
{
	public sealed class StaticPageContentSanitizer : IStaticPageContentSanitizer
	{
		private readonly HtmlSanitizer sanitizer;

		public StaticPageContentSanitizer()
		{
			sanitizer = new HtmlSanitizer();
			sanitizer.AllowedSchemes.Clear();
			sanitizer.AllowedSchemes.Add("http");
			sanitizer.AllowedSchemes.Add("https");
			sanitizer.AllowedSchemes.Add("mailto");
		}

		public string Sanitize(string content)
		{
			if (string.IsNullOrWhiteSpace(content))
			{
				return string.Empty;
			}

			return sanitizer.Sanitize(content);
		}
	}
}
