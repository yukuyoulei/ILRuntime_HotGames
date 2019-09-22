using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

public class AComponentParam : AComponentBase
{
	Dictionary<string, AParam> dParams = new Dictionary<string, AParam>();
	public AComponentParam(AAvatar owner) : base(owner)
	{
	}
	public override void InitComponent()
	{
	}

	private List<string> _AllParamsToSync;
	public List<string> AllParamsToSync
	{
		get
		{
			if (_AllParamsToSync == null)
			{
				_AllParamsToSync = new List<string>();
				foreach (var p in dParams)
				{
					if (p.Value.bSyncToClient)
					{
						_AllParamsToSync.Add(p.Key);
						p.Value.needSync = false;
					}
				}
			}
			return _AllParamsToSync;
		}
	}
	public List<string> ParamsNeedToSync
	{
		get
		{
			var result = new List<string>();
			foreach (var p in dParams)
			{
				if (p.Value.needSync)
				{
					result.Add(p.Key);
					p.Value.needSync = false;
				}
			}
			return result;
		}
	}
	List<string> lDirtyParams = new List<string>();
	public virtual void OnSave()
	{
		if (lDirtyParams.Count == 0)
		{
			return;
		}
		var u = new List<UpdateDefinition<BsonDocument>>();
		foreach (var v in lDirtyParams)
		{
			if (!dParams.ContainsKey(v))
			{
				AOutput.LogError("Cannot find param " + v);
				return;
			}
			var aparam = dParams[v];
			switch (aparam.eParamType)
			{
				case EParamType.String:
					u.Add(ADBAccessor.update(v, aparam.sParamValue));
					break;
				case EParamType.Double:
					u.Add(ADBAccessor.update(v, aparam.dParamValue));
					break;
				case EParamType.Int:
					u.Add(ADBAccessor.update(v, aparam.iParamValue));
					break;
				case EParamType.Long:
					u.Add(ADBAccessor.update(v, aparam.lParamValue));
					break;
			}
		}
		ADatabaseConfigsManager.avatarDB.UpdateOneData(ADatabaseConfigsManager.tAvatarData
			, ADBAccessor.filter_eq(InfoNameDefs.AvatarName, owner.OnGetStringParamValue(InfoNameDefs.AvatarName)),
			Builders<BsonDocument>.Update.Combine(u));

		AOutput.Log("avatar " + owner.OnGetStringParamValue(InfoNameDefs.AvatarName) + " saved " + lDirtyParams.Count + " params");
		lDirtyParams.Clear();
	}
	public void RegistParam(string sparam, EParamType eParamType = EParamType.String, bool bSaveToDB = true, bool bSyncToClient = true)
	{
		dParams.Add(sparam, new AParam() { paramName = sparam, paramValue = "", eParamType = eParamType, bSyncToClient = bSyncToClient, bSaveToDB = bSaveToDB });
	}
	public void OnSetParamValue(string paramname, int paramvalue, bool bFirst = false)
	{
		if (dParams.ContainsKey(paramname))
		{
			dParams[paramname].iParamValue = paramvalue;
			if (dParams[paramname].dbdirty)
			{
				if (!bFirst && !lDirtyParams.Contains(paramname))
				{
					lDirtyParams.Add(paramname);
				}
				dParams[paramname].saved();
			}
		}
	}
	public void OnSetParamValue(string paramname, long paramvalue, bool bFirst = false)
	{
		if (dParams.ContainsKey(paramname))
		{
			dParams[paramname].lParamValue = paramvalue;
			if (dParams[paramname].dbdirty)
			{
				if (!bFirst && !lDirtyParams.Contains(paramname))
				{
					lDirtyParams.Add(paramname);
				}
				dParams[paramname].saved();
			}
		}
	}
	public void OnSetParamValue(string paramname, double paramvalue, bool bFirst = false)
	{
		if (dParams.ContainsKey(paramname))
		{
			dParams[paramname].dParamValue = paramvalue;
			if (dParams[paramname].dbdirty)
			{
				if (!bFirst && !lDirtyParams.Contains(paramname))
				{
					lDirtyParams.Add(paramname);
				}
				dParams[paramname].saved();
			}
		}
	}
	public void OnSetParamValue(string paramname, string paramvalue, bool bFirst = false)
	{
		if (dParams.ContainsKey(paramname))
		{
			dParams[paramname].sParamValue = paramvalue;
			if (dParams[paramname].dbdirty)
			{
				if (!bFirst && !lDirtyParams.Contains(paramname))
				{
					lDirtyParams.Add(paramname);
				}
				dParams[paramname].saved();
			}
		}
	}
	public double OnGetDoubleParamValue(string sparam)
	{
		if (dParams.ContainsKey(sparam))
		{
			return dParams[sparam].dParamValue;
		}
		return 0;
	}
	public long OnGetInt64ParamValue(string sparam)
	{
		if (dParams.ContainsKey(sparam))
		{
			return dParams[sparam].lParamValue;
		}
		return 0;
	}
	public int OnGetIntParamValue(string sparam)
	{
		if (dParams.ContainsKey(sparam))
		{
			return dParams[sparam].iParamValue;
		}
		return 0;
	}
	public string OnGetStringParamValue(string sparam)
	{
		if (dParams.ContainsKey(sparam))
		{
			return dParams[sparam].sParamValue;
		}
		return "";
	}
	public void OnRead(BsonDocument document)
	{
		foreach (var p in dParams)
		{
			if (!dParams.ContainsKey(p.Key))
			{
				AOutput.LogError("Cannot find param " + p.Key);
				return;
			}
			if (document.Contains(p.Key))
			{
				var aparam = dParams[p.Key];
				switch (aparam.eParamType)
				{
					case EParamType.String:
						OnSetParamValue(p.Key, document[p.Key].AsString, true);
						break;
					case EParamType.Int:
						OnSetParamValue(p.Key, document[p.Key].AsInt32, true);
						break;
					case EParamType.Double:
						OnSetParamValue(p.Key, document[p.Key].AsDouble, true);
						break;
					case EParamType.Long:
						OnSetParamValue(p.Key, document[p.Key].AsInt64, true);
						break;
				}
			}
		}
	}
}
