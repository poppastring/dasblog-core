using System;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace DasBlog.Core.Common
{
	public static class Utils
	{
		public static string GetGravatarHash(string email)
		{
			string hash = string.Empty;
			byte[] data, enc;

			data = Encoding.Default.GetBytes(email.ToLowerInvariant());

			using (MD5 md5 = MD5.Create())
			{
				enc = md5.TransformFinalBlock(data, 0, data.Length);
				foreach (byte b in md5.Hash)
				{
					hash += Convert.ToString(b, 16).ToLower().PadLeft(2, '0');
				}
				md5.Clear();
			}

			return hash;
		}
	}
}
