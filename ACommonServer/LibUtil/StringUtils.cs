using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LibUtils
{
	public abstract class StringUtils
	{
		public static int GetRealLength(string str)
		{
			return System.Text.Encoding.GetEncoding("GB2312").GetByteCount(str);
		}
		public static string GetGUIDString()
		{
			return Guid.NewGuid().ToString().Replace("-", "");
		}
	}
}
