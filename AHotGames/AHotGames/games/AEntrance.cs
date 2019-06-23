using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine.UI;
using UnityEngine;

public class AEntrance : AHotBase
{
    protected override void InitComponents()
    {
        UHotLog.Log($"Environment.IsEditor {Environment.IsEditor}");
        UHotLog.Log($"Utils.ConfigSaveDir {Utils.ConfigSaveDir}");
        PreDownloadResources();
    }

    private void PreDownloadResources()
    {
        UHotAssetBundleLoader.Instance.OnDownloadResources(new List<string> { Utils.GetPlatformFolder(Application.platform)
            , Utils.GetPlatformFolder(Application.platform)+ ".manifest" }, () =>
        {
            UHotLog.Log($"OnDownloadResources {Application.platform}");
            UHotAssetBundleLoader.Instance.OnDownloadResources(new List<string> { "ui/uiloading.ab" }, () =>
            {
                UHotLog.Log("OnDownloadResources uiloading");
                LoadUI<UILoading>();
                //UILoading.OnSetLoadingActions(() => { LoadUI<UILogin>(); });
            }, null);
        }, null);
    }
}

