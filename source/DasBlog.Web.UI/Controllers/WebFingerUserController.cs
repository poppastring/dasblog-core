using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Encodings.Web;
using System.Text.Json;
using AutoMapper;
using DasBlog.Managers;
using DasBlog.Managers.Interfaces;
using DasBlog.Services;
using DasBlog.Services.XmlRpc.MoveableType;
using DasBlog.Web.Models.ActivityPubModels;
using DasBlog.Web.Models.BlogViewModels;
using DasBlog.Web.Settings;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Quartz.Util;

namespace DasBlog.Web.Controllers
{
	public class WebFingerUserController : DasBlogBaseController
	{
		private readonly IDasBlogSettings dasBlogSettings;
		private readonly IActivityPubManager activityPubManager;
		private readonly IMapper mapper;

		public WebFingerUserController(IActivityPubManager pubManager, IDasBlogSettings settings, IMapper mapper) : base(settings) 
		{
			this.dasBlogSettings = settings;
			this.activityPubManager = pubManager;
			this.mapper = mapper;
		}

		[Produces("text/json")]
		[HttpGet("/.well-known/webfinger")]
		public ActionResult WebFinger(string resource)
		{	
			var webfinger = activityPubManager.WebFinger(resource);
			if (webfinger != null)
			{
				var wfvm = mapper.Map<WebFingerViewModel>(webfinger);
				wfvm.links = webfinger.Links.Select(entry => mapper.Map<WebFingerLinkViewModel>(entry)).ToList();
				wfvm.aliases = webfinger.Aliases.Select(entry => mapper.Map<string>(entry)).ToList();

				return Json(wfvm, jsonSerializerOptions);
			}

			return NoContent();
		}
	}
}
