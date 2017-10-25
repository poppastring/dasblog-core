using System;
using System.IO;
using newtelligence.DasBlog.Util;
using NUnit.Framework;

namespace newtelligence.DasBlog.Runtime.Test
{
	/// <summary>
	/// Summary description for RuntimeTest.
	/// </summary>
	public class TestBaseServer
	{
		protected DirectoryInfo testContent;
		protected DirectoryInfo testLogs;

		protected const string Username = "admin";
		protected const string Password = "admin";

		protected DirectoryInfo dasBlogContent;
		protected IBlogDataService localhostBlogService;
		protected ILoggingDataService loggingService;

		[TestFixtureSetUp]
		public void SetUpForTests()
		{
			// This method will be run each and every time a test method is run.
			// Place common initialization steps here that should be run before
			// every test.

			DirectoryInfo root = new DirectoryInfo(ReflectionHelper.CodeBase());
			testLogs = new DirectoryInfo(Path.Combine(root.Parent.FullName, "TestLogs"));
			Assert.IsTrue(testLogs.Exists);

			loggingService = LoggingDataServiceFactory.GetService(
				testLogs.FullName);
			Assert.IsNotNull(loggingService);

			dasBlogContent = new DirectoryInfo("../../../newtelligence.DasBlog.Web/content");
			localhostBlogService = GetDataService();
		}

		[SetUp]
		public void SetUp()
		{
			BlogDataServiceFactory.GetService(dasBlogContent.FullName, null);
		}

		[TearDown]
		public void TearDown()
		{
			BlogDataServiceFactory.RemoveService(dasBlogContent.FullName);
		}

		[TestFixtureTearDown]
		public void TearDownTests()
		{
			// This method will be run each and every time a test method is run.
			// Place common cleanup steps here that should be run after
			// every test.
		}

		protected IBlogDataService GetDataService()
		{
			BlogDataServiceFactory.RemoveService(dasBlogContent.FullName);
			localhostBlogService = BlogDataServiceFactory.GetService(
				dasBlogContent.FullName,
				loggingService);
			Assert.IsNotNull(localhostBlogService);	

			return localhostBlogService;
		}
	}
}
