using System;
using NUnit.Framework;
using RJH.CommandLineHelper;



namespace RJH.CommandLineHelper.Test
{
	/// <summary>
	/// Summary description for ParserTest.
	/// </summary>
	[TestFixture]
	public class ParserTest
	{
		[SetUp]
		public void SetUpForTests()
		{
			// This method will be run each and every time a test method is run.
			// Place common initialization steps here that should be run before
			// every test.
		}

		[TearDown]
		public void TearDownTests()
		{
			// This method will be run each and every time a test method is run.
			// Place common cleanup steps here that should be run after
			// every test.
		}

		[Test]
		public void ParseStringWithoutQuotesAndParams()
		{
			Parser parser = new Parser( "Jee.exe", this);
			parser.Parse();
			Assert.AreEqual("Jee.exe",parser.ApplicationName);
			Assert.AreEqual(0,parser.Parameters.Length);
		}

		[Test]
		public void ParseStringWithoutQuotesAndOneParam()
		{
			Parser parser = new Parser( "Jee.exe test", this);
			parser.Parse();
			Assert.AreEqual("Jee.exe",parser.ApplicationName);
			Assert.AreEqual(1, parser.Parameters.Length );
		}

		[Test]
		public void ParseStringWithoutQuotesAndTwoParams()
		{
			Parser parser = new Parser( "Jee.exe test1 test2", this);
			parser.Parse();
			Assert.AreEqual("Jee.exe",parser.ApplicationName);
			Assert.AreEqual(2,parser.Parameters.Length);
		}

		[Test]
		public void ParseStringWithoutQuotesAndParamWithQuotes()
		{
			Parser parser = new Parser( "Jee.exe test3 \'test1 test2\' test4", this);
			parser.Parse();
			Assert.AreEqual("Jee.exe",parser.ApplicationName);
			Assert.AreEqual(3,parser.Parameters.Length);
		}

		[Test]
		public void ParseStringWithQuotesAndOneParam()
		{
			Parser parser = new Parser( "'C:\\Program Files\\Microsoft Virtual PC\\Virtual PC.exe' test", this);
			parser.Parse();
			Assert.AreEqual("C:\\Program Files\\Microsoft Virtual PC\\Virtual PC.exe", parser.ApplicationName);
			Assert.AreEqual(1,parser.Parameters.Length);
		}

		[Test]
		public void ParseStringWithQuotesAndTwoParams()
		{
			Parser parser = new Parser( "'C:\\Program Files\\Microsoft Virtual PC\\Virtual PC.exe' test1 test2", this);
			parser.Parse();
			Assert.AreEqual("C:\\Program Files\\Microsoft Virtual PC\\Virtual PC.exe",parser.ApplicationName);
			Assert.AreEqual( 2, parser.Parameters.Length);
		}

		[Test]
		public void ParseStringWithQuotesAndParamWithQuote()
		{
			Parser parser = new Parser( "'C:\\Program Files\\Microsoft Virtual PC\\Virtual PC.exe' 'test1 test2'", this);
			parser.Parse();
			Assert.AreEqual("C:\\Program Files\\Microsoft Virtual PC\\Virtual PC.exe",parser.ApplicationName);
			Assert.AreEqual( 1, parser.Parameters.Length);
		}

		[Test]
		public void ParseStringWithDoubleQuotesAndParamWithQuote()
		{
			Parser parser = new Parser( "\"C:\\Program Files\\Microsoft Virtual PC\\Virtual PC.exe\" 'test1 test2'", this);
			parser.Parse();
			Assert.AreEqual("C:\\Program Files\\Microsoft Virtual PC\\Virtual PC.exe",parser.ApplicationName);
			Assert.AreEqual( 1, parser.Parameters.Length);
		}

		[Test]
		public void ParseStringWithDoubleQuotesAndParamWithDoubleQuotes()
		{
			Parser parser = new Parser( "\"C:\\Program Files\\Microsoft Virtual PC\\Virtual PC.exe\" \"test1 test2\"", this);
			parser.Parse();
			Assert.AreEqual("C:\\Program Files\\Microsoft Virtual PC\\Virtual PC.exe",parser.ApplicationName);
			Assert.AreEqual( 1, parser.Parameters.Length);
		}

		[Test]
		public void QuotesTest1()
		{
			Parser parser = new Parser( "\"C:\\Program Files\\Microsoft Virtual PC\\Virtual PC.exe\" /p\"test1\" /a'test2'", this);
			parser.Parse();
			Assert.AreEqual("C:\\Program Files\\Microsoft Virtual PC\\Virtual PC.exe",parser.ApplicationName);
			Assert.AreEqual(2, parser.Parameters.Length);
		}

		[Test]
		public void QuotesTest2()
		{
			Parser parser = new Parser( "'C:\\Program Files\\Microsoft Virtual PC\\Virtual PC.exe' /p'test1' /a'test2'", this);
			parser.Parse();
			Assert.AreEqual("C:\\Program Files\\Microsoft Virtual PC\\Virtual PC.exe",parser.ApplicationName);
			Assert.AreEqual(2, parser.Parameters.Length);
		}

		[Test]
		public void QuotesTest3()
		{
			Parser parser = new Parser( "\"C:\\Program Files\\Microsoft Virtual PC\\Virtual PC.exe\" /p \"test1\" /a \"test2\"", this);
			parser.Parse();
			Assert.AreEqual("C:\\Program Files\\Microsoft Virtual PC\\Virtual PC.exe",parser.ApplicationName);
			Assert.AreEqual(4, parser.Parameters.Length);
		}

		[Test]
		public void QuotesTest4()
		{
			Parser parser = new Parser( "\"C:\\Program Files\\Microsoft Virtual PC\\Virtual PC.exe\" /p:\"test1\" /a:\"test2\"", this);
			parser.Parse();
			Assert.AreEqual("C:\\Program Files\\Microsoft Virtual PC\\Virtual PC.exe",parser.ApplicationName);
			Assert.AreEqual(2, parser.Parameters.Length);
		}

		[Test]
		public void AutoLoadWithQuotes()
		{
			CommandLine commandLine = new CommandLine();
			Parser parser = new Parser("DummyProgram.exe /SourceDir:\"c:\\Program Files\"", commandLine);
			parser.Parse();
			Assert.AreEqual("c:\\Program Files", commandLine.SourceDirectory, "The source parameter is not correct.");
		}

		[Test]
		public void GetHelpText()
		{
			string expectedText =
				"DummyProgramName.exe [/SourceType:Radio] [/{ContentDir|to}] [/{SourceDir|from}] [/Help] \n\n" +
				"	/SourceType       -Specify the blog program from which to import content.\n" +
				"	/ContentDir       -The DasBlog content directory into which the entries are placed.\n" +
				"                              Aliases: to\n" +
				"	/SourceDir        -The source directory from which content will be imported.\n" +
				"                              Aliases: from\n" +
				"	/Help             -Displays the command line help.\n";

			CommandLine commandLine= new CommandLine();
			Parser parser = new Parser("DummyProgramName.exe /Help", commandLine);			
			parser.Parse();
			Console.WriteLine("Actual:\n" + parser.GetHelpText());
			Console.WriteLine("\nExpected:\n" + expectedText);

			Assert.IsTrue(commandLine.Help, "The CommandLine was unexpectedly not set to true when using the '/Help' parameters.");

			Assert.AreEqual(
				expectedText,
				parser.GetHelpText(),
				"The help text returned was not as expected.");
		}

		// [Test, ExpectedException(typeof(System.ApplicationException)), Ignore("Not yet supported")]
		public void InvalidEnumSpecified()
		{
			CommandLine commandLine= new CommandLine();
			Parser parser = new Parser("DummyProgramName.exe /Source:InvalidSource", commandLine);
			parser.Parse();
		}

	}
}
