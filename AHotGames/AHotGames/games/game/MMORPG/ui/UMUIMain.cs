using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine.UI;
using UnityEngine;

public class UMUIMain : AHotBase
{
    private static UMUIMain sintance;
    private Action updateParam;
    protected override void InitComponents()
    {
        sintance = this;

        LoadAnother<UMMap>();

        var AvatarHeadIcon = FindWidget<RawImage>("AvatarHeadIcon");
        var sexPrefab = (UMRemoteAvatarData.data.AvatarSex == 1 ? "avatarFemale" : "avatarMale");
        LoadPrefab("UI/UIResources/" + sexPrefab, (obj) =>
        {
            var image = obj.GetComponent<RawImage>();
            AvatarHeadIcon.texture = image.texture;
        });

        var AvatarName = FindWidget<Text>("AvatarName");
        var GoldValue = FindWidget<Text>("GoldValue");
        var MoneyValue = FindWidget<Text>("MoneyValue");
        var AvatarPos = FindWidget<Text>("AvatarPos");

        updateParam = () =>
        {
            AvatarName.text = UMRemoteAvatarData.data.AvatarName;
            GoldValue.text = UMRemoteAvatarData.data.AvatarGold.ToString();
            MoneyValue.text = UMRemoteAvatarData.data.AvatarMoney.ToString();
            AvatarPos.text = "x:" + UMRemoteAvatarData.data.MapX + " y:" + UMRemoteAvatarData.data.MapY;
        };
        updateParam();
    }

    public static void OnParamUpdate()
    {
        sintance?.updateParam?.Invoke();
    }
}

