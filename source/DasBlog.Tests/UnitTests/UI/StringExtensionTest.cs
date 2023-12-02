using DasBlog.Core.Extensions;
using Xunit;

namespace DasBlog.Tests.UnitTests.Misc
{
	public class StringExtensionTest
	{
		public StringExtensionTest() { }

		[Fact]
		[Trait("StringExtension", "UnitTest")]
		public void FindHeroImage_ReturnsHeroImage_WhenHeroImageExists()
		{
			string blogContent = "<img class=\"hero-image\" src=\"image.jpg\">";
			string result = blogContent.FindHeroImage();

			Assert.Equal("image.jpg", result);
		}

		[Fact]
		[Trait("StringExtension", "UnitTest")]
		public void FindHeroImage_ReturnsFirstImage_WhenHeroImageDoesNotExist()
		{
			string blogContent = "<img src=\"image.jpg\">";
			string result = blogContent.FindHeroImage();

			Assert.Equal("image.jpg", result);
		}

		[Fact]
		[Trait("StringExtension", "UnitTest")]
		public void FindHeroImage_ReturnsNoImage_WhenNoImageExist()
		{
			string blogContent = "<p>This is some test...</p";
			string result = blogContent.FindHeroImage();

			Assert.Empty(result);
		}

	}
}
