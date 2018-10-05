using System;
using System.IO;
using DasBlog.Tests.FunctionalTests.Common;
using DasBlog.Tests.Support;
using DasBlog.Tests.Support.Common;
using DasBlog.Tests.Support.Interfaces;
using Xunit;
using Xunit.Abstractions;

namespace DasBlog.Tests.FunctionalTests.TestInfrastructureTests 
{
	[Collection(Constants.TestInfrastructureUsersCollection)]
	public class XPathTests : IClassFixture<InfrastructureTestPlatform>, IDisposable
	{
		private readonly IDasBlogSandbox dasBlogSandbox;
		private const string defaultDayEntryFileName = "2018-08-03.dayentry.xml";
		private readonly ITestDataProcessor testDataProcessor;

		public XPathTests(ITestOutputHelper testOutputHelper, InfrastructureTestPlatform platform)
		{
			platform.CompleteSetup(testOutputHelper);
			dasBlogSandbox = platform.CreateSandbox(Constants.VanillaEnvironment);
			testDataProcessor = new TestDataProcesor(dasBlogSandbox.TestEnvironmentPath);
		}

		[Fact]
		[Trait(Constants.CategoryTraitType, Constants.TestInfrastructureTestTraitValue)]
		public void GetTitle_FromVanillaEntry_ReturnsTitle()
		{
			var result = testDataProcessor.GetValue(
				Path.Combine(Constants.ContentDirectory, defaultDayEntryFileName)
				, "/post:DayEntry/post:Entries/post:Entry[post:EntryId='5125c596-d6d5-46fe-9f9b-c13f851d8b0d']/post:Created");
			Assert.Equal("2018-08-03T07:09:58+01:00", result.value);
		}
		[Fact]
		[Trait(Constants.CategoryTraitType, Constants.TestInfrastructureTestTraitValue)]
		public void GetRubbish_FromVanillaEntry_RetursnFalse()
		{
			var result =  testDataProcessor.GetValue(
				Path.Combine(Constants.ContentDirectory, defaultDayEntryFileName)
				, "/post:DayEntry/post:Entries/post:Entry[post:EntryId='5125c596-d6d5-46fe-9f9b-c13f851d8b0d']/Rubbish");
			Assert.False(result.success);
		}
		[Fact]
		[Trait(Constants.CategoryTraitType, Constants.TestInfrastructureTestTraitValue)]
		public void GetValidValue_WhereXPathAllowsDuplicates_ThrowsException()
		{
			Assert.Throws<Exception>(() => testDataProcessor.GetValue(
				Path.Combine(Constants.ContentDirectory, defaultDayEntryFileName)
				, "/post:DayEntry/post:Entries/post:Entry/post:Title"));
		}
		[Fact]
		[Trait(Constants.CategoryTraitType, Constants.TestInfrastructureTestTraitValue)]
		public void GetConfigValue_ForValidKey_ReturnsValue()
		{
			var value = testDataProcessor.GetSiteConfigValue("EnableTitlePermaLinkUnique");
			Assert.Equal("false", value.value);
		}
		[Fact]
		[Trait(Constants.CategoryTraitType, Constants.TestInfrastructureTestTraitValue)]
		public void GetSecurityConfigValue_ForValidKey_ReturnsValue()
		{
			var value = testDataProcessor.GetSiteSecurityConfigValue("myemail@myemail.com","Role");
			Assert.Equal("Admin", value.value);
		}

		[Fact]
		[Trait(Constants.CategoryTraitType, Constants.TestInfrastructureTestTraitValue)]
		public void SetSiteConfigValue_WithReplacementValue_IsReflectedInFile()
		{
			testDataProcessor.SetSiteConfigValue( "Root", "Mike");
			var value = testDataProcessor.GetSiteConfigValue("Root");
			Assert.Equal("Mike", value.value);
		}
		[Fact]
		[Trait(Constants.CategoryTraitType, Constants.TestInfrastructureTestTraitValue)]
		public void SetSiteConfigValue_WithAdditionalValue_IsReflectedInFile()
		{
			testDataProcessor.SetSiteConfigValue( "Root2", "Mike2");
			var value = testDataProcessor.GetSiteConfigValue("Root2");
			Assert.Equal("Mike2", value.value);
		}
		[Fact]
		[Trait(Constants.CategoryTraitType, Constants.TestInfrastructureTestTraitValue)]
		public void SetSiteSecurityConfigValue_WithReplacementValue_IsReflectedInFile()
		{
			testDataProcessor.SetSiteSecurityConfigValue( "myemail@myemail.com", "Role", "Contributor");
			var value = testDataProcessor.GetSiteSecurityConfigValue("myemail@myemail.com", "Role");
			Assert.Equal("Contributor", value.value);
		}
		[Fact]
		[Trait(Constants.CategoryTraitType, Constants.TestInfrastructureTestTraitValue)]
		public void SetSiteSecurityConfigValue_WithAddtionalValue_IsReflectedInFile()
		{
			testDataProcessor.SetSiteSecurityConfigValue( "myemail@myemail.com", "Role2", "Contributor2");
			var value = testDataProcessor.GetSiteSecurityConfigValue("myemail@myemail.com", "Role2");
			Assert.Equal("Contributor2", value.value);
		}
		
		public void Dispose()
		{
			dasBlogSandbox?.Dispose();
		}
	}
}
