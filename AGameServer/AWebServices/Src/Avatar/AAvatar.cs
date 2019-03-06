using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


public class AAvatar : AGameObj
{
	public string username;
	public AAvatar(string username, string nickname, BsonDocument dbdocument)
	{
		this.username = username;

		RegisterAllComponents();
		RegisterAllParams();

		if (dbdocument != null)
		{
			componentParam.OnRead(dbdocument);
		}

		InitAllComponents();
	}

	public void InitAllComponents()
	{
		foreach (var c in allComponents)
		{
			c.InitComponent();
		}
	}

	public override void RegisterAllParams()
	{
		componentParam.RegistParam(InfoNameDefs.AvatarName);
		componentParam.RegistParam(InfoNameDefs.AvatarGold, EParamType.Long);
		componentParam.RegistParam(InfoNameDefs.AvatarHead, EParamType.Int);
		componentParam.RegistParam(InfoNameDefs.AvatarMoney, EParamType.Long);
		componentParam.RegistParam(InfoNameDefs.DailyCheckCount, EParamType.Long);
		componentParam.RegistParam(InfoNameDefs.LastDailyCheckTime, EParamType.Long);
        componentParam.RegistParam(InfoNameDefs.AvatarSex, EParamType.Int);
    }

    #region components
    AComponentParam _componentParam;
	public AComponentParam componentParam
	{
		get
		{
			if (_componentParam == null)
			{
				_componentParam = OnGetComponent<AComponentParam>();
			}
			return _componentParam;
		}
	}

	internal void OnTick()
	{
		if (componentParam == null)
		{
			return;
		}
		componentParam.OnSave();
	}

	AComponentBag _componentBag;
	public AComponentBag componentBag
	{
		get
		{
			if (_componentBag == null)
			{
				_componentBag = OnGetComponent<AComponentBag>();
			}
			return _componentBag;
		}
	}

	#endregion
	public override void RegisterAllComponents()
	{
		RegisterComponent(new AComponentBag(this));
		RegisterComponent(new AComponentParam(this));
	}
	public static string[] SyncToClientInfoNames = new string[]
	{
		InfoNameDefs.AvatarName,
		InfoNameDefs.AvatarGold,
		InfoNameDefs.AvatarMoney,
        InfoNameDefs.AvatarSex,
		InfoNameDefs.AvatarHead,
		InfoNameDefs.DailyCheckCount,
		InfoNameDefs.LastDailyCheckTime,

	};
	public long LastDailyCheckTime
	{
		get
		{
			return OnGetInt64ParamValue(InfoNameDefs.LastDailyCheckTime);
		}
		set
		{
			OnSetParamValue(InfoNameDefs.LastDailyCheckTime, value);
		}
	}
	public long DailyCheckCount
	{
		get
		{
			return OnGetInt64ParamValue(InfoNameDefs.DailyCheckCount);
		}
		set
		{
			OnSetParamValue(InfoNameDefs.DailyCheckCount, value);
		}
	}
	public string AvatarName
	{
		get
		{
			return OnGetStringParamValue(InfoNameDefs.AvatarName);
		}
	}
	public int AvatarHead
	{
		get
		{
			return OnGetIntParamValue(InfoNameDefs.AvatarHead);
		}
		set
		{
			OnSetParamValue(InfoNameDefs.AvatarHead, value);
		}
	}
	public long AvatarGold
	{
		get
		{
			return OnGetInt64ParamValue(InfoNameDefs.AvatarGold);
		}
		set
		{
			OnSetParamValue(InfoNameDefs.AvatarGold, value);
		}
	}
	public long AvatarMoney
	{
		get
		{
			return OnGetInt64ParamValue(InfoNameDefs.AvatarMoney);
		}
		set
		{
			OnSetParamValue(InfoNameDefs.AvatarMoney, value);
		}
	}

	internal string[] ToAll()
	{
		var infos = new List<string>();
		foreach (var n in SyncToClientInfoNames)
		{
			var value = OnGetStringParamValue(n);
			if (!string.IsNullOrEmpty(value))
			{
				infos.Add(n);
				infos.Add(value);
			}
		}

		infos.Add("bag");
		infos.Add(componentBag.ToAll());

		return infos.ToArray();
	}

	public const string TableChat = "chat";
	public static ChatMessage OnAddChatHistory(string senderUsername, string targetUsername, string type, string content, string head)
	{
		var time = ApiDateTime.SecondsFromBegin();

		var cm = new ChatMessage();
		cm.s = senderUsername;
		cm.t = targetUsername;
		cm.tp = typeParser.intParse(type);
		cm.c = content;
		cm.tm = time;
		cm.h = typeParser.intParse(head);

		AWebServices.Avatar.dbavatar.UpdateOneData(TableChat
			, ADBAccessor.filter_eq(InfoNameDefs.SenderName, cm.s)
			   & ADBAccessor.filter_eq(InfoNameDefs.TargetName, cm.t)
			   & ADBAccessor.filter_eq(InfoNameDefs.SendTime, cm.tm)
			   & ADBAccessor.filter_eq(InfoNameDefs.SendContent, cm.c)
			, ADBAccessor.updates_build(
				 ADBAccessor.update(InfoNameDefs.SendTime, cm.tm)
			   , ADBAccessor.update(InfoNameDefs.SenderName, cm.s)
			   , ADBAccessor.update(InfoNameDefs.TargetName, cm.t)
			   , ADBAccessor.update(InfoNameDefs.ChatType, cm.tp)
			   , ADBAccessor.update(InfoNameDefs.SendTime, cm.tm)
			   , ADBAccessor.update(InfoNameDefs.SendContent, cm.c)
			)
			, true);
		return cm;
	}

	internal List<ChatMessage> OnGetHistory(string targetUsername, int page, int count)
	{
		var lcm = new List<ChatMessage>();
		var start = page * count;
		var res = AWebServices.Avatar.dbavatar.FindManyData(TableChat
			, (ADBAccessor.filter_eq(InfoNameDefs.SenderName, username)
				& ADBAccessor.filter_eq(InfoNameDefs.TargetName, targetUsername))
				| (ADBAccessor.filter_eq(InfoNameDefs.SenderName, targetUsername)
				& ADBAccessor.filter_eq(InfoNameDefs.TargetName, username))
				, null, count, start
				, ADBAccessor.sort_Descending(InfoNameDefs.SendTime));
		foreach (var r in res)
		{
			ChatMessage cm = new ChatMessage();
			var sender = AAvatarManager.Instance.OnGetAvatar(r[InfoNameDefs.SenderName].AsString);
			var target = AAvatarManager.Instance.OnGetAvatar(r[InfoNameDefs.TargetName].AsString);
			cm.s = sender.AvatarName;
			cm.t = target.AvatarName;
			cm.h = sender.AvatarHead;
			cm.tm = r[InfoNameDefs.SendTime].AsInt32;
			cm.c = r[InfoNameDefs.SendContent].AsString.Replace("[", "").Replace("]", "").Replace("\"", "").Replace("\'", "");
			cm.tp = r[InfoNameDefs.ChatType].AsInt32;
			lcm.Add(cm);
		}
		return lcm;
	}
}

public class ChatMessage
{
	public int tp; //type
	public string c;//content
	public int h;//head
	public int tm;//chat time
	public string s;//senderName
	public string t;//targetName
	public string ToJson()
	{
		return Newtonsoft.Json.JsonConvert.SerializeObject(this).Replace("\r", "").Replace("\n", "").Replace("\t", "");
	}
}