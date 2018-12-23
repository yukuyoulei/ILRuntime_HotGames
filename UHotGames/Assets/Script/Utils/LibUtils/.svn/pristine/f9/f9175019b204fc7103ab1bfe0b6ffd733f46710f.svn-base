using System;
using System.Collections.Generic;

public class ApiRandom
{
	public static Random Instance
	{
		get
		{
			return m_rand;
		}
	}
	private static Random m_rand = new Random();

	/// <summary>
	/// 取得一组指定个数的不重复随机整数，从minValue到maxValue，包括minValue和maxValue
	/// </summary>
	/// <param name="minValue"></param>
	/// <param name="maxValue"></param>
	/// <param name="count"></param>
	/// <returns></returns>
	public static int[] GetRandom(int minValue, int maxValue, int count, int? exceptValue = null)
	{
		maxValue = maxValue - minValue + 1;
		int[] intList = new int[maxValue];
		int n = maxValue;
		for (int i = 0; i < maxValue; i++)
		{
			if (exceptValue.HasValue 
				&& i + minValue == exceptValue.Value)
			{
				n--;
				continue;
			}
			intList[i] = i + minValue;
		}
		int[] intRet = new int[count];
		for (int i = 0; i < count; i++)
		{
			int index = ApiRandom.Instance.Next(0, n);
			intRet[i] = intList[index];
			intList[index] = intList[--n];
		}

		return intRet;
	}

    public static T RandomOne<T>(List<T> list) where T : RandomObject
    {
        int totalWeight = 0;
        T selected = default(T);
        foreach (var data in list)
        {
            int weight = data.Weight;
            //Random ran = new Random(GetRandomSeed());  //GetRandomSeed()防止快速频繁调用导致随机一样的问题
			int r = m_rand.Next(totalWeight + weight + 1);
            if (r >= totalWeight)
                selected = data;
            totalWeight += weight + 1;
        }
        return selected;
    }

    public static List<T> Old_RandomList<T>(List<T> list, int iCount) where T : RandomObject
    {
        List<T> result = new List<T>();
        int totalWeight = 0;
        foreach (var data in list)
        {
            int weight = data.Weight;
            //Random ran = new Random(GetRandomSeed());  //GetRandomSeed()防止快速频繁调用导致随机一样的问题
            int r = m_rand.Next(totalWeight + weight);
            if (r >= totalWeight)
            {
                result.Insert(0, data);
            }
            else
            {
                result.Add(data);
            }
            totalWeight += weight;
        }
        int iResultNum = iCount > result.Count ? result.Count : iCount;
        return result.GetRange(0, iResultNum);
    }

    public static List<T> RandomList<T>(List<T> list, int iCount,bool bCanCountEqual = false) where T : RandomObject
    {
        if (iCount > list.Count) return list;
		if (!bCanCountEqual && iCount == list.Count) return list;
        List<T> result = new List<T>();
        Dictionary<int, T> resultDict = new Dictionary<int, T>();
        while (result.Count < iCount && resultDict.Count < iCount)
        {
            T randomOne = RandomOne(list);
            if (!resultDict.ContainsKey(randomOne.GetHashCode()))
            {
                resultDict.Add(randomOne.GetHashCode(), randomOne);
                result.Add(randomOne);
            }
        }
        return result;
    }

	/// <summary>
	/// 根据权重随机选取指定条数记录
	/// 算法：
	/// 1.每个项权重+1命名为w，防止为0情况。
	/// 2.计算出总权重n。
	/// 3.每个项权重w加上从0到(n-1)的一个随机数（即总权重以内的随机数），得到新的权重排序值s。
	/// 4.根据得到新的权重排序值s进行排序，取前面s最大几个。
	/// </summary>
	/// <param name="list">原始列表</param>
	/// <param name="count">随机抽取条数</param>
	/// <returns></returns>
	public static List<T> GetRandomList<T>(List<T> list, int count, bool bCanCountEqual = false) where T : RandomObject
	{
		if (list == null || list.Count < count || count <= 0)
		{
			return list;
		}
		if (!bCanCountEqual && list.Count == count)
		{
			return list;
		}

		//计算权重总和
		int totalWeights = 0;
		for (int i = 0; i < list.Count; i++)
		{
			totalWeights += list[i].Weight + 1;  //权重+1，防止为0情况。
		}

		//随机赋值权重
		//Random ran = new Random(GetRandomSeed());  //GetRandomSeed()防止快速频繁调用导致随机一样的问题 
		List<KeyValuePair<int, int>> wlist = new List<KeyValuePair<int, int>>();    //第一个int为list下标索引、第二个int为权重排序值
		for (int i = 0; i < list.Count; i++)
		{
			int w = (list[i].Weight + 1) + m_rand.Next(0, totalWeights);   // （权重+1） + 从0到（总权重-1）的随机数
			wlist.Add(new KeyValuePair<int, int>(i, w));
		}

		//排序
		wlist.Sort(
		  delegate(KeyValuePair<int, int> kvp1, KeyValuePair<int, int> kvp2)
		  {
			  return kvp2.Value - kvp1.Value;
		  });

		//根据实际情况取排在最前面的几个
		List<T> newList = new List<T>();
		for (int i = 0; i < count; i++)
		{
			T entiy = list[wlist[i].Key];
			newList.Add(entiy);
		}

		return newList;
	}


	/// <summary>
	/// 随机种子值
	/// </summary>
	/// <returns></returns>
/*
	private static int GetRandomSeed()
	{
		byte[] bytes = new byte[4];
		System.Security.Cryptography.RNGCryptoServiceProvider rng = new System.Security.Cryptography.RNGCryptoServiceProvider();
		rng.GetBytes(bytes);
		return BitConverter.ToInt32(bytes, 0);
	}
*/
}

/// <summary>
/// 权重对象
/// </summary>
public class RandomObject
{
	/// <summary>
	/// 权重
	/// </summary>
	public int Weight { set; get; }
}

