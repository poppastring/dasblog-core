using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.PlatformAbstractions;

namespace DasBlog.Tests.UnitTests
{
    public class HostingEnvironmentTest : IHostingEnvironment
    {
        public HostingEnvironmentTest()
        {
            EnvironmentName = "Development";
            ApplicationName = "DasBlog.Web.UI";
            WebRootPath = PlatformServices.Default.Application.ApplicationBasePath + "_contentroot"; ;
            WebRootFileProvider = null;
            ContentRootPath = PlatformServices.Default.Application.ApplicationBasePath;
            ContentRootFileProvider = null;
        }

        public string EnvironmentName { get; set; }
        public string ApplicationName { get; set; }
        public string WebRootPath { get; set; }
        public IFileProvider WebRootFileProvider { get; set; }
        public string ContentRootPath { get; set; }
        public IFileProvider ContentRootFileProvider { get; set; }
    }
}
