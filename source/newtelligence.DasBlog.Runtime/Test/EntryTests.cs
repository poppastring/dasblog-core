using System;
using System.Text;
using newtelligence.DasBlog.Runtime;
using NUnit.Framework;

namespace newtelligence.DasBlog.Runtime.Test
{
	/// <summary>
	/// Summary description for EntryTests.
	/// </summary>
	[TestFixture]
	public class EntryTests : TestBaseLocal
	{
		#region Tests
		[Test]
		public void CompareEntry()
		{
			Entry entry1 = TestEntry.CreateEntry("Test Entry", 5, 2);
			Entry entry2 = TestEntry.CreateEntry("Test Entry", 5, 2);

			// check to see if they contain the same simple types
			Assert.IsTrue(entry1.CompareTo(entry1) == 0);
			Assert.IsTrue(entry1.CompareTo(entry2) == 0);

			// change a simple type
			entry2.Title = "Test Entry 2";
			Assert.IsTrue(entry1.CompareTo(entry2) == 1, "entry1 is equal to entry2");

			entry2.Description = "Some Description";
			Assert.IsTrue(entry1.CompareTo(entry2) == 1);
		}

		[Test]
		public void CopyEntry()
		{
			Entry entry = new Entry();
			entry.Initialize();
			
			Entry same = entry;
			Assert.AreSame(entry, same);

			Entry copy = entry.Clone();
			bool equals = entry.Equals(copy);
			Assert.IsTrue(!equals);
		}

        [Test]
        public void TitleTests()
        {
            TitleTest( @" This    is a  test ", "This+Is+A+Test");
            // removed these characters in revision 385
			// TitleTest( @" - _ . ! * ' ( ) ", "-+_+.+!+*+'+(+)");
            TitleTest( @" - _ . ! * ' ( ) ", "");
			TitleTest( @"So <em>Not</em> true", "So+Not+True");
            TitleTest( @"Three is < four", "Three+Is+Four");
            TitleTest( @"Three is < four but > one", "Three+Is+One");
            TitleTest( @"My <sarcasm>favorite</sarcasm> bug", "My+Favorite+Bug");
            TitleTest( "\u00C0\u00C3\u00C5\u00c6\u00c8 \u00CC\u00CD\u00D0 \u0105\u0157\u0416\u042D \u0628\u0645\u1E84\uFB73",
                    "%c3%80%c3%83%c3%85%c3%86%c3%88+%c3%8c%c3%8d%c3%90+%c4%84%c5%97%d0%96%d0%ad+%d8%a8%d9%85%e1%ba%84%ef%ad%b3");
        }

        private void TitleTest(string title, string expected)
        {
            string result = Entry.InternalCompressTitle(title);
//          Console.WriteLine("<{0}> -> <{1}>", title, result);
            Assert.AreEqual(expected, result);
        }
		#endregion Tests
	}

	[Serializable]
	public class TestEntry
	{
		private static readonly string Sentence = "The quick brown fox jumps over the lazy dog. ";

		public static Entry CreateEntry (string title, int lineCount, int paragraphCount)
		{
			Entry entry = new Entry();
			entry.Initialize();
			entry.Title = title;

			StringBuilder lines = new StringBuilder(Sentence.Length * lineCount + 6);
			lines.Append("<p>");
			for (int i = 0; i < lineCount; i++)
			{
				lines.Append(Sentence);
			}
			lines.Append("</p>");

			StringBuilder paragraphs = new StringBuilder(Sentence.Length * lines.Length * paragraphCount);
			for (int i = 0; i < paragraphCount; i++)
			{
				paragraphs.Append(lines.ToString());
			}

			entry.Content = paragraphs.ToString();

			return entry;
		}
	}
}
