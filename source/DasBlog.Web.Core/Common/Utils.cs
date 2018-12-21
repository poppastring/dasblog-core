using System;
using System.Linq.Expressions;
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

			using (MD5 md5 = new MD5CryptoServiceProvider())
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

	public static class Veriifier
	{
		public static void VerifyParam(Expression<Func<bool>> pred)
		{
			if (!pred.Compile()())
			{
				throw new Exception($"The following expectation was not met {pred}");
							// a bloke's got to have a bit of fun
			}
		}
	}
}
