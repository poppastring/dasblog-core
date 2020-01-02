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
			if (SystemCheck())
			{
				return Task.FromResult(
					HealthCheckResult.Healthy("A healthy result."));
			}

			return Task.FromResult(
				HealthCheckResult.Unhealthy("An unhealthy result."));
		}

		private bool SystemCheck()
		{
			if (!File.Exists(fileConfigOption.IISUrlRewriteFilePath))
			{
				return false;
			}

			if (!File.Exists(fileConfigOption.MetaConfigFilePath))
			{
				return false;
			}

			if (!File.Exists(fileConfigOption.SecurityConfigFilePath))
			{
				return false;
			}

			if (!File.Exists(fileConfigOption.SiteConfigFilePath))
			{
				return false;
			}

			if (!Directory.Exists(fileConfigOption.ThemesFolder))
			{
				return false;
			}

			return true;		
		}
	}
}
