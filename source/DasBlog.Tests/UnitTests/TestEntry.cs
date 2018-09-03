using System;
using System.Text;
using newtelligence.DasBlog.Runtime;

namespace DasBlog.Tests.UnitTests
{
	[Serializable]
	public class TestEntry
	{
		private static readonly string Sentence = "The quick brown fox jumps over the lazy dog. ";

		public static Entry CreateEntry(string title, int lineCount, int paragraphCount)
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
