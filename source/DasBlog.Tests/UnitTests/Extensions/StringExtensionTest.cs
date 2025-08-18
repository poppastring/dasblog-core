using DasBlog.Core.Extensions;
using Xunit;

namespace DasBlog.Tests.UnitTests.Extensions
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

		[Fact]
		[Trait("StringExtension", "UnitTest")]
		public void RemoveLineBreaks_RemovesAllLineBreaks()
		{
			string input = "Hello\r\nWorld\nTest\rString";
			string expected = "Hello  World Test String";
			Assert.Equal(expected, input.RemoveLineBreaks());
		}

		[Fact]
		[Trait("StringExtension", "UnitTest")]
		public void StripHtml_RemovesHtmlTagsAndEntities()
		{
			string input = "<p>Hello <b>World</b><br>&quot;</p>";
			string expected = "Hello World  ";
			Assert.Equal(expected, input.StripHtml());
		}

		[Fact]
		[Trait("StringExtension", "UnitTest")]
		public void StripHTMLFromText_RemovesHtmlAndDecodesEntities()
		{
			string input = "<p>Hello &amp; &quot;World&quot;</p>";
			string expected = "Hello & 'World' ...".Replace("'", ((char)39).ToString());
			string result = input.StripHTMLFromText();
			Assert.StartsWith("Hello & ", result); // Should decode &amp; and remove HTML
			Assert.Contains("World", result);
		}

		[Fact]
		[Trait("StringExtension", "UnitTest")]
		public void RemoveDoubleSpaceCharacters_ReplacesMultipleSpacesWithSingle()
		{
			string input = "This   is   a   test.";
			string expected = "This is a test.";
			Assert.Equal(expected, input.RemoveDoubleSpaceCharacters());
		}

		[Fact]
		[Trait("StringExtension", "UnitTest")]
		public void CutLongString_TruncatesAndAppendsEllipsis()
		{
			string input = "This is a long string that should be cut.";
			string result = input.CutLongString(10);
			Assert.EndsWith(" ...", result);
			Assert.True(result.Length <= 14); // 10 chars + ' ...'
		}

		[Fact]
		[Trait("StringExtension", "UnitTest")]
		public void RemoveQuotationMarks_ReplacesVariousQuotationMarks()
		{
			string input = "\"Test\" ¨Test¨ “Test” „Test„";
			string expected = "'Test' 'Test' 'Test' 'Test'";
			Assert.Equal(expected, input.RemoveQuotationMarks());
		}

		[Fact]
		[Trait("StringExtension", "UnitTest")]
		public void FindFirstImage_ReturnsFirstImageSrc()
		{
			string input = "<img src='first.jpg'><img src=\"second.jpg\">";
			string expected = "first.jpg";
			Assert.Equal(expected, input.FindFirstImage());
		}

		[Fact]
		[Trait("StringExtension", "UnitTest")]
		public void FindFirstImage_ReturnsEmpty_WhenNoImage()
		{
			string input = "<p>No images here</p>";
			Assert.Empty(input.FindFirstImage());
		}

		[Fact]
		[Trait("StringExtension", "UnitTest")]
		public void FindFirstYouTubeVideo_ReturnsFirstIframeSrc()
		{
			string input = "<iframe src='https://youtube.com/embed/abc'></iframe>";
			string expected = "https://youtube.com/embed/abc";
			Assert.Equal(expected, input.FindFirstYouTubeVideo());
		}

		[Fact]
		[Trait("StringExtension", "UnitTest")]
		public void FindFirstYouTubeVideo_ReturnsEmpty_WhenNoIframe()
		{
			string input = "<p>No video here</p>";
			Assert.Empty(input.FindFirstYouTubeVideo());
		}
	}
}
