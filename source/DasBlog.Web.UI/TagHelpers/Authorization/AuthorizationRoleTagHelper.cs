using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace DasBlog.Web.TagHelpers
{
	[HtmlTargetElement(Attributes = "dasblog-authorized")]
	[HtmlTargetElement(Attributes = "dasblog-authorized,dasblog-roles")]
	public class AuthorizationRoleTagHelper : TagHelper, IAuthorizeData
	{
		private readonly IPolicyEvaluator policyEvaluator;
		private readonly IAuthorizationPolicyProvider authPolicyProvider;
		private readonly IHttpContextAccessor httpContextAccessor;

		[HtmlAttributeName("dasblog-roles")]
		public string Roles { get; set; }

		public string Policy { get; set; }

		public string AuthenticationSchemes { get; set; }

		public AuthorizationRoleTagHelper(IHttpContextAccessor httpContextAccessor, IAuthorizationPolicyProvider policyProvider, IPolicyEvaluator policyEvaluator)
		{
			this.httpContextAccessor = httpContextAccessor;
			authPolicyProvider = policyProvider;
			this.policyEvaluator = policyEvaluator;
		}

		public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
		{
			var policy = await AuthorizationPolicy.CombineAsync(authPolicyProvider, new[] { this });
			var authenticateResult = await policyEvaluator.AuthenticateAsync(policy, httpContextAccessor.HttpContext);
			var authorizeResult = await policyEvaluator.AuthorizeAsync(policy, authenticateResult, httpContextAccessor.HttpContext, null);
			
			if (!authorizeResult.Succeeded)
			{
				output.SuppressOutput();
			}	
		}
	}
}
