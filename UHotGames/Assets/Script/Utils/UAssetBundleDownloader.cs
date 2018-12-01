using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using UnityEngine;

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
    Dictionary<string, string> dFileRemoteVersions = new Dictionary<string, string>();
    private IEnumerator DoCheckRemoteVersion(Action<List<string>> downloadComplete, Action<int, int, float> downloadProgress, Func<string, bool> filter, bool NoDependeces, params string[] resources)
    {
        if (string.IsNullOrEmpty(UStaticFuncs.DownLoadConfigURL))
        {
            Debug.LogError("资源下载地址为空。");
        }
        else
        {
            Debug.Log("正在检查资源版本");
            var resourcesList = resources.ToList();
            if (!NoDependeces)
            {
                foreach (var r in resources)
                {
                    var files = UAssetBundleDownloader.Instance.OnGetAssetBundleDependeces(r);
                    foreach (var f in files)
                    {
                        if (resourcesList.Contains(f))
                        {
                            continue;
                        }
                        resourcesList.Add(f);
                    }
                }
            }
            var downloadedResources = new List<string>();
            var url = UStaticFuncs.DownLoadConfigURL + "versions.txt?" + ApiDateTime.Now.Ticks;
            if (UStaticFuncs.GetPlatformFolder(Application.platform) == UStaticFuncs.GetPlatformFolder(RuntimePlatform.WindowsPlayer))
            {
                if (!url.Contains("://"))
                {
                    url = "file://" + url;
                }
            }
            www = new WWW(url);
            yield return www;
            if (string.IsNullOrEmpty(www.error))
            {
                var text = www.text;
                var atext = text.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var t in atext)
                {
                    var name = "";
                    var av = t.Split('|');
                    name = av[0];
                    if (name.StartsWith("/") || name.StartsWith("\\"))
                    {
                        name = name.Substring(1);
                    }
                    name = name.Replace("\\", "/");
                    if (dFileRemoteVersions.ContainsKey(name))
                    {
                        dFileRemoteVersions[name] = av[1];
                    }
                    else
                    {
                        dFileRemoteVersions.Add(name, av[1]);
                    }

                    if (filter != null && filter(name))
                    {
                        if (!resourcesList.Contains(name))
                        {
                            resourcesList.Add(name);
                        }
                        if (!NoDependeces)
                        {
                            var files = OnGetAssetBundleDependeces(name);
                            foreach (var f in files)
                            {
                                if (resourcesList.Contains(f))
                                {
                                    continue;
                                }
                                resourcesList.Add(f);
                            }
                        }
                    }
                }

                var remove = new List<string>();
                foreach (var n in resourcesList)
                {
                    if (dFileRemoteVersions.ContainsKey(n))
                    {
                        continue;
                    }
                    remove.Add(n);
                }
                foreach (var r in remove)
                {
                    resourcesList.Remove(r);
                }

                if (resourcesList.Count == 0)
                {
                    downloadComplete?.Invoke(resourcesList);
                }
                else
                {
                    StartToDownloadResources(downloadComplete, downloadProgress, downloadedResources, resourcesList);

                    StartCoroutine(DelayUpdateProgress(downloadProgress, downloadedResources, resourcesList));
                }
            }
            else
            {
                Debug.Log("检查资源版本失败，" + www.error);
            }
        }
    }

    private IEnumerator DelayUpdateProgress(Action<int, int, float> downloadProgress, List<string> downloadedResources, List<string> resourcesList)
    {
        yield return new WaitForSeconds(0.5f);

        if (www != null)
        {
            downloadProgress?.Invoke(downloadedResources.Count, resourcesList.Count, www.progress / resourcesList.Count);

            StartCoroutine(DelayUpdateProgress(downloadProgress, downloadedResources, resourcesList));
        }
    }

    WWW www;
    private void StartToDownloadResources(Action<List<string>> downloadComplete, Action<int, int, float> downloadProgress, List<string> downloadedResources, List<string> resourcesList)
    {
        var name = resourcesList[downloadedResources.Count];
        var download = true;
        var durl = UStaticFuncs.DownLoadConfigURL + name;
        var file = UFileManager.Instance.OnGetFile(name);
        if (file != null && file.version == dFileRemoteVersions[name])
        {
            download = false;
            durl = "file://" + UStaticFuncs.ConfigSaveDir + name;
        }
        if (download)
        {
            //UILoading.SetLoadingContent("正在下载" + name);
        }
        else
        {
            //UILoading.SetLoadingContent("正在加载" + name);
        }
        StartCoroutine(DoDownloadResources(downloadComplete, downloadProgress, downloadedResources, resourcesList
            , durl, download, name, dFileRemoteVersions[name]));
    }

    private IEnumerator DoDownloadResources(Action<List<string>> downloadComplete, Action<int, int, float> downloadProgress, List<string> downloadedResources, List<string> resourcesList
        , string url, bool bDownload, string name, string version)
    {
        if (!url.EndsWith(".manifest")
            && !dAssetBundles.ContainsKey(name))
        {
            www = new WWW(url);
            yield return www;
            if (string.IsNullOrEmpty(www.error))
            {
                dAssetBundles.Add(name, www.assetBundle);
                if (bDownload)
                {
                    var filepath = UStaticFuncs.ConfigSaveDir + name;
                    var fi = new FileInfo(filepath);
                    if (!fi.Directory.Exists)
                    {
                        fi.Directory.Create();
                    }
                    File.WriteAllBytes(filepath, www.bytes);
                    UFileManager.Instance.OnAddFile(name, version);
                }
            }
            else
            {
                Debug.Log("加载远端配置失败，" + www.url + " " + www.error);
            }
            www.Dispose();
            www = null;
        }

        downloadedResources.Add(name);
        if (downloadedResources.Count == resourcesList.Count)
        {
            downloadComplete?.Invoke(resourcesList);
        }
        else
        {
            downloadProgress?.Invoke(downloadedResources.Count, resourcesList.Count, 0);

            StartToDownloadResources(downloadComplete, downloadProgress, downloadedResources, resourcesList);
        }
    }
    AssetBundle manifestBundle;
    AssetBundleManifest manifest;
    Dictionary<string, AssetBundle> dAssetBundles = new Dictionary<string, AssetBundle>();
    public string[] OnGetAssetBundleDependeces(string name, List<string> dependens = null)
    {
        if (!dAssetBundles.ContainsKey(UStaticFuncs.GetPlatformFolder(Application.platform)))
        {
            return new string[] { };
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
        T prefab = null;
#if UNITY_EDITOR
        if (!UConfigManager.bUsingAb)
        {
            prefab = UnityEditor.AssetDatabase.LoadAssetAtPath("Assets/RemoteResources/" + assetBundlePath, typeof(T)) as T;
            if (prefab != null)
            {
                return prefab;
            }
            if (assetBundlePath.Contains("."))
            {
                assetBundlePath = assetBundlePath.Substring(0, assetBundlePath.LastIndexOf("."));
            }
        }
#endif
        if (!UConfigManager.bUsingAb)
            prefab = Resources.Load<T>(assetBundlePath);
        if (prefab)
        {
            return prefab;
        }

        assetBundlePath = assetBundlePath.ToLower();
        var assetName = UStaticFuncs.GetAssetBundleName(assetBundlePath);
        if (dAssets.ContainsKey(assetName) && dAssets[assetName] != null && dAssets[assetName] is T)
        {
            return dAssets[assetName] as T;
        }
        var deps = OnGetAssetBundleDependeces(assetBundlePath + AssetBundleSuffix);
        foreach (var dep in deps)
        {
            OnGetAssetBundle(UStaticFuncs.GetAssetBundleName(dep));
        }
        var ab = OnGetAssetBundle(assetBundlePath);
        if (ab == null)
        {
            return Resources.Load<T>(assetBundlePath);
        }
        var t = ab.LoadAsset<T>(assetName);
        dAssets.Add(assetName, t);
        return t;
    }
    public static string AssetBundleSuffix = ".ab";
    public AssetBundle OnGetAssetBundle(string name, bool NoDependences = false)
    {
        name = name.ToLower();
        if (!name.EndsWith(AssetBundleSuffix))
        {
            name += AssetBundleSuffix;
        }
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
        if (dAssetBundles.ContainsKey(name))
        {
            return dAssetBundles[name];
        }
        return null;
    }
}
