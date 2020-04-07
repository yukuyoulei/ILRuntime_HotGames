using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LibCommon.GameObj
{
	public static class CakeCache
	{
		//id, cake type, param name , param value
		public static Dictionary<string, Dictionary<string, Dictionary<string, AParam>>> dIDDirtyParams = new Dictionary<string, Dictionary<string, Dictionary<string, AParam>>>();
		//id, iid, cake type, param name , param value
		public static Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, AParam>>>> dIIDDirtyParams = new Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, AParam>>>>();
	}
	public class Cake
	{
		private static Dictionary<string, Action<Cake>> _registers;
		protected static Dictionary<string, Action<Cake>> registers
		{
			get
			{
				if (_registers == null)
				{
					_registers = new Dictionary<string, Action<Cake>>();
					_registers.Add("pinfo", cake =>
					{
						cake.RegisterParam(EParamType.String, ParamNameDefs.UnionID);
						cake.RegisterParam(EParamType.Int, ParamNameDefs.PartnerID);
						cake.RegisterParam(EParamType.String, ParamNameDefs.AvatarName);
						cake.RegisterParam(EParamType.Int, ParamNameDefs.AvatarLevel);
						cake.RegisterParam(EParamType.Long, ParamNameDefs.AvatarMoney);
						cake.RegisterParam(EParamType.Long, ParamNameDefs.AvatarGold);
						cake.RegisterParam(EParamType.Int, ParamNameDefs.AvatarSex);
						cake.RegisterParam(EParamType.Int, ParamNameDefs.CreateTime);
						cake.RegisterParam(EParamType.Int, ParamNameDefs.LastDailyCheckTime);
						cake.RegisterParam(EParamType.Int, ParamNameDefs.DailyCheckCount);
					});
					_registers.Add("items", cake =>
					{
						cake.RegisterParam(EParamType.String, ParamNameDefs.OwnerID);
						cake.RegisterParam(EParamType.String, ParamNameDefs.ContentID);
						cake.RegisterParam(EParamType.Int, ParamNameDefs.Count);
						cake.RegisterParam(EParamType.Int, ParamNameDefs.CreateTime);
					});
					_registers.Add("order", cake =>
					{
						cake.RegisterParam(EParamType.String, ParamNameDefs.OwnerID);
						cake.RegisterParam(EParamType.String, ParamNameDefs.ContentID);
						cake.RegisterParam(EParamType.Int, ParamNameDefs.ProductID);
						cake.RegisterParam(EParamType.Int, ParamNameDefs.Price);
						cake.RegisterParam(EParamType.Int, ParamNameDefs.CreateTime);
					});
				}
				return _registers;
			}
		}

		protected bool bRemove;
		public void DoRemove()
		{
			bRemove = true;
		}

		private static string[] multiList = new string[] { "items", "order" };
		protected virtual bool IsMulti { get { return multiList.Contains(cakeType); } }
		protected Dictionary<string, Cake> subCakes = new Dictionary<string, Cake>();
		public Cake(string cakeType, string id)
		{
			this.cakeType = cakeType;
			this.id = id;
			RegisterAllParams();
		}
		public Cake(string cakeType, string id, string iid) : this(cakeType, id)
		{
			this.iid = iid;
		}
		private void RegisterAllParams()
		{
			registers[this.cakeType].Invoke(this);
		}
		protected void RegisterParam(EParamType eParamType, string paramName, bool bSyncToClient = true, bool bSaveToDB = true)
		{
			dParams.Add(paramName, new AParam(eParamType, paramName, bSyncToClient, bSaveToDB));
		}
		protected bool bInited;
		public string cakeType { get; private set; }
		public string id { get; private set; }
		public string iid { get; protected set; }
		protected bool bNew;
		public Dictionary<string, AParam> dParams = new Dictionary<string, AParam>();
		protected virtual bool Init()
		{
			//return true if cached
			if (!IsMulti)
			{
				var bcached = false;
				if (!dCached.ContainsKey(id))
					dCached.Add(id, new Dictionary<string, Dictionary<string, AParam>>());
				if (!dCached[id].ContainsKey(cakeType))
					dCached[id].Add(cakeType, new Dictionary<string, AParam>());
				else
				{
					var ks = dCached[id][cakeType].Keys;
					foreach (var kv in ks)
						dParams[kv] = dCached[id][cakeType][kv];

					bcached = true;
				}
				bInited = true;
				return bcached;
			}
			else if (!string.IsNullOrEmpty(iid))
			{
				if (dCachedTwoKeys.ContainsKey(id) && dCachedTwoKeys[id].ContainsKey(cakeType) && dCachedTwoKeys[id][cakeType].ContainsKey(iid))
				{
					foreach (var kv in dCachedTwoKeys[id][cakeType][iid])
						dParams[kv.Key] = kv.Value;

					bInited = true;
					return true;
				}
				else
				{
					if (!dCachedTwoKeys.ContainsKey(id))
						dCachedTwoKeys.Add(id, new Dictionary<string, Dictionary<string, Dictionary<string, AParam>>>());
					if (!dCachedTwoKeys[id].ContainsKey(cakeType))
						dCachedTwoKeys[id].Add(cakeType, new Dictionary<string, Dictionary<string, AParam>>());
					if (!dCachedTwoKeys[id][cakeType].ContainsKey(iid))
						dCachedTwoKeys[id][cakeType].Add(iid, dParams);
					dParams = dCachedTwoKeys[id][cakeType][iid];
				}
			}
			else
			{
				if (!dCachedTwoKeys.ContainsKey(id))
					dCachedTwoKeys.Add(id, new Dictionary<string, Dictionary<string, Dictionary<string, AParam>>>());
				if (!dCachedTwoKeys[id].ContainsKey(cakeType))
					dCachedTwoKeys[id].Add(cakeType, new Dictionary<string, Dictionary<string, AParam>>());
				else
				{
					GenerateSubCakes(dCachedTwoKeys[id][cakeType].Keys.ToArray());
					return true;
				}
			}
			return false;
		}

		protected virtual void GenerateSubCakes(string[] iids) { }

		public string GetStringValue(string paramName) { var p = GetValue(paramName); if (p == null) return ""; return p.sParamValue; }
		public int GetIntValue(string paramName) { var p = GetValue(paramName); if (p == null) return 0; return p.iParamValue; }
		public int AddValue(string paramName, int count = 1)
		{
			var icur = GetIntValue(paramName);
			icur += count;
			SetValue(paramName, icur);
			return icur;
		}
		public long GetLongValue(string paramName) { var p = GetValue(paramName); if (p == null) return 0; return p.lParamValue; }
		public double GetDoubleValue(string paramName) { var p = GetValue(paramName); if (p == null) return 0; return p.dParamValue; }
		public AParam GetValue(string paramName)
		{
			if (!dParams.ContainsKey(paramName)) throw new Exception($"Not registerred param {paramName} in {cakeType} dParams.Count:{dParams.Count}");
			return dParams[paramName];
		}
		public void SetValue(string paramName, double paramValue)
		{
			if (!dParams.ContainsKey(paramName)) throw new Exception($"Not registerred param {paramName} in {cakeType}");
			dParams[paramName].paramValue = paramValue;
			Save(paramName);
		}
		public void SetValue(string paramName, long paramValue)
		{
			if (!dParams.ContainsKey(paramName)) throw new Exception($"Not registerred param {paramName} in {cakeType}");
			dParams[paramName].paramValue = paramValue;
			Save(paramName);
		}
		public void SetValue(string paramName, int paramValue)
		{
			if (!dParams.ContainsKey(paramName)) throw new Exception($"Not registerred param {paramName} in {cakeType}");
			dParams[paramName].paramValue = paramValue;
			Save(paramName);
		}
		public void SetValue(string paramName, string paramValue)
		{
			if (!dParams.ContainsKey(paramName)) throw new Exception($"Not registerred param {paramName} in {cakeType}");
			dParams[paramName].paramValue = paramValue;
			Save(paramName);
		}
		public void UpdateFromPkt(LibPacket.PktParamUpdate pkt)
		{
			if (pkt.cakeType != cakeType) return;
			if (!string.IsNullOrEmpty(id) && pkt.id != id) return;
			if (!string.IsNullOrEmpty(iid) && pkt.iid != iid) return;
			id = pkt.id;
			iid = pkt.iid;
			foreach (var info in pkt.lInfos)
			{
				SetValue(info.paramName, info.paramValue);
			}
		}
		public LibPacket.PktParamUpdate GetDirtyPkt()
		{
			var pkt = new LibPacket.PktParamUpdate();
			pkt.cakeType = cakeType;
			pkt.id = id;
			pkt.iid = iid;
			foreach (var p in dParams.Values)
			{
				if (!p.dirty || !p.needSync) continue;
				p.needSync = false;
				pkt.lInfos.Add(new LibPacket.ParamInfo() { paramName = p.paramName, paramValue = p.paramValue.ToString() });
			}
			return pkt;
		}
		protected virtual void Save(string paramName)
		{
			if (!string.IsNullOrEmpty(iid))
				dCachedTwoKeys[id][cakeType][iid][paramName] = dParams[paramName];
			else if (!IsMulti)
				dCached[id][cakeType][paramName] = dParams[paramName];
		}
		//id, cake type, paramname, paramvalue
		private static Dictionary<string, Dictionary<string, Dictionary<string, AParam>>> _dCached;
		protected static Dictionary<string, Dictionary<string, Dictionary<string, AParam>>> dCached
		{
			get
			{
				if (_dCached == null) _dCached = new Dictionary<string, Dictionary<string, Dictionary<string, AParam>>>();
				return _dCached;
			}
		}
		//id, cake type, iid, param 
		private static Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, AParam>>>> _dCachedTwoKeys;
		protected static Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, AParam>>>> dCachedTwoKeys
		{
			get
			{
				if (_dCachedTwoKeys == null) _dCachedTwoKeys = new Dictionary<string, Dictionary<string, Dictionary<string, Dictionary<string, AParam>>>>();
				return _dCachedTwoKeys;
			}
		}

	}
}
