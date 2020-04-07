using LibCommon;
using LibServer.GameObj;
using System;
using System.Collections.Generic;
using System.Text;

public static class AItems
{
	public static void AddItem(string psid, int itemID, int itemNum)
	{
		var cakes = new CakeItems(psid);
		var cake = cakes.GetSingleItem(itemID.ToString());
		cake.AddValue(ParamNameDefs.Count, itemNum);
		if (cake.GetIntValue(ParamNameDefs.Count) < 1)
			cake.DoRemove();
	}
	public static void AddItems(string psid, Dictionary<int, int> items)
	{
		foreach (var kv in items)
		{
			AddItem(psid, kv.Key, kv.Value);
		}
	}
}
