//Pawel Lesnikowski lesnikowski@o2.pl
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Lesnikowski.Pawel.Mail.Pop3
{
	public class Pop3
	{
		// Constants
		private const string POS_STAT_IND = "+OK";
		//private const string NEG_STAT_IND = "-ERR";
		
		public string host;
		public string userName;
		public string password;
		public int port=110;
		
		private Socket socket;
				
		private int _messageCount;	// number of messages
		public int messageCount
		{
			get
			{
				return _messageCount;
			}
		}

		// constructors
		public Pop3()
		{		
			socket = null;
		}

		public void Connect()
		{
			if (host==null)
				throw new Exception("no host");

			IPHostEntry hostEntry = Dns.GetHostEntry(host);

			if (hostEntry==null)
				throw new Exception("can't get ip");
				
			// construct the endpoint
			IPEndPoint endPoint=new IPEndPoint(hostEntry.AddressList[0],port);
			
			if (endPoint==null)
				throw new Exception("can't get endpoint");
				
			// initialize socket
			socket = new Socket(AddressFamily.InterNetwork,SocketType.Stream,ProtocolType.Tcp);
			
			// set the socket timeout
			//socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.SendTimeout, sendTimeout);
			//socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReceiveTimeout, receiveTimeout);
			
			socket.Connect(endPoint);
			ReceiveLine();
		}

		public void Login()
		{
			string Username = "USER " + this.userName;
			string Password = "PASS " + this.password;
			Send(Username);
			ReceiveLine();
			Send(Password);
			ReceiveLine();
		}

		public void GetAccountStat()
		{
			Send("STAT");
			string tmp=ReceiveLine();
			tmp=tmp.Substring(tmp.IndexOf(" ") + 1);
			// parse number of messages
			_messageCount = Int32.Parse(tmp.Substring(0,tmp.IndexOf(" ")).Trim());
		}

		/// <summary>
		/// Deletes message with given index when Close() is called
		/// </summary>
		public void DeleteMessage(int messageNumber) 
		{
			string command="DELE "+messageNumber.ToString();
			Send(command);
			ReceiveLine();
		}
		
		public Pop3Message GetMessageHeader(int messageNumber)
		{
			Send("TOP "+messageNumber+" 0");
			string tmp=Receive();
			//delete +OK (3 chars) and create message
			Pop3Message message=new Pop3Message(tmp.Substring(3),true);
			return message;
		}

		public Pop3Message GetMessage(int messageNumber)
		{
			Send("RETR "+messageNumber.ToString());
			string tmp=Receive();
			//delete +OK (3 chars) and create message
			Pop3Message message=new Pop3Message(tmp.Substring(3),false);
			return message;
		}


		public void Close() 
		{
			if (socket==null)
				return;
			try
			{
				Send("QUIT");
				ReceiveLine();
			}
			finally
			{
				socket.Close();
			}
		}
	
		private void Send(string command) 
		{
			// add CRLF to the message
			command+="\r\n";
	
			// convert message buffer to array of chars
			byte[] buffer=Encoding.ASCII.GetBytes(command.ToCharArray());

			// send buffer
			int bytesSent=socket.Send(buffer,buffer.Length, 0);

			if (bytesSent!=buffer.Length)
				throw new Exception("failed to send request to server");
		}

		private string ReceiveLine()
		{
			const int bufferLength=1024;
			byte[] buffer=new byte[bufferLength+1];	// extra char for null terminator
			StringBuilder message=new StringBuilder(bufferLength);
			int bytesRead;

			for(;;)
			{
				bytesRead=socket.Receive(buffer, bufferLength, 0);
				if (bytesRead==0)
					break;
				
				// add null terminator
				buffer[bytesRead]=0;
	
				// conver char array to string
				message.Append(Encoding.ASCII.GetChars(buffer, 0, bytesRead));
				if (buffer[bytesRead-1]==10)
					break;
			}
			string tmp=message.ToString();
			if(tmp.StartsWith(POS_STAT_IND)==false)
				throw new Exception("Received negative response from POP3 server");
			return tmp;
		}
		
		static bool EndsWith(StringBuilder b,string s)
		{
            int bLength=b.Length;
			int sLength=s.Length;
			if (bLength<sLength)
				return false;

			for(int i=0;i<sLength;i++)
			{
				if (s[sLength-i-1]!=b[bLength-i-1])
					return false;
			}
			return true;
		}

		private string Receive()
		{
			const int bufferLength=1024;
			byte[] buffer=new byte[bufferLength+1];	// extra char for null terminator
			StringBuilder message=new StringBuilder(bufferLength);
			int bytesRead;

			for(;;)
			{
				if (EndsWith(message,"\r\n.\r\n")==true)
				{
					//this is rather the end wait short
					if (socket.Poll(100, SelectMode.SelectRead)==false)
						break;//no data in socket
				}
				else
				{
					//it dosn't look like the end wait longer 5s
					if (socket.Poll(5000000, SelectMode.SelectRead)==false)
						break;//no data in socket
				}

				bytesRead=socket.Receive(buffer, bufferLength, 0);
				if (bytesRead==0)
					break;
				
				// add null terminator
				buffer[bytesRead]=0;

				//load with some 8bit coding to allow charset changing
				message.Append(Encoding.Default.GetChars(buffer, 0, bytesRead));
			}
			message.Replace("\n..","\n.");
			string tmp=message.ToString();

			if(tmp.StartsWith(POS_STAT_IND)==false)
				throw new Exception("Received negative response from POP3 server");

			return tmp;
	
		}

	}
}	
