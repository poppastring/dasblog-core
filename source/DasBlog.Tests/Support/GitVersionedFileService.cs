using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
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
		private readonly string testDataPath;
		private IScriptRunner scriptRunner;

		public GitVersionedFileService(ILogger<GitVersionedFileService> logger
		  ,IOptions<GitVersionedFileServiceOptions> optionsAccessor
		  ,IScriptRunner scriptRunner)
		{
			this.scriptRunner = scriptRunner;
			this.testDataPath = Path.Combine(optionsAccessor.Value.GitRepoDirectory, optionsAccessor.Value.TestDataDirectroy);
			logger.LogInformation("Git Repo specified at {Path}", optionsAccessor.Value.GitRepoDirectory);
			this.logger = logger;
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
			return ExtractFieldViaRegex(NUMBERS, $@"git version (?<{NUMBERS}>[0-9\\.]*)", gitVersionString);
		}

		private string ExtractFieldViaRegex(string fieldName, string regex, string text)
		{
			var map = RegexToMap(regex, text);
			return map.Where(g => g.Key == fieldName).Select(m => m.Value).DefaultIfEmpty(string.Empty).First();
		}
		private IReadOnlyDictionary<string, string> RegexToMap(string regexArg, string text)
		{
			try
			{
				IDictionary<string, string> map = new Dictionary<string, string>();
				Regex regex = new Regex(regexArg);
				Match match = regex.Match(text);
				return match.Groups.Values.Where(g => !string.IsNullOrWhiteSpace(g.Name)).ToDictionary(g => g.Name, g => g.Value);
			}
			catch (Exception )
			{
				throw;	// probably duplicate group names
			}
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
				_ = e;
				return (0, 0);
			}
		}

		/// <summary>
		/// throws an exception if git acccess fails for some reason
		/// </summary>
		/// <param name="environment">e.g. "Vanilla"</param>
		/// <param name="suppressLog"></param>
		/// <returns>false if there were changes in the working directory
		///   and the list of modified files (or any extraneous output from "git status --short"</returns>
		public (bool clean, string errorMessage) IsClean(string environment, bool suppressLog)
		{
			(int exitCode, string[] outputs ) = scriptRunner.Run(
				Constants.DetectChangesScriptId, scriptRunner.DefaultEnv, suppressLog,Path.Combine(this.testDataPath, environment));
					// e.g. "C:\alt\projs\dasblog-core\source\DasBlog.Tests\Resources\Environments\Vanilla"
			if (exitCode != 0)
			{
				FormatErrorsAndThrow(Constants.DetectChangesScriptId, exitCode, outputs);
			}
			int numChanges = outputs.Count(o => !string.IsNullOrWhiteSpace(o));
			if (numChanges > 0)
			{
				return (false, "working directory contains modified files:" + Environment.NewLine
				  + string.Join(Environment.NewLine, outputs.Where(o => !string.IsNullOrWhiteSpace(o))));
			}
			return (true, string.Empty);
		}

		public void StashCurrentStateIfDirty(string environment, bool suppressLog)
		{
			if (IsClean(environment, suppressLog).clean)
			{
				if (!suppressLog) logger.LogInformation("No changes were made to the test environment during the course of this test");
				return;
			}
			Guid guid = Guid.NewGuid();
			(int exitCode, string[] outputs ) = scriptRunner.Run(
				Constants.StashCurrentStateScriptId, scriptRunner.DefaultEnv, suppressLog,Path.Combine(this.testDataPath, environment)
				,guid.ToString());
			if (exitCode != 0)
			{
				FormatErrorsAndThrow(Constants.StashCurrentStateScriptId, exitCode, outputs);
			}

			string stashHash = GetHashField(outputs);
			ConfirmValidStash(stashHash, guid, suppressLog);
			if (!suppressLog) logger.LogInformation("A copy of the state of the file system has been made at the end of this test"
			  + Environment.NewLine + $"You can restore the state by doing 'git stash apply {stashHash}'");
		}

		public string TestDataPath => testDataPath;

		private void ConfirmValidStash(string stashHash, Guid guid, bool suppressLog)
		{
			(int exitCode, string[] outputs) = scriptRunner.Run(
				Constants.ConfirmStashScriptId, scriptRunner.DefaultEnv, suppressLog,stashHash);
			if (exitCode != 0)
			{
				FormatErrorsAndThrow(Constants.ConfirmStashScriptId, exitCode, outputs);
			}

			string stashGuid = GetGuidFromStash(outputs);
			if (string.IsNullOrWhiteSpace(stashGuid) || stashGuid != guid.ToString() )
			{
				throw new Exception($"Was expecting the stash, {stashHash}, to contain the guid {guid}."
				                    + Environment.NewLine +
				                    "Please examine the git log and stash to ensure all is well."
				                    + Environment.NewLine +
				                    Environment.NewLine +
				                    $"Errors:{Environment.NewLine}{FormatForDisplay(outputs)}{Environment.NewLine}Outputs:{FormatForDisplay(outputs)}"
				                    );
			}
		}

		private string FormatForDisplay(string[] lines)
		{
			string message = string.Join(Environment.NewLine, lines.Where(e => !string.IsNullOrWhiteSpace(e)).DefaultIfEmpty(string.Empty));
			return message;
		}

		private string GetGuidFromStash(string[] outputs)
		{
			const string GUID = "guid";
			foreach (string output in outputs)
			{
				string stashedGuid = ExtractFieldViaRegex(GUID, $@".* (?<{GUID}>[0-9a-z\-]*)$", output ?? string.Empty);
				if (!string.IsNullOrWhiteSpace(stashedGuid))
				{
					return stashedGuid;
				}
			}
			return string.Empty;
		}

		private string GetHashField(string[] outputs)
		{
			const string HASH = "hash";
			foreach (string output in outputs)
			{
				string stashHash = ExtractFieldViaRegex(HASH, $@".* \((?<{HASH}>[0-9a-z]*)\)$", output ?? string.Empty);
				if (!string.IsNullOrWhiteSpace(stashHash))
				{
					return stashHash;
				}
			}

			return string.Empty;
		}

		/// <summary>
		/// returns false if no git or git fails
		/// </summary>
		/// <returns>e.g "git version 2.15.0.windows.1" or "git version 1.9.1",
		///   "'git' is not recognized as an internal or external command,"</returns>
		private (bool success, string versionString) GetGitVersionString()
		{
			(int exitCode, string[] outputs ) = scriptRunner.Run(
				Constants.GetGitVersionScriptId, scriptRunner.DefaultEnv, false);
			if (exitCode != 0)
			{
				string detail = FormatOutputOrErrors(outputs);
				return (false, "Detail: " + detail);
			}
			string versionString = outputs.FirstOrDefault(e => !string.IsNullOrWhiteSpace(e) && e.StartsWith("git version"));
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
	}
}
