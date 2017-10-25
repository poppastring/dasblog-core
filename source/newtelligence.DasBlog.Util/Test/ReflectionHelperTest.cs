using System;
using System.Reflection;
using newtelligence.DasBlog.Util;
using NUnit.Framework;

namespace newtelligence.DasBlog.Util.Test
{
	[TestFixture]
	public class CodeBaseTest
	{
		#region METHODS

		[Test]
		public void TestConvert()
		{
			// get the actual path to the executing assembly
			Assembly thisAssembly = Assembly.GetExecutingAssembly();
			string codeBasePath = thisAssembly.CodeBase;

			// get the converted path
			string convertedPath = ReflectionHelper.CodeBase();

			string[] codeBasePathItems = codeBasePath.Split('/');
			string[] convertedPathItems = convertedPath.Split('\\');

			Assert.IsTrue(codeBasePathItems.Length > convertedPathItems.Length,
				"Covert failed to remove the url information");

			// Since the filename is also removed, start at Length - 2...
			// Also, there is an empty element at the end of the converted
			// path where the filename used to be.
			int lastCodeBasePathIndex = codeBasePathItems.Length - 2;
			int lastConvertedPathIndex = convertedPathItems.Length - 2;
			int offset = lastCodeBasePathIndex - lastConvertedPathIndex;

			for (int i = lastConvertedPathIndex; i >= 0; i--)
			{
				int currentOffset = i + offset;

				string expectedPathItem = codeBasePathItems[currentOffset];
				string convertedPathItem = convertedPathItems[i];
				Assert.IsTrue(string.Compare(expectedPathItem,convertedPathItem,true) == 0,
					"Convert failed to produce the expected path items");
			}
		}

		#endregion METHODS
	}
}

