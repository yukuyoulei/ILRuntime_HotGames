using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine.UI;
using UnityEngine;

public class UMMORPG : AHotBase
{
    protected override void InitComponents()
    {
        UStaticWebRequests.DoSelectAvatar(UILogin.CachedUsername, UILogin.token
            , (jres) =>
            {
                var data = new UMRemoteAvatarData();
                data.OnFormat(jres);
                UMRemoteDataManager.Instance.OnAdd(data);

                UnloadThis();
                LoadAnother<UMUIMain>();
            }, (err) =>
            {
                if (err == "3")
                {
                    UnloadThis();
                    LoadAnother<UMUICreateAvatar>();
                }
                else
                {
                    UIAlert.Show("选择角色失败，" + err);
                }
            }, (err) =>
            {
                UIAlert.Show("选择角色失败，" + err);
            });
    }
}

