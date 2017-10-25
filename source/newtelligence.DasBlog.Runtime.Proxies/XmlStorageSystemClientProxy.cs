#region Copyright (c) 2003, newtelligence AG. All rights reserved.
/*
// Copyright (c) 2003, newtelligence AG. (http://www.newtelligence.com)
// All rights reserved.
//  
// Redistribution and use in source and binary forms, with or without modification, are permitted 
// provided that the following conditions are met: 
//  
// (1) Redistributions of source code must retain the above copyright notice, this list of 
// conditions and the following disclaimer. 
// (2) Redistributions in binary form must reproduce the above copyright notice, this list of 
// conditions and the following disclaimer in the documentation and/or other materials 
// provided with the distribution. 
// (3) Neither the name of the newtelligence AG nor the names of its contributors may be used 
// to endorse or promote products derived from this software without specific prior 
// written permission.
//      
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS 
// OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY 
// AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL 
// DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, 
// DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER 
// IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT 
// OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
// -------------------------------------------------------------------------
//
// newtelligence is a registered trademark of newtelligence Aktiengesellschaft.
// 
// For portions of this software, the some additional copyright notices may apply 
// which can either be found in the license.txt file included in the source distribution
// or following this notice. 
//
*/
#endregion

using System;
using System.Security.Cryptography;
using System.Text;
using System.Xml;
using CookComputing.XmlRpc;


namespace newtelligence.DasBlog.Runtime.Proxies
{
    [XmlRpcMissingMapping(MappingAction.Ignore)]
    public struct XSSReply
    {
        public bool flError;
        public string message;
    }

    [XmlRpcMissingMapping(MappingAction.Ignore)]
    public struct XSSSaveMultipleFilesReply 
    {
        public bool flError;
        public string message;
        public string[] urlList;
    }

    [XmlRpcMissingMapping(MappingAction.Ignore)]
    public struct XSSDeleteMultipleFilesReply
    {
        public bool flError;
        public string message;
        public string[] errorList;
    }

    [XmlRpcMissingMapping(MappingAction.Ignore)]
    public struct XSSServerCapabilities
    {
        public string[] legalFileExtensions;
        public int maxFileSize;
        public int maxBytesPerUser;
        public int ctBytesInUse;
        public string yourUpstreamFolderUrl; 
    }

    public struct XSSUserInfo
    {
    }

	/// <summary>
	/// Summary description for XmlStorageSystem.
	/// </summary>
	public class XmlStorageSystemClientProxy : XmlRpcClientProtocol
	{
        public XmlStorageSystemClientProxy()
        {
            this.Url = "http://radio.xmlstoragesystem.com/RPC2";
        }

        public static string EncodePassword( string password )
        {
            StringBuilder sb = new StringBuilder();
            byte[] asciiString = Encoding.ASCII.GetBytes(password);
            MD5 md5 = MD5.Create();
            byte[] hash = md5.ComputeHash(asciiString);
            for( int i=0;i<hash.Length;i++)
            {
                sb.AppendFormat("{0:x}{1:x}",hash[i]/16,hash[i]%16);
            }
            return sb.ToString();
        }

        [XmlRpcMethod("xmlStorageSystem.registerUser")]
        public XSSReply RegisterUser(string email, string username, string password, int clientPort, string userAgent, string serialNumber)
        {
            return (XSSReply)this.Invoke("RegisterUser",new object[]{email, username, password, clientPort, userAgent, serialNumber });
        }


        [XmlRpcMethod("xmlStorageSystem.saveMultipleFiles")]
        public XSSSaveMultipleFilesReply SaveMultipleFiles(string username, string password, string[] relativepathList, byte[][] fileTextList)
        {
            return (XSSSaveMultipleFilesReply)this.Invoke("SaveMultipleFiles", new object[]{username, password, relativepathList, fileTextList });
        }


        [XmlRpcMethod("xmlStorageSystem.deleteMultipleFiles")]
        public XSSDeleteMultipleFilesReply DeleteMultipleFiles (string username, string password, string[] relativepathList)
        {
            return (XSSDeleteMultipleFilesReply)this.Invoke("DeleteMultipleFiles",new object[] { username, password, relativepathList});
        }

        [XmlRpcMethod("xmlStorageSystem.getServerCapabilities")]
        public XSSServerCapabilities GetServerCapabilities( string username, string password)
        {
            return (XSSServerCapabilities)this.Invoke("GetServerCapabilities", new object[]{username,password});
        }
		
        [XmlRpcMethod("xmlStorageSystem.mailPasswordToUser")]
        public XSSReply MailPasswordToUser (string username)
        {
            return (XSSReply)this.Invoke("MailPasswordToUser",new object[]{username});
        }

        [XmlRpcMethod("xmlStorageSystem.requestNotification")]
        public XSSReply RequestNotification(string notifyProcedure, int port, string path, string protocol, string[] urlList, XSSUserInfo userinfo)
        {
            return (XSSReply)this.Invoke("RequestNotification", new object[]{notifyProcedure, port, path, protocol, urlList, userinfo });
        }

        [XmlRpcMethod("xmlStorageSystem.ping")]
        public XSSReply Ping( string username, string password, int status, int clientPort, XSSUserInfo userinfo)
        {
            return (XSSReply)this.Invoke("Ping", new object[]{username, password, status, clientPort, userinfo });
        }

	}
}
