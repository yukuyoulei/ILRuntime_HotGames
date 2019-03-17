using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine.UI;
using UnityEngine;

public class UMUIMain : AHotBase
{
    protected override void InitComponents()
    {
        var AvatarHeadIcon = FindWidget<RawImage>("AvatarHeadIcon");
        var sexPrefab = (UMRemoteAvatarData.data.AvatarSex == 1 ? "avatarFemale" : "avatarMale");
        LoadPrefab("UI/UIResources/" + sexPrefab, (obj) =>
        {
            var image = obj.GetComponent<RawImage>();
            AvatarHeadIcon.texture = image.texture;
        });

        var AvatarName = FindWidget<Text>("AvatarName");
        AvatarName.text = UMRemoteAvatarData.data.AvatarName;

        var GoldValue = FindWidget<Text>("GoldValue");
        GoldValue.text = UMRemoteAvatarData.data.AvatarGold.ToString();

        var MoneyValue = FindWidget<Text>("MoneyValue");
        MoneyValue.text = UMRemoteAvatarData.data.AvatarMoney.ToString();
    }

}

