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
    Dictionary<string, RemoteFileCell> dFileRemoteVersions = new Dictionary<string, RemoteFileCell>();
    long totalBytes;
    private IEnumerator DoCheckRemoteVersion(Action<List<string>> downloadComplete, Action<int, int, float> downloadProgress, Func<string, bool> filter, bool NoDependeces, params string[] resources)
    {
        if (string.IsNullOrEmpty(UStaticFuncs.DownLoadConfigURL))
        {
            Debug.Log("资源下载地址为空。");
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
            if (dFileRemoteVersions.Count == 0)
            {
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
                        if (av.Length < 3)
                        {
                            Debug.LogError("remote version file check failed");
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
                    if (!resourcesList.Contains(rver.Key))
                    {
                        resourcesList.Add(rver.Key);
                    }
                    if (!NoDependeces)
                    {
                        var files = OnGetAssetBundleDependeces(rver.Key);
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

                var remove = new List<string>();
                foreach (var n in resourcesList)
                {
                    if (dAssetBundles.ContainsKey(n)
                        || !dFileRemoteVersions.ContainsKey(n))
                    {
                        remove.Add(n);
                    }
                }
                foreach (var r in remove)
                {
                    resourcesList.Remove(r);
                }
            }

            totalBytes = 0;
            foreach (var r in resourcesList)
            {
                totalBytes += dFileRemoteVersions[r].bytes;
            }

            if (dFileRemoteVersions.Count != 0)
            {
                if (resourcesList.Count == 0)
                {
                    downloadComplete?.Invoke(resourcesList);
                }
                else
                {
                    StartToDownloadResources(downloadComplete, downloadProgress, downloadedResources, resourcesList);

#if UNITY_IOS || UNITY_EDITOR
                    StartCoroutine(DelayUpdateProgress(downloadProgress, downloadedResources, resourcesList));
#endif
                }
            }

        }
    }

    internal void OnUnloadAssetBundles()
    {
        if (!UConfigManager.bUsingAb)
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
            foreach (var d in downloaded)
            {
                total += dFileRemoteVersions[d].bytes;
            }
            return total;
        }
    }
    private IEnumerator DelayUpdateProgress(Action<int, int, float> downloadProgress, List<string> downloadedResources, List<string> resourcesList)
    {
        yield return new WaitForSeconds(0.5f);

        if (downloadedResources.Count < resourcesList.Count)
        {
            if (www != null)
            {
                if (downloadProgress != null)
                {
                    downloadProgress(downloadedResources.Count, resourcesList.Count, www.progress / resourcesList.Count);
                }
                else
                {

                    var progress = (downloadedBytes + www.bytesDownloaded) / totalBytes;
                }

                StartCoroutine(DelayUpdateProgress(downloadProgress, downloadedResources, resourcesList));
            }
        }
    }

    public static bool bShowDownload;
    WWW www;
    private void StartToDownloadResources(Action<List<string>> downloadComplete, Action<int, int, float> downloadProgress, List<string> downloadedResources, List<string> resourcesList)
    {
        var name = resourcesList[downloadedResources.Count];
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
                Debug.Log("正在加载数据，请稍候。");
            }
            StartCoroutine(DoDownloadResources(downloadComplete, downloadProgress, downloadedResources, resourcesList
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
                Debug.Log("正在加载数据，请稍候。");
            }
            OnDownloaded(downloadComplete, downloadProgress, downloadedResources, resourcesList, name);
        }
    }

    List<string> downloaded = new List<string>();
    private IEnumerator DoDownloadResources(Action<List<string>> downloadComplete, Action<int, int, float> downloadProgress, List<string> downloadedResources, List<string> resourcesList
        , string url, bool bDownload, string name, string version)
    {
        if (!url.EndsWith(".manifest")
            && !downloaded.Contains(name))
        {
            www = new WWW(url);
            yield return www;
            yield return www.isDone;
            if (string.IsNullOrEmpty(www.error))
            {
                downloaded.Add(name);
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

        OnDownloaded(downloadComplete, downloadProgress, downloadedResources, resourcesList, name);
    }
    private void OnDownloaded(Action<List<string>> downloadComplete, Action<int, int, float> downloadProgress, List<string> downloadedResources, List<string> resourcesList, string name)
    {
        downloadedResources.Add(name);
        if (downloadedResources.Count == resourcesList.Count)
        {
            downloadComplete?.Invoke(resourcesList);
        }
        else
        {
            if (downloadProgress != null)
            {
                downloadProgress(downloadedResources.Count, resourcesList.Count, 0);
            }

            StartToDownloadResources(downloadComplete, downloadProgress, downloadedResources, resourcesList);
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
#if UNITY_EDITOR
        if (!UConfigManager.bUsingAb)
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
        if (!UConfigManager.bUsingAb)
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
    public static List<string> lPreloads = new List<string>();
    public static List<string> lBaseResources = new List<string>();

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
        var path = UStaticFuncs.ConfigSaveDir + "/" + assetBundlePath;
        if (!File.Exists(path))
        {
            return null;
        }
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
}

struct RemoteFileCell
{
    public string version;
    public Int64 bytes;
}