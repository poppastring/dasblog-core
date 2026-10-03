using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using DasBlog.Services;
using DasBlog.Services.ConfigFile;
using DasBlog.Web.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace DasBlog.Tests.UnitTests.Controllers
{
	public class RequireBlogFeaturesFilterTests
	{
		[Fact]
		[Trait("Category", "UnitTest")]
		public async Task OnActionExecutionAsync_BlogFeaturesDisabled_AuthenticatedWithoutOverride_ReturnsNotFoundWithoutExecutingAction()
		{
			var filter = CreateFilter(false);
			var context = CreateContext();
			context.HttpContext.User = new ClaimsPrincipal(new ClaimsIdentity(new[]
			{
				new Claim(ClaimTypes.Name, "admin")
			}, "Test"));
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

		[Fact]
		[Trait("Category", "UnitTest")]
		public async Task OnActionExecutionAsync_BlogFeaturesDisabled_AuthenticatedWhenAllowed_ExecutesAction()
		{
			var filter = CreateFilter(false, allowAuthenticatedWhenDisabled: true);
			var context = CreateContext();
			context.HttpContext.User = new ClaimsPrincipal(new ClaimsIdentity(new[]
			{
				new Claim(ClaimTypes.Name, "admin")
			}, "Test"));
			var actionExecuted = false;

			await filter.OnActionExecutionAsync(context, () =>
			{
				actionExecuted = true;
				return Task.FromResult<ActionExecutedContext>(null);
			});

			Assert.Null(context.Result);
			Assert.True(actionExecuted);
		}

		[Fact]
		[Trait("Category", "UnitTest")]
		public async Task OnActionExecutionAsync_BlogFeaturesDisabled_AnonymousWhenAllowed_ReturnsNotFoundWithoutExecutingAction()
		{
			var filter = CreateFilter(false, allowAuthenticatedWhenDisabled: true);
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

		private static RequireBlogFeaturesFilter CreateFilter(bool enableBlogFeatures, bool allowAuthenticatedWhenDisabled = false)
		{
			var settings = new Mock<IDasBlogSettings>();
			settings.SetupGet(value => value.SiteConfiguration).Returns(new SiteConfig
			{
				EnableBlogFeatures = enableBlogFeatures
			});

			var services = new ServiceCollection();
			services.AddSingleton(settings.Object);
			var attribute = new RequireBlogFeaturesAttribute(allowAuthenticatedWhenDisabled);
			using var serviceProvider = services.BuildServiceProvider();
			return (RequireBlogFeaturesFilter)attribute.CreateInstance(serviceProvider);
		}

		private static ActionExecutingContext CreateContext()
		{
			var actionContext = new ActionContext(new DefaultHttpContext(), new RouteData(), new ActionDescriptor());
			return new ActionExecutingContext(actionContext, new List<IFilterMetadata>(), new Dictionary<string, object>(), new object());
		}
	}
}
