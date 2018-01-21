//using CookComputing.XmlRpc;
//using Microsoft.AspNetCore.Builder;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Html;
//using System;
//using System.IO;
//using System.Threading.Tasks;
//using DasBlog.Web.Core.XmlRpc.MoveableType;
//using newtelligence.DasBlog.Runtime;
//using System.Collections.Generic;
//using System.Reflection;
//using DasBlog.Web.Core;
//using DasBlog.Web.Core.Exceptions;
//using DasBlog.Web.Core.Security;
//using DasBlog.Web.Core.XmlRpc;

//namespace DasBlog.Web.UI.Middleware
//{
//    public class XmlRpcMiddleware 
//    {
//        private readonly IDasBlogSettings _dasBlogSettings;

//        public XmlRpcMiddleware(RequestDelegate next, IDasBlogSettings settings)
//        {
//            _dasBlogSettings = settings;
//        }

//        public async Task Invoke(HttpContext context)
//        {
//            try
//            {
//                await HandleHttpRequest(context);
//            }
//            catch (Exception ex)
//            {
//                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
//                // Need to log the problem somewhere????
//            }
//        }

//        public async Task HandleHttpRequest(HttpContext context)
//        {
//            if (context.Request.Method == "GET")
//            {
//                XmlRpcServiceAttribute xmlRpcServiceAttribute = (XmlRpcServiceAttribute)Attribute.GetCustomAttribute(base.GetType(), typeof(XmlRpcServiceAttribute));
//                if (xmlRpcServiceAttribute != null && !xmlRpcServiceAttribute.AutoDocumentation)
//                {
//                    HandleUnsupportedMethod(context);
//                }
//                else
//                {
//                    bool autoDocVersion = true;
//                    if (xmlRpcServiceAttribute != null)
//                    {
//                        autoDocVersion = xmlRpcServiceAttribute.AutoDocVersion;
//                    }
//                    await HandleGET(context, autoDocVersion);
//                }
//            }
//            else if (context.Request.Method != "POST")
//            {
//                HandleUnsupportedMethod(context);
//            }
//            else
//            {
//                Stream src = base.Invoke(context.Request.Body);
//                Stream outputStream = context.Response.Body;
//                Util.CopyStream(src, outputStream);
//                outputStream.Flush();
//                context.Response.ContentType = "text/xml";
//            }
//        }

//        protected async Task HandleGET(HttpContext context, bool autoDocVersion)
//        {
//            // Microsoft.AspNetCore.Html.

//            // HtmlTextWriter wrtr = new HtmlTextWriter(context.Response.Body);
//            // XmlRpcDocWriter.WriteDoc(wrtr, base.GetType(), autoDocVersion);

//            context.Response.StatusCode = StatusCodes.Status200OK;
//        }

//        protected void HandleUnsupportedMethod(HttpContext context)
//        {
//            context.Response.StatusCode = StatusCodes.Status405MethodNotAllowed;
//        }
//    }
//}
