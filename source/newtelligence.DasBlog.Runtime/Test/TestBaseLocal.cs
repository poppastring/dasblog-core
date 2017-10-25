using System;
using System.IO;
using newtelligence.DasBlog.Util;
using NUnit.Framework;

namespace newtelligence.DasBlog.Runtime.Test
{
	/// <summary>
	/// Summary description for RuntimeTest.
	/// </summary>
	public class TestBaseLocal
	{
		protected ILoggingDataService loggingService;
		protected IBlogDataService blogService;
		protected DirectoryInfo testContent;
		protected DirectoryInfo testLogs;

		[TestFixtureSetUp]
		public void SetUpForTests()
		{
			// This method will be run each and every time a test method is run.
			// Place common initialization steps here that should be run before
			// every test.

			DirectoryInfo root = new DirectoryInfo(ReflectionHelper.CodeBase());
			
			testContent = new DirectoryInfo(Path.Combine(root.Parent.FullName, "TestContent"));
			Assert.IsTrue(testContent.Exists);
			
			testLogs = new DirectoryInfo(Path.Combine(root.Parent.FullName, "TestLogs"));
			Assert.IsTrue(testLogs.Exists);
			
			loggingService = LoggingDataServiceFactory.GetService(
				testLogs.FullName);
			Assert.IsNotNull(loggingService);
			
			blogService = BlogDataServiceFactory.GetService(
				testContent.FullName,
				loggingService);
			Assert.IsNotNull(blogService);

			loggingService.AddEvent(new EventDataItem(EventCodes.ApplicationStartup, "", ""));
		}

		[TestFixtureTearDown]
		public void TearDownTests()
		{
			// This method will be run each and every time a test method is run.
			// Place common cleanup steps here that should be run after
			// every test.

			foreach (FileInfo file in testContent.GetFiles("*dayfeedback.xml"))
			{
				file.Delete();
			}
		}
	}
}
