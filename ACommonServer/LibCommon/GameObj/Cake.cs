using System;
using System.Collections.Generic;
using System.Text;

namespace LibCommon.GameObj
{
	public class Cake
	{
		public Cake(string cakeType, string id, string iid)
		{
			this.cakeType = cakeType;
			this.id = id;
			this.iid = iid;
		}
		protected bool bInited;
		public string cakeType { get; private set; }
		public string id { get; private set; }
		public string iid { get; private set; }
		public Dictionary<string, string> dParams;
		public virtual bool Init()
		{
			if (string.IsNullOrEmpty(iid))
			{
				if (dCached.ContainsKey(id))
				{
					dParams = dCached[id];
					bInited = true;
					return true;
				}
				else
				{
					dParams = new Dictionary<string, string>();
					dCached.Add(id, dParams);
				}
			}
			else
			{
				if (dCachedTwoKeys.ContainsKey(id) && dCachedTwoKeys[id].ContainsKey(iid))
				{
					dParams = dCachedTwoKeys[id][iid];
					bInited = true;
					return true;
				}
				else
				{
					dParams = new Dictionary<string, string>();
					if (!dCachedTwoKeys.ContainsKey(id))
						dCachedTwoKeys.Add(id, new Dictionary<string, Dictionary<string, string>>());
					dCachedTwoKeys[id].Add(iid, dParams);
				}
			}
			return false;
		}
		public string GetValue(string paramName) { return dParams[paramName]; }
		public virtual void SetValue(string paramName, string paramValue)
		{
			if (dParams.ContainsKey(paramName)) dParams[paramName] = paramValue;
			else dParams.Add(paramName, paramValue);
		}
		private static Dictionary<string, Dictionary<string, string>> dCached = new Dictionary<string, Dictionary<string, string>>();
		private static Dictionary<string, Dictionary<string, Dictionary<string, string>>> dCachedTwoKeys = new Dictionary<string, Dictionary<string, Dictionary<string, string>>>();
	}
}
