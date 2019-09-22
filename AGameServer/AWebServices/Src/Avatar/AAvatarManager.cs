using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AWebServices;

public class AAvatarManager : Singleton<AAvatarManager>
{
	private Dictionary<string, AAvatar> dAvatars = new Dictionary<string, AAvatar>();
	public void OnAddAvatar(AAvatar avatar)
	{
		string username = avatar.username;
		if (dAvatars.ContainsKey(username))
		{
			dAvatars[username] = avatar;
		}
		else
		{
			dAvatars.Add(username, avatar);
		}
	}
	public AAvatar OnGetAvatar(string username)
	{
		if (dAvatars.ContainsKey(username))
		{
			return dAvatars[username];
		}
		return LoadFromDB(username);
	}
	private AAvatar LoadFromDB(string username)
	{
		var result = ADatabaseConfigsManager.avatarDB.FindOneData(ADatabaseConfigsManager.tAvatarData, ADBAccessor.filter_eq(InfoNameDefs.Username, username), null);
		if (result != null && result.Contains(InfoNameDefs.AvatarName))
		{
			var a = new AAvatar(username, result[InfoNameDefs.AvatarName].AsString, result);
			OnAddAvatar(a);
			return a;
		}
		return null;
	}

	public void OnTick()
	{
		foreach (var a in dAvatars.Values)
		{
			a.OnTick();
		}
	}

	internal AAvatar OnCreateAvatar(string username, string avatarname, int isex)
	{
		var updateRes = ADatabaseConfigsManager.avatarDB.UpdateOneData(ADatabaseConfigsManager.tAvatarData, ADBAccessor.filter_eq(InfoNameDefs.Username, username)
					, ADBAccessor.updates_build(
						ADBAccessor.update(InfoNameDefs.AvatarName, avatarname)
					)
					, true);
		if (updateRes)
		{
			var a = new AAvatar(username, avatarname, null);
			OnAddAvatar(a);

			a.OnSetParamValue(InfoNameDefs.AvatarName, avatarname);
			a.OnSetParamValue(InfoNameDefs.AvatarSex, isex);
			return a;
		}
		return null;
	}
}
