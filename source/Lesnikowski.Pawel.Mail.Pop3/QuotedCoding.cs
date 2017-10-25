using System;
using System.Text;
using System.Text.RegularExpressions;

namespace Lesnikowski.Pawel.Mail.Pop3
{
	/// <summary>
	/// Summary description for Coding.
	/// </summary>
	public abstract class QuotedCoding
	{
		/// <summary>
		/// zwraca tablice bajtow
		/// zamienia 3 znaki np '=A9' na odp wartosc.
		/// zamienia '_' na znak 32
		/// </summary>
		/// <param name="s">Kupis_Pawe=B3</param>
		/// <returns>Kupis Pawe³</returns>
		public static byte[] GetByteArray(string s)
		{
            // clemensv -- added check
			if ( s == null || s.Length == 0 )
                return new byte[0];
            
            byte[] buffer=new byte[s.Length];


			int bufferPosition=0;
			for(int i=0;i<s.Length;i++)
			{
                if (s[i]=='=' )
                {
                    if ( i<(s.Length-2) )
                    {
                        if ( s[i+1]=='\r' && s[i+2]=='\n')
                        {
                            bufferPosition--;
                        }
                        else
                        {
                            buffer[bufferPosition]=System.Convert.ToByte(s.Substring(i+1,2),16);
                        }
                        i+=2;
                    }
                    else
                    {
                        break;
                    }
                }
                else if (s[i]=='_')
                {
                    buffer[bufferPosition]=32;
                }
                else
                {
                    buffer[bufferPosition]=(byte)s[i];
                }
				bufferPosition++;
			}
			byte[] newArray=new byte[bufferPosition];
			Array.Copy(buffer,newArray,bufferPosition);
			return newArray;
		}

		/// <summary>
		/// Decoduje string "=?iso-8859-2?Q?Kupis_Pawe=B3?=" 
		/// lub zakodowany base64
		/// na poprawny
		/// </summary>
		/// <param name="s">"=?iso-8859-2?Q?Kupis_Pawe=B3?="</param>
		/// <returns>Kupis Pawe³</returns>
		public static string DecodeOne(string charset, string cmd, string subject )
		{
            byte[] bArray;

			if (cmd=="Q") //querystring
            {
                bArray=GetByteArray(subject);
            }
            else if (cmd=="B")//base64
            {
                bArray=Convert.FromBase64String(subject);
            }
            else
            {
                return subject;
            }
			Encoding encoding=Encoding.GetEncoding(charset); 
			return encoding.GetString(bArray);
		}

		private static readonly Regex splitterRegex = new Regex(@"(?<unencoded>((?!=\?).)*)?(?:=\?(?<charset>.*?)\?(?<cmd>\w)\?(?<subject>.*?)\?=)?",RegexOptions.Compiled);
		/// <summary>
		/// decoduje string zamienia wpisy (=?...?=) na odp wartosci
		/// </summary>
		/// <param name="s">"ala i =?iso-8859-2?Q?Kupis_Pawe=B3?= ma kota"</param>
		/// <returns>"ala i Pawe³ Kupis ma kota"</returns>
		public static string Decode(string s)
		{
			StringBuilder retString=new StringBuilder();
            foreach( Match m in splitterRegex.Matches(s) )
            {
                if ( m.Groups["unencoded"].Success )
                {
                     retString.Append(m.Groups["unencoded"].Value);
                }
                if ( m.Groups["charset"].Success && m.Groups["cmd"].Success && m.Groups["subject"].Success )
                {
                     retString.Append(DecodeOne(m.Groups["charset"].Value,m.Groups["cmd"].Value,m.Groups["subject"].Value));
                }
            }
            return retString.ToString();
		}

	}
}
