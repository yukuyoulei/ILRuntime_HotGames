using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace AWebServices.api
{
	[RoutePrefix("api/Login")]
	public class LoginController : ApiController
	{
		[HttpGet]
		public HttpResponseMessage Get()
		{
			return ResultToJson.GetErrorJsonResponse("0");
		}
		[HttpGet]
		public HttpResponseMessage Login(string username, string password)
		{
			var res = ADatabaseConfigsManager.userDB.FindOneData(ADatabaseConfigsManager.tUserData, ADBAccessor.filter_eq(InfoNameDefs.Username, username) & ADBAccessor.filter_eq(InfoNameDefs.UserPassword, MD5String.Hash32(password)));
			if (res == null)
			{
				return ResultToJson.GetErrorJsonResponse("Username and password mismatch.");
			}
			var token = "";
			if (res.Contains(InfoNameDefs.UserToken))
			{
				token = res[InfoNameDefs.UserToken].ToString();
			}
			else
			{
				token = AWebServerUtils.GetEncryptCode(12);
				ADatabaseConfigsManager.userDB.UpdateOneData(ADatabaseConfigsManager.tUserData, ADBAccessor.filter_eq(InfoNameDefs.Username, username), ADBAccessor.update(InfoNameDefs.UserToken, token));
			}
			return ResultToJson.GetJsonResponse(InfoNameDefs.UserToken, res[InfoNameDefs.UserToken].ToString());
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