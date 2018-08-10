using System;
using DasBlog.Tests.SmokeTest.Support.Interfaces;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;

namespace DasBlog.Tests.SmokeTest.Support
{
	internal class DasBlogInstallation : IDasBlogInstallation
	{
		private readonly IVersionedFileService fileService;
		private readonly string path;
		private readonly ILogger<DasBlogInstallation> logger;
		public DasBlogInstallation(ILogger<DasBlogInstallation> logger
			,IVersionedFileService fileService, IOptions<DasBlogInstallationOptions> optionsAccessor)
		{
			this.logger = logger;
			this.fileService = fileService;
			this.path = optionsAccessor.Value.ContentRootPath;
		}
		public void Init()
		{
			(bool active, string errorMessage) = fileService.IsActive();
			if (!active)
			{
				logger.LogError(errorMessage);
			}

			(bool clean, string errorMessage2) = fileService.IsClean();
			if (!clean)
			{
				logger.LogError(errorMessage2);
			}
		}

		public void Terminate()
		{
			throw new NotImplementedException();
		}

		public string GetConfigPathAndFile()
		{
			throw new NotImplementedException();
		}

		public string GetContentDirectoryPath()
		{
			throw new NotImplementedException();
		}

		public string GetLogDirectoryPath()
		{
			throw new NotImplementedException();
		}

		public string GetWwwRootDirectoryPath()
		{
			throw new NotImplementedException();
		}
	}
}
