using LibCommon.GameObj;
using System;
using System.Collections.Generic;
using System.Text;
using LibCommon;
using MongoDB.Bson;
using MongoDB.Driver;
using LibPacket;
using System.Linq;

namespace LibServer.GameObj
{
	public class CakeServer : Cake
	{
		public CakeServer(string cakeType, string id, string iid)
			: base(cakeType, id, iid)
		{
			var cached = Init();
			bNew = !cached;
		}
		public CakeServer(string cakeType, string id)
			: base(cakeType, id)
		{
			Init();
		}
		protected override bool Init()
		{
			var bCached = base.Init();
			bInited = true;
			if (!IsMulti)
			{
				if (!bCached)
				{
					var filter = ADBAccessor.filter_eq(ParamNameDefs.CollectionID, ObjectId.Parse(id));
					var res = ADBManager.Get(InitValueDefs.dbconnect, InitValueDefs.dbname)
										.FindOneData(cakeType
											, filter);
					if (res == null)
					{
						var ct = ApiDateTime.SecondsFromBegin();
						ADBManager.Get(InitValueDefs.dbconnect, InitValueDefs.dbname)
									.UpdateOneData(cakeType, filter
									, ADBAccessor.update(ParamNameDefs.CreateTime, ct), true);
					}
					else
						foreach (var item in res.Names)
						{
							if (item == ParamNameDefs.CollectionID) continue;
							SetValue(item, res[item].ToString());
						}
				}
				return bCached;
			}
			else
			{
				if (!bCached)
				{
					var filter = ADBAccessor.filter_eq(ParamNameDefs.OwnerID, id);
					var res = ADBManager.Get(InitValueDefs.dbconnect, InitValueDefs.dbname)
										.FindManyData(cakeType, filter);
					foreach (var r in res)
					{
						var cid = r[ParamNameDefs.ContentID].ToString();
						var cake = new CakeServer(cakeType, id, cid);
						foreach (var item in r.Names)
						{
							if (item == ParamNameDefs.CollectionID) continue;
							cake.SetValue(item, r[item].ToString());
						}
						subCakes.Add(cid, cake);
					}
				}
			}
			return bCached;
		}
		protected override void GenerateSubCakes(string[] iids)
		{
			subCakes.Clear();
			foreach (var iid in iids)
			{
				subCakes.Add(iid, new CakeServer(cakeType, id, iid));
			}
		}

		internal void Sync()
		{
			if (IsMulti)
			{
				foreach (var cake in subCakes)
				{
					DoSync(cake.Value);
				}
			}
			else
				DoSync(this);
		}
		private static void DoSync(Cake cake)
		{
			var pkt = new PktParamUpdate();
			pkt.id = cake.id;
			pkt.iid = cake.iid;
			pkt.cakeType = cake.cakeType;
			foreach (var p in cake.dParams)
			{
				if (!p.Value.bSyncToClient) continue;
				if (string.IsNullOrEmpty(p.Key)) throw new Exception($"Empty paramname!");
				if (p.Value.paramValue != null)
					pkt.lInfos.Add(new ParamInfo() { paramName = p.Key, paramValue = p.Value.paramValue.ToString() });
			}
			APlayerManager.SendToClient(cake.id, pkt);
		}

		protected override void Save(string paramName)
		{
			base.Save(paramName);
			if (!bInited) return;

			if (string.IsNullOrEmpty(iid))
			{
				if (!CakeCache.dIDDirtyParams.ContainsKey(id))
					CakeCache.dIDDirtyParams.Add(id, new Dictionary<string, Dictionary<string, AParam>>());
				if (!CakeCache.dIDDirtyParams[id].ContainsKey(cakeType))
					CakeCache.dIDDirtyParams[id].Add(cakeType, new Dictionary<string, AParam>());
				if (!CakeCache.dIDDirtyParams[id][cakeType].ContainsKey(paramName))
					CakeCache.dIDDirtyParams[id][cakeType].Add(paramName, GetValue(paramName));
				else
					CakeCache.dIDDirtyParams[id][cakeType][paramName] = GetValue(paramName);

				APlayerManager.SendToClient(id, GetDirtyPkt());
			}
			else
			{
				if (!CakeCache.dIIDDirtyParams.ContainsKey(id))
					CakeCache.dIIDDirtyParams.Add(id, new Dictionary<string, Dictionary<string, Dictionary<string, AParam>>>());
				if (!CakeCache.dIIDDirtyParams[id].ContainsKey(iid))
					CakeCache.dIIDDirtyParams[id].Add(iid, new Dictionary<string, Dictionary<string, AParam>>());
				if (!CakeCache.dIIDDirtyParams[id][iid].ContainsKey(cakeType))
					CakeCache.dIIDDirtyParams[id][iid].Add(cakeType, new Dictionary<string, AParam>());
				if (!CakeCache.dIIDDirtyParams[id][iid][cakeType].ContainsKey(paramName))
					CakeCache.dIIDDirtyParams[id][iid][cakeType].Add(paramName, GetValue(paramName));
				else
					CakeCache.dIIDDirtyParams[id][iid][cakeType][paramName] = GetValue(paramName);

				APlayerManager.SendToClient(id, GetSubCake(iid).GetDirtyPkt());
			}
		}

		private CakeServer GetSubCake(string iid)
		{
			if (!subCakes.ContainsKey(iid))
				subCakes.Add(iid, new CakeServer(cakeType, id, iid));
			return subCakes[iid] as CakeServer;
		}

		public CakeServer GetSingleItem(string iid)
		{
			if (!IsMulti) throw new Exception($"{cakeType} is not multi!");
			var cake = new CakeServer(cakeType, id, iid);
			if (cake.bNew)
			{
				subCakes.Add(iid, cake);
				cake.bNew = false;
			}
			return cake;
		}
		public static void Tick()
		{
			if (CakeCache.dIDDirtyParams.Count > 0)
			{
				foreach (var idk in CakeCache.dIDDirtyParams)
				{
					var filter = ADBAccessor.filter_eq(ParamNameDefs.CollectionID, ObjectId.Parse(idk.Key));
					foreach (var cakeTypek in idk.Value)
					{
						var lupdate = new List<UpdateDefinition<BsonDocument>>();
						var vs = cakeTypek.Value.Values.ToArray();
						foreach (var pv in vs)
						{
							lupdate.Add(pv.GetUpdate());
						}
						ADBManager.Get(InitValueDefs.dbconnect, InitValueDefs.dbname).UpdateOneData(cakeTypek.Key, filter
							, Builders<BsonDocument>.Update.Combine(lupdate));
					}

					if (!dAccessTimes.ContainsKey(idk.Key)) dAccessTimes.Add(idk.Key, DateTime.Now);
					else dAccessTimes[idk.Key] = DateTime.Now;
				}
				CakeCache.dIDDirtyParams.Clear();
			}
			if (CakeCache.dIIDDirtyParams.Count > 0)
			{
				foreach (var idk in CakeCache.dIIDDirtyParams)
				{
					var ll = idk.Value.ToArray();
					foreach (var iidk in ll)
					{
						var filter = ADBAccessor.filter_eq(ParamNameDefs.OwnerID, idk.Key)
							& ADBAccessor.filter_eq(ParamNameDefs.ContentID, iidk.Key);
						var lll = iidk.Value.ToArray();
						foreach (var cakeTypek in lll)
						{
							var lupdate = new List<UpdateDefinition<BsonDocument>>();
							var llll = cakeTypek.Value.ToArray();
							foreach (var pv in llll)
							{
								lupdate.Add(pv.Value.GetUpdate());
							}
							lupdate.Add(ADBAccessor.update(ParamNameDefs.ContentID, iidk.Key));
							lupdate.Add(ADBAccessor.update(ParamNameDefs.OwnerID, idk.Key));
							ADBManager.Get(InitValueDefs.dbconnect, InitValueDefs.dbname).UpdateOneData(cakeTypek.Key, filter
								, Builders<BsonDocument>.Update.Combine(lupdate), true);
						}
					}

					if (!dAccessTimes.ContainsKey(idk.Key)) dAccessTimes.Add(idk.Key, DateTime.Now);
					else dAccessTimes[idk.Key] = DateTime.Now;
				}
				CakeCache.dIIDDirtyParams.Clear();
			}
			var remove = new List<string>();
			foreach (var dk in dAccessTimes)
			{
				if (APlayerManager.Instance.IsOnline(dk.Key)) continue;
				if ((DateTime.Now - dk.Value).TotalSeconds > 30)
				{
					remove.Add(dk.Key);
				}
			}
			foreach (var r in remove)
			{
				dAccessTimes.Remove(r);
				if (CakeCache.dIDDirtyParams.ContainsKey(r)) CakeCache.dIDDirtyParams.Remove(r);
				if (CakeCache.dIIDDirtyParams.ContainsKey(r)) CakeCache.dIIDDirtyParams.Remove(r);
			}
		}
		private static Dictionary<string, DateTime> dAccessTimes = new Dictionary<string, DateTime>();
	}

	public static class AParamExtern
	{
		public static UpdateDefinition<BsonDocument> GetUpdate(this AParam param)
		{
			switch (param.eParamType)
			{
				case EParamType.Double:
					return ADBAccessor.update(param.paramName, param.dParamValue);
				case EParamType.Int:
					return ADBAccessor.update(param.paramName, param.iParamValue);
				case EParamType.Long:
					return ADBAccessor.update(param.paramName, param.lParamValue);
				case EParamType.String:
					return ADBAccessor.update(param.paramName, param.sParamValue);
			}
			return ADBAccessor.update(param.paramName, param.paramValue);
		}
	}
}
