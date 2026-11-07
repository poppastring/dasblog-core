using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Text;
using System.Threading;
using DasBlog.Services.ConfigFile;
using DasBlog.Services.FileManagement;
using DasBlog.Services.FileManagement.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Xunit;
using Xunit.Abstractions;

namespace DasBlog.Test.Integration
{
	public class AtomicFileWriterTests
	{
		private readonly ITestOutputHelper output;
		public AtomicFileWriterTests(ITestOutputHelper output) => this.output = output;

		[SkippableFact]
		public void Write_WhenFirstPublishHitsDestinationLock_ReleasesLockAndRetriesSuccessfully()
		{
			Skip.IfNot(OperatingSystem.IsWindows(), "Requires Windows replacement lock semantics.");
			using var fixture = new TemporaryDestination();
			var replacement = Encoding.UTF8.GetBytes("<SiteConfig><Theme>darkly</Theme><Description>" + new string('x', 32768) + "</Description></SiteConfig>");
			var writer = new AtomicFileWriter();
			var destinationLock = new FileStream(fixture.Path, FileMode.Open, FileAccess.Read, FileShare.Read);
			var threadId = Thread.CurrentThread.ManagedThreadId;
			IOException observed = null;
			Exception observerFailure = null;
			var releaseCount = 0;
			var observedThreadId = 0;
			var observedHResult = 0;
			EventHandler<FirstChanceExceptionEventArgs> observer = (_, args) =>
			{
				if (Thread.CurrentThread.ManagedThreadId != threadId || releaseCount != 0
					|| args.Exception is not IOException exception || !IsLockError(exception)) return;
				try
				{
					// Attribute the notification to this synchronous publication, not serialization or another test.
					if (!new StackTrace().GetFrames().Any(frame => frame.GetMethod()?.DeclaringType == typeof(AtomicFileWriter)
						&& frame.GetMethod()?.Name == "PublishOnce")) return;
					observed = exception;
					observedHResult = exception.HResult;
					observedThreadId = Thread.CurrentThread.ManagedThreadId;
					releaseCount = 1;
					var ownedLock = destinationLock;
					destinationLock = null;
					ownedLock.Dispose();
				}
				catch (Exception failure) { observerFailure = failure; }
			};
			AppDomain.CurrentDomain.FirstChanceException += observer;
			try
			{
				writer.Write(fixture.Path, stream => stream.Write(replacement, 0, replacement.Length));
			}
			finally
			{
				AppDomain.CurrentDomain.FirstChanceException -= observer;
				destinationLock?.Dispose();
			}

			Assert.Null(observerFailure);
			Assert.NotNull(observed);
			Assert.True(IsLockError(observed));
			Assert.Equal(observed.HResult, observedHResult);
			Assert.Equal(threadId, observedThreadId);
			Assert.Equal(1, releaseCount);
			Assert.Equal(replacement, File.ReadAllBytes(fixture.Path));
			Assert.Empty(Directory.GetFiles(fixture.DirectoryPath, "*.tmp"));
			Report(observed);
		}

		[SkippableFact]
		public void SaveConfig_WhenDestinationRemainsLocked_LogsAndRethrowsOriginalIOException()
		{
			Skip.IfNot(OperatingSystem.IsWindows(), "Requires Windows replacement lock semantics.");
			using var fixture = new TemporaryDestination();
			var logger = new ServiceLogger();
			var service = new SiteConfigFileService(
				Options.Create(new ConfigFilePathsDataOption { SiteConfigFilePath = fixture.Path }), logger, new AtomicFileWriter());
			IOException caught;
			var threadId = Thread.CurrentThread.ManagedThreadId;
			var publishFailures = new List<IOException>();
			Exception observerFailure = null;
			var observing = false;
			var lastObservedHResult = 0;
			EventHandler<FirstChanceExceptionEventArgs> observer = (_, args) =>
			{
				if (Thread.CurrentThread.ManagedThreadId != threadId || observing) return;
				observing = true;
				try
				{
					if (args.Exception is not IOException exception || !IsLockError(exception)) return;
					if (!new StackTrace().GetFrames().Any(frame => frame.GetMethod()?.DeclaringType == typeof(AtomicFileWriter)
						&& frame.GetMethod()?.Name == "PublishOnce")) return;
					// Rethrows notify again; count only distinct publication failures by reference identity.
					if (publishFailures.Any(failure => ReferenceEquals(failure, exception))) return;
					publishFailures.Add(exception);
					lastObservedHResult = exception.HResult;
				}
				catch (Exception failure) { observerFailure = failure; }
				finally { observing = false; }
			};
			using (var destinationLock = new FileStream(fixture.Path, FileMode.Open, FileAccess.Read, FileShare.Read))
			{
				AppDomain.CurrentDomain.FirstChanceException += observer;
				try
				{
					caught = Assert.Throws<IOException>(() => service.SaveConfig(new SiteConfig { Theme = "darkly", Description = "Replacement configuration" }));
				}
				finally
				{
					AppDomain.CurrentDomain.FirstChanceException -= observer;
				}
				Assert.Null(observerFailure);
				Assert.Equal(6, publishFailures.Count);
				Assert.Same(publishFailures.Last(), caught);
				Assert.Equal(lastObservedHResult, caught.HResult);
				Assert.True(IsLockError(caught));
				Assert.False(destinationLock.SafeFileHandle.IsClosed);
			}
			var entry = Assert.Single(logger.Entries);
			Assert.Equal(LogLevel.Error, entry.Level);
			Assert.Same(caught, entry.Exception);
			Assert.Equal(caught.HResult, entry.Exception.HResult);
			Assert.Equal(lastObservedHResult, entry.Exception.HResult);
			Assert.Equal(fixture.Path, entry.State["Path"]);
			Assert.Equal("Failed to save site configuration to {Path}", entry.State["{OriginalFormat}"]);
			Assert.Equal(fixture.Original, File.ReadAllBytes(fixture.Path));
			Assert.Empty(Directory.GetFiles(fixture.DirectoryPath, "*.tmp"));
			Report(caught);
		}

		private static bool IsLockError(IOException exception) => (exception.HResult & 0xFFFF) is 32 or 33 or 1175;
		private void Report(IOException exception) => output.WriteLine($"Observed {exception.GetType().FullName}: HResult=0x{exception.HResult:X8}, low code={exception.HResult & 0xFFFF}");

		private sealed class TemporaryDestination : IDisposable
		{
			public string DirectoryPath { get; } = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "dasblog-lock-regression-" + Guid.NewGuid().ToString("N"));
			public string Path => System.IO.Path.Combine(DirectoryPath, "site.config");
			public byte[] Original { get; } = Encoding.UTF8.GetBytes("<?xml version=\"1.0\"?><SiteConfig><Theme>dasblog</Theme><Description>Original configuration</Description></SiteConfig>");
			public TemporaryDestination()
			{
				Directory.CreateDirectory(DirectoryPath);
				try { File.WriteAllBytes(Path, Original); }
				catch { Directory.Delete(DirectoryPath, true); throw; }
			}
			public void Dispose() => Directory.Delete(DirectoryPath, true);
		}

		private sealed class ServiceLogger : ILogger<SiteConfigFileService>
		{
			public sealed record Entry(LogLevel Level, Exception Exception, Dictionary<string, object> State);
			public List<Entry> Entries { get; } = new();
			public IDisposable BeginScope<TState>(TState state) where TState : notnull => null;
			public bool IsEnabled(LogLevel logLevel) => true;
			public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
				=> Entries.Add(new Entry(logLevel, exception, ((IEnumerable<KeyValuePair<string, object>>)state).ToDictionary(pair => pair.Key, pair => pair.Value)));
		}
	}
}
