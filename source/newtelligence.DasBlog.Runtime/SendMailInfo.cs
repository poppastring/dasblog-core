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


using System;
using System.Net.Mail;

namespace newtelligence.DasBlog.Runtime
{
    
    [Serializable]
    public class SendMailInfo
    {
        MailMessage message;
        string smtpServer;
		bool enableSmtpAuthentication;
		string smtpUserName;
		string smtpPassword;
		int smtpPort;
		public bool useSSL;

    	/// <summary>
		/// Initializes a new instance of the <see cref="SendMailInfo"/> class.
		/// </summary>
        public SendMailInfo() : this(null, null, false, false, null, null, 25)
        {
        }

		/// <summary>
		/// Initializes a new instance of the <see cref="SendMailInfo"/> class.
		/// </summary>
		/// <param name="message">The message.</param>
		/// <param name="smtpServer">The SMTP server.</param>
        public SendMailInfo( MailMessage message, string smtpServer ) : this(message, smtpServer, false, false, null, null, 25)
        {
        }

		/// <summary>
		/// Initializes a new instance of the <see cref="SendMailInfo"/> class.
		/// </summary>
		/// <param name="message">The message.</param>
		/// <param name="smtpServer">The SMTP server.</param>
		/// <param name="smtpAuthEnabled">if set to <c>true</c> [SMTP auth enabled].</param>
		/// <param name="smtpUserName">Name of the SMTP user.</param>
		/// <param name="smtpPassword">The SMTP password.</param>
		/// <param name="smtpPort">The SMTP port.</param>
        /// <param name="useSSL">Indicates whether to send over SSL or not.</param>
		public SendMailInfo (MailMessage message, string smtpServer, bool smtpAuthEnabled, bool useSSL ,string smtpUserName, string smtpPassword, int smtpPort) 
		{
			this.message = message;
			this.smtpServer = smtpServer;
			this.enableSmtpAuthentication = smtpAuthEnabled;
			this.smtpUserName = smtpUserName;
			this.smtpPassword = smtpPassword;
			this.useSSL = useSSL;
			this.smtpPort = smtpPort;
		}

		/// <summary>
		/// Sends my message.
		/// </summary>
		public void SendMyMessage()
		{
			//remove any existing headers.  enables send twice, but could also lose any headers
			//added directly to the message by a developer.  however, they should add the info
			//that drives the need for the header to SendMailInfo and add them to AddAppropriateCDOHeaders
			//TODO a debug trace so devs don't get caught out.
			this.ClearCDOMessageHeaders();

			//configure and send the message
            SmtpClient mailClient = new SmtpClient(this.smtpServer, this.SmtpPort);
            if (this.enableSmtpAuthentication)
            {
                mailClient.UseDefaultCredentials = false;
                mailClient.Credentials = new System.Net.NetworkCredential(this.smtpUserName, this.smtpPassword);
            }
            mailClient.EnableSsl = this.useSSL;
            mailClient.Send(this.message);
		}

		/// <summary>
		/// Clears the CDO message headers.
		/// </summary>
		protected virtual void ClearCDOMessageHeaders()
		{
			//this.message.Fields.Clear();
            this.message.Headers.Clear(); // KH add
		}


        public MailMessage Message { get { return message; } set { message = value; }}
        public string SmtpServer { get { return smtpServer; } set { smtpServer = value; }}
		public bool EnableSmtpAuthentication { get { return enableSmtpAuthentication; } set { enableSmtpAuthentication = value; }}
		public bool UseSSL { get { return useSSL; } set { useSSL = value; }}
		public string SmtpUserName { get { return smtpUserName; } set { smtpUserName = value; }}
		public string SmtpPassword { get { return smtpPassword; } set { smtpPassword = value; }}
		public int SmtpPort { get { return smtpPort; } set { smtpPort = value; } }
    }
}
