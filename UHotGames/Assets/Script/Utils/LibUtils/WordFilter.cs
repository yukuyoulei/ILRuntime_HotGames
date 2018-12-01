using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

public abstract class WordFilter
{
	public static string[] s_filters = null;

	/// <summary>
	/// 初始化s_filters之后调用filter函数
	/// </summary>
	/// <param name="content">欲过滤的内容</param>
	/// <param name="result_str">执行过滤之后的内容</param>
	/// <param name="filter_deep">检测深度，即s_filters数组中的每个词中的插入几个字以内会被过滤掉，例：检测深度为2，s_filters中有个词是中国，那么“中国”、“中*国”，“中**国”都会被过滤掉（*是任意字）。</param>
	/// <param name="check_only">是否只检测而不执行过滤操作</param>
	/// <param name="bTrim">过滤之前是否要去掉头尾的空字符</param>
	/// <param name="replace_str">将检测到的敏感字替换成的字符</param>
	/// <returns></returns>
	public static bool filter(string content, out string result_str, int filter_deep = 1, bool check_only = false, bool bTrim = false, string replace_str = "*", string[] filters = null)
	{
		string[] using_filter = filters;
		if (using_filter == null)
		{
			using_filter = s_filters;
		}

		string result = content;
		if (bTrim)
		{
			result = result.Trim();
		}
		result_str = result;

		if (using_filter == null)
		{
			return false;
		}

		bool check = false;
		foreach (string str in using_filter)
		{
			string s = str.Replace(replace_str, "");
			if (s.Length == 0)
			{
				continue;
			}

			bool bFiltered = true;
			while (bFiltered)
			{
				int result_index_start = -1;
				int result_index_end = -1;
				int idx = 0;
				while (idx < s.Length)
				{
					string one_s = s.Substring(idx, 1);
					if (one_s == replace_str)
					{
						continue;
					}
					if (result_index_end + 1 >= result.Length)
					{
						bFiltered = false;
						break;
					}
					int new_index = result.IndexOf(one_s, result_index_end + 1, StringComparison.OrdinalIgnoreCase);
					if (new_index == -1)
					{
						bFiltered = false;
						break;
					}
					if (idx > 0 && new_index - result_index_end > filter_deep + 1)
					{
						bFiltered = false;
						break;
					}
					result_index_end = new_index;

					if (result_index_start == -1)
					{
						result_index_start = new_index;
					}
					idx++;
				}

				if (bFiltered)
				{
					if (check_only)
					{
						return true;
					}
					check = true;
					string result_left = result.Substring(0, result_index_start);
					for (int i = result_index_start; i <= result_index_end; i++)
					{
						result_left += replace_str;
					}
					string result_right = result.Substring(result_index_end + 1);
					result = result_left + result_right;
				}
			}
		}
		result_str = result;
		return check;
	}
}
