using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public static class UDebugHotLog
{
	public static void Log(string log)
	{
		Debug.Log("[" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "]" + "[Hot]" + log);
	}
}
