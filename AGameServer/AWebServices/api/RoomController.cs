using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace AWebServices.api
{
	[RoutePrefix("api/Room")]
	public class RoomController : ApiController
	{
		[HttpGet]
		public HttpResponseMessage Count()
		{
			return ResultToJson.ToNormalResponse(ARoomManager.Instance.Count.ToString());
		}
	}
}