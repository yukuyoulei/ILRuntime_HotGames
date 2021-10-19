using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

public abstract class AGameObj
{
	public string objID { get; set; }
	protected List<AComponentBase> allComponents = new List<AComponentBase>();
	public abstract void RegisterAllComponents();
	public abstract void RegisterAllParams();
	public void RegisterComponent<T>(T t) where T : AComponentBase
	{
		allComponents.Add(t);
	}
	public virtual void OnDispose()
	{
		foreach (var c in allComponents)
		{
			c.OnDispose();
		}
		allComponents.Clear();
	}

	public T OnGetComponent<T>() where T : AComponentBase
	{
		foreach (var c in allComponents)
		{
			if (c is T)
			{
				return c as T;
			}
		}
		return null;
	}
	public void OnSetParamValue(string paramname, string paramvalue)
	{
		var paramComponent = OnGetComponent<AComponentParamCommon>();
		if (paramComponent != null)
		{
			paramComponent.OnSetParamValue(paramname, paramvalue);
		}
	}
	public void OnSetParamValue(string paramname, long paramvalue)
	{
		var paramComponent = OnGetComponent<AComponentParamCommon>();
		if (paramComponent != null)
		{
			paramComponent.OnSetParamValue(paramname, paramvalue);
		}
	}
	public void OnSetParamValue(string paramname, double paramvalue)
	{
		var paramComponent = OnGetComponent<AComponentParamCommon>();
		if (paramComponent != null)
		{
			paramComponent.OnSetParamValue(paramname, paramvalue);
		}
	}
	public void OnSetParamValue(string paramname, int paramvalue)
	{
		var paramComponent = OnGetComponent<AComponentParamCommon>();
		if (paramComponent != null)
		{
			paramComponent.OnSetParamValue(paramname, paramvalue);
		}
	}
	public string OnGetStringParamValue(string paramname)
	{
		var paramComponent = OnGetComponent<AComponentParamCommon>();
		if (paramComponent != null)
		{
			return paramComponent.OnGetStringParamValue(paramname);
		}
		return "";
	}
	public int OnGetIntParamValue(string paramname)
	{
		var paramComponent = OnGetComponent<AComponentParamCommon>();
		if (paramComponent != null)
		{
			return paramComponent.OnGetIntParamValue(paramname);
		}
		return 0;
	}
	public long OnGetInt64ParamValue(string paramname)
	{
		var paramComponent = OnGetComponent<AComponentParamCommon>();
		if (paramComponent != null)
		{
			return paramComponent.OnGetInt64ParamValue(paramname);
		}
		return 0;
	}
	public double OnGetDoubleParamValue(string paramname)
	{
		var paramComponent = OnGetComponent<AComponentParamCommon>();
		if (paramComponent != null)
		{
			return paramComponent.OnGetDoubleParamValue(paramname);
		}
		return 0;
	}
}
