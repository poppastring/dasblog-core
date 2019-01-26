using System;
using System.IO;
using System.Threading;

namespace newtelligence.DasBlog.Runtime.Util
{
    public static class FileUtils
    {
        public static void Delete(string fileName)
        {
            // init defaults
            int retries = 10;
            int msecsBetweenRetries = 100;

            // attempts
            while (retries > 0)
            {
                try
                {
                    // the actual delete
                    File.Delete(fileName);
                }
                catch
                {
                    // one less try
                    retries--;
                    // yield control to other threads so that we get a little
                    // wait before we retry.
                    Thread.Sleep(msecsBetweenRetries);
                    continue;
                }
                break;
            }

            // failed to delete
            if (retries == 0)
            {
                throw new IOException(String.Format("DasBlog.Util.FileUtils: Error deleting {0}, perhaps it is in use by another process?", fileName));
            }
        }

        public static FileStream OpenForWrite(string fileName)
        {
            FileStream fileStream = null;
            int retries = 10;
            int msecsBetweenRetries = 100;

            while (retries > 0)
            {
                try
                {
                    fileStream = new FileStream(
                        fileName,
                        FileMode.Create,
                        FileAccess.ReadWrite,
                        FileShare.None);
                }
                catch
                {
                    retries--;
                    // yield control to other threads so that we get a little
                    // wait before we retry.
                    Thread.Sleep(msecsBetweenRetries);
                    continue;
                }
                break;
            }
            InternalAssertIfNull(fileStream, fileName);
            return fileStream;
        }

        public static FileStream OpenForReadWrite(string fileName)
        {
            FileStream fileStream = null;
            int retries = 10;
            int msecsBetweenRetries = 100;

            while (retries > 0)
            {
                try
                {
                    fileStream = new FileStream(
                        fileName,
                        FileMode.OpenOrCreate,
                        FileAccess.ReadWrite,
                        FileShare.None);
                }
                catch
                {
                    retries--;
                    // yield control to other threads so that we get a little
                    // wait before we retry.
                    Thread.Sleep(msecsBetweenRetries);
                    continue;
                }
                break;
            }
            InternalAssertIfNull(fileStream, fileName);
            return fileStream;
        }

        public static FileStream OpenForRead(string fileName)
        {
            FileStream fileStream = null;
            int retries = 10;
            int msecsBetweenRetries = 100;

            while (retries > 0)
            {
                try
                {
                    fileStream = new FileStream(
                        fileName,
                        FileMode.OpenOrCreate,
                        FileAccess.Read,
                        FileShare.Read);
                }
                catch
                {
                    retries--;
                    // yield control to other threads so that we get a little
                    // wait before we retry.
                    Thread.Sleep(msecsBetweenRetries);
                    continue;
                }
                break;
            }
            InternalAssertIfNull(fileStream, fileName);
            return fileStream;
        }

        private static void InternalAssertIfNull(FileStream file, string name)
        {
            if (file == null)
            {
                throw new IOException(String.Format("DasBlog.Util.FileUtils: Error accessing {0}, perhaps it is in use by another process?", name));
            }
        }
    }
}
