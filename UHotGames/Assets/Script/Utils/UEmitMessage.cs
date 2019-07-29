using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class UEmitMessage : MonoBehaviour
{
	public void EmitMessage(string msg)
	{
		Debug.Log("Receive message " + msg);
		var amsg = msg.Split(new char[] { '|' });
		if (amsg[0] == "load")
		{
			var obj = UAssetBundleDownloader.Instance.OnLoadAsset<GameObject>(amsg[2]);
			if (!obj)
			{
				obj = new GameObject(amsg[1]);
			}
			else
			{
				obj = Instantiate(obj);
			}
			ILRuntimeHandler.Instance.OnLoadClass(amsg[1], obj);
		}
		else if (amsg[0] == "loadprefab")
		{
			var obj = UAssetBundleDownloader.Instance.OnLoadAsset<GameObject>(amsg[1]);
			if (!obj)
			{
				obj = new GameObject(amsg[1]);
			}
			else
			{
				obj = Instantiate(obj);
			}
			ILRuntimeHandler.Instance.EmitGameObject(amsg[1], obj, amsg.Length > 2 ? amsg[2] : "");
		}
		else if (amsg[0] == "unloadall")
		{
			ILRuntimeHandler.Instance.OnUnloadAllClasses();
		}
		else if (amsg[0] == "invoke")
		{
			var ainvoke = amsg[1].Split(new char[] { ':' }, 3);
			if (ainvoke.Length >= 2)
			{
				var objname = ainvoke[0];
				var obj = MonoInstancePool.OnGetInstance(objname);
				if (obj == null)
				{
					var aobj = objname.Split('/');
					if (aobj.Length == 2)
					{
						obj = GameObject.Find(aobj[0]);
						if (obj != null)
						{
							var tr = UStaticFuncs.FindChildComponent<Transform>(obj.transform, aobj[1]);
							if (tr != null)
							{
								obj = tr.gameObject;
							}
						}
					}
					else
					{
						obj = GameObject.Find(objname);
					}
					if (obj == null)
					{
						Debug.Log($"Cannot find obj {objname}");
						return;
					}
				}
				if (ainvoke.Length == 2)
				{
					obj.SendMessage(ainvoke[1]);
				}
				else
				{
					obj.SendMessage(ainvoke[1], ainvoke[2]);
				}
			}
			else
			{
				Debug.Log($"Invoke 参数数量错误 {msg}");
			}
		}
	}
}
