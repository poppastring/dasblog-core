#region Copyright (c) 2003, newtelligence AG. All rights reserved.
/*
// Copyright (c) 2003, newtelligence AG. (http://www.newtelligence.com)
// Original BlogX Source Code: Copyright (c) 2003, Chris Anderson (http://simplegeek.com)
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
// Original BlogX source code (c) 2003 by Chris Anderson (http://simplegeek.com)
// 
// newtelligence is a registered trademark of newtelligence Aktiengesellschaft.
// 
// For portions of this software, the some additional copyright notices may apply 
// which can either be found in the license.txt file included in the source distribution
// or following this notice. 
//
*/
#endregion


namespace newtelligence.DasBlog.Web.Services
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.IO;
    using System.Web.Services;
    using System.Web.Services.Description;
    using System.Web.Services.Protocols;
    using System.Xml;
    using System.Xml.Serialization;
    using newtelligence.DasBlog.Web.Core;
    
    /// <summary>
    /// Summary description for ConfigEditing.
    /// </summary>
    [WebService(Namespace="urn:schemas-newtelligence-com:dasblog:config-services",
                Description="Note that this service will be switched to WSE 2.0 once "+
                            "that becomes available. Once that happens, we'll switch "+
                            "authentication to WS-Security with WS-Policy")]
    [SoapDocumentService(SoapBindingUse.Literal,SoapParameterStyle.Bare)]
    
    public class ConfigEditingServiceImplementation : System.Web.Services.WebService
    {
        
        public AuthenticationHeader authenticationHeader;
        public SoapUnknownHeader[] unknownHeaders;

        public ConfigEditingServiceImplementation()
        {
            InitializeComponent();
        }

        #region Component Designer generated code
		
        //Required by the Web Services Designer 
        private IContainer components = null;
				
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose( bool disposing )
        {
            if(disposing && components != null)
            {
                components.Dispose();
            }
            base.Dispose(disposing);		
        }
		
        #endregion

        
        [WebMethod]
        [SoapDocumentMethod(ParameterStyle=SoapParameterStyle.Bare)]
        [SoapHeader("authenticationHeader",Direction=SoapHeaderDirection.In)]
        [SoapHeader("unknownHeaders")]
        [return:XmlElement("siteConfig")]
        public SiteConfig GetSiteConfig()
        {
            SiteConfig siteConfig = SiteConfig.GetSiteConfig();
            if ( !siteConfig.EnableConfigEditService )
            {
                throw new ServiceDisabledException();
            }

            if (authenticationHeader == null || 
                 SiteSecurity.Login(authenticationHeader.userName, authenticationHeader.password).Role != "admin")
            {
                throw new Exception("Invalid Password");
            }
            return siteConfig;
        }

        [WebMethod]
        [SoapDocumentMethod(ParameterStyle=SoapParameterStyle.Bare)]
        [SoapHeader("authenticationHeader",Direction=SoapHeaderDirection.In)]
        [SoapHeader("unknownHeaders")]
        public void UpdateSiteConfig(SiteConfig siteConfig)
        {
            SiteConfig currentSiteConfig = SiteConfig.GetSiteConfig();
            if ( !currentSiteConfig.EnableConfigEditService )
            {
                throw new ServiceDisabledException();
            }

            if (authenticationHeader == null || 
                SiteSecurity.Login(authenticationHeader.userName, authenticationHeader.password).Role != "admin")
            {
                throw new Exception("Invalid Password");
            }

            XmlSerializer ser = new XmlSerializer(typeof(SiteConfig));

            using (StreamWriter writer = new StreamWriter(SiteConfig.GetConfigFilePathFromCurrentContext()))
            {
                ser.Serialize(writer, siteConfig);
            }
        }

        [WebMethod]
        [SoapDocumentMethod(ParameterStyle=SoapParameterStyle.Bare)]
        [SoapHeader("authenticationHeader",Direction=SoapHeaderDirection.In)]
        [SoapHeader("unknownHeaders")]
        public string[] EnumBlogrolls()
        {
            List<string> listFiles = new List<string>();
            SiteConfig siteConfig = SiteConfig.GetSiteConfig();
            if ( !siteConfig.EnableConfigEditService )
            {
                throw new ServiceDisabledException();
            }

            if (authenticationHeader == null || 
                SiteSecurity.Login(authenticationHeader.userName, authenticationHeader.password).Role != "admin")
            {
                throw new Exception("Invalid Password");
            }

            foreach( string file in Directory.GetFiles(SiteConfig.GetConfigPathFromCurrentContext(),"*.opml"))
            {
                listFiles.Add( Path.GetFileNameWithoutExtension(file) );
            }
            if ( listFiles.Count == 0 )
            {
                listFiles.Add("blogroll");
            }
            return listFiles.ToArray();
        }


        private XmlElement LoadBlogroll( string fileName )
        {
            // This may seem horribly inefficient, but we need to run this
            // through the object model once to make sure we're schema compliant
            // We can't emit OPML schema into WSDL, because the friggin' spec defines 
            // no namespace
            XmlSerializer ser = new XmlSerializer(typeof(Opml));
            Opml opmlTree=null;
            
            if ( File.Exists( fileName ) )
            {
                using (Stream s = File.OpenRead(fileName))
                {
                    opmlTree = ser.Deserialize(s) as Opml;
                }

            }
            if ( opmlTree == null )
            {
                opmlTree = new Opml("Generated by newtelligence dasBlog 1.4");
            }
            
            XmlDocument xmlDoc = new XmlDocument();
            MemoryStream memStream = new MemoryStream();
            XmlTextWriter xtw = new XmlTextWriter(memStream,System.Text.Encoding.UTF8);
            ser.Serialize( xtw, opmlTree );
            xtw.Flush();

            memStream.Position = 0;

            XmlTextReader xtr = new XmlTextReader(memStream);            
            xmlDoc.Load( xtr );
            return xmlDoc.DocumentElement;
        }

        private void SaveBlogroll( string fileName, XmlElement opmlXml )
        {
            // This may seem horribly inefficient, but we need to run this
            // through the object model once to make sure we're schema compliant
            // We can't emit OPML schema into WSDL, because the friggin' spec defines 
            // no namespace

            XmlSerializer ser = new XmlSerializer(typeof(Opml));

            Opml opmlTree;
            XmlNodeReader xnr = new XmlNodeReader(opmlXml);
            opmlTree = (Opml)ser.Deserialize( xnr );            
            using (StreamWriter sw = new StreamWriter( fileName, false,System.Text.Encoding.UTF8))
            {
                ser.Serialize(sw, opmlTree);
            }
        }

        [WebMethod]
        [SoapDocumentMethod(ParameterStyle=SoapParameterStyle.Bare)]
        [SoapHeader("authenticationHeader",Direction=SoapHeaderDirection.In)]
        [SoapHeader("unknownHeaders")]
        public XmlElement GetBlogroll( string blogRollName )
        {
            SiteConfig siteConfig = SiteConfig.GetSiteConfig();
            if ( !siteConfig.EnableConfigEditService )
            {
                throw new ServiceDisabledException();
            }

            if (authenticationHeader == null || 
                SiteSecurity.Login(authenticationHeader.userName, authenticationHeader.password).Role != "admin")
            {
                throw new Exception("Invalid Password");
            }

            string blogRollPath = Path.Combine(SiteConfig.GetConfigPathFromCurrentContext(),blogRollName+".opml");
            return LoadBlogroll(blogRollPath);
        }

        [WebMethod(false)]
        [SoapDocumentMethod(ParameterStyle=SoapParameterStyle.Bare)]
        [SoapHeader("authenticationHeader",Direction=SoapHeaderDirection.In)]
        [SoapHeader("unknownHeaders")]
        public void PostBlogroll( string blogRollName, XmlElement blogRoll )
        {
            SiteConfig siteConfig = SiteConfig.GetSiteConfig();
            if ( !siteConfig.EnableConfigEditService )
            {
                throw new ServiceDisabledException();
            }

            if (authenticationHeader == null || 
                SiteSecurity.Login(authenticationHeader.userName, authenticationHeader.password).Role != "admin")
            {
                throw new Exception("Invalid Password");
            }

            string blogRollPath = Path.Combine(SiteConfig.GetConfigPathFromCurrentContext(),blogRollName+".opml");
            SaveBlogroll( blogRollPath, blogRoll );
        }
        
    }


    /// <summary>
    /// This header is temporary
    /// </summary>
    [XmlType("authenticationHeader", Namespace="urn:schemas-newtelligence-com:dasblog:config-services:auth-temp")]
    [XmlRoot("authenticationHeader", Namespace="urn:schemas-newtelligence-com:dasblog:config-services:auth-temp")]
    public class AuthenticationHeader : SoapHeader 
    {
        public string userName;
        public string password;

    }
 
}
