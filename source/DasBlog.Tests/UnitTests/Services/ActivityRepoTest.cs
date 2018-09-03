using System;
using DasBlog.Core.Services;
using Xunit;

namespace DasBlog.Tests.UnitTests.Services
{
	public class ActivityRepoTest
	{
		[Fact]
		[Trait("Category", "UnitTest")]
		public void GetEventLines_SingleLineLog_ReturnsOneLIne()
		{
			string path = "..\\..\\..\\logs";		// up from bin/netcoreappn.n
			int ctr = 0;
			using (ActivityRepo repo = new ActivityRepo(path))
			{
				foreach (var line in repo.GetEventLines(new DateTime(2018, 7, 19)))
				{
					ctr++;
				}
			}
			Assert.Equal(1, ctr);
			
		}
	}
}
