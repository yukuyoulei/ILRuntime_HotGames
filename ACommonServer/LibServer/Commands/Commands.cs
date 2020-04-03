using LibCommon.GameObj;
using LibCommon;
using LibServer.GameObj;
using System;
using System.Collections.Generic;
using System.Text;

public static class Commands
{
	public static void DailyCheck(string psid)
	{
		var cake = new CakeAvatar(psid);
		if (ApiDateTime.IsSameDay(cake.GetIntValue(ParamNameDefs.LastDailyCheckTime))) return;
		var icount = cake.AddValue(ParamNameDefs.DailyCheckCount);
		var data = DailyCheckLoader.Instance.OnGetData(icount);
		if (data == null)
		{
			icount = 1;
			cake.SetValue(ParamNameDefs.DailyCheckCount, icount);
			data = DailyCheckLoader.Instance.OnGetData(icount);
		}
		
	}
}
