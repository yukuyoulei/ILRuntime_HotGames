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
             UAssetBundleDownloader.Instance.DownloadResources((l1) =>
             {
                 UAssetBundleDownloader.Instance.DownloadResources((l2) =>
                 {
                     var dll = "Dll/AHotGames";
#if UNITY_EDITOR
                     dll += ".bytes";
#endif
                     ILRuntimeHandler.Instance.DoLoadDll("ahotmages", UAssetBundleDownloader.Instance.OnLoadAsset<TextAsset>(dll).bytes);
                     ILRuntimeHandler.Instance.SetUnityMessageReceiver(MonoInstancePool.getInstance<UEmitMessage>(true).gameObject);

                     ILRuntimeHandler.Instance.OnLoadClass("AEntrance", new GameObject("AEntrance"));
                 }
                 , null, (arg) => { return arg.Contains("dll/"); }, true);
             }
             , null, null, true, UStaticFuncs.GetPlatformFolder(Application.platform)
             , UStaticFuncs.GetPlatformFolder(Application.platform) + ".manifest");

         }, () => { Debug.LogError("下载配置文件失败。"); });
    }
}
