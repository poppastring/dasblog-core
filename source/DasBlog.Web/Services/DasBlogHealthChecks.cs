using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DasBlog.Services;
using DasBlog.Services.FileManagement;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;

namespace DasBlog.Web.Services
{
	public class DasBlogHealthChecks : IHealthCheck
	{
		private readonly IDasBlogSettings dasBlogSettings;
		private readonly ConfigFilePathsDataOption fileConfigOption;

		public DasBlogHealthChecks(IDasBlogSettings dasBlogSettings, IOptions<ConfigFilePathsDataOption> fileConfigOption)
		{
			this.dasBlogSettings = dasBlogSettings;
			this.fileConfigOption = fileConfigOption.Value;
		}

		public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
		{
			var systemCheck = SystemCheck(); 
			if (SystemCheck().result)
			{
				return Task.FromResult(
					HealthCheckResult.Healthy("A healthy result."));
			}

			return Task.FromResult(
				HealthCheckResult.Unhealthy(systemCheck.why)); //this will be logged
		}

		private (bool result, string why) SystemCheck()
		{
			if (!File.Exists(fileConfigOption.IISUrlRewriteFilePath))
			{
				return (false, $"IIS Rewrite File Path {fileConfigOption.IISUrlRewriteFilePath}");
			}

			if (!File.Exists(fileConfigOption.MetaConfigFilePath))
			{
				return (false, $"MetaConfig File Path {fileConfigOption.MetaConfigFilePath}");
			}

			if (!File.Exists(fileConfigOption.SecurityConfigFilePath))
			{
				return (false, $"Security Config File Path {fileConfigOption.SecurityConfigFilePath}");
			}

			if (!File.Exists(fileConfigOption.SiteConfigFilePath))
			{
				return (false, $"Site Config File Path {fileConfigOption.SiteConfigFilePath}");
			}

			if (!Directory.Exists(fileConfigOption.ThemesFolder))
			{
				return (false, $"Themes Folder {fileConfigOption.ThemesFolder}");
			}

			return (true, String.Empty);		
		}
	}
}
