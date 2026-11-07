using System;
using System.IO;
using DasBlog.Services.FileManagement;
using Xunit;

namespace DasBlog.Tests.UnitTests.Services
{
	public class AtomicFileWriterTest : IDisposable
	{
		private readonly string tempDirectory;
		private readonly AtomicFileWriter writer = new();

		public AtomicFileWriterTest()
		{
			tempDirectory = Path.Combine(Path.GetTempPath(), "dasblog-test-" + Guid.NewGuid().ToString("N"));
			Directory.CreateDirectory(tempDirectory);
		}

		[Fact]
		public void Write_CreatesDestinationAndRemovesTemporaryFile()
		{
			var destinationPath = Path.Combine(tempDirectory, "config.txt");

			writer.Write(destinationPath, stream => stream.Write("complete"u8));

			Assert.Equal("complete", File.ReadAllText(destinationPath));
			Assert.Empty(Directory.GetFiles(tempDirectory, "*.tmp"));
		}

		[Fact]
		public void Write_WhenSerializationFails_PreservesDestinationAndRemovesTemporaryFile()
		{
			var destinationPath = Path.Combine(tempDirectory, "config.txt");
			File.WriteAllText(destinationPath, "original");

			Assert.Throws<InvalidOperationException>(() => writer.Write(destinationPath, stream =>
			{
				stream.Write("partial"u8);
				throw new InvalidOperationException("Serialization failed.");
			}));

			Assert.Equal("original", File.ReadAllText(destinationPath));
			Assert.Empty(Directory.GetFiles(tempDirectory, "*.tmp"));
		}

		public void Dispose()
		{
			Directory.Delete(tempDirectory, recursive: true);
		}
	}
}
