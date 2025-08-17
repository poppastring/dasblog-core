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
			string blogContent = "<img src=\"incorrect.jpg\" /> <img class=\"hero-image\" src=\"image.jpg\" alt=\"hero alt\">";
			var result = blogContent.FindHeroImage();

			Assert.Equal("image.jpg", result.url);
			Assert.Equal("hero alt", result.alt);
		}

		[Fact]
		[Trait("StringExtension", "UnitTest")]
		public void FindHeroImage_ReturnsHeroImage_MoreThanOneClassHeroNotFirst()
		{
			string blogContent = "<img src=\"incorrect.jpg\" /> <img class=\"somestyle hero-image\" src=\"image.jpg\" alt=\"hero alt\">";
			var result = blogContent.FindHeroImage();

			Assert.Equal("image.jpg", result.url);
			Assert.Equal("hero alt", result.alt);
		}

		[Fact]
		[Trait("StringExtension", "UnitTest")]
		public void FindHeroImage_ReturnsHeroImage_WhenClassIsNotFirst()
		{
			string blogContent = "<img src=\"incorrect.jpg\" /> <img src=\"image2.jpg\" alt=\"alt2\" class=\"hero-image\">";
			var result = blogContent.FindHeroImage();

			Assert.Equal("image2.jpg", result.url);
			Assert.Equal("alt2", result.alt);
		}

		[Fact]
		[Trait("StringExtension", "UnitTest")]
		public void FindHeroImage_ReturnsHeroImage_WithoutAlt()
		{
			string blogContent = "<img src=\"incorrect.jpg\" /> <img class=\"hero-image\" src=\"image3.jpg\">";
			var result = blogContent.FindHeroImage();

			Assert.Equal("image3.jpg", result.url);
			Assert.Equal(string.Empty, result.alt);
		}

		[Fact]
		[Trait("StringExtension", "UnitTest")]
		public void FindHeroImage_ReturnsFirstImage_WhenHeroImageDoesNotExist()
		{
			string blogContent = "<img src=\"fallback.jpg\" /> <img src=\"image.jpg\" alt=\"first alt\">";
			var result = blogContent.FindHeroImage();

			Assert.Equal("fallback.jpg", result.url);
			Assert.Equal(string.Empty, result.alt); // alt is not returned for fallback
		}

		[Fact]
		[Trait("StringExtension", "UnitTest")]
		public void FindHeroImage_ReturnsNoImage_WhenNoImageExist()
		{
			string blogContent = "<p>This is some test...</p>";
			var result = blogContent.FindHeroImage();

			Assert.Equal(string.Empty, result.url);
			Assert.Equal(string.Empty, result.alt);
		}
	}
}
