using LibPacket;
using System;
using System.Collections.Generic;
using System.Linq;
using LibCommon;

public abstract class AAvatarCommon : AGameObj
{
	public string uid;
	public bool bAI;
	public AAvatarCommon()
	{
	}

	public virtual void InitAllComponents() { }

	public override void RegisterAllParams()
	{
		componentParam.RegistParam(ParamNameDefs.AvatarName);
		componentParam.RegistParam(ParamNameDefs.AvatarGold, EParamType.Long);
		componentParam.RegistParam(ParamNameDefs.AvatarHead, EParamType.Int);
		componentParam.RegistParam(ParamNameDefs.AvatarMoney, EParamType.Long);
		componentParam.RegistParam(ParamNameDefs.DailyCheckCount, EParamType.Long);
		componentParam.RegistParam(ParamNameDefs.LastDailyCheckTime, EParamType.Long);
		componentParam.RegistParam(ParamNameDefs.AvatarSex, EParamType.Int);
		componentParam.RegistParam(ParamNameDefs.AvatarLevel, EParamType.Int);
		componentParam.RegistParam(ParamNameDefs.CurExp, EParamType.Long);
		componentParam.RegistParam(ParamNameDefs.MaxExp, EParamType.Long, false);
	}

	#region components
	AComponentParamCommon _componentParam;
	public AComponentParamCommon componentParam
	{
		get
		{
			if (_componentParam == null)
			{
				_componentParam = OnGetComponent<AComponentParamCommon>();
			}
			return _componentParam;
		}
	}
	#endregion

	public long LastDailyCheckTime
	{
		get
		{
			return OnGetInt64ParamValue(ParamNameDefs.LastDailyCheckTime);
		}
		set
		{
			OnSetParamValue(ParamNameDefs.LastDailyCheckTime, value);
		}
	}
	public long DailyCheckCount
	{
		get
		{
			return OnGetInt64ParamValue(ParamNameDefs.DailyCheckCount);
		}
		set
		{
			OnSetParamValue(ParamNameDefs.DailyCheckCount, value);
		}
	}
	public string AvatarName
	{
		get
		{
			return OnGetStringParamValue(ParamNameDefs.AvatarName);
		}
		set
		{
			OnSetParamValue(ParamNameDefs.AvatarName, value);
		}
	}
	public int AvatarLevel
	{
		get
		{
			return OnGetIntParamValue(ParamNameDefs.AvatarLevel);
		}
		set
		{
			OnSetParamValue(ParamNameDefs.AvatarLevel, value);
		}
	}
	public int AvatarHead
	{
		get
		{
			return OnGetIntParamValue(ParamNameDefs.AvatarHead);
		}
		set
		{
			OnSetParamValue(ParamNameDefs.AvatarHead, value);
		}
	}
	public long AvatarGold
	{
		get
		{
			return OnGetInt64ParamValue(ParamNameDefs.AvatarGold);
		}
		set
		{
			OnSetParamValue(ParamNameDefs.AvatarGold, value);
		}
	}
	public long AvatarMoney
	{
		get
		{
			return OnGetInt64ParamValue(ParamNameDefs.AvatarMoney);
		}
		set
		{
			OnSetParamValue(ParamNameDefs.AvatarMoney, value);
		}
	}
	public long CurExp
	{
		get
		{
			return OnGetInt64ParamValue(ParamNameDefs.CurExp);
		}
		set
		{
			OnSetParamValue(ParamNameDefs.CurExp, value);
		}
	}

	public long MaxEXP
	{
		get
		{
			return (AvatarLevel + 1) * (AvatarLevel + 1) * 10;
		}
		set
		{
			OnSetParamValue(ParamNameDefs.MaxExp, value);
		}
	}
}
