using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;

public class ATokenManager : Singleton<ATokenManager>
{
	public enum ECheckTokenResult
	{
		Success,
		NotInCache,
		Invalid,
	}
	public bool OnCheckToken(string username, string token)
	{
		var eCheck = DoCheckToken(username, token);
		if (eCheck == ECheckTokenResult.NotInCache)
		{
			var res = ADatabaseConfigsManager.userDB.FindOneData(ADatabaseConfigsManager.tUserData, ADBAccessor.filter_eq(InfoNameDefs.Username, username) & ADBAccessor.filter_eq(InfoNameDefs.UserToken, token));
			if (res == null)
			{
				return false;
			}
		}
		else if (eCheck == ECheckTokenResult.Invalid)
		{
			return false;
		}
		OnSetToken(username, token);
		return true;
	}
	public void OnRemoveToken(string username)
	{
		if (dTokens.ContainsKey(username))
		{
			dTokens.Remove(username);
		}
	}

	Dictionary<string, string> dTokens = new Dictionary<string, string>();
	private ECheckTokenResult DoCheckToken(string username, string token)
	{
		if (string.IsNullOrEmpty(token)
			|| string.IsNullOrEmpty(username))
			return ECheckTokenResult.Invalid;
		if (!dTokens.ContainsKey(username))
			return ECheckTokenResult.NotInCache;
		if (dTokens[username] == token)
			return ECheckTokenResult.Success;
		return ECheckTokenResult.Invalid;
	}
	public void OnSetToken(string username, string token)
	{
		if (dTokens.ContainsKey(username))
		{
			dTokens[username] = token;
		}
		else
		{
			dTokens.Add(username, token);
		}
	}
}
