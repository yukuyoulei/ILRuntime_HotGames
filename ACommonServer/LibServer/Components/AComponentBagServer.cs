using System;
using System.Collections.Generic;
using System.Linq;
using LibCommon;

public class AComponentBagServer : AComponentBagCommon
{
	public AAvatarServer ownerServer
	{
		get
		{
			return owner as AAvatarServer;
		}
	}
	public static string tableName = "tbag";
	public AComponentBagServer(AGameObj owner) : base(owner)
	{
	}
	public override void InitComponent()
	{
		var result = ADBManager.Get(InitValueDefs.dbconnect, InitValueDefs.dbname)
			.FindOneData(tableName, ADBAccessor.filter_eq(ParamNameDefs.CollectionID, ownerServer.objectId), null);
		if (result != null)
		{
			foreach (var r in result)
			{
				if (r.Name.StartsWith(ParamNameDefs.BagSlotPre))
				{
					var itemID = r.Name.Substring(ParamNameDefs.BagSlotPre.Length);
					var itemNum = r.Value.AsInt32;
					DoAddItem(typeParser.intParse(itemID), itemNum);
				}
			}
		}
		else
		{
			ADBManager.Get(InitValueDefs.dbconnect, InitValueDefs.dbname).UpdateOneData(tableName, ADBAccessor.filter_eq(ParamNameDefs.CollectionID, ownerServer.objectId)
				, ADBAccessor.updates_build(ADBAccessor.update(ParamNameDefs.AvatarName, owner.OnGetStringParamValue(ParamNameDefs.AvatarName))), true);
		}
	}
	Dictionary<int, int> dItems = new Dictionary<int, int>();
	public override void OnAddItem(int itemID, int count)
	{
		base.OnAddItem(itemID, count);

		if (dItems.ContainsKey(itemID))
		{
			ADBManager.Get(InitValueDefs.dbconnect, InitValueDefs.dbname).UpdateOneData(tableName, ADBAccessor.filter_eq(ParamNameDefs.CollectionID, ownerServer.objectId),
				ADBAccessor.updates_build(ADBAccessor.update(ParamNameDefs.BagSlotPre + itemID, dItems[itemID])), true);
		}
		else
		{
			ADBManager.Get(InitValueDefs.dbconnect, InitValueDefs.dbname).UpdateOneData(tableName, ADBAccessor.filter_eq(ParamNameDefs.CollectionID, ownerServer.objectId),
				ADBAccessor.updates_build(ADBAccessor.update_unset(ParamNameDefs.BagSlotPre + itemID)));
		}
	}

}
