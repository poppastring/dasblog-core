using System;
using DasBlog.SmokeTest;
using DasBlog.Tests.Support.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using LibGit2Sharp;

namespace DasBlog.Tests.Support
{
	/// <summary>
	/// uses git (libgit2sharp) to check that the installation directories are in a pristine
	/// state at the start of the test and that they are returned there at the end.
	/// </summary>
	public class GitVersionedFileService : IVersionedFileService
	{
		private ILogger<GitVersionedFileService> logger;
		private string path;
		private readonly string gitRepoLocation;
		private Repository gitRepo;
		
		public GitVersionedFileService(ILogger<GitVersionedFileService> logger
		  ,IOptions<GitVersionedFileServiceOptions> optionsAccessor)
		{
			this.path = optionsAccessor.Value.GitRepo;
			logger.LogInformation("Git Repo specified at {Path}", this.path);
			this.logger = logger;
			gitRepoLocation = path;
		}
		public (bool active, string errorMessage) IsActive()
		{
			try
			{
				gitRepo = new Repository(path);
				logger.LogInformation(gitRepo?.Branches["master"]?.CanonicalName);
				return (true, string.Empty);
			}
			catch (Exception e)
			{
				string errorMessage = $@"Unable to find a Git repository at {this.path}

integration tests use Git to handle versioning of the application data.
Particularly restoring the data at the end of the test.

The git path needed is the root of the git tree (containing the .gitt directory).
It is the local repo not the remote repo that is used.
It can be specified in the environment as e.g. DAS_BLOG_GIT_REPO=c:/projects/dasblog-core
In the absence of a command line argument the path defaults based on the normal
dasblog-core project layout using the assembly location as a starting point
e.g. Assembly.Path/../../../../../../";
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
