using System;
using System.Collections.Generic;
using System.Text;
using newtelligence.DasBlog.Runtime;
using NodaTime;
using Xunit;

namespace DasBlog.Tests.UnitTests.Services
{
    public class BlogDataServiceTest
    {
		/// <summary>
		/// All test methods should follow this naming pattern
		/// </summary>
		[Fact]
		public void UnitOfWork_StateUnderTest_ExpectedBehavior()
		{

		}

		[Theory]
		[MemberData(nameof(DasBlogDataService))]
		public void BlogDataService_EntryRetrieval_Successful(IBlogDataService blogdataservice)
		{
			var entries = blogdataservice.GetEntriesForDay(DateTime.MaxValue.AddDays(-2), DateTimeZone.Utc, string.Empty, int.MaxValue, int.MaxValue, string.Empty);
			Assert.NotNull(entries);
		}

		public static TheoryData<IBlogDataService> DasBlogDataService = new TheoryData<IBlogDataService>
		{
			BlogDataServiceFactory.GetService(UnitTestsConstants.TestContentLocation, LoggingDataServiceFactory.GetService(UnitTestsConstants.TestLoggingLocation))
		};


	}
}
