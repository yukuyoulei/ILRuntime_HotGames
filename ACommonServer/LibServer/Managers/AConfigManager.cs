using System;
using System.Collections.Generic;
using System.Text;

namespace LibServer.Managers
{
	public class AConfigManager : Singleton<AConfigManager>
	{
		public MapData OnGetMapData(string wuxing, int level)
		{
			if (level < 1) level = 1;
			switch (wuxing)
			{
				case "jin":
					return MapLoader.Instance.OnGetData(JinLoader.Instance.OnGetData(level).Name);
				case "shui":
					return MapLoader.Instance.OnGetData(ShuiLoader.Instance.OnGetData(level).Name);
				case "mu":
					return MapLoader.Instance.OnGetData(MuLoader.Instance.OnGetData(level).Name);
				case "huo":
					return MapLoader.Instance.OnGetData(HuoLoader.Instance.OnGetData(level).Name);
				case "tu":
					return MapLoader.Instance.OnGetData(TuLoader.Instance.OnGetData(level).Name);
			}
			return null;
		}
	}
}
