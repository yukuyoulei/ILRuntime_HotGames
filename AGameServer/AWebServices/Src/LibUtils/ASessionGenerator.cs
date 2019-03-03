using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class ASessionGenerator
{
	private const string session_auth_key = "7dd734a4fad3377b41f2976d8d26d372";

	public static string generate(int intArg, string strArg, int iSessionArg)
	{
		DateTime dt = DateTime.Now;
		int iMinute;
/*
		if (iSessionArg == 0)
		{
			iMinute = (dt - new DateTime(1970, 1, 1)).Minutes + iSessionArg;
		}
		else
		{
*/
			iMinute = iSessionArg;
/*
		}
*/
		string sResult = MD5String.Hash32(intArg + strArg + iMinute + session_auth_key);
		return sResult;
	}

	public static string generate_still_session(int intArg, string strArg)
	{
		return intArg + strArg + session_auth_key;
	}

	public static bool check_session(string session, int intArg, string strArg, int iSessionArg, int argLimit = 0, int limitRange = 60)
	{
		if (argLimit == 0)
		{
			return string.Equals(generate(intArg, strArg, iSessionArg), session);
		}
		int ml = 0;
		while (ml <= argLimit)
		{
			int arg = ((iSessionArg - ml) + limitRange) % limitRange;
			if (string.Equals(generate(intArg, strArg, arg), session))
			{
				return true;
			}
			ml++;
		}
		ml = 0;
		while (ml < argLimit)
		{
			ml++;
			int arg = iSessionArg % limitRange;
			if (string.Equals(generate(intArg, strArg, iSessionArg + ml), session))
			{
				return true;
			}
		}
		return false;
	}
}
