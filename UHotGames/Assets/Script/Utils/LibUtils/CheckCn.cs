using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public class CheckCn
{
	public static bool isHasChzn_C(string str)
	{
		byte[] strASCII = System.Text.ASCIIEncoding.ASCII.GetBytes(str);
		int tmpNum = 0;
		for (int i = 0; i < str.Length; i++)
		{
			//中文检查
			if ((int)strASCII[i] >= 63 && (int)strASCII[i] < 91)
			{

				tmpNum += 2;
			}
		}
		if (tmpNum > 2)
		{
			return true;
		}
		else
		{
			return false;
		}
	}
}
