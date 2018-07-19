using System;
using System.Collections.Generic;
using System.Text;
using Castle.Components.DictionaryAdapter;
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
			ActivityService service = new ActivityService(factory, parser);
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
			IEventLineParser parser = new MockEventLineParser(false);
			ActivityService service = new ActivityService(factory, parser);
			int ctr = 0;
			foreach (var eddi in service.GetEventsForDay(new DateTime(1954, 1, 25)))
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
			ActivityService service = new ActivityService(factory, parser);
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
			ActivityService service = new ActivityService(factory, parser);
			int ctr = 0;
			foreach (var eddi in service.GetEventsForDay(new DateTime(1951, 12, 8)))
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
			,{new DateTime(1951, 12, 8), ("non-event-stacktrace"
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
		};

		
		public void Dispose()
		{
			
		}

		public IEnumerable<string> GetEventLines(DateTime date)
		{
			foreach (var line in results[date].lines.Split("\n"))
			{
				yield return line;
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

namespace DasBlog.Core.Services.Interfaces
{
	public interface IActivityRepoFactory
	{
		IActivityRepo GetRepo();
	}
	public interface IActivityRepo : IDisposable
	{
		IEnumerable<String> GetEventLines(DateTime date);
	}
}

namespace DasBlog.Core.Services
{
	public class ActivityService : IActivityService
	{
		private IActivityRepoFactory repoFactory;
		private IEventLineParser parser;
		public ActivityService(IActivityRepoFactory repoFactory, IEventLineParser parser)
		{
			this.repoFactory = repoFactory;
			this.parser = parser;
		}
		public List<EventDataDisplayItem> GetEventsForDay(DateTime date)
		{
			List<EventDataDisplayItem> events = new EditableList<EventDataDisplayItem>();
			bool isEvent = false;
			StringBuilder stackTrace = new StringBuilder();
			// add in the stacktrace to the last event
			void UpdatePreviousEvent()
			{
				EventDataDisplayItem existingEddi = events[events.Count - 1];

				var completeEddi = new EventDataDisplayItem(
					existingEddi.EventCode
					, existingEddi.HtmlMessage + stackTrace
					, existingEddi.Date);
				events[events.Count -1] = completeEddi;
				stackTrace.Clear();
			}
			using (var repo = repoFactory.GetRepo())
			{
				foreach (var line in repo.GetEventLines(date))
				{
					char[] chars = line.ToCharArray();
					if (chars.Length > 0 && !Char.IsDigit(chars[0])) goto stack_trace;
					(bool success, EventDataDisplayItem eddi) = parser.Parse(line);
					if (success) goto event_line;
					goto non_event_line;
					event_line:
						if (isEvent)	// previous event still in progress
						{
							UpdatePreviousEvent();
						}
						events.Add(eddi);
						isEvent = true;
						continue;
					non_event_line:
						if (isEvent)	// previous event still in progress
						{
							UpdatePreviousEvent();
						}
						isEvent = false;
						continue;
					stack_trace:
						if (isEvent)
						{
							stackTrace.Append(line);
						}
						continue;
				}
			}

			if (isEvent)
			{
				UpdatePreviousEvent();
			}
			return events;
		}
	}
}
