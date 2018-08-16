using System;
using DasBlog.Web;
using Microsoft.Extensions.DependencyInjection;

namespace DasBlog.Tests.FunctionalTests.IntegrationTests.Support
{
	public class ServiceResolver
	{
		private IServiceProvider serviceProvider;
		public T GetService<T>()
		{
			if (serviceProvider == null)
			{
				serviceProvider = Startup.DasBlogServices.BuildServiceProvider();
			}
			return (T)serviceProvider.GetService(typeof(T));
		}
	}
}
