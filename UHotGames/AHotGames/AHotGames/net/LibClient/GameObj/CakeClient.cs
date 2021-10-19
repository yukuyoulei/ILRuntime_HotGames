using LibCommon.GameObj;
using System;
using System.Collections.Generic;
using System.Text;

namespace LibClient.GameObj
{
	public class CakeClient : Cake
	{
		public CakeClient(string cakeType, string id, string iid) : base(cakeType, id, iid)
		{
			Init();
		}
		protected override void Save(string paramName)
		{
			base.Save(paramName);
		}

		private static Dictionary<string, Dictionary<string, Dictionary<string, CakeClient>>> dCakesWithTwoKeys = new Dictionary<string, Dictionary<string, Dictionary<string, CakeClient>>>();
		private static Dictionary<string, Dictionary<string, CakeClient>> dCakes = new Dictionary<string, Dictionary<string, CakeClient>>();
		public static CakeClient GetCake(string cakeType, string id, string iid = "")
		{
			if (!string.IsNullOrEmpty(iid))
				if (dCakesWithTwoKeys.ContainsKey(id) && dCakesWithTwoKeys[id].ContainsKey(iid) && dCakesWithTwoKeys[id][iid].ContainsKey(cakeType))
					return dCakesWithTwoKeys[id][iid][cakeType];
				else if (dCakes.ContainsKey(id) && dCakes[id].ContainsKey(cakeType))
					return dCakes[id][cakeType];
			return new CakeClient(cakeType, id, iid);
		}
		public static void AddCake(CakeClient cake)
		{
			if (string.IsNullOrEmpty(cake.iid))
			{
				if (!dCakes.ContainsKey(cake.id))
					dCakes.Add(cake.id, new Dictionary<string, CakeClient>());
				if (dCakes[cake.id].ContainsKey(cake.cakeType))
					dCakes[cake.id][cake.cakeType] = cake;
				else
					dCakes[cake.id].Add(cake.cakeType, cake);
			}
			else
			{
				if (!dCakesWithTwoKeys.ContainsKey(cake.id))
					dCakesWithTwoKeys.Add(cake.id, new Dictionary<string, Dictionary<string, CakeClient>>());
				if (!dCakesWithTwoKeys[cake.id].ContainsKey(cake.iid))
					dCakesWithTwoKeys[cake.id].Add(cake.iid, new Dictionary<string, CakeClient>());
				if (dCakesWithTwoKeys[cake.id][cake.iid].ContainsKey(cake.cakeType))
					dCakesWithTwoKeys[cake.id][cake.iid][cake.cakeType] = cake;
				else
					dCakesWithTwoKeys[cake.id][cake.iid].Add(cake.cakeType, cake);
			}

		}
	}
}
