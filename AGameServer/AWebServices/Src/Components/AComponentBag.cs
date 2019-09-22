using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AWebServices;

public class AComponentBag : AComponentBase
{
	public static string tableName = "tbag";
	public AComponentBag(AGameObj owner) : base(owner)
	{
	}
	public override void InitComponent()
	{
		var result = ADatabaseConfigsManager.avatarDB.FindOneData(tableName, ADBAccessor.filter_eq(InfoNameDefs.AvatarName, owner.OnGetStringParamValue(InfoNameDefs.AvatarName)), null);
		if (result != null && result.Contains(InfoNameDefs.AvatarName))
		{
			foreach (var r in result)
			{
				if (r.Name.StartsWith(InfoNameDefs.BagSlotPre))
				{
					var itemID = r.Name.Substring(InfoNameDefs.BagSlotPre.Length);
					var itemNum = r.Value.AsInt32;
					DoAddItem(typeParser.intParse(itemID), itemNum);
				}
			}
		}
		else
		{
			ADatabaseConfigsManager.avatarDB.UpdateOneData(tableName, ADBAccessor.filter_eq(InfoNameDefs.AvatarName, owner.OnGetStringParamValue(InfoNameDefs.AvatarName))
				, ADBAccessor.updates_build(ADBAccessor.update(InfoNameDefs.AvatarName, owner.OnGetStringParamValue(InfoNameDefs.AvatarName))), true);
		}
	}
	Dictionary<int, int> dItems = new Dictionary<int, int>();
	public void OnAddItem(int itemID, int count)
	{
		DoAddItem(itemID, count);

		if (dItems[itemID] <= 0)
		{
			dItems.Remove(itemID);

			ADatabaseConfigsManager.avatarDB.UpdateOneData(tableName, ADBAccessor.filter_eq(InfoNameDefs.AvatarName, owner.OnGetStringParamValue(InfoNameDefs.AvatarName)),
				ADBAccessor.updates_build(ADBAccessor.update(InfoNameDefs.BagSlotPre + itemID, 0, false)));
		}
		else
		{
			ADatabaseConfigsManager.avatarDB.UpdateOneData(tableName, ADBAccessor.filter_eq(InfoNameDefs.AvatarName, owner.OnGetStringParamValue(InfoNameDefs.AvatarName)),
				ADBAccessor.updates_build(ADBAccessor.update(InfoNameDefs.BagSlotPre + itemID, dItems[itemID])), true);
		}
	}

	internal void OnRemoveItem(int id, int count)
	{
		if (count > 0)
		{
			count = -count;
		}
		OnAddItem(id, count);
	}

	private void DoAddItem(int itemID, int count)
	{
		if (dItems.ContainsKey(itemID))
		{
			dItems[itemID] += count;
		}
		else
		{
			dItems.Add(itemID, count);
		}
	}

	public int OnGetItemCount(int itemID)
	{
		if (dItems.ContainsKey(itemID))
		{
			return dItems[itemID];
		}
		return 0;
	}

	internal string ToAll()
	{
		var l = new List<List<string>>();
		foreach (var item in dItems)
		{
			var ais = new List<string>();
			ais.Add("id");
			ais.Add(item.Key.ToString());
			ais.Add("count");
			ais.Add(item.Value.ToString());
			l.Add(ais);
		}
		return AWebServerUtils.ToJsonArray(l.ToArray());
	}
}
