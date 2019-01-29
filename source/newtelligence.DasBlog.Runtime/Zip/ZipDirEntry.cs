// ZipDirEntry.cs
//
// Copyright (c) 2006, 2007 Microsoft Corporation.  All rights reserved.
//
// Part of an implementation of a zipfile class library. 
// See the file ZipFile.cs for further information.
//
// Tue, 27 Mar 2007  15:30


using System;

namespace newtelligence.DasBlog.Runtime.Zip
{


    public class ZipDirEntry
    {

        internal const int ZipDirEntrySignature = 0x02014b50;

        private bool _Debug = false;

        private ZipDirEntry() { }

        private DateTime _LastModified;
        public DateTime LastModified
        {
            get { return _LastModified; }
        }

        private string _FileName;
        public string FileName
        {
            get { return _FileName; }
        }

        private string _Comment;
        public string Comment
        {
            get { return _Comment; }
        }

        private Int16 _VersionMadeBy;
        public Int16 VersionMadeBy
        {
            get { return _VersionMadeBy; }
        }

        private Int16 _VersionNeeded;
        public Int16 VersionNeeded
        {
            get { return _VersionNeeded; }
        }

        private Int16 _CompressionMethod;
        public Int16 CompressionMethod
        {
            get { return _CompressionMethod; }
        }

        private Int32 _CompressedSize;
        public Int32 CompressedSize
        {
            get { return _CompressedSize; }
        }

        private Int32 _UncompressedSize;
        public Int32 UncompressedSize
        {
            get { return _UncompressedSize; }
        }

        public Double CompressionRatio
        {
            get
            {
                return 100 * (1.0 - (1.0 * CompressedSize) / (1.0 * UncompressedSize));
            }
        }

        private Int16 _BitField;
        private Int32 _LastModDateTime;

        private Int32 _Crc32;
        private byte[] _Extra;

        internal ZipDirEntry(ZipEntry ze) { }


        public static ZipDirEntry Read(System.IO.Stream s)
        {
            return Read(s, false);
        }


        public static ZipDirEntry Read(System.IO.Stream s, bool TurnOnDebug)
        {

            int signature = Shared.ReadSignature(s);
            // return null if this is not a local file header signature
            if (SignatureIsNotValid(signature))
            {
                s.Seek(-4, System.IO.SeekOrigin.Current);
                if (TurnOnDebug) System.Console.WriteLine("  ZipDirEntry::Read(): Bad signature ({0:X8}) at position {1}", signature, s.Position);
                return null;
            }

            byte[] block = new byte[42];
            int n = s.Read(block, 0, block.Length);
            if (n != block.Length) return null;

            int i = 0;
            ZipDirEntry zde = new ZipDirEntry();

            zde._Debug = TurnOnDebug;
            zde._VersionMadeBy = (short)(block[i++] + block[i++] * 256);
            zde._VersionNeeded = (short)(block[i++] + block[i++] * 256);
            zde._BitField = (short)(block[i++] + block[i++] * 256);
            zde._CompressionMethod = (short)(block[i++] + block[i++] * 256);
            zde._LastModDateTime = block[i++] + block[i++] * 256 + block[i++] * 256 * 256 + block[i++] * 256 * 256 * 256;
            zde._Crc32 = block[i++] + block[i++] * 256 + block[i++] * 256 * 256 + block[i++] * 256 * 256 * 256;
            zde._CompressedSize = block[i++] + block[i++] * 256 + block[i++] * 256 * 256 + block[i++] * 256 * 256 * 256;
            zde._UncompressedSize = block[i++] + block[i++] * 256 + block[i++] * 256 * 256 + block[i++] * 256 * 256 * 256;

            zde._LastModified = Shared.PackedToDateTime(zde._LastModDateTime);

            Int16 filenameLength = (short)(block[i++] + block[i++] * 256);
            Int16 extraFieldLength = (short)(block[i++] + block[i++] * 256);
            Int16 commentLength = (short)(block[i++] + block[i++] * 256);
            Int16 diskNumber = (short)(block[i++] + block[i++] * 256);
            Int16 internalFileAttrs = (short)(block[i++] + block[i++] * 256);
            Int32 externalFileAttrs = block[i++] + block[i++] * 256 + block[i++] * 256 * 256 + block[i++] * 256 * 256 * 256;
            Int32 Offset = block[i++] + block[i++] * 256 + block[i++] * 256 * 256 + block[i++] * 256 * 256 * 256;

            block = new byte[filenameLength];
            n = s.Read(block, 0, block.Length);
            zde._FileName = Shared.StringFromBuffer(block, 0, block.Length);

            zde._Extra = new byte[extraFieldLength];
            n = s.Read(zde._Extra, 0, zde._Extra.Length);

            block = new byte[commentLength];
            n = s.Read(block, 0, block.Length);
            zde._Comment = Shared.StringFromBuffer(block, 0, block.Length);

            return zde;
        }

        private static bool SignatureIsNotValid(int signature)
        {
            return (signature != ZipDirEntrySignature);
        }

    }



}
