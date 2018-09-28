using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using DasBlog.SmokeTest;
using DasBlog.Tests.Support.Common;
using DasBlog.Tests.Support.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace DasBlog.Tests.Support
{
	/// <summary>
	/// uses git to check that the installation directories are in a pristine
	/// state at the start of the test and that they are returned there at the end.
	/// The implementation is by cmd script rather than LibGit2Sharp as the library
	/// did not seem to make things simple
	/// </summary>
	public class GitVersionedFileService : IVersionedFileService
	{
		private ILogger<GitVersionedFileService> logger;
		private string path;
		private readonly string gitRepoLocation;
		private IScriptRunner scriptRunner;

		public GitVersionedFileService(ILogger<GitVersionedFileService> logger
		  ,IOptions<GitVersionedFileServiceOptions> optionsAccessor
		  ,IScriptRunner scriptRunner)
		{
			this.scriptRunner = scriptRunner;
			this.path = optionsAccessor.Value.GitRepo;
			logger.LogInformation("Git Repo specified at {Path}", this.path);
			this.logger = logger;
			gitRepoLocation = path;
		}
		/// <summary>
		/// is git installed and up-to-date - at least v2.15
		/// </summary>
		/// <returns>true if everything is ok (errorMessage is an empty string), otherwise false - probably with the
		///   the actual version number</returns>
		public (bool active, string errorMessage) IsActive()
		{
			(bool success, string gitVersionString) = GetGitVersionString();
			if (!success)
			{
				return (false, $"git failed with error: {gitVersionString}");
			}
			try
			{
				(int major, int minor) = ConvertGitVersionToMajorMinor(gitVersionString);
				if (major > Constants.GitRequiredMajorVersion 
				  || major >= Constants.GitRequiredMajorVersion && minor >= Constants.GitRequiredMinorVersion)
				{
					return (true, string.Empty);
				}
				return (false, $"Git Version number is {major}.{minor}.  The version must be at least 2.15.  If it's 0.0 then look in the logs (maybe)");
			}
			catch (Exception e)
			{
				logger.LogError(e, "Unable to continue");
				return (false, e.Message);
			}
		}
		/// <summary>
		/// returns a half useful version string for further proessing
		/// </summary>
		/// <param name="gitVersionString">e.g. "git version 2.15.windows1" or "something non numeric'</param>
		/// <returns>e.g. "2.15." or ""</returns>
		private string ExtractGitNumericVersion(string gitVersionString)
		{
			const string NUMBERS = "numbers";
			Regex regex = new Regex($@"git version (?<{NUMBERS}>[0-9\\.]*)");
			Match match = regex.Match(gitVersionString);
			string numbers = match.Groups.Where(g => (g.Name ?? string.Empty) == NUMBERS).Select(g => g.Value.TrimEnd() ?? string.Empty).First();
			return numbers;
		}

		/// <summary>
		/// returns a version a person can work with
		/// </summary>
		/// <param name="gitVersionString">e.g. "git version 2.15.windows1" or "something non numeric'</param>
		/// <returns>e.g. (2, 15), or (0, 0))</returns>
		private (int major, int minor) ConvertGitVersionToMajorMinor(string gitVersionString)
		{
			try
			{
				string numericString = ExtractGitNumericVersion(gitVersionString);
				string[] parts = numericString.Split('.');
				int major = 0, minor = 0;
				if (parts.Length > 0)
				{
					major = int.Parse(parts[0]);
				}
				if (parts.Length > 1)
				{
					minor = int.Parse(parts[1]);
				}
				return (major, minor);
			}
			catch (Exception e)
			{
				return (0, 0);
			}
		}

		/// <summary>
		/// throws an exception if git acccess fails for some reason
		/// </summary>
		/// <returns>the list of modified files (or any extraneous output from "git status --short"</returns>
		public (bool clean, string errorMessage) IsClean()
		{
			(int exitCode, string[] outputs, string[] errors ) = scriptRunner.Run(
				Constants.DetectChangesScriptName, scriptRunner.DefaultEnv
				,Path.Combine(Utils.GetProjectRootDirectory(), Constants.VanillaTestData));
			if (exitCode != 0)
			{
				FormatErrorsAndThrow(Constants.DetectChangesScriptName, exitCode, errors);
			}

			int numChanges = outputs.Count(o => !string.IsNullOrWhiteSpace(o));
			if (numChanges > 0)
			{
				return (false, "working directory contains modified files:" + Environment.NewLine
				  + string.Join(Environment.NewLine, outputs.Where(o => !string.IsNullOrWhiteSpace(o))));
			}
			return (true, string.Empty);
		}

		/// <summary>
		/// returns false if no git or git fails
		/// </summary>
		/// <returns>e.g "git version 2.15.0.windows.1" or "git version 1.9.1",
		///   "'git' is not recognized as an internal or external command,"</returns>
		private (bool success, string versionString) GetGitVersionString()
		{
			(int exitCode, string[] outputs, string[] errors ) = scriptRunner.Run(
				Constants.GetGitVersionScriptName, scriptRunner.DefaultEnv);
			if (exitCode != 0)
			{
				string detail = FormatOutputOrErrors(errors);
				return (false, "Detail: " + detail);
			}
			string versionString = outputs.FirstOrDefault(e => !string.IsNullOrWhiteSpace(e));
			return (true, versionString);
		}
		private void FormatErrorsAndThrow(string scriptName, int exitCode, string[] errors)
		{
			string message = errors.FirstOrDefault(e => !string.IsNullOrWhiteSpace(e));
			int numErrors = errors.Count(e => !string.IsNullOrWhiteSpace(e));
			string extraErrors = numErrors == 1 ? string.Empty : $"( + {numErrors - 1} more errors - see logs)";
			throw new Exception($"{scriptName} failed with exitcode {exitCode} details; {message ?? "none provided"}{extraErrors}");
		}

		private string FormatOutputOrErrors(string[] lines)
		{
			string message = lines.FirstOrDefault(e => !string.IsNullOrWhiteSpace(e));
			int numNonEmptyLines = lines.Count(e => !string.IsNullOrWhiteSpace(e));
			string extraLines = numNonEmptyLines == 1 ? string.Empty : $"( + {numNonEmptyLines - 1} more lines - see logs (maybe))";
			return $"{message ?? "nune provided"}{extraLines}";
		}
		public void Restore()
		{
			throw new NotImplementedException();
		}
	}
}
