#if ILRuntime
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class ConfigManager : Singleton<ConfigManager>
{
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
		RegistLoadFunc("Jin", JinLoader.Instance.OnLoadContent);
		RegistLoadFunc("Shui", ShuiLoader.Instance.OnLoadContent);
		RegistLoadFunc("Mu", MuLoader.Instance.OnLoadContent);
		RegistLoadFunc("Huo", HuoLoader.Instance.OnLoadContent);
		RegistLoadFunc("Tu", TuLoader.Instance.OnLoadContent);
		RegistLoadFunc("Map", MapLoader.Instance.OnLoadContent);
		RegistLoadFunc("Items", ItemsLoader.Instance.OnLoadContent);
		RegistLoadFunc("Payment", PaymentLoader.Instance.OnLoadContent);
		RegistLoadFunc("DailyCheck", DailyCheckLoader.Instance.OnLoadContent);
	}
	public ConfigManager()
	{
		RegistAllLoadFuncs();
	}

	public void DownloadConfig(Action downloadConfigComplete)
	{
		var ks = dExcelLoaders.Keys.ToArray();
		var l = new List<string>();
		foreach (var k in ks)
		{
			l.Add($"txt/{k}");
		}
		UHotAssetBundleLoader.Instance.OnDownloadResources(() =>
		{
			foreach (var k in dExcelLoaders.Keys)
			{
				var tb = "txt/" + k;
				var ta = UHotAssetBundleLoader.Instance.OnLoadAsset<TextAsset>(tb);
				if (ta != null)
					dExcelLoaders[k].Invoke(ta.text);
				else
					AOutput.Log($"Load config invalid {tb}");
			}
			downloadConfigComplete();
		}, l.ToArray());
	}
}
#else
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

public class ConfigManager : Singleton<ConfigManager>
{
	public void Init()
	{
		JinLoader.Instance.OnLoadContent(GetContent("Jin"));
		ShuiLoader.Instance.OnLoadContent(GetContent("Shui"));
		MuLoader.Instance.OnLoadContent(GetContent("Mu"));
		HuoLoader.Instance.OnLoadContent(GetContent("Huo"));
		TuLoader.Instance.OnLoadContent(GetContent("Tu"));
		MapLoader.Instance.OnLoadContent(GetContent("Map"));
		ItemsLoader.Instance.OnLoadContent(GetContent("Items"));
		PaymentLoader.Instance.OnLoadContent(GetContent("Payment"));
		DailyCheckLoader.Instance.OnLoadContent(GetContent("DailyCheck"));
	}
	private string GetContent(string file)
	{
		return File.ReadAllText(GetPath(file));
	}
	private string GetPath(string file)
	{
		return "./Configs/txt/" + file + ".txt";
	}
}
#endif
