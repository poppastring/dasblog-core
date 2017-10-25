using System;
using System.IO;

namespace Lesnikowski.Pawel.Mail.Pop3
{
	public class Attachment : BaseEntity
	{
       	public string contentDisposition;
		public bool isRealAttachment;
		public string fileName;
        public string contentID;
		public byte[] data;
        
		public Attachment(string buffer) : base(buffer)
		{
			this.contentDisposition=this.GetValueFromHeader("Content-Disposition: ");
            this.fileName=GetValueEqualFromHeader("name=");
            this.contentID=GetValueFromHeader("Content-ID: ");			

			if (contentDisposition.IndexOf("attachment")!=-1)
			{
				this.fileName=GetValueEqualFromHeader("filename=");
				this.isRealAttachment=true;
			}

            if(contentType.IndexOf("multipart")!=-1)
            {
                string boundary="--"+GetValueEqualFromHeader("boundary=");				
                hasAttachments=true;
                ParseMixedMessage(buffer, boundary);
            }
            else
            {
                hasAttachments=false;
            
                //data
                int start=buffer.IndexOf("\r\n\r\n");
                if (start==-1)
                    throw new Exception("could not find beginning of MIME body");

                start+=4;
                if (this.contentTransferEncoding.ToLower().IndexOf("base64")!=-1)
                {
                    this.data=Convert.FromBase64String(buffer.Substring(start));
                }
                else
                    if (this.contentTransferEncoding.IndexOf("quoted-printable")!=-1)
                {
                    this.data=QuotedCoding.GetByteArray(buffer.Substring(start));
                }
                else
                {
                    //change charset if contentTransferEncoding is 8bit
                    if (this.contentTransferEncoding.IndexOf("8bit")!=-1)
                        this.data=StringOperations.GetByteArray(StringOperations.Change(buffer.Substring(start),charset));
                    else
                        this.data=StringOperations.GetByteArray(buffer.Substring(start));
                }
            }
		}

		public void SaveToFile(string fileName)
		{
			if (this.isRealAttachment==false)
				return;

			FileStream fs=File.Create(fileName);
			fs.Write(data,0,data.Length);
			fs.Close();
		}

	}
}





