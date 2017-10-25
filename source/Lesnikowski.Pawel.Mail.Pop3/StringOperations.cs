using System;
using System.Text;

namespace Lesnikowski.Pawel.Mail.Pop3
{
	/// <summary>
	/// Summary description for StringOperations.
	/// </summary>
	public abstract class StringOperations
	{
		/// <summary>
		/// zwraca tablice bajtow dla stringa
		/// </summary>
		public static byte[] GetByteArray(string s)
		{
			return Encoding.Default.GetBytes(s);
			/*
			byte[] bArray=new byte[s.Length];
			for(int i=0;i<s.Length;i++)
				bArray[i]=(byte)s[i];
			return bArray;
			*/
		}
		/// <summary>
		/// zwraca stringa dla tabicy bajtow
		/// </summary>
		public static string GetString(byte[] data)
		{
			return Encoding.Default.GetString(data);
			/*
			char[] c=new char[data.Length];
			for(int i=0;i<data.Length;i++)
				c[i]=(char)data[i];
			return new string(c);
			*/
		}

		public static string Change(string text,string charset)
		{
			if (charset==null || charset=="")
				return text;
			byte[] b=Encoding.Default.GetBytes(text);
			return new string(Encoding.GetEncoding(charset).GetChars(b));
		}

		public static string Decode(string text,string charset)
		{
			byte[] b=Convert.FromBase64String(text);
			return new string(Encoding.GetEncoding(charset).GetChars(b));
		}
	}
}
