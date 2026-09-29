using System;
using System.Threading.Tasks;
using DasBlog.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace DasBlog.Web.Controllers
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
	public sealed class RequireBlogFeaturesAttribute : TypeFilterAttribute
	{
		public RequireBlogFeaturesAttribute(bool allowAuthenticatedWhenDisabled = false)
			: base(typeof(RequireBlogFeaturesFilter))
		{
			Arguments = new object[] { allowAuthenticatedWhenDisabled };
		}
	}

	public sealed class RequireBlogFeaturesFilter : IAsyncActionFilter
	{
		private readonly IDasBlogSettings dasBlogSettings;
		private readonly bool allowAuthenticatedWhenDisabled;

		public RequireBlogFeaturesFilter(IDasBlogSettings dasBlogSettings, bool allowAuthenticatedWhenDisabled = false)
		{
			this.dasBlogSettings = dasBlogSettings;
			this.allowAuthenticatedWhenDisabled = allowAuthenticatedWhenDisabled;
		}

		public Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
		{
			if (!dasBlogSettings.SiteConfiguration.EnableBlogFeatures &&
				!(allowAuthenticatedWhenDisabled && context.HttpContext.User.Identity?.IsAuthenticated == true))
			{
				context.Result = new NotFoundResult();
				return Task.CompletedTask;
			}

			return next();
		}
	}
}
