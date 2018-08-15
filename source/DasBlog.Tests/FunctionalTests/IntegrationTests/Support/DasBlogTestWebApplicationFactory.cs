using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace DasBlog.Tests.FunctionalTests.IntegrationTests.Support
{
	public class DasBlogTestWebApplicationFactory
	  : WebApplicationFactory<DasBlog.Web.Startup>
	{
		protected override void ConfigureWebHost(IWebHostBuilder builder)
		{
			base.ConfigureWebHost(builder);
			const string prefix = "c:/projects/dasblog-core/source/";
			builder.UseContentRoot(Path.Combine(prefix, "DasBlog.Web.UI"));
			builder.UseWebRoot(Path.Combine(prefix, "DasBlog.Web.UI/wwwroot"));
//			builder.UseContentRoot(Path.Combine(prefix, "DasBlog.Tests/Resources/Environments/Vanilla"));
//			builder.UseWebRoot(Path.Combine(prefix, "DasBlog.Tests/Resources/Environments/wwwroot"));
		}
	}
}
