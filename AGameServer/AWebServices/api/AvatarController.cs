using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace AWebServices.api
{
	[RoutePrefix("api/Avatar")]
	public class AvatarController : ApiController
	{

		ADBAccessor db
		{
			get
			{
				return ADBManager.Get(ADatabaseConfigsManager.avatarDBConnect, ADatabaseConfigsManager.avatarDBName);
			}
		}
		[HttpGet]
		public HttpResponseMessage Get()
		{
			return ResultToJson.GetErrorJsonResponse("0");
		}
		[HttpGet]
		public HttpResponseMessage SelectAvatar(string username, string token)
		{
			if (!ATokenManager.Instance.OnCheckToken(username, token))
			{
				return ResultToJson.GetErrorJsonResponse("Invalid token.");
			}
			var avatar = AAvatarManager.Instance.OnGetAvatar(username);
			if (avatar == null)
			{
				return ResultToJson.GetErrorJsonResponse(ErrorDefs.NoAvatar);
			}
			return avatar.GetDiryParamResponse();
		}
		[HttpGet]
		public HttpResponseMessage CreateAvatar(string username, string token, string avatarname, string sex)
		{
			var check = ATokenManager.Instance.OnCheckToken(username, token);
			if (!check)
			{
				return ResultToJson.GetErrorJsonResponse(ErrorDefs.InvalidToken);
			}
			avatarname = CommonUtil.filtername(avatarname);
			var len = CommonUtil.GetStringLength(avatarname);
			if (len <= 2 || len > 12)
			{
				return ResultToJson.GetErrorJsonResponse(ErrorDefs.AvatarNameInvalidLength);
			}
			var isex = typeParser.intParse(sex);
			if (isex != 0 && isex != 1)
			{
				return ResultToJson.GetErrorJsonResponse(ErrorDefs.InvalidSex);
			}
			{
				var a = AAvatarManager.Instance.OnGetAvatar(username);
				if (a != null)
				{
					return ResultToJson.GetErrorJsonResponse(ErrorDefs.AlreadyHasAvatar);
				}
			}
			var findRes = ADatabaseConfigsManager.avatarDB.FindOneData(ADatabaseConfigsManager.tAvatarData, ADBAccessor.filter_eq(InfoNameDefs.AvatarName, avatarname));
			if (findRes != null && findRes.Contains(InfoNameDefs.AvatarName))
			{
				return ResultToJson.GetErrorJsonResponse(ErrorDefs.DuplicateAvatarName);
			}
			else
			{
				var avatar = AAvatarManager.Instance.OnCreateAvatar(username, avatarname, isex);
				if (avatar != null)
					return avatar.GetDiryParamResponse();
				return ResultToJson.GetErrorJsonResponse(ErrorDefs.DBError);
			}
		}
		[HttpGet]
		public HttpResponseMessage DailyCheck(string username, string token)
		{
			if (!ATokenManager.Instance.OnCheckToken(username, token))
			{
				return ResultToJson.GetErrorJsonResponse("Invalid token.");
			}
			var avatar = AAvatarManager.Instance.OnGetAvatar(username);
			if (avatar == null)
			{
				return ResultToJson.GetErrorJsonResponse(ErrorDefs.NoAvatar);
			}
			return avatar.OnDailyCheck();
		}
		[HttpGet]
		public HttpResponseMessage CaiDaXiao(string username, string token, int multi, int isBig)
		{
			if (!ATokenManager.Instance.OnCheckToken(username, token))
			{
				return ResultToJson.GetErrorJsonResponse("Invalid token.");
			}
			var avatar = AAvatarManager.Instance.OnGetAvatar(username);
			if (avatar == null)
			{
				return ResultToJson.GetErrorJsonResponse(ErrorDefs.NoAvatar);
			}

			return avatar.OnCaiDaXiao(multi, isBig);
		}
	}
}