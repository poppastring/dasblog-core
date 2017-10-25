using newtelligence.DasBlog.Util;
using newtelligence.DasBlog.Runtime;

using NUnit.Framework;

using System;
using System.IO;

namespace newtelligence.DasBlog.Runtime.Test
{
	public class BlogServiceContainer
	{

		public readonly string CONTENT_DIRECTORY_PATH = 
			Path.Combine(ReflectionHelper.CodeBase(),@"Content");
		public readonly string TEST_CONTENT_DIRECTORY_PATH = 
			Path.Combine(ReflectionHelper.CodeBase(),@"MockContent");

		// IBlogDataService does not appear to be CLS compliant.  Hmmm??
		[CLSCompliant(false)]
		protected IBlogDataService BlogDataService
		{
			[System.Diagnostics.DebuggerStepThrough()]
			get 
			{ 
				return m_BlogDataService; 
			}  
			[System.Diagnostics.DebuggerStepThrough()]
			set
			{ 
				m_BlogDataService = value; 
			}  
		} 
		private IBlogDataService m_BlogDataService;

		#region SETUP/TEARDOWN FIXTURE
		[TestFixtureSetUp]
		public void SetUpTestFixture()
		{
			FileInfo testSourceFile;
			FileInfo file;

			// Remove all files from Content directory so as to have a clean environment.
			// This includes deleting the cache files.
			foreach(string fileName in Directory.GetFiles(CONTENT_DIRECTORY_PATH))
			{
				File.Delete(fileName);
			}
			foreach(string fileName in Directory.GetFiles(TEST_CONTENT_DIRECTORY_PATH))
			{
				testSourceFile = new FileInfo(fileName);
				file = testSourceFile.CopyTo(Path.Combine(CONTENT_DIRECTORY_PATH, 
					Path.GetFileName(fileName)));
				// Change the file to read-write
				file.Attributes = file.Attributes & ~FileAttributes.ReadOnly;
			}
		}
		#endregion SETUP/TEARDOWN FIXTURE

		#region SETUP/TEARDOWN TESTS

		[SetUp]
		public void SetUpTests()
		{
			ILoggingDataService loggingService;
			loggingService = LoggingDataServiceFactory.GetService(
				Path.Combine(ReflectionHelper.CodeBase(),@"logs"));
			BlogDataService = BlogDataServiceFactory.GetService(
				Path.Combine(ReflectionHelper.CodeBase(),@"Content"),
				loggingService);
		}
		#endregion SETUP/TEARDOWN TESTS


	}
}
