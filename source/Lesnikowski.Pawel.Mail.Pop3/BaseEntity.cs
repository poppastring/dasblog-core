using System;
using System.Collections;

namespace Lesnikowski.Pawel.Mail.Pop3
{
	public class BaseEntity
	{
		protected string header; //all header 
		public string contentType;
		public string contentTransferEncoding;
		public string charset;
        public string boundary;
        public bool hasAttachments=false;
        public ArrayList attachments=new ArrayList();

		public BaseEntity(string buffer)
		{
			GetHeader(buffer);
            this.contentType			=this.GetValueFromHeader("Content-Type: ");
            //Try again? Not per spec...
            if (this.contentType == null || this.contentType.Length == 0)
            {
                this.contentType = this.GetValueFromHeader("Content-type: ");
            }
            this.contentTransferEncoding=this.GetValueFromHeader("Content-Transfer-Encoding: ");

			if (contentType.IndexOf("text/plain")!=-1 || contentType.IndexOf("text/html")!=-1)
				this.charset=GetValueEqualFromHeader2("charset=");
		}

        internal bool ParseMixedMessage(string buffer, string boundary)
        {
            int start=0,end;
				
            for(;;)
            {
                start=buffer.IndexOf(boundary,start);
				
                if (start==-1)
                    break;
				
                end=buffer.IndexOf(boundary,start+1);

                if (end==-1)
                    break;

                // remove boundary from buffer
                start += boundary.Length;

                Attachment a=new Attachment( buffer.Substring(start,end-start-2) );

                this.attachments.Add(a);
            }
            return true;
        }

		protected void GetHeader(string buffer) 
		{
			int end=buffer.IndexOf("\r\n\r\n");

			if (end==-1)
				throw new Exception("can't get header");
			
			header=buffer.Substring(0, end);
			header+="\r\n\r\n";
		}

		protected string GetValueFromHeader(string key)
		{
			int start=header.ToLower().IndexOf(key.ToLower());
			if(start<0)
				return "";
			start=header.IndexOf(' ',start);
			if (start<0)
				return "";
			int end=header.IndexOf("\r\n",start);
			if (end<=0)
				return "";
			return QuotedCoding.Decode(header.Substring(start+1, end-start-1).Trim());
		}

		/// <summary>
		/// z 'ala="123"'
		/// </summary>
		/// <param name="key">ala=</param>
		/// <returns>123</returns>
		protected string GetValueEqualFromHeader(string key)
		{
			int start=header.IndexOf(key);
			if (start==-1)
				return "";
			start=header.IndexOf('"',start);
			int end=header.IndexOf('"',start+1);
			if (start==-1 || end==-1)
				return "";
			return QuotedCoding.Decode(header.Substring(start+1,end-start-1));
		}

		/// <summary>
		/// z 'ala="123"'
		/// </summary>
		/// <param name="key">ala=</param>
		/// <returns>123</returns>
		protected string GetValueEqualFromHeader2(string key)
		{
			bool noQuotes = false;
			int end = -1;
			int start = header.IndexOf(key);
			if (start == -1)
				return "";
			start = header.IndexOf('"', start);
			if (start == -1)
			{
				noQuotes = true;
				start = header.IndexOf('=', header.IndexOf(key));
			}
			if (noQuotes)
			{
				end = header.IndexOf("\r\n", start + 1);
			}
			else
			{
				end = header.IndexOf('"', start + 1);
			}
			if (start == -1 || end == -1)
				return "";
			return QuotedCoding.Decode(header.Substring(start + 1, end - start - 1));
		}

	}
}





