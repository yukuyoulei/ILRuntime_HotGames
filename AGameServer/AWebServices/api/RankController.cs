using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace AWebServices.api
{
	[RoutePrefix("api/Rank")]
	public class RankController : ApiController
	{
		[HttpGet]
		public HttpResponseMessage Get()
		{
			return ResultToJson.GetErrorJsonResponse("0");
		}
		[HttpGet]
		public HttpResponseMessage GetTopLevel(string count)
		{
			var icount = typeParser.intParse(count);
			if (icount > 10 || icount < 1)
				icount = 10;
			var ttop = ADatabaseConfigsManager.avatarDB.FindManyData(ADatabaseConfigsManager.tAvatarData, ADBAccessor.filter_Gte(InfoNameDefs.AvatarLevel, 0), ADBAccessor.projections(InfoNameDefs.AvatarName, InfoNameDefs.AvatarLevel), icount, 0, ADBAccessor.sort_Descending(InfoNameDefs.AvatarLevel, InfoNameDefs.CurExp, InfoNameDefs.AvatarMoney));
			List<List<string>> lresults = new List<List<string>>();
			foreach (var t in ttop)
			{
				if (t.Contains(InfoNameDefs.AvatarLevel) && t.Contains(InfoNameDefs.AvatarName))
				{
					var l = new List<string>();
					l.Add(InfoNameDefs.AvatarName);
					l.Add(t[InfoNameDefs.AvatarName].ToString());
					l.Add("value");
					l.Add(t[InfoNameDefs.AvatarLevel].ToString());
					lresults.Add(l);
				}
			}
			return ResultToJson.GetJsonResponse(ResultToJson.JsonFormatArray("r", lresults));
		}
		[HttpGet]
		public HttpResponseMessage GetTopGold(string count)
		{
			var icount = typeParser.intParse(count);
			if (icount > 10 || icount < 1)
				icount = 10;
			var ttop = ADatabaseConfigsManager.avatarDB.FindManyData(ADatabaseConfigsManager.tAvatarData, ADBAccessor.filter_Gte(InfoNameDefs.AvatarLevel, 0), ADBAccessor.projections(InfoNameDefs.AvatarName, InfoNameDefs.AvatarGold), icount, 0, ADBAccessor.sort_Descending(InfoNameDefs.AvatarLevel, InfoNameDefs.CurExp, InfoNameDefs.AvatarGold));
			List<List<string>> lresults = new List<List<string>>();
			foreach (var t in ttop)
			{
				if (t.Contains(InfoNameDefs.AvatarGold) && t.Contains(InfoNameDefs.AvatarName))
				{
					var l = new List<string>();
					l.Add(InfoNameDefs.AvatarName);
					l.Add(t[InfoNameDefs.AvatarName].ToString());
					l.Add("value");
					l.Add(t[InfoNameDefs.AvatarGold].ToString());
					lresults.Add(l);
				}
			}
			return ResultToJson.GetJsonResponse(ResultToJson.JsonFormatArray("r", lresults));
		}
		[HttpGet]
		public HttpResponseMessage Register(string username, string password, string mail)
		{
			if (!mail.Contains("@") || !mail.Contains(".") || mail.IndexOf("@") > mail.IndexOf("."))
			{
				return ResultToJson.GetErrorJsonResponse("Invalid mail address.");
			}

			if (username.Length < 4)
			{
				return ResultToJson.GetErrorJsonResponse("Too short username.");
			}
			if (username.Length > 16)
			{
				return ResultToJson.GetErrorJsonResponse("Too long username.");
			}

			var res = ADatabaseConfigsManager.userDB.FindOneData(ADatabaseConfigsManager.tUserData, ADBAccessor.filter_eq(InfoNameDefs.Username, username));
			if (res != null)
			{
				return ResultToJson.GetErrorJsonResponse("Username registered.");
			}
			var token = AWebServerUtils.GetEncryptCode(12);
			ADatabaseConfigsManager.userDB.UpdateOneData(ADatabaseConfigsManager.tUserData, ADBAccessor.filter_eq(InfoNameDefs.Username, username)
				, ADBAccessor.updates_build(
					ADBAccessor.update(InfoNameDefs.UserToken, token)
					, ADBAccessor.update(InfoNameDefs.Username, username)
					, ADBAccessor.update(InfoNameDefs.UserMail, mail)
					, ADBAccessor.update(InfoNameDefs.UserPassword, MD5String.Hash32(password))
				)
				, true);

			ATokenManager.Instance.OnSetToken(username, token);
			return ResultToJson.GetJsonResponse(InfoNameDefs.Username, username, InfoNameDefs.UserToken, token);
		}
		[HttpGet]
		public HttpResponseMessage CheckToken(string username, string token)
		{
			var eCheck = ATokenManager.Instance.OnCheckToken(username, token);
			if (!eCheck)
			{
				return ResultToJson.GetErrorJsonResponse("Invalid token.");
			}
			return ResultToJson.GetJsonResponse(InfoNameDefs.UserToken, token);
		}
		[HttpGet]
		public HttpResponseMessage Logout(string username, string token)
		{
			var eCheck = ATokenManager.Instance.OnCheckToken(username, token);
			if (!eCheck)
			{
				return ResultToJson.GetErrorJsonResponse("Invalid token.");
			}

			ATokenManager.Instance.OnRemoveToken(username);
			ADatabaseConfigsManager.userDB.UpdateOneData(ADatabaseConfigsManager.tUserData, ADBAccessor.filter_eq(InfoNameDefs.Username, username), ADBAccessor.update_unset(InfoNameDefs.UserToken));
			return ResultToJson.GetErrorJsonResponse();
		}
	}
}