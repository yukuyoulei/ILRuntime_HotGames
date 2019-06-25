using ILRuntime.Runtime.Intepreter;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using ILRuntime.Runtime.Stack;
using ILRuntime.CLR.Method;
using ILRuntime.CLR.TypeSystem;
using System;
using System.Linq;

public class Enter : MonoBehaviour
{
	public static string ConfigURL = "http://www.fscoding2019.top/hotgame/Config.txt";
	public bool UseAB;
	private void Start()
	{
		gameObject.AddComponent<UConsoleDebug>();

		var fa = "fa" + Utils_Plugins.Util_GetBundleVersion();
		AOutput.Log($"local version {fa}");
		if (!PlayerPrefs.HasKey(fa))
		{
			AOutput.Log($"start copy files {fa}");
			MonoInstancePool.getInstance<UCopyFilesFromStreamingAssets>().OnCopy("versions.txt", UStaticFuncs.ConfigSaveDir, () =>
			{
				InitRemoteConfig();
				PlayerPrefs.SetInt("fa" + Utils_Plugins.Util_GetBundleVersion(), 1);
			});
		}
		else
		{
			InitRemoteConfig();
		}
	}

	private void InitRemoteConfig()
	{
		AOutput.Log("InitRemoteConfig");
		ConfigDownloader.Instance.StartToDownload(ConfigURL, () =>
		{
			AOutput.Log($"ConfigURL {ConfigURL}");
			ParseConfigs();
			if (UConfigManager.bUsingAb)
			{
				StartCoroutine(OnDownloadDll(ConfigDownloader.Instance.OnGetValue("dll")));
			}
			else
			{
				LoadDll(File.ReadAllBytes("Assets/RemoteResources/Dll/ahotgames.bytes")
					, File.ReadAllBytes("Assets/RemoteResources/Dll/ahotgames.pdb"));
			}

		}, () => { Debug.LogError("下载配置文件失败。"); });

	}

	private void ParseConfigs()
	{
		UConfigManager.bUsingAb = ConfigDownloader.Instance.OnGetIntValue("useab") == 1;

#if UNITY_EDITOR
		UConfigManager.bUsingAb = UseAB;
#endif
	}

	IEnumerator OnDownloadDll(string dllPath)
	{
		AOutput.Log($"dllPath {dllPath}");
		var www = new WWW(dllPath);
		yield return www;
		if (string.IsNullOrEmpty(www.error))
		{
			var dllBytes = www.bytes;
			www = new WWW(dllPath.Replace(".bytes", ".pdb"));
			AOutput.Log($"www {www.url}");
			yield return www;
			LoadDll(dllBytes, www.bytes);
		}
		else
		{
			AOutput.Log($"www {www.url} error {www.error}");
		}
	}

	private void LoadDll(byte[] bytes, byte[] pdbBytes)
	{
		ILRuntimeHandler.Instance.DoLoadDll("ahotmages", bytes, pdbBytes);

		ILRuntimeHandler.Instance.SetUnityMessageReceiver(MonoInstancePool.getInstance<UEmitMessage>(true).gameObject);

		ILRuntimeHandler.Instance.OnLoadClass("AEntrance", new GameObject("AEntrance"), false, UConfigManager.bUsingAb.ToString());
	}
}
