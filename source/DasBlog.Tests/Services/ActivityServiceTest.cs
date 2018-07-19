using System;
using System.Collections.Generic;
using Castle.Components.DictionaryAdapter;
using DasBlog.Core;
using DasBlog.Core.Services;
using DasBlog.Core.Services.Interfaces;
using Xunit;
using Xunit.Sdk;

namespace DasBlog.Tests.Services
{
	public class ActivityServiceTest
	{
		[Fact]
		public void ShoulProcessSingleLine()
		{
			IActivityRepoFactory factory = new MockActivityRepoFactory();
			IEventLineParser parser = new MockEventLineParser();
			ActivityService service = new ActivityService(factory, parser);
			int ctr = 0;
			foreach (var eddi in service.GetEventsForDay(DateTime.Today))
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
	 
		private readonly string[] singleLine = new string[] {
			@"2018-07-18 09:10:08.388 +01:00 [Information] Microsoft.AspNetCore.Authorization.DefaultAuthorizationService: Authorization failed for user: (null)."
		};

		
		public void Dispose()
		{
			
		}

		public IEnumerable<string> GetEventLines(DateTime date)
		{
			foreach (var line in singleLine)
			{
				yield return line;
			}
		}
	}

	internal class MockEventLineParser : IEventLineParser
	{
		public (bool, EventDataDisplayItem) Parse(string eventLine)
		{
			return (true, new EventDataDisplayItem(EventCodes.Error, "stuff", DateTime.Now));
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
			using (var repo = repoFactory.GetRepo())
			{
				foreach (var line in repo.GetEventLines(DateTime.Now))
				{
					events.Add( parser.Parse(line).Item2);
				}
			}

			return events;
		}
	}
}
