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
		public RequireBlogFeaturesAttribute()
			: base(typeof(RequireBlogFeaturesFilter))
		{
		}
	}

	public sealed class RequireBlogFeaturesFilter : IAsyncActionFilter
	{
		private readonly IDasBlogSettings dasBlogSettings;

		public RequireBlogFeaturesFilter(IDasBlogSettings dasBlogSettings)
		{
			this.dasBlogSettings = dasBlogSettings;
		}

		public Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
		{
			if (!dasBlogSettings.SiteConfiguration.EnableBlogFeatures)
			{
				context.Result = new NotFoundResult();
				return Task.CompletedTask;
			}

			return next();
		}
	}
}
