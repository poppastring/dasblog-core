using System;
using Microsoft.AspNetCore.Mvc.Filters;

namespace DasBlog.Web.Controllers
{
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
	public sealed class NoIndexAttribute : Attribute, IFilterMetadata
	{
	}
}
