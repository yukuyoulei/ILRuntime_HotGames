using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

public class AParam
{
    public string paramName { get; set; }
    public EParamType eParamType { get; set; }
    public bool dirty { get; private set; }
    public void saved()
    {
        dirty = false;
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
