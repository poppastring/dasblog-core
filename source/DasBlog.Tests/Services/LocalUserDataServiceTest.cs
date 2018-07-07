﻿using System.Collections.Generic;
using System.Runtime.InteropServices;
using DasBlog.Core.Security;
using Xunit;
using DasBlog.Web.Services;
using DasBlog.Web.Services.Interfaces;
using Microsoft.Extensions.Options;
using System.Linq;

namespace DasBlog.Tests.Services
{
	public class LocalUserDataServiceTest
	{
		[Fact]
		public void ShouldLoadUsers()
		{
			ILocalUserDataService service = new LocalUserDataService(
			  new Options<LocalUserDataOptions>{ Value = new LocalUserDataOptions{Path = string.Empty}});
			List<User> users = service.LoadUsers().ToList();

			Assert.Single(users);
		}
	}

	public class Options<T> : IOptions<T> where T : class, new()
	{
		public T Value { get; set; }
	}
}