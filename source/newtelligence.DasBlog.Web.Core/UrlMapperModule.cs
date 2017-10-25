#region Copyright (c) 2003, newtelligence AG. All rights reserved.
/*
// Copyright (c) 2003, newtelligence AG. (http://www.newtelligence.com)
// Original BlogX Source Code: Copyright (c) 2003, Chris Anderson (http://simplegeek.com)
// All rights reserved.
//  
// Redistribution and use in source and binary forms, with or without modification, are permitted 
// provided that the following conditions are met: 
//  
// (1) Redistributions of source code must retain the above copyright notice, this list of 
// conditions and the following disclaimer. 
// (2) Redistributions in binary form must reproduce the above copyright notice, this list of 
// conditions and the following disclaimer in the documentation and/or other materials 
// provided with the distribution. 
// (3) Neither the name of the newtelligence AG nor the names of its contributors may be used 
// to endorse or promote products derived from this software without specific prior 
// written permission.
//      
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS 
// OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY 
// AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL 
// DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, 
// DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER 
// IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT 
// OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
// -------------------------------------------------------------------------
//
// Original BlogX source code (c) 2003 by Chris Anderson (http://simplegeek.com)
// 
// newtelligence is a registered trademark of newtelligence Aktiengesellschaft.
// 
// For portions of this software, the some additional copyright notices may apply 
// which can either be found in the license.txt file included in the source distribution
// or following this notice. 
//
*/
#endregion

namespace newtelligence.DasBlog.Web.Core
{
    using System;
    using System.Collections.Specialized;
    using System.Configuration;
    using System.Text.RegularExpressions;
    using System.Web;


	// reqister this as "newtelligence.DasBlog.UrlMapper"
    public class UrlMapperModuleSectionHandler :
		NameValueSectionHandler
	{
		public static string ConfigSectionName
		{
			get
			{
				return "newtelligence.DasBlog.UrlMapper";
			}
		}

		public UrlMapperModuleSectionHandler()
		{
		
		}

		protected override string KeyAttributeName
		{
			get { return "matchExpression"; }
		}

		protected override string ValueAttributeName
		{
			get { return "mapTo"; }
		}
	}

	public class UrlMapperModule : IHttpModule
	{
		private EventHandler onBeginRequest;
		
		public UrlMapperModule()
		{
			onBeginRequest = new EventHandler(this.HandleBeginRequest);
		}

		void IHttpModule.Dispose()
		{
		}

		void IHttpModule.Init(HttpApplication context)
		{
			context.BeginRequest += onBeginRequest;
		}

	
		private void HandleBeginRequest( object sender, EventArgs evargs )
		{
			HttpApplication app = sender as HttpApplication;

            DataCache cache = CacheFactory.GetCache();

            // need to exclude deleteItem.ashx from URL Rewriting so we can find the permalink for other dasBlog sites
			if ( app != null && app.Context.Request.Url.LocalPath.ToLower().EndsWith("deleteitem.ashx") == false)
			{
				// changed this to support 'c#' as a category name
				Uri requestUri = app.Context.Request.Url;
				string requestUrl = app.Context.Request.Path + (requestUri.Query != null && requestUri.Query.Length > 0 ? requestUri.Query : "");

                // bypass the mapping use the cached mapping
                string mapToFromCache=cache[requestUrl] as string;
                if( mapToFromCache != null && mapToFromCache.Length > 0){
                    app.Context.RewritePath(mapToFromCache);
                    return;
                }

                // no cached mapping proceed as usual
                NameValueCollection urlMaps = (NameValueCollection)ConfigurationManager.GetSection(UrlMapperModuleSectionHandler.ConfigSectionName);
				if ( urlMaps != null )
				{
					for ( int loop=0;loop<urlMaps.Count;loop++)
					{
						string matchExpression = urlMaps.GetKey(loop);
						Regex regExpression = new Regex(matchExpression,RegexOptions.IgnoreCase|RegexOptions.Singleline|RegexOptions.CultureInvariant|RegexOptions.Compiled);
						Match matchUrl = regExpression.Match(requestUrl);
						if ( matchUrl != null && matchUrl.Success )
						{
							bool isCategoryQuery = false;

							string mapTo = urlMaps[matchExpression];
//							Regex regMap = new Regex("\\{(?<expr>\\w+)\\}");
							foreach( Match matchExpr in regMap.Matches(mapTo) )
							{
								Group urlExpr;
								string expr = matchExpr.Groups["expr"].Value;
								urlExpr = matchUrl.Groups[expr];
								if ( urlExpr != null )
								{
									if (urlExpr.Value == "category")
									{
										isCategoryQuery = true;
									}
									if (expr == "value" & isCategoryQuery)
									{
										string queryParams = urlExpr.Value;
										
										// OmarS: hacking the category expression to support a page parameter.
										if (queryParams.IndexOf(",") != -1)
										{
											int pos = queryParams.LastIndexOf(",");
											string pageNumber = queryParams.Substring(queryParams.LastIndexOf(",") + 1, queryParams.Length - ( pos + 1));
											
											string categoryList = "";

											if (Char.IsDigit(pageNumber, 0))
											{
												categoryList = queryParams.Substring(0, pos);
											}
											else
											{
												categoryList = queryParams;
											}
											
											categoryList = categoryList.Replace(",", "|");

                                            mapTo = mapTo.Replace("{" + expr + "}", app.Server.UrlEncode(categoryList));
											mapTo = mapTo + "&page=" + pageNumber;
										}
										else
										{
											string categoryList = urlExpr.Value.Replace(",", "|");
											mapTo = mapTo.Replace("{"+expr+"}", categoryList);
										}
									}
									else
									{
										mapTo = mapTo.Replace("{"+expr+"}", urlExpr.Value);
									}
								}
							}
				            
                            // micro cache the mapped url
                            cache.Insert(requestUrl, mapTo, new TimeSpan(0, 0, 0, 5));

                            app.Context.RewritePath(mapTo);
							break;
						}
					}
				}
			}
		}

        private readonly static Regex regMap = new Regex("\\{(?<expr>\\w+)\\}", RegexOptions.Compiled);
	}
}



