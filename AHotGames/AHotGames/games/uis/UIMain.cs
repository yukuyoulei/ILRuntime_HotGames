using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine.UI;
using UnityEngine;

public class UIMain : AHotBase
{
    Button menuCell;
    protected override void InitComponents()
    {
        var textUsername = FindWidget<Text>("textUsername");
        textUsername.text = UILogin.CachedUsername;

        menuCell = FindWidget<Button>("menuCell");
        menuCell.gameObject.SetActive(false);

        var btnLogout = FindWidget<Button>("btnLogout");
        btnLogout.onClick.AddListener(() =>
        {
            UStaticWebRequests.DoLogout(UILogin.CachedUsername, UILogin.token
                , (jres) =>
                {
                    UnloadThis();
                    LoadAnother<UILogin>();
                }, (err) =>
                {
                    UIAlert.Show("注销失败：" + Utils.ErrorFormat(err));
                }, (err) =>
                {
                    UIAlert.Show("web error:" + err);
                });

        });

        var menu = GameObject.Instantiate(menuCell, menuCell.transform.parent);
        menu.gameObject.SetActive(true);
        menu.GetComponentInChildren<Text>().text = "RPG游戏";
        menu.onClick.AddListener(() =>
        {
            UnloadThis();
            LoadAnother<UMMORPG>();
        });
    }
}

