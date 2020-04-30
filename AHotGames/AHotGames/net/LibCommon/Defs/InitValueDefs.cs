using System.Collections.Generic;

namespace LibCommon
{
	public static class InitValueDefs
	{
		public static Dictionary<string, string> wuxing = new Dictionary<string, string> {
			{"jin", "金" }, {"shui", "水"}, {"mu", "木"}, {"huo", "火"}, {"tu", "土"}
		};

		public static string dbconnect;
		public static string dbname;

		public static int money = 1;
		public static int gold = 2;

		public static int CityID = 1;
	}

	public enum ECommonMethod
	{
		EnterConta = 1,
		EnterScene = 2,
		BeginFight = 3,
	}

}
