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
        if (!PlayerPrefs.HasKey("firstlaunch" + Utils_Plugins.Util_GetBundleVersion()))
        {
            MonoInstancePool.getInstance<UCopyFilesFromStreamingAssets>().OnCopy("versions.txt", UStaticFuncs.ConfigSaveDir, () =>
            {
                InitRemoteConfig();
                PlayerPrefs.SetInt("firstlaunch" + Utils_Plugins.Util_GetBundleVersion(), 1);
            });
        }
        else
        {
            InitRemoteConfig();
        }
    }

    private void InitRemoteConfig()
    {
        ConfigDownloader.Instance.StartToDownload(ConfigURL, () =>
        {
            ParseConfigs();

            Debug.Log("UConfigManager.bUsingAb " + UConfigManager.bUsingAb);
            UAssetBundleDownloader.Instance.DownloadResources((l1) =>
            {
                UAssetBundleDownloader.Instance.DownloadResources((l2) =>
                {
                    var dll = "Dll/AHotGames";
                    byte[] pdbBytes = null;
#if UNITY_EDITOR
                    if (!UConfigManager.bUsingAb)
                    {
                        dll += ".bytes";
                    }
                    pdbBytes = System.IO.File.ReadAllBytes("Assets/RemoteResources/Dll/AHotGames.pdb");
#endif
                    ILRuntimeHandler.Instance.DoLoadDll("ahotmages"
                        , UAssetBundleDownloader.Instance.OnLoadAsset<TextAsset>(dll).bytes, pdbBytes);

                    ILRuntimeHandler.Instance.SetUnityMessageReceiver(MonoInstancePool.getInstance<UEmitMessage>(true).gameObject);

                    ILRuntimeHandler.Instance.OnLoadClass("AEntrance", new GameObject("AEntrance"));
                }
                , null, (arg) =>
                {
                    var preloads = ConfigDownloader.Instance.OnGetValue("preloads");
                    var lPreloads = preloads.Split(',').ToList();
                    foreach (var p in lPreloads)
                    {
                        if (arg.ToLower().StartsWith(p.ToLower()))
                        {
                            return true;
                        }
                    }
                    return false;
                });
            }
            , null, null, true, UStaticFuncs.GetPlatformFolder(Application.platform)
            , UStaticFuncs.GetPlatformFolder(Application.platform) + ".manifest");

        }, () => { Debug.LogError("下载配置文件失败。"); });
    }

    private void ParseConfigs()
    {
        UConfigManager.bUsingAb = ConfigDownloader.Instance.OnGetIntValue("useab") == 1;

#if UNITY_EDITOR
        UConfigManager.bUsingAb = UseAB;
#endif
    }
}
