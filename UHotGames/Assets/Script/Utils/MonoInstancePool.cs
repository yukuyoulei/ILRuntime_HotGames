using System.Collections.Generic;
using UnityEngine;

public static class MonoInstancePool
{
	private static Dictionary<string, Component> sInstancePool = new Dictionary<string, Component>();

	//创建单场景唯一脚本实例
	public static T getInstance<T>(bool bDontDestroy = false, string sGameObjectName = null)
		where T : Component
	{
		string sFullName = typeof(T).FullName;
		if (sGameObjectName != null)
		{
			sFullName = sGameObjectName;
		}

		if (sInstancePool.ContainsKey(sFullName))
		{
			if (sInstancePool[sFullName] != null)
			{
				return sInstancePool[sFullName] as T;
			}
			else
			{
				sInstancePool.Remove(sFullName);
			}
		}


		T tInstance = CreateNewInstance<T>(bDontDestroy, sGameObjectName) as T;
		sInstancePool.Add(sFullName, tInstance);

		return sInstancePool[sFullName] as T;
	}

	private static GameObject _pool;
	private static GameObject pool
	{
		get
		{
			if (_pool == null)
			{
				_pool = new GameObject("MonoInstancePool");
				GameObject.DontDestroyOnLoad(_pool);
			}
			return _pool;
		}
	}
	private static T CreateNewInstance<T>(bool bDontDestroy = false, string name = null) where T : Component
	{
		string objName = typeof(T).FullName;
		if (name != null)
		{
			objName = name;
		}
		GameObject obj = GameObject.Find(objName);
		if (obj == null)
		{
			obj = new GameObject();
		}
		obj.name = objName;

		if (bDontDestroy)
		{
			obj.transform.parent = pool.transform;
		}

		return obj.AddComponent<T>() as T;
	}
}