using System;
using System.Linq.Expressions;

namespace DasBlog.Core.Common
{
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
