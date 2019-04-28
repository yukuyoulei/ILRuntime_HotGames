using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine.UI;
using UnityEngine;

public class UMUISettings : AHotBase
{
    protected override void InitComponents()
    {
        {
            var AvatarHeadIcon = FindWidget<RawImage>("AvatarHeadIcon");
            UMUIMain.UpdateHead(AvatarHeadIcon);
        }
        {
            var AvatarName = FindWidget<Text>("AvatarName");
            UMUIMain.UpdateName(AvatarName);
        }

        var btnClose = FindWidget<Button>("btnClose");
        btnClose.onClick.AddListener(() =>
        {
            UnloadThis();
        });

        var btnLogout = FindWidget<Button>("btnLogout");
        btnLogout.onClick.AddListener(() =>
        {
            UnloadAllClasses();
            LoadAnother<UILogin>();
        });
    }
}

