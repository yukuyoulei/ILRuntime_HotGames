using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public static class URemoteData
{
	static UAvatarData curAvatarData;
	public static void OnReceiveAvatarData(string sjson)
	{
		var obj = JsonConvert.DeserializeObject(sjson) as JObject;
		if (curAvatarData == null)
			curAvatarData = new UAvatarData(obj);
		else
			curAvatarData.DeserializeParamJson(obj);
	}
	public static void OnRemoveAvatarData()
	{
		curAvatarData = null;
	}

	public static string AvatarName
	{
		get
		{
			return curAvatarData?.OnGetParam(InfoNameDefs.AvatarName);
		}
	}
	public static string AvatarLevel
	{
		get
		{
			return curAvatarData?.OnGetParam(InfoNameDefs.AvatarLevel);
		}
	}
	public static int CurExp
	{
		get
		{
			return typeParser.intParse(curAvatarData?.OnGetParam(InfoNameDefs.CurExp), 0);
		}
	}
	public static int MaxExp
	{
		get
		{
			return typeParser.intParse(curAvatarData?.OnGetParam(InfoNameDefs.MaxExp), 0);
		}
	}

	private static Dictionary<string, List<Action>> dListeners = new Dictionary<string, List<Action>>();
	public static void ListeningParam(string param, Action changed)
	{
		if (!dListeners.ContainsKey(param))
		{
			dListeners.Add(param, new List<Action>());
		}
		dListeners[param].Add(changed);
	}
	public static void CancelListeningParam(string param, Action changed)
	{
		if (!dListeners.ContainsKey(param))
		{
			return;
		}
		dListeners[param].Remove(changed);
	}
	public static void OnParamChnaged(string param)
	{
		List<Action> lremove = new List<Action>();
		if (dListeners.ContainsKey(param))
		{
			foreach (var a in dListeners[param])
			{
				if (a.Target == null)
				{
					lremove.Add(a);
					continue;
				}
				a?.Invoke();
			}
			foreach (var r in lremove)
			{
				dListeners[param].Remove(r);
			}
		}
	}
}
public class UAvatarData
{
	public UAvatarData(JObject obj)
	{
		DeserializeParamJson(obj);
	}
	private Dictionary<string, string> dParams = new Dictionary<string, string>();
	public void OnSetParam(string param, string value)
	{
		dParams[param] = value;
		URemoteData.OnParamChnaged(param);
	}
	public string OnGetParam(string param)
	{
		if (!dParams.ContainsKey(param))
		{
			return "";
		}
		return dParams[param];
	}
	public void DeserializeParamJson(JObject obj)
	{
		var enumerator = obj.GetEnumerator();
		while (enumerator.MoveNext())
		{
			if (enumerator.Current.Value.Type == JTokenType.String
				|| enumerator.Current.Value.Type == JTokenType.Integer
				|| enumerator.Current.Value.Type == JTokenType.Float
				|| enumerator.Current.Value.Type == JTokenType.Boolean
				)
			{
				OnSetParam(enumerator.Current.Key, enumerator.Current.Value.ToString());
			}
		}
	}

}
