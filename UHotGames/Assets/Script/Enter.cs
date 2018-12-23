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
    public static string ConfigURL = "http://fscoding.top/hotgames/Config.txt";
    private void Start()
    {
        ConfigDownloader.Instance.StartToDownload(ConfigURL, () =>
         {
             ParseConfigs();

             UAssetBundleDownloader.Instance.DownloadResources((l1) =>
             {
                 UAssetBundleDownloader.Instance.DownloadResources((l2) =>
                 {
                     var dll = "Dll/AHotGames";
#if UNITY_EDITOR
                     if (!UConfigManager.bUsingAb)
                     {
                         dll += ".bytes";
                     }
#endif
                     ILRuntimeHandler.Instance.DoLoadDll("ahotmages"
                         , UAssetBundleDownloader.Instance.OnLoadAsset<TextAsset>(dll).bytes);
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
                 }, true);
             }
             , null, null, true, UStaticFuncs.GetPlatformFolder(Application.platform)
             , UStaticFuncs.GetPlatformFolder(Application.platform) + ".manifest");

         }, () => { Debug.LogError("下载配置文件失败。"); });
    }

    private void ParseConfigs()
    {
        UConfigManager.bUsingAb = ConfigDownloader.Instance.OnGetIntValue("useab") == 1;
    }
}
