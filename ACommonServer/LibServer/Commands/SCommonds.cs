using LibCommon;
using LibServer.GameObj;
using System;
using System.Collections.Generic;
using System.Text;

public static class SCommonds
{
	public static void AddItem(string reason, string psid, int itemID, int itemNum)
	{
		var cakes = new CakeItems(psid);
		var cake = cakes.GetSingleItem(itemID.ToString());
		cake.AddValue(ParamNameDefs.Count, itemNum);
		if (cake.GetIntValue(ParamNameDefs.Count) < 1)
			cake.DoRemove();
	}
	public static void AddItems(string reason, string psid, Dictionary<int, int> items)
	{
		foreach (var kv in items)
		{
			AddItem(reason, psid, kv.Key, kv.Value);
		}
	}
	internal static void Use(string reason, string psid, int itemID, int count)
	{
		if (count > 0) return;
		var cakes = new CakeItems(psid);
		var cake = cakes.GetSingleItem(itemID.ToString());
		cake.AddValue(ParamNameDefs.Count, count);
	}
	internal static bool IsEnough(string psid, int itemID, int count)
	{
		var cakes = new CakeItems(psid);
		var cake = cakes.GetSingleItem(itemID.ToString());
		return cake.GetIntValue(ParamNameDefs.Count) >= count;
	}
}
