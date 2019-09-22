using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public static class UStaticWebRequests
{
	public static void DoLogin(string username, string md5pwd
		, Action<JObject> onSuccess = null, Action<string> onFail = null, Action<string> onErr = null)
	{
		OnWebRequest("Login/Login", string.Format("username={0}&password={1}", username, md5pwd)
			, onSuccess, onFail, onErr);
	}
	public static void DoRegist(string username, string md5pwd, string mail
		, Action<JObject> onSuccess = null, Action<string> onFail = null, Action<string> onErr = null)
	{
		OnWebRequest("Login/Register", "username=" + username + "&password=" + md5pwd + "&mail=" + mail
					, onSuccess, onFail, onErr);

	}
	public static void DoLogout(string username, string token
		, Action<JObject> onSuccess = null, Action<string> onFail = null, Action<string> onErr = null)
	{
		OnWebRequest("Login/Logout", "username=" + username + "&token=" + token
			, onSuccess, onFail, onErr);

	}
	public static void DoSelectAvatar(string username, string token
		, Action<JObject> onSuccess = null, Action<string> onFail = null, Action<string> onErr = null)
	{
		OnWebRequest("Avatar/SelectAvatar", "username=" + username + "&token=" + token
			, onSuccess, onFail, onErr);
	}
	public static void DoCreateAvatar(string username, string token, string avatarname, string sex
		, Action<JObject> onSuccess = null, Action<string> onFail = null, Action<string> onErr = null)
	{
		OnWebRequest("Avatar/CreateAvatar", "username=" + username + "&token=" + token + "&avatarname=" + avatarname + "&sex=" + sex
			, onSuccess, onFail, onErr);

	}
	public static void OnWebRequest(string reqUrl, string content
		, Action<JObject> onSuccess = null, Action<string> onFail = null, Action<string> onErr = null)
	{
		UWebSender.OnRequest(Utils.BaseURL + reqUrl, content, (res) =>
		{
			var jres = res;
			var err = jres["err"].ToString();
			if (err == "0")
			{
				onSuccess?.Invoke(jres);
			}
			else
			{
				onFail?.Invoke(err);
			}
		}, (err) =>
		{
			onErr?.Invoke(err);
		});

	}
}
