// ZipFile.cs
//
// Copyright (c) 2006, 2007 Microsoft Corporation.  All rights reserved.
//
// 
// This class library reads and writes zip files, according to the format
// described by pkware, at:
// http://www.pkware.com/business_and_developers/developer/popups/appnote.txt
//
// This implementation is based on the
// System.IO.Compression.DeflateStream base class in the .NET Framework
// v2.0 base class library.
//
// There are other Zip class libraries available.  For example, it is
// possible to read and write zip files within .NET via the J# runtime.
// But some people don't like to install the extra DLL.  Also, there is
// a 3rd party LGPL-based (or is it GPL?) library called SharpZipLib,
// which works, in both .NET 1.1 and .NET 2.0.  But some people don't
// like the GPL. Finally, there are commercial tools (From ComponentOne,
// XCeed, etc).  But some people don't want to incur the cost.
//
// This alternative implementation is not GPL licensed, is free of cost,
// and does not require J#. It does require .NET 2.0 (for the DeflateStream 
// class).  
// 
// This code is released under the Microsoft Permissive License of OCtober 2006. 
// See the License.txt for details.  
//
// Notes:
// This is at best a cripppled and naive implementation. 
//
// Bugs:
// 1. does not do 0..9 compression levels (not supported by DeflateStream)
// 2. does not do encryption
// 3. no support for reading or writing multi-disk zip archives
// 4. no support for file comments or archive comments
// 5. does not stream as it compresses; all compressed data is kept in memory.
// 6. no support for double-byte chars in filenames
// 7. no support for asynchronous operation
// 
// But it does read and write basic zip files, and it gets reasonable compression. 
//
// NB: PKWare's zip specification states: 
//
// ----------------------
//   PKWARE is committed to the interoperability and advancement of the
//   .ZIP format.  PKWARE offers a free license for certain technological
//   aspects described above under certain restrictions and conditions.
//   However, the use or implementation in a product of certain technological
//   aspects set forth in the current APPNOTE, including those with regard to
//   strong encryption or patching, requires a license from PKWARE.  Please 
//   contact PKWARE with regard to acquiring a license.
// ----------------------
//    
// Fri, 31 Mar 2006  14:43
//
// update Thu, 22 Feb 2007  19:03
//  Fixed a problem with archives that had bit-3 (0x0008) set, 
//  where the CRC, Compressed Size, and Uncompressed size 
//  actually followed the compressed file data. 
//


using System;

namespace newtelligence.DasBlog.Runtime.Zip
{


    public class ZipFile : System.Collections.Generic.IEnumerable<ZipEntry>,
      IDisposable
    {
        private string _name;
        public string Name
        {
            get { return _name; }
        }



      // when this is set, we trim the volume (eg C:) off any fully-qualified pathname, 
      // before writing the ZipEntry into the ZipFile. 
      // We default this to true.  This allows Windows Explorer to read the zip archives properly. 
      private bool _TrimVolumeFromFullyQualifiedPaths= true;
      public bool TrimVolumeFromFullyQualifiedPaths
      {
            get { return _TrimVolumeFromFullyQualifiedPaths; }
            set { _TrimVolumeFromFullyQualifiedPaths= value; }
      }

        // when this is set the entire directory tree is included in the 
        // zipfile. When set to false the added files are added to the 
        // root of the zip file, instead of using the original directory tree.
       private bool _IncludeDirectoryTree = true;
       public bool IncludeDirectoryTree 
       {
           get { return _IncludeDirectoryTree; }
           set { _IncludeDirectoryTree = value; }
       }



        private System.IO.Stream ReadStream
        {
            get
            {
                if (_readstream == null)
                {
                    _readstream = System.IO.File.OpenRead(_name);
                }
                return _readstream;
            }
        }

        private System.IO.FileStream WriteStream
        {
            get
            {
                if (_writestream == null)
                {
                    _writestream = new System.IO.FileStream(_name, System.IO.FileMode.CreateNew);
                }
                return _writestream;
            }
        }

        private ZipFile() { }


        #region For Writing Zip Files

        public ZipFile(string NewZipFileName)
        {
            // create a new zipfile
            _name = NewZipFileName;
            if (System.IO.File.Exists(_name))
                throw new System.Exception(String.Format("That file ({0}) already exists.", NewZipFileName));
            _entries = new System.Collections.Generic.List<ZipEntry>();
        }


        public void AddItem(string FileOrDirectoryName)
        {
            AddItem(FileOrDirectoryName, false);
        }

        public void AddItem(string FileOrDirectoryName, bool WantVerbose)
        {
            if (System.IO.File.Exists(FileOrDirectoryName))
                AddFile(FileOrDirectoryName, WantVerbose);
            else if (System.IO.Directory.Exists(FileOrDirectoryName))
                AddDirectory(FileOrDirectoryName, WantVerbose);

            else
                throw new Exception(String.Format("That file or directory ({0}) does not exist!", FileOrDirectoryName));
        }

        public void AddFile(string FileName)
        {
            AddFile(FileName, false);
        }

        public void AddFile(string FileName, bool WantVerbose)
        {
            ZipEntry ze = ZipEntry.Create(FileName);
            ze.TrimVolumeFromFullyQualifiedPaths= TrimVolumeFromFullyQualifiedPaths;
            ze.IncludeDirectoryTree = IncludeDirectoryTree;
            if (WantVerbose) Console.WriteLine("adding {0}...", FileName);
            _entries.Add(ze);
        }

        public void AddDirectory(string DirectoryName)
        {
            AddDirectory(DirectoryName, false);
        }

        public void AddDirectory(string DirectoryName, bool WantVerbose)
        {
            String[] filenames = System.IO.Directory.GetFiles(DirectoryName);
            foreach (String filename in filenames)
            {
                AddFile(filename, WantVerbose);
            }

            String[] dirnames = System.IO.Directory.GetDirectories(DirectoryName);
            foreach (String dir in dirnames)
            {
                AddDirectory(dir, WantVerbose);
            }
        }


        public void Save()
        {
            // an entry for each file
            foreach (ZipEntry e in _entries)
            {
                e.Write(WriteStream);
            }

            WriteCentralDirectoryStructure();
            WriteStream.Close();
            _writestream = null;
        }


        private void WriteCentralDirectoryStructure()
        {
            // the central directory structure
            long Start = WriteStream.Length;
            foreach (ZipEntry e in _entries)
            {
                e.WriteCentralDirectoryEntry(WriteStream);
            }
            long Finish = WriteStream.Length;

            // now, the footer
            WriteCentralDirectoryFooter(Start, Finish);
        }


        private void WriteCentralDirectoryFooter(long StartOfCentralDirectory, long EndOfCentralDirectory)
        {
            byte[] bytes = new byte[1024];
            int i = 0;
            // signature
            UInt32 EndOfCentralDirectorySignature = 0x06054b50;
            bytes[i++] = (byte)(EndOfCentralDirectorySignature & 0x000000FF);
            bytes[i++] = (byte)((EndOfCentralDirectorySignature & 0x0000FF00) >> 8);
            bytes[i++] = (byte)((EndOfCentralDirectorySignature & 0x00FF0000) >> 16);
            bytes[i++] = (byte)((EndOfCentralDirectorySignature & 0xFF000000) >> 24);

            // number of this disk
            bytes[i++] = 0;
            bytes[i++] = 0;

            // number of the disk with the start of the central directory
            bytes[i++] = 0;
            bytes[i++] = 0;

            // total number of entries in the central dir on this disk
            bytes[i++] = (byte)(_entries.Count & 0x00FF);
            bytes[i++] = (byte)((_entries.Count & 0xFF00) >> 8);

            // total number of entries in the central directory
            bytes[i++] = (byte)(_entries.Count & 0x00FF);
            bytes[i++] = (byte)((_entries.Count & 0xFF00) >> 8);

            // size of the central directory
            Int32 SizeOfCentralDirectory = (Int32)(EndOfCentralDirectory - StartOfCentralDirectory);
            bytes[i++] = (byte)(SizeOfCentralDirectory & 0x000000FF);
            bytes[i++] = (byte)((SizeOfCentralDirectory & 0x0000FF00) >> 8);
            bytes[i++] = (byte)((SizeOfCentralDirectory & 0x00FF0000) >> 16);
            bytes[i++] = (byte)((SizeOfCentralDirectory & 0xFF000000) >> 24);

            // offset of the start of the central directory 
            Int32 StartOffset = (Int32)StartOfCentralDirectory;  // cast down from Long
            bytes[i++] = (byte)(StartOffset & 0x000000FF);
            bytes[i++] = (byte)((StartOffset & 0x0000FF00) >> 8);
            bytes[i++] = (byte)((StartOffset & 0x00FF0000) >> 16);
            bytes[i++] = (byte)((StartOffset & 0xFF000000) >> 24);

            // zip comment length
            bytes[i++] = 0;
            bytes[i++] = 0;

            WriteStream.Write(bytes, 0, i);
        }

        #endregion

        #region For Reading Zip Files

        /// <summary>
        /// This will throw if the zipfile does not exist. 
        /// </summary>
        public static ZipFile Read(string zipfilename)
        {
            return Read(zipfilename, false);
        }

        /// <summary>
        /// This will throw if the zipfile does not exist. 
        /// </summary>
        public static ZipFile Read(string zipfilename, bool TurnOnDebug)
        {

            ZipFile zf = new ZipFile();
            zf._Debug = TurnOnDebug;
            zf._name = zipfilename;
            zf._entries = new System.Collections.Generic.List<ZipEntry>();
            ZipEntry e;
            while ((e = ZipEntry.Read(zf.ReadStream, zf._Debug)) != null)
            {
                if (zf._Debug) System.Console.WriteLine("  ZipFile::Read(): ZipEntry: {0}", e.FileName);
                zf._entries.Add(e);
            }

            // read the zipfile's central directory structure here.
            zf._direntries = new System.Collections.Generic.List<ZipDirEntry>();

            ZipDirEntry de;
            while ((de = ZipDirEntry.Read(zf.ReadStream, zf._Debug)) != null)
            {
                if (zf._Debug) System.Console.WriteLine("  ZipFile::Read(): ZipDirEntry: {0}", de.FileName);
                zf._direntries.Add(de);
            }

            return zf;
        }

        public System.Collections.Generic.IEnumerator<ZipEntry> GetEnumerator()
        {
            foreach (ZipEntry e in _entries)
                yield return e;
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }


        public void ExtractAll(string path)
        {
            ExtractAll(path, false);
        }


        public void ExtractAll(string path, bool WantVerbose)
        {
            bool header = WantVerbose;
            foreach (ZipEntry e in _entries)
            {
                if (header)
                {
                    System.Console.WriteLine("\n{1,-22} {2,-6} {3,4}   {4,-8}  {0}",
                                 "Name", "Modified", "Size", "Ratio", "Packed");
                    System.Console.WriteLine(new System.String('-', 72));
                    header = false;
                }
                if (WantVerbose)
                    System.Console.WriteLine("{1,-22} {2,-6} {3,4:F0}%   {4,-8} {0}",
                                 e.FileName,
                                 e.LastModified.ToString("yyyy-MM-dd HH:mm:ss"),
                                 e.UncompressedSize,
                                 e.CompressionRatio,
                                 e.CompressedSize);
                e.Extract(path);
            }
        }


        public void Extract(string filename)
        {
            this[filename].Extract();
        }


        public void Extract(string filename, System.IO.Stream s)
        {
            this[filename].Extract(s);
        }


        public ZipEntry this[String filename]
        {
            get
            {
                foreach (ZipEntry e in _entries)
                {
                    if (e.FileName == filename) return e;
                }
                return null;
            }
        }

        #endregion




        // the destructor
        ~ZipFile()
        {
            // call Dispose with false.  Since we're in the
            // destructor call, the managed resources will be
            // disposed of anyways.
            Dispose(false);
        }

        public void Dispose()
        {
            // dispose of the managed and unmanaged resources
            Dispose(true);

            // tell the GC that the Finalize process no longer needs
            // to be run for this object.
            GC.SuppressFinalize(this);
        }


        protected virtual void Dispose(bool disposeManagedResources)
        {
            if (!this._disposed)
            {
                if (disposeManagedResources)
                {
                    // dispose managed resources
                    if (_readstream != null)
                    {
                        _readstream.Dispose();
                        _readstream = null;
                    }
                    if (_writestream != null)
                    {
                        _writestream.Dispose();
                        _writestream = null;
                    }
                }
                this._disposed = true;
            }
        }


        private System.IO.Stream _readstream;
        private System.IO.FileStream _writestream;
        private bool _Debug = false;
        private bool _disposed = false;
        private System.Collections.Generic.List<ZipEntry> _entries = null;
        private System.Collections.Generic.List<ZipDirEntry> _direntries = null;
    }

}


// Example usage: 
// 1. Extracting all files from a Zip file: 
//
//     try 
//     {
//       using(ZipFile zip= ZipFile.Read(ZipFile))
//       {
//         zip.ExtractAll(TargetDirectory, true);
//       }
//     }
//     catch (System.Exception ex1)
//     {
//       System.Console.Error.WriteLine("exception: " + ex1);
//     }
//
// 2. Extracting files from a zip individually:
//
//     try 
//     {
//       using(ZipFile zip= ZipFile.Read(ZipFile)) 
//       {
//         foreach (ZipEntry e in zip) 
//         {
//           e.Extract(TargetDirectory);
//         }
//       }
//     }
//     catch (System.Exception ex1)
//     {
//       System.Console.Error.WriteLine("exception: " + ex1);
//     }
//
// 3. Creating a zip archive: 
//
//     try 
//     {
//       using(ZipFile zip= new ZipFile(NewZipFile)) 
//       {
//
//         String[] filenames= System.IO.Directory.GetFiles(Directory); 
//         foreach (String filename in filenames) 
//         {
//           zip.Add(filename);
//         }
//
//         zip.Save(); 
//       }
//
//     }
//     catch (System.Exception ex1)
//     {
//       System.Console.Error.WriteLine("exception: " + ex1);
//     }
//
//
// ==================================================================
//
//
//
// Information on the ZIP format:
//
// From
// http://www.pkware.com/business_and_developers/developer/popups/appnote.txt
//
//  Overall .ZIP file format:
//
//     [local file header 1]
//     [file data 1]
//     [data descriptor 1]  ** sometimes
//     . 
//     .
//     .
//     [local file header n]
//     [file data n]
//     [data descriptor n]   ** sometimes
//     [archive decryption header] 
//     [archive extra data record] 
//     [central directory]
//     [zip64 end of central directory record]
//     [zip64 end of central directory locator] 
//     [end of central directory record]
//
// Local File Header format:
//         local file header signature     4 bytes  (0x04034b50)
//         version needed to extract       2 bytes
//         general purpose bit flag        2 bytes
//         compression method              2 bytes
//         last mod file time              2 bytes
//         last mod file date              2 bytes
//         crc-32                          4 bytes
//         compressed size                 4 bytes
//         uncompressed size               4 bytes
//         file name length                2 bytes
//         extra field length              2 bytes
//         file name                       varies
//         extra field                     varies
//
//
// Data descriptor:  (used only when bit 3 of the general purpose bitfield is set)
//         local file header signature     4 bytes  (0x08074b50)
//         crc-32                          4 bytes
//         compressed size                 4 bytes
//         uncompressed size               4 bytes
//
//
//   Central directory structure:
//
//       [file header 1]
//       .
//       .
//       . 
//       [file header n]
//       [digital signature] 
//
//
//       File header:  (This is ZipDirEntry in the code above)
//         central file header signature   4 bytes  (0x02014b50)
//         version made by                 2 bytes
//         version needed to extract       2 bytes
//         general purpose bit flag        2 bytes
//         compression method              2 bytes
//         last mod file time              2 bytes
//         last mod file date              2 bytes
//         crc-32                          4 bytes
//         compressed size                 4 bytes
//         uncompressed size               4 bytes
//         file name length                2 bytes
//         extra field length              2 bytes
//         file comment length             2 bytes
//         disk number start               2 bytes
//         internal file attributes        2 bytes
//         external file attributes        4 bytes
//         relative offset of local header 4 bytes
//         file name (variable size)
//         extra field (variable size)
//         file comment (variable size)
//
// End of central directory record:
//
//         end of central dir signature    4 bytes  (0x06054b50)
//         number of this disk             2 bytes
//         number of the disk with the
//         start of the central directory  2 bytes
//         total number of entries in the
//         central directory on this disk  2 bytes
//         total number of entries in
//         the central directory           2 bytes
//         size of the central directory   4 bytes
//         offset of start of central
//         directory with respect to
//         the starting disk number        4 bytes
//         .ZIP file comment length        2 bytes
//         .ZIP file comment       (variable size)
//
// date and time are packed values, as MSDOS did them
// time: bits 0-4 : second
//            5-10: minute
//            11-15: hour
// date  bits 0-4 : day
//            5-8: month
//            9-15 year (since 1980)
//
// see http://www.vsft.com/hal/dostime.htm

