using System;
using System.Collections.Concurrent;
using System.IO;
using System.Threading;
using DasBlog.Services.FileManagement.Interfaces;

namespace DasBlog.Services.FileManagement
{
	public sealed class AtomicFileWriter : IAtomicFileWriter
	{
		private readonly ConcurrentDictionary<string, object> pathLocks = new(
			OperatingSystem.IsWindows() ? StringComparer.OrdinalIgnoreCase : StringComparer.Ordinal);

		public void Write(string destinationPath, Action<Stream> write)
		{
			ArgumentException.ThrowIfNullOrWhiteSpace(destinationPath);
			ArgumentNullException.ThrowIfNull(write);

			var fullPath = Path.GetFullPath(destinationPath);
			var pathLock = pathLocks.GetOrAdd(fullPath, static _ => new object());

			lock (pathLock)
			{
				var tempPath = $"{fullPath}.{Guid.NewGuid():N}.tmp";

				try
				{
					using (var stream = new FileStream(tempPath, FileMode.CreateNew, FileAccess.Write, FileShare.None))
					{
						write(stream);
						stream.Flush(flushToDisk: true);
					}

					Publish(tempPath, fullPath);
				}
				finally
				{
					File.Delete(tempPath);
				}
			}
		}

		private static void Publish(string tempPath, string destinationPath)
		{
			for (var attempt = 0; ; attempt++)
			{
				try
				{
					PublishOnce(tempPath, destinationPath);
					return;
				}
				catch (IOException exception) when (OperatingSystem.IsWindows()
					&& attempt < 5 && (exception.HResult & 0xFFFF) is 32 or 33 or 1175)
				{
					// Windows configuration readers can briefly hold the file without delete sharing.
					Thread.Sleep(20 << attempt);
				}
			}
		}

		private static void PublishOnce(string tempPath, string destinationPath)
		{
			if (File.Exists(destinationPath))
			{
				File.Replace(tempPath, destinationPath, destinationBackupFileName: null);
				return;
			}

			try
			{
				File.Move(tempPath, destinationPath);
			}
			catch (IOException) when (File.Exists(destinationPath))
			{
				File.Replace(tempPath, destinationPath, destinationBackupFileName: null);
			}
		}
	}
}
