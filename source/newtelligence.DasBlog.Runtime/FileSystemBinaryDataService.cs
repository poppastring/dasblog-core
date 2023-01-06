using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace newtelligence.DasBlog.Runtime
{

    /// <summary>
    /// 
    /// </summary>
    public static class BinaryDataServiceFactory
    {
        private static Dictionary<string, IBinaryDataService> services = new Dictionary<string, IBinaryDataService>(StringComparer.OrdinalIgnoreCase);
        private static object serviceLock = new object();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="contentLocation"></param>
        /// <returns></returns>
        public static IBinaryDataService GetService(string contentLocation, Uri rootUrl, ILoggingDataService loggingService, ICdnManager cdnManager)
        {
            IBinaryDataService service;

            lock (serviceLock)
            {
                if (!services.TryGetValue(contentLocation, out service))
                {
	                service = new FileSystemBinaryDataService(contentLocation, rootUrl, loggingService, cdnManager);
                    services.Add(contentLocation, service);
                }
            }
            return service;
        }

        public static bool RemoveService(string contentLocation)
        {
            lock (serviceLock)
            {
                if (services.ContainsKey(contentLocation))
                {
                    return services.Remove(contentLocation);
                }
            }

            return false;
        }
    }

    internal sealed class FileSystemBinaryDataService : IBinaryDataService
    {
	    /// <summary>
	    /// 
	    /// </summary>
	    /// <param name="contentLocation">The location of the content on disk.</param>
	    /// <param name="binaryRootUrl">The relative url to the binary content from the root of the site.</param>
	    /// <param name="loggingService">The logging service.</param>
	    /// <param name="cdnManager">Creates content URis for CDN locations.</param>
	    internal FileSystemBinaryDataService(string contentLocation, Uri binaryRootUrl, ILoggingDataService loggingService, ICdnManager cdnManager)
        {
            // parameter validation
            if (string.IsNullOrEmpty(contentLocation))
            {
                throw new ArgumentException("contentLocation must be specified.", "contentLocation");
            }

            if (!Path.IsPathRooted(contentLocation))
            {
                throw new ArgumentException("content location must be an absolute path.");
            }

            if (loggingService == null)
            {
                throw new ArgumentNullException("loggingService");
            }

            if (binaryRootUrl == null )
            {
                throw new ArgumentNullException("binaryRootUrl");
            }

            this.contentLocation = contentLocation;
            this.loggingService = loggingService;
            this.cdnManager = cdnManager;
            this.binaryRoot = binaryRootUrl;
        }

        public string SaveFile(System.IO.Stream inputFile, ref string fileName)
        {
            // parameter validation 
            if (inputFile == null) { throw new ArgumentNullException("inputFile"); }
            if (!inputFile.CanRead) { throw new ArgumentException("input file doesn't support reading."); }
            if (string.IsNullOrEmpty(fileName)) { throw new ArgumentException("filename must be specified."); }
            if (Path.IsPathRooted(fileName)) { throw new ArgumentException("the fileName must be a relative filename."); }

            string targetPath = Path.Combine(contentLocation, fileName);
            FileInfo file = new FileInfo(targetPath);

            // check if we need to create the directory
            try
            {
                if (file.Directory.Exists == false && String.CompareOrdinal(file.DirectoryName, contentLocation) != 0)
                {
                    file.Directory.Create();
                }
            }
            catch (Exception exc)
            {
                ErrorTrace.Trace(TraceLevel.Error, exc);
                throw;
            }


            int cnt = 1; // counter to make a unique filename
            bool saveFile = true;

            while (file.Exists)
            {
                if (FilesAreIdentical(file, inputFile))
                {
                    saveFile = false;
                    break;
                }

                string newFileName = String.Format("{0}[{1}]{2}",
                    Path.GetFileNameWithoutExtension(file.Name),
                    cnt++,
                    file.Extension
                    );


                file = new FileInfo(Path.Combine(file.DirectoryName, newFileName));
            }

            if (saveFile)
            {
                using (FileStream fs = file.OpenWrite())
                {
                    CopyStream(inputFile, fs);
                }
            }

            string relUri;

            string absUri = GetAbsoluteFileUri(file.FullName, out relUri);

			fileName = relUri; 
			
			absUri = cdnManager.ApplyCdnUri(absUri);

			return absUri;
        }

        private string GetAbsoluteFileUri(string fullPath, out string relFileUri)
        {
			//var relPath = fullPath.Replace(contentLocation, "").TrimStart('\\') ;
			var relPath = fullPath.Replace(contentLocation, "").TrimStart(Path.DirectorySeparatorChar);

			var relUri = new Uri( relPath, UriKind.Relative);

            relFileUri = relUri.ToString();

            return new Uri(binaryRoot, relPath).ToString();
        }

        public bool DeleteFile(string path)
        {
            return false;
        }

        private bool FilesAreIdentical(FileInfo existing, Stream newFile)
        {
            byte[] targetBuffer = new byte[0x200];
            byte[] sourceBuffer = new byte[0x200];
            int targetBytesRead, sourceBytesRead;

            using (FileStream targetFile = existing.OpenRead())
            {
                targetBytesRead = targetFile.Read(targetBuffer, 0, targetBuffer.Length);
                sourceBytesRead = newFile.Read(sourceBuffer, 0, targetBytesRead);
                // return to start position
                newFile.Position = 0;
                return (targetBytesRead == sourceBytesRead && EqualBuffers(targetBuffer, sourceBuffer));
            }
        }

        private long CopyStream(Stream input, Stream output)
        {
            byte[] buffer = new byte[0x10000];

            int bytesRead = 0;
            long totalBytes = 0;

            while ((bytesRead = input.Read(buffer, 0, buffer.Length)) > 0)
            {
                output.Write(buffer, 0, bytesRead);
                totalBytes += bytesRead;
            }
            output.Flush();

            return totalBytes;
        }

        private bool EqualBuffers(byte[] buf1, byte[] buf2)
        {
            if (object.Equals(buf1, buf2)) { return true; }

            if (buf1.Length == buf2.Length)
            {
                for (int l = 0; l < buf1.Length; l++)
                {
                    if (buf1[l] != buf2[l])
                        return false;
                }
                return true;
            }
            return false;
        }

        private string contentLocation;
        private Uri binaryRoot;
        private ILoggingDataService loggingService;
        private readonly ICdnManager cdnManager;
    }
}
