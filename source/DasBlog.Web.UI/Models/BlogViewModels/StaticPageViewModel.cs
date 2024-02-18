using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;

namespace DasBlog.Web.Models.BlogViewModels
{
	public class StaticPageViewModel
	{
		public string Name {get; set; }
		
		[DataType(DataType.MultilineText)]
		public string Content { get; set; }

	}
}
