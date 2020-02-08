using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using LibCommon;

public class AComponentParamServer : AComponentParamCommon
{
	public AAvatarServer ownerServer
	{
		get
		{
			return owner as AAvatarServer;
		}
	}
	public AComponentParamServer(AAvatarServer owner) : base(owner)
	{
	}
	public override void InitComponent()
	{
	}
	List<string> lDirtyParams = new List<string>();
	public override void OnSave()
	{
		if (lDirtyParams.Count == 0)
		{
			return;
		}
		var u = new List<UpdateDefinition<BsonDocument>>();
		foreach (var v in lDirtyParams)
		{
			if (!dParams.ContainsKey(v))
			{
				AOutput.LogError("Cannot find param " + v);
				return;
			}
			var aparam = dParams[v];
			switch (aparam.eParamType)
			{
				case EParamType.String:
					u.Add(ADBAccessor.update(v, aparam.sParamValue));
					break;
				case EParamType.Double:
					u.Add(ADBAccessor.update(v, aparam.dParamValue));
					break;
				case EParamType.Int:
					u.Add(ADBAccessor.update(v, aparam.iParamValue));
					break;
				case EParamType.Long:
					u.Add(ADBAccessor.update(v, aparam.lParamValue));
					break;
			}
		}
		ADBManager.Get(InitValueDefs.dbconnect, InitValueDefs.dbname).UpdateOneData(ParamNameDefs.TableAvatar
			, ADBAccessor.filter_eq(ParamNameDefs.CollectionID, ownerServer.objectId),
			Builders<BsonDocument>.Update.Combine(u));

		AOutput.Log("avatar " + owner.OnGetStringParamValue(ParamNameDefs.AvatarName) + " saved " + lDirtyParams.Count + " params");
		var update = new LibPacket.PktParamUpdate();
		foreach (var dp in lDirtyParams)
		{
			var info = new LibPacket.ParamInfo();
			info.paramName = dp;
			info.paramValue = OnGetStringParamValue(dp);
			update.lInfos.Add(info);
		}
		ownerServer.OnSendToClient(update);

		lDirtyParams.Clear();
	}
	public void OnRead(BsonDocument document)
	{
		foreach (var p in dParams)
		{
			if (!dParams.ContainsKey(p.Key))
			{
				AOutput.LogError("Cannot find param " + p.Key);
				return;
			}
			if (document.Contains(p.Key))
			{
				var aparam = dParams[p.Key];
				switch (aparam.eParamType)
				{
					case EParamType.String:
						OnSetParamValue(p.Key, document[p.Key].AsString, true);
						break;
					case EParamType.Int:
						OnSetParamValue(p.Key, document[p.Key].AsInt32, true);
						break;
					case EParamType.Double:
						OnSetParamValue(p.Key, document[p.Key].AsDouble, true);
						break;
					case EParamType.Long:
						OnSetParamValue(p.Key, document[p.Key].AsInt64, true);
						break;
				}
			}
		}
	}
}
