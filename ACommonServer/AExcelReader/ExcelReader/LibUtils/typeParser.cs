using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class typeParser
{
	public static double doubleParse(string s, double dDefault = -1)
	{
        double result;
        if (double.TryParse(s, out result) == false)
        {
            result = dDefault;
        }
        return result;
	}

	public static int intParse(string s, int iDefault = 0)
	{
		if (string.IsNullOrEmpty(s))
		{
			return iDefault;
		}
		int result = iDefault;
		int.TryParse(s, out result);
		return result;
	}

	public static float floatParse(string s, float fDefault = -1)
	{
		if (string.IsNullOrEmpty(s))
		{
			return fDefault;
		}
		float result = fDefault;
		float.TryParse(s, out result);
		return result;
	}

	public static Int64 Int64Parse(string s, Int64 iDefault = 0)
	{
        Int64 result;
        if (Int64.TryParse(s, out result) == false)
        {
            result = iDefault;
        }
        return result;
	}

}


