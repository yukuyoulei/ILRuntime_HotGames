using LibCommon.GameObj;
using System;
using System.Collections.Generic;
using System.Text;
using LibCommon;
using MongoDB.Bson;
using MongoDB.Driver;

namespace LibServer.GameObj
{
	public class CakeServer : Cake
	{
		//id, cake type, param name , param value
		public static Dictionary<string, Dictionary<string, Dictionary<string, string>>> dIDDirtyParams = new Dictionary<string, Dictionary<string, Dictionary<string, string>>>();
		//id, iid, cake type, param name , param value
		public static Dictionary<string, Dictionary<string, AParam>> dIIDDirtyParams = new Dictionary<string, Dictionary<string, Dictionary<string, UpdateDefinition<BsonDocument>>>>();
		public CakeServer(string cakeType, string id, string iid = "")
			: base(cakeType, id, iid)
		{
			var bCached = Init();
			if (!bCached)
			{
				var res = ADBManager.Get(InitValueDefs.dbconnect, InitValueDefs.dbname)
					.FindOneData(cakeType
						, ADBAccessor.filter_eq(ParamNameDefs.CollectionID, string.IsNullOrEmpty(iid) ? ObjectId.Parse(id) : ObjectId.Parse(iid)));
				if (res == null)
				{
					var ct = ApiDateTime.SecondsFromBegin();
					ADBManager.Get(InitValueDefs.dbconnect, InitValueDefs.dbname)
											.UpdateOneData(cakeType
											, ADBAccessor.filter_eq(ParamNameDefs.CollectionID, string.IsNullOrEmpty(iid) ? ObjectId.Parse(id) : ObjectId.Parse(iid))
											, ADBAccessor.update(ParamNameDefs.CreateTime, ct), true);
					SetValue(ParamNameDefs.CreateTime, ct.ToString());
				}
				else
					foreach (var item in res.Names)
					{
						SetValue(item, res[item].ToString());
					}
			}
			bInited = true;
		}
		public override void SetValue(string paramName, string paramValue)
		{
			base.SetValue(paramName, paramValue);

			if (!bInited) return;


		}
	}
}
