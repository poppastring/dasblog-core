using System;
using System.Diagnostics;
using AutoMapper;
using DasBlog.Managers.Interfaces;
using DasBlog.Services;
using DasBlog.Services.Site;
using DasBlog.Web.Controllers;
using DasBlog.Web.Models;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace DasBlog.Tests.UnitTests.Controllers
{
	public class HomeControllerTest
	{
		[Fact]
		public void Error_WithExceptionFeature_LogsOriginalExceptionPathAndTraceIdentifier()
		{
			var previous = Activity.Current;
			try
			{
				Activity.Current = null;
				var logger = new RegressionLogger<HomeController>();
				var exception = new InvalidOperationException("Rendering failed");
				var controller = Create(logger, exception);
				AssertError(controller, logger, exception, "trace-error-123");
			}
			finally { Activity.Current = previous; }
		}

		[Fact]
		public void Error_WithCurrentActivity_PrefersActivityIdInLogAndModel()
		{
			var previous = Activity.Current;
			try
			{
				using var activity = new Activity("home-error-regression").Start();
				var logger = new RegressionLogger<HomeController>();
				var exception = new InvalidOperationException("Activity render failed");
				var controller = Create(logger, exception);
				Assert.NotNull(activity.Id);
				Assert.NotEqual(controller.HttpContext.TraceIdentifier, activity.Id);
				AssertError(controller, logger, exception, activity.Id);
			}
			finally { Activity.Current = previous; }
		}

		[Fact]
		public void Error_WithoutExceptionFeature_ReturnsTraceIdentifierWithoutUnhandledErrorLog()
		{
			var previous = Activity.Current;
			try
			{
				Activity.Current = null;
				var logger = new RegressionLogger<HomeController>();
				var controller = Create(logger, null);
				var view = Assert.IsType<ViewResult>(controller.Error());
				var model = Assert.IsType<ErrorViewModel>(view.Model);
				Assert.Equal("trace-error-123", model.RequestId);
				Assert.True(model.ShowRequestId);
				Assert.Empty(logger.Entries);
			}
			finally { Activity.Current = previous; }
		}

		private static void AssertError(HomeController controller, RegressionLogger<HomeController> logger, Exception exception, string requestId)
		{
			var view = Assert.IsType<ViewResult>(controller.Error());
			var model = Assert.IsType<ErrorViewModel>(view.Model);
			Assert.Equal(requestId, model.RequestId);
			Assert.True(model.ShowRequestId);
			var entry = Assert.Single(logger.Entries);
			Assert.Equal(LogLevel.Error, entry.Level);
			Assert.Same(exception, entry.Exception);
			Assert.Equal("/post/broken-render", entry.State["Path"]);
			Assert.Equal(requestId, entry.State["RequestId"]);
			Assert.Equal("Unhandled request error at {Path}; request ID {RequestId}", entry.State["{OriginalFormat}"]);
		}

		private static HomeController Create(RegressionLogger<HomeController> logger, Exception exception)
		{
			var context = new DefaultHttpContext { TraceIdentifier = "trace-error-123" };
			if (exception != null)
				context.Features.Set<IExceptionHandlerPathFeature>(new ExceptionHandlerFeature { Error = exception, Path = "/post/broken-render" });
			return new HomeController(Mock.Of<IBlogManager>(), Mock.Of<ICommentManager>(), Mock.Of<IDasBlogSettings>(),
				Mock.Of<IMapper>(), logger, Mock.Of<IMemoryCache>(), Mock.Of<IExternalEmbeddingHandler>())
			{
				ControllerContext = new ControllerContext { HttpContext = context }
			};
		}
	}
}
