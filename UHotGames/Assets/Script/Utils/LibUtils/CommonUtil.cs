using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Text.RegularExpressions;

public class CommonUtil
{
	//验证给定的字符串inputGuid是否为合法的Guid
	public static bool IsGuidValid(string inputGuid)
	{
		bool isGuid = false;
		string pattern = @"^[0-9a-fA-F]{8}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{4}\-[0-9a-fA-F]{12}$";
		if (inputGuid != null && !inputGuid.Equals(""))
		{
			isGuid = Regex.IsMatch(inputGuid, pattern);
		}
		return isGuid;
	}

	//把一组Guid转成以逗号分割的字符串
	public static string ConvertGuidToString(List<Guid> vGuids)
	{
		StringBuilder guids = new StringBuilder("");
		foreach (var g in vGuids)
		{
			guids.Append(String.Format(",'{0}'", g.ToString()));
		}
		if (guids.Length > 0)
		{
			return guids.Remove(0, 1).ToString();
		}
		return null;
	}

	//CollectionToString([1,2,3,4,5], ",") ===> "1,2,3,4,5"
	public static string CollectionToString(IEnumerable<long> coll, string separator)
	{
		string[] result = new String[coll.Count()];
		int i = 0;
		foreach (var item in coll)
		{
			result[i] = item.ToString();
			i++;
		}
		return string.Join(separator, result);
	}

	public static int[] StringToArray(string sContent, char cSeparator)
	{
		string[] tempArray = sContent.Split(cSeparator);
		int[] result = new int[tempArray.Length];
		for (int i = 0; i < tempArray.Length; i++)
		{
			result[i] = typeParser.intParse(tempArray[i]);
		}
		return result;
	}

	//取得所给字符串的长度，一个汉字等于两个长度
	public static int GetStringLength(string sStr)
	{
		return System.Text.UnicodeEncoding.Default.GetByteCount(sStr);
	}

	//计算32位无符号整数对应二进制中1的个数
	public static UInt32 BitCountInUint32(UInt32 iParam)
	{
		UInt32 iCount = 0;
		while (0 < iParam)
		{
			iParam &= (iParam - 1);
			iCount++;
		}
		return iCount;
	}

}
