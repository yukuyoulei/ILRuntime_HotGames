using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine.UI;
using UnityEngine;

public class UILoading : AHotBase
{
    Text TextContent;
    Slider Progress;
    protected override void InitComponents()
    {
        TextContent = FindWidget<Text>("TextContent");
        Progress = FindWidget<Slider>("Progress");
        Progress.value = 0;

        addUpdateAction(() =>
        {
            if (UHotAssetBundleLoader.Instance.fProgress == -1)
            {
                return false;
            }
            var p = UHotAssetBundleLoader.Instance.fProgress;
            if (p >= 1)
            {
                actionAfterLoaded?.Invoke();
                UnloadThis();
                actionAfterLoaded = null;
                return true;
            }
            Progress.value = p;
            return false;
        });
    }
    static Action actionAfterLoaded;
    public static void OnSetLoadingActions(Action afterLoadedAction)
    {
        actionAfterLoaded = afterLoadedAction;
    }
}

