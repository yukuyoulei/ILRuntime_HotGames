using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.AccessControl;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class UAssetBundleDownloader : MonoBehaviour
{
	private static UAssetBundleDownloader sintance;
	public static UAssetBundleDownloader Instance
	{
		get
		{
			if (sintance == null)
			{
				sintance = MonoInstancePool.getInstance<UAssetBundleDownloader>(true);
			}
			return sintance;
		}
	}
	public static List<string> lPreloads = new List<string>();
	public static List<string> lBaseResources = new List<string>();
	public void DownloadResources(params string[] resources)
	{
		DownloadResources(null, null, null, resources);
	}
	public void DownloadResources(Action<List<string>> downloadComplete, Action<int, int, float> downloadProgress, Func<string, bool> filter, params string[] resources)
	{
		DownloadResources(downloadComplete, downloadProgress, filter, false, resources);
	}
	public void DownloadResources(Action<List<string>> downloadComplete, Action<int, int, float> downloadProgress, Func<string, bool> filter, bool NoDependeces, params string[] resources)
	{
		StartCoroutine(DoCheckRemoteVersion(downloadComplete, downloadProgress, filter, NoDependeces, resources));
	}
	Dictionary<string, RemoteFileCell> dFileRemoteVersions = new Dictionary<string, RemoteFileCell>();
	long totalBytes;
	Action<List<string>> downloadComplete;
	List<string> waitingForDownload;
	List<string> downloadingResources;
	void ResetDownloadStatus()
	{
		totalBytes = 0;
		lastDownloadedBytes = 0;
		downloadStartTime = DateTime.Now;
		DownloadSpeed = 0;
		downloadedResources.Clear();
	}
	private IEnumerator DoCheckRemoteVersion(Action<List<string>> downloadComplete, Action<int, int, float> downloadProgress, Func<string, bool> filter, bool NoDependeces, params string[] resources)
	{
		yield return new WaitForEndOfFrame();

		this.downloadComplete = downloadComplete;
		downloading = true;

		ResetDownloadStatus();

		if (string.IsNullOrEmpty(UStaticFuncs.DownLoadConfigURL))
		{
			Debug.Log("资源下载地址为空");
		}
		else
		{
			Debug.Log("正在检查资源版本");
			downloadingResources = resources.ToList();
			if (!NoDependeces)
			{
				foreach (var r in resources)
				{
					var files = UAssetBundleDownloader.Instance.OnGetAssetBundleDependeces(r);
					foreach (var f in files)
					{
						if (downloadingResources.Contains(f))
						{
							continue;
						}
						downloadingResources.Add(f);
					}
				}
			}
			var downloadedResources = new List<string>();
			if (dFileRemoteVersions.Count == 0)
			{
				var url = UStaticFuncs.DownLoadConfigURL + "versions.txt?-" + ApiDateTime.Now.Ticks;
				if (UStaticFuncs.GetPlatformFolder(Application.platform) == UStaticFuncs.GetPlatformFolder(RuntimePlatform.WindowsPlayer))
				{
					if (!url.Contains("://"))
					{
						url = "file://" + url;
					}
				}
				WWW www = new WWW(url);
				yield return www;
				if (string.IsNullOrEmpty(www.error))
				{
					var text = www.text;
					var atext = text.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
					foreach (var t in atext)
					{
						var name = "";
						var av = t.Split('|');
						if (av.Length < 3)
						{
							continue;
						}
						name = av[0];
						if (name.StartsWith("/") || name.StartsWith("\\"))
						{
							name = name.Substring(1);
						}
						name = name.Replace("\\", "/");
						var filecell = new RemoteFileCell();
						filecell.version = av[1];
						filecell.bytes = typeParser.Int64Parse(av[2]);
						if (dFileRemoteVersions.ContainsKey(name))
						{
							dFileRemoteVersions[name] = filecell;
						}
						else
						{
							dFileRemoteVersions.Add(name, filecell);
						}
					}
				}
				else
				{
					Debug.Log("检查资源版本失败，" + www.error);
				}
			}

			foreach (var rver in dFileRemoteVersions)
			{
				if (filter != null && filter(rver.Key))
				{
					if (!downloadingResources.Contains(rver.Key))
					{
						downloadingResources.Add(rver.Key);
					}
					if (!NoDependeces)
					{
						var files = OnGetAssetBundleDependeces(rver.Key);
						foreach (var f in files)
						{
							if (downloadingResources.Contains(f))
							{
								continue;
							}
							downloadingResources.Add(f);
						}
					}
				}

				var remove = new List<string>();
				foreach (var n in downloadingResources)
				{
					if (!dFileRemoteVersions.ContainsKey(n))
					{
						remove.Add(n);
					}
				}
				foreach (var r in remove)
				{
					downloadingResources.Remove(r);
				}
			}

			foreach (var r in downloadingResources)
			{
				totalBytes += dFileRemoteVersions[r].bytes;
			}

			if (dFileRemoteVersions.Count != 0)
			{
				waitingForDownload = new List<string>();
				waitingForDownload.AddRange(downloadingResources);
				if (downloadingResources.Count == 0)
				{
					OnDownloadComplete(waitingForDownload);
				}
				else
				{
					for (var i = 0; i < DownloadHandlerCount && i < downloadingResources.Count; i++)
					{
						StartToDownloadResources(downloadComplete, downloadProgress);
					}
				}
			}

		}
	}
	public static int DownloadHandlerCount = 5;

	private void OnDownloadComplete(List<string> resourcesList)
	{
		Debug.Log("DownloadComplete using " + (DateTime.Now - downloadStartTime).TotalSeconds.ToString("f2") + "S");
		downloading = false;
		ResetDownloadStatus();
		downloadComplete?.Invoke(resourcesList);
	}

	internal void OnUnloadAssetBundles()
	{
		if (!Enter.bUsingAb)
		{
			return;
		}
		var dep = OnGetAssetBundleDependeces("ui/uiloading.ab").ToList();
		dep.Add(UStaticFuncs.GetPlatformFolder(Application.platform));
		var remove = new List<string>();
		foreach (var d in dAssetBundles)
		{
			if (dep.Contains(d.Key))
			{
				continue;
			}
			d.Value.Unload(true);
			remove.Add(d.Key);
		}
		foreach (var r in remove)
		{
			dAssetBundles.Remove(r);
		}
	}

	long downloadedBytes
	{
		get
		{
			long total = 0;
			foreach (var d in downloadedResources)
			{
				total += dFileRemoteVersions[d].bytes;
			}
			return total;
		}
	}
	long lastDownloadedBytes;

	public static bool bShowDownload;
	List<WWW> lWebReqs = new List<WWW>();
	private void StartToDownloadResources(Action<List<string>> downloadComplete, Action<int, int, float> downloadProgress)
	{
		if (downloadingResources.Count <= 0)
		{
			return;
		}
		var name = downloadingResources[0];
		downloadingResources.RemoveAt(0);

		if (!dFileRemoteVersions.ContainsKey(name))
		{
			Debug.LogError("无法检测版本：" + name);
		}

		var download = true;
		var durl = UStaticFuncs.DownLoadConfigURL + name;
		var file = UFileManager.Instance.OnGetFile(name);
		if (file != null && file.version == dFileRemoteVersions[name].version)
		{
			download = false;
			durl = "file://" + UStaticFuncs.ConfigSaveDir + name;
		}
		if (download)
		{
			if (bShowDownload)
			{
				Debug.Log("正在下载" + name);
			}
			else
			{
				Debug.Log("正在加载数据，请稍候");
			}
			StartCoroutine(DoDownloadResources(downloadComplete, downloadProgress
				, durl, download, name, dFileRemoteVersions[name].version));
		}
		else
		{
			if (bShowDownload)
			{
				Debug.Log("正在加载" + name);
			}
			else
			{
				Debug.Log("正在加载数据，请稍候");
			}
			OnDownloaded(downloadComplete, downloadProgress, name);
		}
	}

	private IEnumerator DoDownloadResources(Action<List<string>> downloadComplete, Action<int, int, float> downloadProgress
		, string url, bool bDownload, string name, string version)
	{
		if (!downloadedResources.Contains(name))
		{
			WWW webReq = new WWW(url);
			lWebReqs.Add(webReq);
			yield return webReq;
			if (string.IsNullOrEmpty(webReq.error))
			{
				if (bDownload)
				{
					var filepath = UStaticFuncs.ConfigSaveDir + name;
					var fi = new FileInfo(filepath);
					if (!fi.Directory.Exists)
					{
						fi.Directory.Create();
					}
					File.WriteAllBytes(filepath, webReq.bytes);
					UFileManager.Instance.OnAddFile(name, version);
				}
			}
			else
			{
				Debug.Log("加载远端配置失败，" + webReq.url + " " + webReq.error);
			}
			lWebReqs.Remove(webReq);
			webReq.Dispose();
			webReq = null;
		}

		OnDownloaded(downloadComplete, downloadProgress, name);
	}
	List<string> downloadedResources = new List<string>();
	private void OnDownloaded(Action<List<string>> downloadComplete, Action<int, int, float> downloadProgress, string name)
	{
		downloadedResources.Add(name);
		if (downloadingResources.Count > 0)
		{
			if (downloadProgress != null)
			{
				downloadProgress(downloadedResources.Count, waitingForDownload.Count, 0);
			}

			StartToDownloadResources(downloadComplete, downloadProgress);
		}
		else if (downloadedBytes >= totalBytes)
		{
			OnDownloadComplete(waitingForDownload);
		}
	}
	AssetBundle manifestBundle;
	AssetBundleManifest manifest;
	Dictionary<string, AssetBundle> dAssetBundles = new Dictionary<string, AssetBundle>();
	public string[] OnGetAssetBundleDependeces(string name, List<string> dependens = null)
	{
		var platform = UStaticFuncs.GetPlatformFolder(Application.platform);
		if (!dAssetBundles.ContainsKey(platform))
		{
			var ab = DoGetAssetBundle(platform);
			if (ab == null)
			{
				return new string[] { };
			}
		}
		if (manifestBundle == null)
		{
			manifestBundle = dAssetBundles[UStaticFuncs.GetPlatformFolder(Application.platform)];
		}
		if (manifest == null)
		{
			manifest = (AssetBundleManifest)manifestBundle.LoadAsset("AssetBundleManifest");
		}
		var total = dependens;
		if (dependens != null)
		{
			foreach (var d in dependens)
			{
				if (!total.Contains(d))
				{
					total.Add(d);
				}
			}
		}
		else
		{
			total = new List<string>();
		}
		var result = manifest.GetAllDependencies(name);
		foreach (var d in result)
		{
			if (!total.Contains(d))
			{
				total.Add(d);
			}
		}
		foreach (var r in result)
		{
			if (dependens != null && dependens.Contains(r))
			{
				continue;
			}
			var deps = OnGetAssetBundleDependeces(r, total);
			foreach (var d in deps)
			{
				if (!total.Contains(d))
				{
					total.Add(d);
				}
			}
		}
		return total.ToArray();
	}

	Dictionary<string, UnityEngine.Object> dAssets = new Dictionary<string, UnityEngine.Object>();
	public T OnLoadAsset<T>(string assetBundlePath) where T : UnityEngine.Object
	{
		Debug.Log($"OnLoadAsset Enter.bUsingAb:{Enter.bUsingAb}");
#if UNITY_EDITOR
		if (!Enter.bUsingAb)
		{
			var prefab = UnityEditor.AssetDatabase.LoadAssetAtPath("Assets/RemoteResources/" + assetBundlePath, typeof(T)) as T;
			if (prefab != null)
			{
				return prefab;
			}
			else
			{
				prefab = UnityEditor.AssetDatabase.LoadAssetAtPath("Assets/RemoteResources/" + assetBundlePath + ".prefab", typeof(T)) as T;
				if (prefab != null)
				{
					return prefab;
				}
			}
		}
#endif
		if (!Enter.bUsingAb)
		{
			var path = assetBundlePath;
			if (path.Contains("."))
			{
				path = path.Split('.')[0];
			}
			return Resources.Load<T>(path);
		}

		var assetName = UStaticFuncs.GetAssetBundleName(assetBundlePath);
		if (dAssets.ContainsKey(assetName) && dAssets[assetName] != null && dAssets[assetName] is T)
		{
			return dAssets[assetName] as T;
		}
		var ab = OnGetAssetBundle(assetBundlePath);
		if (ab == null)
		{
			return Resources.Load<T>(assetBundlePath);
		}
		var deps = OnGetAssetBundleDependeces(assetBundlePath + AssetBundleSuffix);
		foreach (var dep in deps)
		{
			OnGetAssetBundle(dep);
		}
		if (dAssets.ContainsKey(assetName)
			&& dAssets[assetName] != null)
		{
			return dAssets[assetName] as T;
		}
		var t = ab.LoadAsset<T>(assetName);
		if (t == null)
		{
			return t;
		}
		if (dAssets.ContainsKey(assetName))
		{
			dAssets[assetName] = t;
		}
		else
		{
			dAssets.Add(assetName, t);
		}
		return t;
	}
	public static string AssetBundleSuffix = ".ab";
	public AssetBundle OnGetAssetBundle(string assetBundlePath, bool NoDependences = false)
	{
		if (!assetBundlePath.EndsWith(AssetBundleSuffix))
		{
			assetBundlePath += AssetBundleSuffix;
		}
		assetBundlePath = assetBundlePath.ToLower();
		if (!NoDependences && dAssetBundles.ContainsKey(UStaticFuncs.GetPlatformFolder(Application.platform)))
		{
			if (manifestBundle == null)
			{
				manifestBundle = dAssetBundles[UStaticFuncs.GetPlatformFolder(Application.platform)];
			}
			if (manifest == null)
			{
				manifest = (AssetBundleManifest)manifestBundle.LoadAsset("AssetBundleManifest");
			}
		}
		if (dAssetBundles.ContainsKey(assetBundlePath))
		{
			return dAssetBundles[assetBundlePath];
		}
		return DoGetAssetBundle(assetBundlePath);
	}
	private AssetBundle DoGetAssetBundle(string assetBundlePath)
	{
		if (dAssetBundles.ContainsKey(assetBundlePath))
		{
			return dAssetBundles[assetBundlePath];
		}
		var path = UStaticFuncs.ConfigSaveDir + assetBundlePath;
		var ab = AssetBundle.LoadFromFile(path);
		if (ab == null)
		{
			return null;
		}
		dAssetBundles.Add(assetBundlePath, ab);
		var depends = OnGetAssetBundleDependeces(UStaticFuncs.GetAssetBundleName(assetBundlePath));
		foreach (var d in depends)
		{
			OnGetAssetBundle(assetBundlePath);
		}
		return ab;
	}

	float deltaTime;
	DateTime downloadStartTime;
	long deltaBytes;
	long lastUpdatedDownloadedBytes;
	bool downloading;
	public static long DownloadSpeed;
	private void Update()
	{
		if (!downloading)
		{
			return;
		}

		deltaTime += Time.deltaTime;
		if (deltaTime > 1)
		{
			deltaTime -= 1;
			var d = lastDownloadedBytes - lastUpdatedDownloadedBytes;
			if (d > 0)
			{
				DownloadSpeed = d;
			}
			lastUpdatedDownloadedBytes = lastDownloadedBytes;
		}

		long downloadedbytes = downloadedBytes;
		foreach (var webReq in lWebReqs)
		{
			downloadedbytes += webReq.bytesDownloaded;
		}

		var downloadingProgress = (float)((double)lastDownloadedBytes / totalBytes);
	}
}

struct RemoteFileCell
{
	public string version;
	public long bytes;
}