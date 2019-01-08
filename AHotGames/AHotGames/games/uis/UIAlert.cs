using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine.UI;
using UnityEngine;

public class UIAlert : AHotBase
{
    Text Content;
    Button btnConfirm;
    Button btnCancel;
    protected override void InitComponents()
    {
        sintance = this;

        Content = FindWidget<Text>("Content");
        Content.text = sContent;

        btnConfirm = FindWidget<Button>("btnConfirm");
        btnConfirm.onClick.AddListener(() =>
        {
            actionConfirm?.Invoke();
            UnloadThis();
        });
        btnCancel = FindWidget<Button>("btnCancel");
        btnCancel.onClick.AddListener(() =>
        {
            actionCancel?.Invoke();
            UnloadThis();
        });

        if (bHideConfirmButton)
        {
            btnConfirm.gameObject.SetActive(false);
        }

        if (bHideCancelButton)
        {
            btnCancel.gameObject.SetActive(false);
        }
    }

    //热更里的预设都是异步加载的，所以这里只能用静态的变量来传递
    private static Action actionConfirm, actionCancel;
    private static bool bHideConfirmButton, bHideCancelButton;
    private static string sContent = "";

    private static UIAlert sintance;
    public static void Show(string content)
    {
        Show(content, null, null, true);
    }
    public static void Show(string content, Action confirmAction, Action cancelAction, bool hideCancelButton = false, bool hideConfirmButton = false)
    {
        sContent = content;
        actionConfirm = confirmAction;
        actionCancel = cancelAction;
        bHideConfirmButton = hideConfirmButton;
        bHideCancelButton = hideCancelButton;

        if (sintance == null
            || sintance.bDestroying)
        {
            LoadAnother<UIAlert>();
        }
    }
    public static void Hide()
    {
        if (sintance != null
            && !sintance.bDestroying)
        {
            sintance.UnloadThis();
        }
    }
}

