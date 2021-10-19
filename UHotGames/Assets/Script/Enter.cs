using System.Collections;
using System.IO;
using UnityEngine;
using System.Linq;
using System.Collections.Generic;

public class Enter : MonoBehaviour
{
	public static bool bUsingLocalCDN
	{
		get
		{
			if (!PlayerPrefs.HasKey("USE_LOCAL_CDN"))
				return false;
			return PlayerPrefs.GetInt("USE_LOCAL_CDN") == 1;
		}
	}
	public static string ConfigURL
	{
		get
		{
			return bUsingLocalCDN
				? "http://127.0.0.1/hotgame/Config.txt"
				: "https://yukuyoulei.github.io/ILRuntime_HotGames/ab1/Config.txt";
		}
	}
	private Transform trUIAlert;
	public static bool bUsingAb
	{
		get
		{
			if (!PlayerPrefs.HasKey("UseAB"))
				return true;
			return PlayerPrefs.GetInt("UseAB") != 0;
		}
		set
		{
			PlayerPrefs.SetInt("UseAB", value ? 1 : 0);
		}
	}

	private void Start()
	{
#if UNITY_ANDROID
		MonoInstancePool.getInstance<SDK_Orientation>(true).ShowBar();
#endif
		Screen.fullScreen = true;
		MonoInstancePool.getInstance<AntiScriptSplit>(true);
		MonoInstancePool.getInstance<SDK_WeChat>(true);

#if UNITY_IOS
		MonoInstancePool.getInstance<SDK_AppleInApp>(true);
#endif

		gameObject.AddComponent<UConsoleDebug>();
#if !UNITY_WEBGL
		var fa = "fa" + Utils_Plugins.Util_GetBundleVersion();
		if (!PlayerPrefs.HasKey(fa))
		{
			MonoInstancePool.getInstance<UCopyFilesFromStreamingAssets>().OnCopy("versions.txt", UStaticFuncs.ConfigSaveDir, () =>
			{
				DoStart();
				PlayerPrefs.SetInt("fa" + Utils_Plugins.Util_GetBundleVersion(), 1);
			});
		}
		else
		{
#endif
			DoStart();
#if !UNITY_WEBGL
		}
#endif
	}
	private void DoStart()
	{
#if ILRUNTIME
#if UNITY_EDITOR
		var path = "./ab1/AHotGames";
		var dllbs = File.ReadAllBytes($"{path}.bytes");
		var pdbbs = File.ReadAllBytes($"{path}.pdb");
		ILRuntimeHandler.OnStartILRuntime(dllbs, pdbbs);
		fprocessing = 1;
#else
		StartCoroutine(OnDownloadConfigs());
#endif
#else
		/*AEntrance.Initialize(
#if UNITY_IOS
			"ios"
#else
			"android"
#endif
		);*/
#endif
	}
	IEnumerator OnDownloadConfigs()
	{
		var www = new WWW(ConfigURL);
		yield return www;
		if (string.IsNullOrEmpty(www.error))
		{
			var all = www.text.Split(new char[] { '\r', '\n' });
			foreach (var line in all)
			{
				if (line.StartsWith("//")) continue;
				if (line.StartsWith("#")) continue;
				if (line.StartsWith("--")) continue;
				var aline = line.Split(new char[] { '=' }, 2);
				if (aline.Length < 2) continue;
				dconfigs.Add(aline[0], aline[1]);
			}

			www = new WWW(OnGetConfig("dll"));
			yield return www;
			ILRuntimeHandler.OnStartILRuntime(www.bytes, null);
			fprocessing = 1;
		}
		else
		{
			Debug.Log($"{ConfigURL} error:{www.error}");
			yield return new WaitForSeconds(5);
			StartCoroutine(OnDownloadConfigs());
		}
	}
	Dictionary<string, string> dconfigs = new Dictionary<string, string>();
	string OnGetConfig(string tag)
	{
		var rawtag = tag;
		if (bUsingLocalCDN)
		{
			tag = $"local{tag}";
			if (dconfigs.ContainsKey(tag))
				return dconfigs[tag];
		}
		tag = rawtag + Application.version;
		Debug.Log($"get config {tag}");
		if (dconfigs.ContainsKey(tag))
			return dconfigs[tag];
		tag = rawtag;
		if (!dconfigs.ContainsKey(tag))
			return "";
		return dconfigs[tag];
	}
	private bool VersionIsSmall(string localVersion, string remoteVersion)
	{
		var alocal = localVersion.Split('.');
		var aremote = remoteVersion.Split('.');
		for (var i = 0; i < alocal.Length; i++)
		{
			if (i >= aremote.Length)
			{
				return false;
			}
			if (alocal[i] == aremote[i])
			{
				continue;
			}
			return int.Parse(alocal[i]) < int.Parse(aremote[i]);
		}
		return alocal.Length < aremote.Length;
	}

	Texture2D _bg;
	Texture2D bg
	{
		get
		{
			if (_bg == null)
			{
				_bg = new Texture2D(1, 1);
			}
			return _bg;
		}
	}
	Texture2D _point;
	Texture2D point
	{
		get
		{
			if (_point == null)
			{
				_point = new Texture2D(1, 1);
				_point.SetPixel(0, 0, Color.cyan);
				_point.Apply();
			}
			return _point;
		}
	}
	float fprocessing = 0;
	float x = 0;
	void OnGUI()
	{
		if (x >= 1) return;
		GUI.DrawTexture(new Rect(0, Screen.height - Screen.height / 20 - 10, Screen.width, Screen.height / 20), bg);
		if (x < fprocessing)
			x += Time.deltaTime * 0.5f;
		GUI.DrawTexture(new Rect(5, Screen.height - Screen.height / 20 - 5, Screen.width * x - 10, Screen.height / 20 - 10), point);
	}
}
