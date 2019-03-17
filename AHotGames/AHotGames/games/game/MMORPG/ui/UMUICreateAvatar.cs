using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine.UI;
using UnityEngine;

public class UMUICreateAvatar : AHotBase
{
    protected override void InitComponents()
    {
        var inputNickname = FindWidget<InputField>("inputNickname");
        var bfemale = false;
        var btnMale = FindWidget<Button>("btnMale");
        var maleSel = FindWidget<Image>(btnMale.transform, "sel");
        var btnFemale = FindWidget<Button>("btnFemale");
        var femaleSel = FindWidget<Image>(btnFemale.transform, "sel");
        btnMale.onClick.AddListener(() =>
        {
            bfemale = false;
            femaleSel.enabled = bfemale;
            maleSel.enabled = !bfemale;
        });
        btnFemale.onClick.AddListener(() =>
        {
            bfemale = true;
            femaleSel.enabled = bfemale;
            maleSel.enabled = !bfemale;
        });
        var btnCreate = FindWidget<Button>("btnCreate");
        btnCreate.onClick.AddListener(() =>
        {
            if (string.IsNullOrEmpty(inputNickname.text))
            {
                return;
            }

            UStaticWebRequests.DoCreateAvatar(UILogin.CachedUsername, UILogin.token, inputNickname.text, bfemale ? "1" : "0"
                , (jcreateres) =>
                {
                    UIAlert.Show("创建角色成功，正在进入游戏。", null, null, true, true);
                    DelayDoSth(() =>
                    {
                        UIAlert.Hide();
                        UStaticWebRequests.DoSelectAvatar(UILogin.CachedUsername, UILogin.token
                            , (jselres) =>
                            {
                                var data = new UMRemoteAvatarData();
                                data.OnFormat(jselres);
                                UMRemoteDataManager.Instance.OnAdd(data);
                                UnloadThis();

                                LoadAnother<UMUIMain>();
                            }, (err) =>
                            {
                                UIAlert.Show("进入游戏失败，" + err);
                            }, (err) =>
                            {
                                UIAlert.Show("进入游戏失败，" + err);
                            });
                    }, 3);
                }, (err) =>
                {
                    UIAlert.Show("创建角色失败，" + err);
                }, (err) =>
                {
                    UIAlert.Show("创建角色失败，" + err);
                });
        });
        var btnReturn = FindWidget<Button>("btnReturn");
        btnReturn.onClick.AddListener(() =>
        {
            UnloadThis();
            LoadAnother<UIMain>();
        });
    }

}

