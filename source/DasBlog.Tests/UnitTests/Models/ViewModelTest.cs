using System;
using DasBlog.Services.ActivityLogs;
using DasBlog.Web.Models.ActivityPubModels;
using Xunit;

namespace DasBlog.Tests.UnitTests.Models
{
	public class ViewModelTest
	{
		[Fact]
		[Trait("Category", "UnitTest")]
		public void SerializationDeserializationTest()
		{
			UserPageViewModel user = new UserPageViewModel();

			string serialized = user.ToJson();
			var nextuser = UserPageViewModel.FromJson(serialized);

			Assert.Equal(user, nextuser);
		}
	}
}
