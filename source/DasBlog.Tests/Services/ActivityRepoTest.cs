using System;
using DasBlog.Core.Services;
using Xunit;

namespace DasBlog.Tests.UnitTests.UnitTests.UnitTests.Services
{
	public class ActivityRepoTest
	{
		[Fact]
		public void GetEventLines_SingleLineLog_ReturnsOneLIne()
		{
			string path = "..\\..\\..\\logs";
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
