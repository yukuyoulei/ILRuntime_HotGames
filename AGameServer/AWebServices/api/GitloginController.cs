using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace AWebServices.api
{
	[RoutePrefix("api/gitlogin")]
	public class GitloginController : ApiController
	{
		[HttpGet]
		public HttpResponseMessage Get(string code)
		{
			return ResultToJson.GetErrorJsonResponse("0");
		}
	}
}