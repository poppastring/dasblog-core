using System.Collections.Generic;
using System.Threading.Tasks;
using DasBlog.Services;
using DasBlog.Services.ConfigFile;
using DasBlog.Web.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Moq;
using Xunit;

namespace DasBlog.Tests.UnitTests.Controllers
{
	public class RequireBlogFeaturesFilterTests
	{
		[Fact]
		[Trait("Category", "UnitTest")]
		public async Task OnActionExecutionAsync_BlogFeaturesDisabled_ReturnsNotFoundWithoutExecutingAction()
		{
			var filter = CreateFilter(false);
			var context = CreateContext();
			var actionExecuted = false;

			await filter.OnActionExecutionAsync(context, () =>
			{
				actionExecuted = true;
				return Task.FromResult<ActionExecutedContext>(null);
			});

			Assert.IsType<Microsoft.AspNetCore.Mvc.NotFoundResult>(context.Result);
			Assert.False(actionExecuted);
		}

		[Fact]
		[Trait("Category", "UnitTest")]
		public async Task OnActionExecutionAsync_BlogFeaturesEnabled_ExecutesAction()
		{
			var filter = CreateFilter(true);
			var context = CreateContext();
			var actionExecuted = false;

			await filter.OnActionExecutionAsync(context, () =>
			{
				actionExecuted = true;
				return Task.FromResult<ActionExecutedContext>(null);
			});

			Assert.Null(context.Result);
			Assert.True(actionExecuted);
		}

		private static RequireBlogFeaturesFilter CreateFilter(bool enableBlogFeatures)
		{
			var settings = new Mock<IDasBlogSettings>();
			settings.SetupGet(value => value.SiteConfiguration).Returns(new SiteConfig
			{
				EnableBlogFeatures = enableBlogFeatures
			});
			return new RequireBlogFeaturesFilter(settings.Object);
		}

		private static ActionExecutingContext CreateContext()
		{
			var actionContext = new ActionContext(new DefaultHttpContext(), new RouteData(), new ActionDescriptor());
			return new ActionExecutingContext(actionContext, new List<IFilterMetadata>(), new Dictionary<string, object>(), new object());
		}
	}
}
