using System.Collections.Generic;
using System.IO;

public class CSCSV
{
	public static bool OpenCSVFile(ref List<string[]> mycsvdt, string filepath)
	{
		string strpath = filepath; //csv文件的路径
		StreamReader mysr = new StreamReader(strpath, System.Text.Encoding.UTF8);
		ParseFile(ref mycsvdt, mysr.ReadToEnd());
		mysr.Close();
		return true;

	}
	public static void ParseFile(ref List<string[]> mycsvdt, string sContent)
	{
		string[] aline = sContent.Split(new char[] { '\r', '\n' }, System.StringSplitOptions.RemoveEmptyEntries);
		foreach (string strline in aline)
		{
			string sres = strline.Replace("\"", "");
			mycsvdt.Add(sres.Split(new char[] { '\t' }));
		}
	}
	public static void ParseFile(ref List<string[]> mycsvdt, string sContent, int countLimit, params char[] spliters)
	{
		string[] aline = sContent.Split(new char[] { '\r', '\n' }, System.StringSplitOptions.RemoveEmptyEntries);
		foreach (string strline in aline)
		{
			string sres = strline.Replace("\"", "");
			mycsvdt.Add(sres.Split(spliters, countLimit));
		}
	}
}

