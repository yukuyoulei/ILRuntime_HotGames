using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace AWebServices.api
{
	[RoutePrefix("api/Question")]
	public class QuestionController : ApiController
	{
		[HttpGet]
		public HttpResponseMessage Get()
		{
			return ResultToJson.GetErrorJsonResponse("0");
		}

		[HttpGet]
		public HttpResponseMessage GetOne(string username, string token)
		{
			var eCheck = ATokenManager.Instance.OnCheckToken(username, token);
			if (!eCheck)
			{
				return ResultToJson.GetErrorJsonResponse("Invalid token.");
			}
			var avatar = AAvatarManager.Instance.OnGetAvatar(username);
			if (avatar == null)
			{
				return ResultToJson.GetErrorJsonResponse("No avatar.");
			}
			var q = avatar.OnGetOneQuestion();
			return ResultToJson.GetJsonResponse("q", q);
		}
		[HttpGet]
		public HttpResponseMessage Answer(string username, string token, string answer)
		{
			var eCheck = ATokenManager.Instance.OnCheckToken(username, token);
			if (!eCheck)
			{
				return ResultToJson.GetErrorJsonResponse("Invalid token.");
			}
			var avatar = AAvatarManager.Instance.OnGetAvatar(username);
			if (avatar == null)
			{
				return ResultToJson.GetErrorJsonResponse("No avatar.");
			}
			var res = avatar.OnAnswer(answer);
			if (res)
			{
				return ResultToJson.GetJsonResponse("avatar", ResultToJson.JsonFormat(avatar.GetDirtyParams()));
			}
			return ResultToJson.GetErrorJsonResponse("wrong");
		}
	}
}