using System;
using System.IO;
using System.Runtime.CompilerServices;
using DasBlog.SmokeTest.Interfaces;
using LibGit2Sharp;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DasBlog.SmokeTest
{
	/// <summary>
	/// uses git (libgit2sharp) to check that the installation directories are in a pristine
	/// state at the start of the test and that they are returned there at the end.
	/// </summary>
	internal class GitVersionedFileService : IVersionedFileService
	{
		private ILogger<GitVersionedFileService> logger;
		private string path;
		private readonly Repository gitRepo;
		
		public GitVersionedFileService(ILogger<GitVersionedFileService> logger
		  ,IOptions<GitVersionedFileServiceOptions> optionsAccessor, IHostingEnvironment env)
		{
			this.path = optionsAccessor.Value.GitRepo;
			logger.LogInformation("Looking for Git Repo at {Path}", Path.GetFullPath(this.path));
			this.logger = logger;
			gitRepo = new Repository(path);
		}
		public (bool active, string errorMessage) IsActive()
		{
			try
			{
				logger.LogInformation(gitRepo.Branches["master"].CanonicalName);
				return (true, string.Empty);
			}
			catch (Exception e)
			{
				string errorMessage = $@"Unable to find a Git repository at {this.path}

{Constants.ApplicationName} uses Git to handle versioning of the application data.
Particularly restoring the data at the end of the test.

The git path needed is the root of the git tree (containing the .gitt directory).
It can be specified on the command line as 'gitRepo=c:/projects/dasblog'
In the absence of a command line argument the path defaults based on the normal
dasblog-core project layout using the assembly location as a starting point";
				logger.LogError(e, "Unable to continue");
				return (false, errorMessage);
			}
		}

		public (bool clean, string errorMessage) IsClean()
		{
			string errorMessage = string.Empty;
			bool b = gitRepo.Index.IsFullyMerged;
			var status = gitRepo.RetrieveStatus();
			if (status.IsDirty)
			{
				errorMessage = "There are uncommitted changes";
			}
			return (!status.IsDirty, errorMessage);
		}

		public void Restore()
		{
			throw new NotImplementedException();
		}
	}
}
