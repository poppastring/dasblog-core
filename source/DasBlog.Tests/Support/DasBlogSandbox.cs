using System;
using System.IO;
using DasBlog.Tests.Support.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.VisualBasic;

namespace DasBlog.Tests.Support
{
	/// <summary>
	/// The process flow is as follows:
	/// DasBlogInstallation.CheckBeforeAllTests
	/// for each test
	/// 	DasBlogInstallation.CleanupBeforeTest
	/// 	run test
	/// 	optionally DasBlogInstallation.SaveState
	/// 	DasBlogInstallation.CleanupBeforeTest
	/// This will protect the user from wiping out their recent changes to the test materials EXCEPT
	/// if the test process is long running (e.g. stopped in a debugger) and the user makes changes
	/// while it is running.
	/// </summary>
	public class DasBlogSandbox : IDasBlogSandbox
	{
		private readonly IVersionedFileService fileService;
		private readonly string environment;	// e.g, "Vanilla"
		private readonly ILogger<DasBlogSandbox> logger;
		public DasBlogSandbox(ILogger<DasBlogSandbox> logger
			,IVersionedFileService fileService, IOptions<DasBlogISandboxOptions> optionsAccessor)
		{
			this.logger = logger;
			this.fileService = fileService;
			this.environment = optionsAccessor.Value.Environment;
		}
		public void Init()
		{
			(bool active, string errorMessage) = fileService.IsActive();
			if (!active)
			{
				logger.LogError(errorMessage);
				throw new Exception(errorMessage);
			}

			(bool clean, string errorMessage2) = fileService.IsClean(environment);
			if (!clean)
			{
				logger.LogError(errorMessage2);
				throw new WorkingDirectoryModifiedException(errorMessage2);
			}
		}
		/// <summary>
		/// CAlled once at the start of all tests in a suite
		/// The expectation is that the tests will be halted if false is returned
		/// </summary>
		/// <returns>false if the environment is dirty, e.g. there are changes that may
		///   be squashed by running the tests, otherwise true</returns>
		public (bool result, string errorMessage) CheckBeforeAllTests()
		{
			throw new NotImplementedException();
		}
		/// <summary>
		/// Typically called before each test.  It destroys any recent changes to the test environment.
		/// Also typically called after each test to leave things in a good state for next time.
		/// </summary>
		public void CleanupBeforeTest()
		{
			throw new NotImplementedException();
		}
		public void SaveState()
		{
			throw new NotImplementedException();
		}

		/// <summary>
		/// Typically called if a test fails
		/// stashes the current state of the test environment under a commit hash and logs this.
		/// The user can do commit apply hash to restore the state and
		/// then commit reset --hard -- &lt;testenvironment-root-directory&gt;
		/// </summary>
		public void Terminate()
		{
			fileService.StashCurrentStateIfDirty(environment);
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
		/// <returns>e.g. "c:/alt/projs/dasblog-core/source/DasBlog.Tests/Resources/Environments/Vanilla"</returns> 
		public string TestEnvironmentPath => Path.Combine(fileService.TestDataPath, environment);

		public void Dispose()
		{
			Terminate();
		}
	}
	public class WorkingDirectoryModifiedException : Exception
	{
		public WorkingDirectoryModifiedException(string message) : base(message)
		{
				
		}
	}
}
