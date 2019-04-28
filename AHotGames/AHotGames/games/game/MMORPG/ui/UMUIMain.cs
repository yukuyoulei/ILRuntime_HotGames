using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine.UI;
using UnityEngine;

public class UMUIMain : AHotBase
{
    public static void UpdateHead(RawImage headIcon)
    {
        var sexPrefab = (UMRemoteAvatarData.data.AvatarSex == 1 ? "avatarFemale" : "avatarMale");
        LoadPrefab("UI/UIResources/" + sexPrefab, (obj) =>
        {
            var image = obj.GetComponent<RawImage>();
            headIcon.texture = image.texture;
        });
    }
    public static void UpdateName(Text nickName)
    {
        nickName.text = UMRemoteAvatarData.data.AvatarName;
    }
    private static UMUIMain sintance;
    private Action updateParam;
    protected override void InitComponents()
    {
        sintance = this;

        LoadAnother<UMMap>();

        {
            var AvatarHeadIcon = FindWidget<RawImage>("AvatarHeadIcon");
            UpdateHead(AvatarHeadIcon);
        }
        {
            var AvatarHeadIcon = FindWidget<Button>("AvatarHeadIcon");
            AvatarHeadIcon.onClick.AddListener(() =>
            {
                LoadAnother<UMUISettings>();
            });
        }

        var AvatarName = FindWidget<Text>("AvatarName");
        var GoldValue = FindWidget<Text>("GoldValue");
        var MoneyValue = FindWidget<Text>("MoneyValue");
        var AvatarPos = FindWidget<Text>("AvatarPos");

        updateParam = () =>
        {
            UpdateName(AvatarName);
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

