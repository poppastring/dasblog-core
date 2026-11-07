using System;
using System.IO;

namespace DasBlog.Services.FileManagement.Interfaces
{
	public interface IAtomicFileWriter
	{
		void Write(string destinationPath, Action<Stream> write);
	}
}
