//Pawel Lesnikowski lesnikowski@o2.pl
using System;
using Lesnikowski.Pawel.Mail.Pop3;
using System.IO;
using System.Text;
using System.Collections;


namespace Example
{
	
	class Example
	{

        static void DumpAttachments( ArrayList attachments )
        {
            for(int i=0;i<attachments.Count;i++)
            {
                Attachment a=(Attachment)attachments[i];

                Console.WriteLine("Attachment ------------------------------------");
                Console.WriteLine("content="+a.contentType+", id="+a.contentID);					
                if ( a.hasAttachments )
                {
                    DumpAttachments( a.attachments );
                }

                //if there are any attachments the 1 attachment 
                //with contentType=text/plain and flag isRealAttachment==false
                //is the message body
                if (a.isRealAttachment==false && a.contentType.IndexOf("text/plain")!=-1 )
                {
                    Console.WriteLine("body:"+StringOperations.GetString(a.data));
                    continue;
                }
                if (a.isRealAttachment==false && a.contentType.IndexOf("text/html")!=-1 )
                {
                    Console.WriteLine("html-body:"+StringOperations.GetString(a.data));
                    continue;
                }
              

                // save attachment
                Console.WriteLine("filename="+a.fileName);						
                //a.SafeToFile("c:\\"+a.fileName);
            }
        }

		[STAThread]
		static void Main(string[] args)
		{
			Pop3 pop3=new Pop3();
			pop3.host="10.1.1.123";
			pop3.userName="BlogCFV";
			pop3.password="george";

			pop3.Connect();
			pop3.Login();
			pop3.GetAccountStat();

			for(int i=1;i<=pop3.messageCount;i++)
			{
				Pop3Message message=pop3.GetMessageHeader(i);

				Console.WriteLine("--mail header #"+i.ToString());
				Console.WriteLine(message.from);
				Console.WriteLine(message.subject);
				Console.WriteLine(message.replyTo);
				Console.WriteLine(message.date);
				Console.WriteLine(message.contentType);
				Console.WriteLine(message.charset);
				
			}
			
			for (int j=1;j<=pop3.messageCount;j++)
			{
				Console.WriteLine("-----first mail all:------");
				Pop3Message message=pop3.GetMessage(j);

				Console.WriteLine(message.from);
				Console.WriteLine(message.subject);

				if (message.hasAttachments==true)
				{
                    DumpAttachments( message.attachments );
					
				}
				else
					Console.WriteLine("body:"+message.body);
			}
			pop3.Close();
			Console.WriteLine("END.");
			Console.ReadLine();
		}
	}
}
