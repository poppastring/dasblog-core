using System;
using System.Collections.Generic;
using DasBlog.Core;
using DasBlog.Core.Services;
using DasBlog.Core.Services.Interfaces;
using Xunit;

namespace DasBlog.Tests.Services
{
	public class ActivityServiceTest
	{
		[Fact]
		public void ShoulProcessSingleLine()
		{
			IActivityRepoFactory factory = new MockActivityRepoFactory();
			IEventLineParser parser = new MockEventLineParser(true);
			ActivityService service = new ActivityService(factory, parser, new Microsoft.Extensions.Logging.Abstractions.NullLogger<ActivityService>());
			int ctr = 0;
			foreach (var eddi in service.GetEventsForDay(new DateTime(1954, 1, 25)))
			{
				ctr++;
			}
			Assert.Equal(1, ctr);
		}
		[Fact]
		public void ShouldOmitUnparsedEvents()
		{
			IActivityRepoFactory factory = new MockActivityRepoFactory();
			IEventLineParser parser = new MockEventLineParser(false);	//fail all parsing
			ActivityService service = new ActivityService(factory, parser, new Microsoft.Extensions.Logging.Abstractions.NullLogger<ActivityService>());
			int ctr = 0;
			foreach (var eddi in service.GetEventsForDay(new DateTime(1954, 1, 25)))
			{
				ctr++;
			}
			Assert.Equal(0, ctr);
		}
		[Fact]
		public void ShouldOmitUnknownDate()
		{
			IActivityRepoFactory factory = new MockActivityRepoFactory();
			IEventLineParser parser = new MockEventLineParser(false);	//fail all parsing
			ActivityService service = new ActivityService(factory, parser, new Microsoft.Extensions.Logging.Abstractions.NullLogger<ActivityService>());
			int ctr = 0;
			foreach (var eddi in service.GetEventsForDay(new DateTime(1, 1, 1)))
			{
				ctr++;
			}
			Assert.Equal(0, ctr);
		}
		[Fact]
		public void ShouldOmitNonEventPlusStacktrace()
		{
			IActivityRepoFactory factory = new MockActivityRepoFactory();
			IEventLineParser parser = new MockEventLineParser(false);
			ActivityService service = new ActivityService(factory, parser, new Microsoft.Extensions.Logging.Abstractions.NullLogger<ActivityService>());
			int ctr = 0;
			foreach (var eddi in service.GetEventsForDay(new DateTime(1951, 12, 8)))
			{
				ctr++;
			}
			Assert.Equal(0, ctr);
		}
		[Fact]
		public void ShouldProcessEventPlusStackTrace()
		{
			IActivityRepoFactory factory = new MockActivityRepoFactory();
			IEventLineParser parser = new MockEventLineParser(true);
			ActivityService service = new ActivityService(factory, parser, new Microsoft.Extensions.Logging.Abstractions.NullLogger<ActivityService>());
			int ctr = 0;
			foreach (var eddi in service.GetEventsForDay(new DateTime(1951, 12, 8)))
			{
				ctr++;
			}
			Assert.Equal(1, ctr);
		}		
		[Fact]
		public void ShouldProcessEventPlusStackTracePlusEvent()
		{
			IActivityRepoFactory factory = new MockActivityRepoFactory();
			IEventLineParser parser = new MockEventLineParser(true);	// all parsing succeeds
			ActivityService service = new ActivityService(factory, parser, new Microsoft.Extensions.Logging.Abstractions.NullLogger<ActivityService>());
			int ctr = 0;
			foreach (var eddi in service.GetEventsForDay(new DateTime(1960, 4, 9)))
			{
				ctr++;
			}
			Assert.Equal(2, ctr);
		}		
		[Fact]
		public void ShouldOmitNonEventPlusStackTracePlusNonEvent()
		{
			IActivityRepoFactory factory = new MockActivityRepoFactory();
			IEventLineParser parser = new MockEventLineParser(false);		// fail all parsing
			ActivityService service = new ActivityService(factory, parser, new Microsoft.Extensions.Logging.Abstractions.NullLogger<ActivityService>());
			int ctr = 0;
			foreach (var eddi in service.GetEventsForDay(new DateTime(1960, 4, 9)))
			{
				ctr++;
			}
			Assert.Equal(0, ctr);
		}		
		[Fact]
		public void ShouldProessNonEventPlusStackTracePlusEvent()
		{
			IActivityRepoFactory factory = new MockActivityRepoFactory();
			IEventLineParser parser = new EventLineParser();
			ActivityService service = new ActivityService(factory, parser, new Microsoft.Extensions.Logging.Abstractions.NullLogger<ActivityService>());
			int ctr = 0;
			foreach (var eddi in service.GetEventsForDay(new DateTime(1990, 3, 17)))
			{
				ctr++;
			}
			Assert.Equal(1, ctr);
		}
		[Fact]
		public void ShouldProessEventPlusStackTracePlusNonEvent()
		{
			IActivityRepoFactory factory = new MockActivityRepoFactory();
			IEventLineParser parser = new EventLineParser();
			ActivityService service = new ActivityService(factory, parser, new Microsoft.Extensions.Logging.Abstractions.NullLogger<ActivityService>());
			int ctr = 0;
			foreach (var eddi in service.GetEventsForDay(new DateTime(2000, 1, 1)))
			{
				ctr++;
			}
			Assert.Equal(1, ctr);
		}
	}

	public class MockActivityRepoFactory : IActivityRepoFactory
	{
		public IActivityRepo GetRepo()
		{
			return new MockActivityRepo();
		}
	}

	internal class MockActivityRepo : IActivityRepo
	{
		private readonly Dictionary<DateTime, (string name, string lines)> results = new Dictionary<DateTime, (string name, string lines)>
		{
			{new DateTime(1954, 01, 25),("single-line", @"2018-07-18 09:10:08.388 +01:00 [Information] Microsoft.AspNetCore.Authorization.DefaultAuthorizationService: Authorization failed for user: (null).") }
			,{new DateTime(1951, 12, 8), ("logline-stacktrace"
,@"2018-07-18 09:22:23.333 +01:00 [Error] Microsoft.AspNetCore.Diagnostics.DeveloperExceptionPageMiddleware: An unhandled exception has occurred while executing the request
System.Exception: Failure in Users Controller
   at DasBlog.Web.Controllers.UsersController.Index(String email) in C:\projects\dasblog-core\source\DasBlog.Web.UI\Controllers\UsersController.cs:line 113
   at lambda_method(Closure , Object , Object[] )
   at Microsoft.Extensions.Internal.ObjectMethodExecutor.Execute(Object target, Object[] parameters)
   at Microsoft.AspNetCore.Mvc.Internal.ControllerActionInvoker.InvokeActionMethodAsync()
   at Microsoft.AspNetCore.Mvc.Internal.ControllerActionInvoker.InvokeNextActionFilterAsync()
   at Microsoft.AspNetCore.Mvc.Internal.ControllerActionInvoker.Rethrow(ActionExecutedContext context)
   at Microsoft.AspNetCore.Mvc.Internal.ControllerActionInvoker.Next(State& next, Scope& scope, Object& state, Boolean& isCompleted)
   at Microsoft.AspNetCore.Mvc.Internal.ControllerActionInvoker.InvokeInnerFilterAsync()
   at Microsoft.AspNetCore.Mvc.Internal.ResourceInvoker.InvokeNextResourceFilter()
   at Microsoft.AspNetCore.Mvc.Internal.ResourceInvoker.Rethrow(ResourceExecutedContext context)
   at Microsoft.AspNetCore.Mvc.Internal.ResourceInvoker.Next(State& next, Scope& scope, Object& state, Boolean& isCompleted)
   at Microsoft.AspNetCore.Mvc.Internal.ResourceInvoker.InvokeFilterPipelineAsync()
   at Microsoft.AspNetCore.Mvc.Internal.ResourceInvoker.InvokeAsync()
   at Microsoft.AspNetCore.Builder.RouterMiddleware.Invoke(HttpContext httpContext)
   at Microsoft.AspNetCore.Authentication.AuthenticationMiddleware.Invoke(HttpContext context)
   at Microsoft.AspNetCore.Diagnostics.DeveloperExceptionPageMiddleware.Invoke(HttpContext context)"
					)}
			,{new DateTime(1960, 4, 9), ("logline-stacktrace-logline"
,@"2018-07-18 09:22:23.333 +01:00 [Error] Microsoft.AspNetCore.Diagnostics.DeveloperExceptionPageMiddleware: An unhandled exception has occurred while executing the request
System.Exception: Failure in Users Controller
   at DasBlog.Web.Controllers.UsersController.Index(String email) in C:\projects\dasblog-core\source\DasBlog.Web.UI\Controllers\UsersController.cs:line 113
   at lambda_method(Closure , Object , Object[] )
   at Microsoft.Extensions.Internal.ObjectMethodExecutor.Execute(Object target, Object[] parameters)
2018-07-18 09:22:23.333 +01:00 [Error] Microsoft.AspNetCore.Diagnostics.DeveloperExceptionPageMiddleware: An unhandled exception has occurred while executing the request"
					)}
			,{new DateTime(1990, 3, 17), ("non-event-stacktrace-event"
,@"2018-07-18 09:22:23.333 +01:00 [Error] Microsoft.AspNetCore.Diagnostics.DeveloperExceptionPageMiddleware: An unhandled exception has occurred while executing the request
System.Exception: Failure in Users Controller
   at DasBlog.Web.Controllers.UsersController.Index(String email) in C:\projects\dasblog-core\source\DasBlog.Web.UI\Controllers\UsersController.cs:line 113
   at lambda_method(Closure , Object , Object[] )
   at Microsoft.Extensions.Internal.ObjectMethodExecutor.Execute(Object target, Object[] parameters)
2018-07-18 14:27:14.453 +01:00 [Information] DasBlog.Web.Controllers.AccountController: SecuritySuccess :: SecuritySuccess logged in successfully :: myemail@myemail.com"
					)}
			,{new DateTime(2000, 1, 1), ("event-stacktrace-non-event"
,@"2018-07-18 14:27:14.453 +01:00 [Information] DasBlog.Web.Controllers.AccountController: SecuritySuccess :: SecuritySuccess logged in successfully :: myemail@myemail.com
System.Exception: Failure in Users Controller
   at DasBlog.Web.Controllers.UsersController.Index(String email) in C:\projects\dasblog-core\source\DasBlog.Web.UI\Controllers\UsersController.cs:line 113
   at lambda_method(Closure , Object , Object[] )
   at Microsoft.Extensions.Internal.ObjectMethodExecutor.Execute(Object target, Object[] parameters)
2018-07-18 09:22:23.333 +01:00 [Error] Microsoft.AspNetCore.Diagnostics.DeveloperExceptionPageMiddleware: An unhandled exception has occurred while executing the request"
					)}
		};

		
		public void Dispose()
		{
			
		}

		public IEnumerable<string> GetEventLines(DateTime date)
		{
			if (date != new DateTime(1, 1, 1))
			{
				foreach (var line in results[date].lines.Split("\n"))
				{
					yield return line;
				}
			}
		}
	}

	internal class MockEventLineParser : IEventLineParser
	{
		private bool successExpected;
		public MockEventLineParser(bool successExpected)
		{
			this.successExpected = successExpected;
		}
		public (bool, EventDataDisplayItem) Parse(string eventLine)
		{
			if (successExpected)
			{
				return (true, new EventDataDisplayItem(EventCodes.Error, "stuff", DateTime.Now));
			}
			else
			{
				return (false, null);
			}
		}
	}
}
