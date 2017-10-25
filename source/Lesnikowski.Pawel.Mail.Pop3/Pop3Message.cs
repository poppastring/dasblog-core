using System;
using System.Collections;

namespace Lesnikowski.Pawel.Mail.Pop3
{
	public class Pop3Message : BaseEntity
	{
		public string from;
		public string replyTo;
		public string subject;
		public string date;
		public string body;
				
		//buffer ma na poczatku "+OK"
		//onlyHeader mowi czy tylko header czy cala wiadomosc
		public Pop3Message(string buffer,bool onlyHeader) : base(buffer)
		{
			this.from			=this.GetValueFromHeader("From: ");
			this.replyTo		=this.GetValueFromHeader("Reply-To: ");
			this.subject		=this.GetValueFromHeader("Subject: ");
			this.date			=this.GetValueFromHeader("Date: ");
			if(contentType.IndexOf("multipart")!=-1)
			{
				string boundary="--"+GetValueEqualFromHeader("boundary=");				
				hasAttachments=true;
				if (onlyHeader==false)
					ParseMixedMessage(buffer, boundary);
			}
			else
			{
				hasAttachments=false;
				if (onlyHeader==false)
					this.body=GetTextMessage(buffer);
			}
		}

		internal string GetTextMessage(string buffer) 
		{
			// find "\r\n\r\n" denoting end of header
			int start=buffer.IndexOf("\r\n\r\n")+4;
			int end=buffer.LastIndexOf("\r\n.\r\n");
			//change charset if contentTransferEncoding is 8bit
			if (this.contentTransferEncoding.IndexOf("8bit")!=-1)
				return StringOperations.Change(buffer.Substring(start,end-start),charset);
			else if (this.contentTransferEncoding.IndexOf("base64")!=-1)
				return StringOperations.Decode(buffer.Substring(start,end-start),charset);
			else
				return buffer.Substring(start,end-start);
		}

		

		/// <summary>
		/// zwraca mail pomiedzy '<' i '>'
		/// </summary>
		static public string GetMail(string s)
		{
			int start=s.IndexOf('<')+1;
			int end=s.LastIndexOf('>')-1;
			if (start==-1 || end==-1)
				return "";
			return s.Substring(start,end-start+1);
		}


	}
}
