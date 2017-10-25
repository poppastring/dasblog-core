#region Copyright (c) 2003, newtelligence AG. All rights reserved.
/*
// Copyright (c) 2003, newtelligence AG. (http://www.newtelligence.com)
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
//
*/
#endregion

using System;
using System.Web;
using System.Configuration;
using System.Text;
using System.Collections.Specialized;

namespace newtelligence.DasBlog.Web.Core.WebControls
{
	

	public class ControlImageModuleSectionHandler :
		NameValueSectionHandler
	{
		public static string ConfigSectionName
		{
			get
			{
				return "newtelligence.ControlImages";
			}
		}

		public ControlImageModuleSectionHandler()
		{
			
		}

		protected override string KeyAttributeName
		{
			get { return "type"; }
		}

		protected override string ValueAttributeName
		{
			get { return "name"; }
		}
	}

	public class ControlImageModule : IHttpModule
	{
		private EventHandler onBeginRequest;
		private const string imageVirtualFile = "_ctrlimg.ashx";

		public ControlImageModule()
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
			
			if ( app != null )
			{
				
				string requestUrl = app.Context.Request.Url.AbsoluteUri;
				if ( requestUrl.EndsWith("/"+imageVirtualFile) )
				{
					try
					{
						string[] parts = requestUrl.Split('/');
						if ( parts.Length >= 2 && 
							parts[parts.Length-1] == imageVirtualFile )
						{
							Type classType;
							string className;
							IRenderControlImage imageRenderer;
							string classPart = parts[parts.Length-2];
							string[] classElements = classPart.Split('!');
							string classAlias = className = classElements[0];
							string[] args = new string[classElements.Length-1];
							
							NameValueCollection nvc = (NameValueCollection)System.Configuration.ConfigurationManager.GetSection(ControlImageModuleSectionHandler.ConfigSectionName);
							if ( nvc != null )
							{
								for ( int loop=0;loop<nvc.Count;loop++)
								{
									if ( nvc.Get(loop) == classAlias )
									{
										className = nvc.Keys[loop];
										break;
									}
								}
							}
							
							for(int loop=0;loop<classElements.Length-1;loop++)
							{
								args[loop] = classElements[loop+1];
							}
							
							classType = Type.GetType(className);
							imageRenderer = (IRenderControlImage)Activator.CreateInstance(classType);
							imageRenderer.Render(app.Context,args);
						}
						app.Context.Response.End();
					}
					catch{}
				}
				
			}
		}

		public static void BreakImageHRef( HttpContext Context, string url, out Type ControlType, out string[] Args )
		{
			ControlType = null;
			Args = null;
		}

		public static string GetImageHRef( HttpContext Context, Type ControlType, string[] Args )
		{
			if ( Context != null )
			{
				Uri imageUri;
				Uri baseUri = Context.Request.Url;
				StringBuilder imagePath = new StringBuilder();
				string typeName;
				
				typeName = ControlType.AssemblyQualifiedName.Replace(" ","");
				NameValueCollection nvc = (NameValueCollection)System.Configuration.ConfigurationManager.GetSection(ControlImageModuleSectionHandler.ConfigSectionName);
				if ( nvc != null )
				{
					for ( int loop=0;loop<nvc.Keys.Count;loop++)
					{
						if ( ControlType == Type.GetType(nvc.Keys[loop],false,true) )
						{
							typeName = nvc.Get(loop);
							break;
						}
					}
				}

				imagePath.Append(typeName);
				if ( Args != null )
				{
					for(int loop=0;loop<Args.Length;loop++)
					{
						imagePath.Append("!").Append(Args[loop]);
					}
				}
				imagePath.Append("/").Append(imageVirtualFile);
				imageUri = new Uri(baseUri,imagePath.ToString());
				return baseUri.MakeRelativeUri(imageUri).ToString();
			}
			else
			{
				return "unknown";
			}
		} 
	}

	public interface IRenderControlImage
	{
		void Render( HttpContext Context, string[] Args);
	}
}
