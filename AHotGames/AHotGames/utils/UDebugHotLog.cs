using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public static class UDebugHotLog
{
	public static void Log(string log)
	{
		UnityEngine.Debug.Log("[" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "]" + "[Hot]" + log);
	}
}
