using System;
using System.Collections.Generic;
using System.Linq;
using LibCommon;

public class AParam
{
	public AParam(EParamType eParamType, string paramName, bool bSyncToClient, bool bSaveToDB)
	{
		this.eParamType = eParamType;
		this.paramName = paramName;
		this.bSyncToClient = bSyncToClient;
		this.bSaveToDB = bSaveToDB;
	}
	public string paramName { get; private set; }
	public bool bSyncToClient { get; private set; }
	public bool bSaveToDB { get; private set; }
	public EParamType eParamType { get; private set; }
	public bool dbdirty { get; private set; }
	public bool needSync { get; set; }
	public bool dirty { get; private set; }
	public void saved()
	{
		dirty = false;
		dbdirty = false;
	}
	private object _paramValue;
	public object paramValue
	{
		get
		{
			return _paramValue;
		}
		set
		{
			if (_paramValue != value)
			{
				if (bSaveToDB)
					dbdirty = true;
				if (bSyncToClient)
					needSync = true;
				dirty = true;
				_paramValue = value;
			}
		}
	}

	public string sParamValue
	{
		get
		{
			return paramValue == null ? "" : paramValue.ToString();
		}
		set
		{
			paramValue = value;
		}
	}
	public int iParamValue
	{
		get
		{
			return paramValue == null ? 0 : typeParser.intParse(paramValue.ToString());
		}
		set
		{
			paramValue = value;
		}
	}
	public long lParamValue
	{
		get
		{
			return paramValue == null ? 0 : typeParser.Int64Parse(paramValue.ToString());
		}
		set
		{
			paramValue = value;
		}
	}

	public double dParamValue
	{
		get
		{
			return paramValue == null ? 0 : typeParser.doubleParse(paramValue.ToString());
		}
		set
		{
			paramValue = value;
		}
	}

}
