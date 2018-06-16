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
	[HtmlTargetElement(Attributes = "dasblog-unauthorized")]
	public class UnAuthorizedRoleTagHelper : TagHelper, IAuthorizeData
	{
		private readonly IPolicyEvaluator _policyEvaluator;
		private readonly IAuthorizationPolicyProvider _authPolicyProvider;
		private readonly IHttpContextAccessor _httpContextAccessor;

		public UnAuthorizedRoleTagHelper(IHttpContextAccessor httpContextAccessor, IAuthorizationPolicyProvider policyProvider, IPolicyEvaluator policyEvaluator)
		{
			_httpContextAccessor = httpContextAccessor;
			_authPolicyProvider = policyProvider;
			_policyEvaluator = policyEvaluator;
		}

		public string Policy { get; set; }

		public string Roles { get; set; }

		public string AuthenticationSchemes { get; set; }

		public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
		{
			var policy = await AuthorizationPolicy.CombineAsync(_authPolicyProvider, new[] { this });
			var authenticateResult = await _policyEvaluator.AuthenticateAsync(policy, _httpContextAccessor.HttpContext);
			var authorizeResult = await _policyEvaluator.AuthorizeAsync(policy, authenticateResult, _httpContextAccessor.HttpContext, null);

			if (authorizeResult.Succeeded)
			{
				output.SuppressOutput();
			}
		}
	}
}
