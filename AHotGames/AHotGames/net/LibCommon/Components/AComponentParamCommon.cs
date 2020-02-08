using System;
using System.Collections.Generic;
using System.Linq;
using LibCommon;

public class AComponentParamCommon : AComponentBase
{
	protected Dictionary<string, AParam> dParams = new Dictionary<string, AParam>();
	public AComponentParamCommon(AAvatarCommon owner) : base(owner)
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
		
	}
	public void RegistParam(string sparam, EParamType eParamType = EParamType.String, bool bSaveToDB = true, bool bSyncToClient = true)
	{
		if (dParams.ContainsKey(sparam)) throw new Exception($"Already registered {sparam}");
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
}
