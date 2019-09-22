using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.Networking;

public class ConfigDownloader : MonoBehaviour
{
	private static ConfigDownloader s_instance;
	public static ConfigDownloader Instance
	{
		get
		{
			if (s_instance == null)
			{
				s_instance = MonoInstancePool.getInstance<ConfigDownloader>(true);
			}
			return s_instance;
		}
	}
	void Start()
	{
		s_instance = this;
	}

	private static Action delDownloadComplete;
	private static Action delDownloadFailed;
	public void StartToDownload(string configUrl, Action del, Action failedDel)
	{
		delDownloadComplete = del;
		delDownloadFailed = failedDel;

		StartCoroutine(DownloadConfig(configUrl));
	}
	Dictionary<string, string> dKeyValue = new Dictionary<string, string>();
	public static int maxRetryTime = 10;
	private static int retried = 0;
	IEnumerator DownloadConfig(string configUrl)
	{
		var url = configUrl + "?" + ApiDateTime.SecondsFromBegin();
		var www = new WWW(url);
		StartCoroutine(OnTimeout(www));
		yield return www;
		if (!string.IsNullOrEmpty(www.error))
		{
			delDownloadFailed?.Invoke();
		}
		else
		{
			StopAllCoroutines();

			var text = www.text;
			string[] alines = text.Split(new string[] { "\r", "\n" }, System.StringSplitOptions.RemoveEmptyEntries);
			for (int i = 0; i < alines.Length; i++)
			{
				if (alines[i].StartsWith("#") || alines[i].StartsWith("//"))
				{
					continue;
				}
				string[] akeyvalue = alines[i].Split(new char[] { '=' }, 2);
				if (!dKeyValue.ContainsKey(akeyvalue[0]))
					dKeyValue.Add(akeyvalue[0], akeyvalue[1]);
			}

			if (delDownloadComplete != null)
			{
				delDownloadComplete();
			}
			UEventListener.Instance.DispatchEvent(UEvents.DownloadCompleteEvent, true);
		}
		www.Dispose();
	}
	IEnumerator OnTimeout(WWW www)
	{
		yield return new WaitForSeconds(2);

		if (!www.isDone)
		{
			if (delDownloadFailed != null)
			{
				delDownloadFailed();
			}
			retried = maxRetryTime;
			delDownloadFailed = null;
			delDownloadComplete = null;
		}
	}
	public string OnGetValue(string skey)
	{
		if (dKeyValue.ContainsKey($"{skey}{UStaticFuncs.GetPlatformFolder(Application.platform)}{Utils_Plugins.Util_GetBundleVersion()}"))
		{
			return dKeyValue[$"{skey}{UStaticFuncs.GetPlatformFolder(Application.platform)}{Utils_Plugins.Util_GetBundleVersion()}"];
		}
		if (dKeyValue.ContainsKey($"{skey}{UStaticFuncs.GetPlatformFolder(Application.platform)}"))
		{
			return dKeyValue[$"{skey}{UStaticFuncs.GetPlatformFolder(Application.platform)}"];
		}
		if (dKeyValue.ContainsKey($"{skey}{Utils_Plugins.Util_GetBundleVersion()}"))
		{
			return dKeyValue[$"{skey}{Utils_Plugins.Util_GetBundleVersion()}"];
		}
		if (dKeyValue.ContainsKey(skey))
		{
			return dKeyValue[skey];
		}
		return "";
	}
	public int OnGetIntValue(string skey)
	{
		return typeParser.intParse(OnGetValue(skey));
	}
	public float OnGetFloatValue(string skey)
	{
		return typeParser.floatParse(OnGetValue(skey));
	}
}
