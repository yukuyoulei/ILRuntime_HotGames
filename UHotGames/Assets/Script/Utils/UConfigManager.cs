using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class UConfigManager : MonoBehaviour
{
	private static UConfigManager mInstance;
	public static UConfigManager Instance
	{
		get
		{
			if (mInstance == null)
			{
				GameObject go = new GameObject("UConfigManager");
				DontDestroyOnLoad(go);
				mInstance = go.AddComponent<UConfigManager>();
			}
			return mInstance;
		}
	}

    public static bool bUsingAb { get; set; }
	private Dictionary<string, Action<string>> dExcelLoaders = new Dictionary<string, Action<string>>();
	private void RegistLoadFunc(string sExcelName, Action<string> func)
	{
		if (bUsingAb)
		{
			sExcelName = sExcelName.ToLower();
		}
		dExcelLoaders.Add(sExcelName, func);
	}
	private void RegistAllLoadFuncs()
	{
		/*RegistLoadFunc("Levels", ULoaderLevel.Instance.LoadData);
		RegistLoadFunc("RoleTemplate", ULoaderRoleTemplate.Instance.LoadData);
		RegistLoadFunc("Localization", ULoaderLocalization.Instance.LoadData);
		RegistLoadFunc("XxlUnit", UXxlUnit.Instance.LoadData);
		RegistLoadFunc("MathLesson", ULoaderMathLesson.Instance.LoadData);*/
	}
	void Awake()
	{
		mInstance = this;
		RegistAllLoadFuncs();
	}

	public void DownloadConfig(Action downloadConfigComplete)
	{
		//UILoading.SetLoadingContent("正在加载远端配置表。");
		UAssetBundleDownloader.Instance.DownloadResources((list) =>
		{
			foreach (var str in list)
			{
				var assetName = UStaticFuncs.GetAssetBundleName(str);
				if (!dExcelLoaders.ContainsKey(assetName))
				{
					continue;
				}
				dExcelLoaders[assetName](UAssetBundleDownloader.Instance.OnLoadAsset<TextAsset>(str).text);
			}

			downloadConfigComplete?.Invoke();
		}
		, null, filter, dExcelLoaders.Keys.ToArray());
	}

	private bool filter(string arg)
	{
		return arg.StartsWith("configs/");
	}

	public void LoadLocalConfig()
	{
		Debug.Log("正在加载本地配置表");
		foreach (var s in dExcelLoaders.Keys)
		{
			var str = s;
#if UNITY_EDITOR
			if (!bUsingAb)
			{
				str += ".txt";
			}
#endif
			var text = UAssetBundleDownloader.Instance.OnLoadAsset<TextAsset>("Configs/" + str);
			if (text == null)
			{
				Debug.Log("Cannot load " + s + " from local");
				continue;
			}
			dExcelLoaders[s](text.text);
		}
	}
}

