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
		var result = Avatar.dbavatar.FindOneData(Avatar.TableName, ADBAccessor.filter_eq(InfoNameDefs.Username, username), null);
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
}
