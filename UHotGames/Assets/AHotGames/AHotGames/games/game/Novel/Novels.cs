using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public static class Novels
{
	static Dictionary<int, Dictionary<int, string>> _contents;
	static Dictionary<int, Dictionary<int, string>> contents
	{
		get
		{
			if (_contents == null)
			{
				_contents = new Dictionary<int, Dictionary<int, string>>();
				AddContent(0, "2050年，由于地球人口的急剧膨胀，粮食危机先后在各国爆发。");
				AddContent(0, "先是一些地少人多的小国，渐渐的，蔓延到了各个较大国家，最终演变成为一场全球危机。");
				AddContent(0, "各国领导人都在本国集思广益，寻求能够平稳度过这场危机的办法。");
				AddContent(0, "然而，随着时间的推移，各种办法都被提出，然后被一一否决。");
				AddContent(0, "最终……");
				AddContent(1, "是夜……");
			}
			return _contents;
		}
	}

	private static void AddContent(int section, string content)
	{
		if (!_contents.ContainsKey(section))
		{
			_contents.Add(section, new Dictionary<int, string>());
		}
		_contents[section].Add(_contents[section].Count, content);
	}

	internal static string GetContent(int iCurSection, int iCurPage)
	{
		if (contents.ContainsKey(iCurSection) && contents[iCurSection].ContainsKey(iCurPage))
		{
			return contents[iCurSection][iCurPage];
		}
		return null;
	}

	internal static bool IsLastPage(int iCurSection, int iCurPage)
	{
		return contents.ContainsKey(iCurSection) && contents[iCurSection].ContainsKey(iCurPage) && contents[iCurSection].Count == iCurPage + 1;
	}
}
