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
using UnityEngine.UI;

public class Enter : MonoBehaviour
{
	public static string ConfigURL = "http://www.fscoding2019.top/hotgame/Config.txt";
	public bool UseAB;
	Transform UILoading;
	Text textProgress;
	Text jindu;
	Image SliderProgress;
	private void Start()
	{
		gameObject.AddComponent<UConsoleDebug>();

#if !UNITY_WEBGL
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
#endif
			InitRemoteConfig();
#if !UNITY_WEBGL
		}
#endif
	}

	private void InitProgressSlider()
	{
		UILoading = UAssetBundleDownloader.Instance.OnLoadAsset<Transform>("ui/uiloading");
		Debug.Log($"UILoading {UILoading}");
		if (UILoading == null)
			UILoading = UStaticFuncs.FindChildComponent<Transform>(transform, "UILoading");
		if (UILoading != null)
		{
			UILoading.gameObject.SetActive(true);

			UILoading.GetComponent<Canvas>().sortingOrder = -1;
			SliderProgress = UStaticFuncs.FindChildComponent<Image>(UILoading, "SliderProgress");
			textProgress = UStaticFuncs.FindChildComponent<Text>(UILoading, "textProgress");
			jindu = UStaticFuncs.FindChildComponent<Text>(UILoading, "jindu");
			SetJindu("正在初始化，请稍候");
		}
	}

	private void InitRemoteConfig()
	{
		SetJindu("正在检查本地资源版本，请稍候");

		AOutput.Log("InitRemoteConfig");
		ConfigDownloader.Instance.StartToDownload(ConfigURL, () =>
		{
			ParseConfigs();
			if (UConfigManager.bUsingAb)
			{
				SetJindu("正在加载脚本资源......");
				StartCoroutine(OnDownloadDll(ConfigDownloader.Instance.OnGetValue("dll")));
			}
			else
			{
				SetJindu("正在加载脚本资源...");
				LoadDll(File.ReadAllBytes("Assets/RemoteResources/Dll/ahotgames.bytes")
					, File.ReadAllBytes("Assets/RemoteResources/Dll/ahotgames.pdb"));
			}

		}, () =>
		{
			SetJindu("正在加载本地资源，请稍候");
			Debug.Log("下载远端配置文件失败，加载本地文件");
			UConfigManager.bUsingAb = false;
			StartCoroutine(OnDownloadDll(UStaticFuncs.GetStreamAssetPath() + UStaticFuncs.GetPlatformFolder(Application.platform) + "/dll/ahotgames.ab"));
		});

	}
	float fPercent;
	void SetJindu(string str, float percent = 0.1f)
	{
		if (jindu != null)
		{
			jindu.text = str;
		}
		fPercent += percent;
		if (fPercent > 1)
		{
			fPercent = 1;
		}
		if (textProgress != null)
		{
			textProgress.text = $"{(fPercent * 100).ToString("f0")}%";
		}
	}

	private void ParseConfigs()
	{
		UConfigManager.bUsingAb = ConfigDownloader.Instance.OnGetIntValue("useab") == 1;

#if UNITY_EDITOR
		UConfigManager.bUsingAb = UseAB;
#endif
	}

	WWW www;
	IEnumerator OnDownloadDll(string dllPath)
	{
		AOutput.Log($"dllPath {dllPath}");
		www = new WWW(dllPath);
		yield return www;
		if (string.IsNullOrEmpty(www.error))
		{
			if (dllPath.EndsWith(".ab"))
			{
				LoadDll(www.assetBundle.LoadAsset<TextAsset>("ahotgames").bytes, null);
			}
			else
			{
				var dllBytes = www.bytes;
#if UNITY_EDITOR
				www = new WWW(dllPath.Replace(".bytes", ".pdb"));
				AOutput.Log($"www {www.url}");
				yield return www;
				LoadDll(dllBytes, www.bytes);
#else
                LoadDll(dllBytes, null);
#endif
			}
		}
		else
		{
			AOutput.Log($"www {www.url} error {www.error}");
		}
		www = null;
	}

	private void Update()
	{
		if (UILoading == null || www == null)
		{
			return;
		}
		if (SliderProgress != null)
		{
			SliderProgress.fillAmount = fPercent;
		}
	}

	private void LoadDll(byte[] bytes, byte[] pdbBytes)
	{
		SetJindu("正在初始化运行环境，请稍候");

		StartCoroutine(DelayLoadDll(bytes, pdbBytes));
	}
	IEnumerator DelayLoadDll(byte[] bytes, byte[] pdbBytes)
	{
		yield return new WaitForEndOfFrame();

		ILRuntimeHandler.Instance.DoLoadDll("ahotgames", bytes, pdbBytes);

		ILRuntimeHandler.Instance.SetUnityMessageReceiver(MonoInstancePool.getInstance<UEmitMessage>(true).gameObject);

		ILRuntimeHandler.Instance.OnLoadClass("AEntrance", new GameObject("AEntrance"), false, UConfigManager.bUsingAb.ToString());

	}
}
