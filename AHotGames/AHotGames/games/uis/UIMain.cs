using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine.UI;
using UnityEngine;

public class UIMain : AHotBase
{
    private void LoadGame<T>() where T : AHotBase, new()
    {
        LoadAnotherUI<T>();
    }
    private Dictionary<string, Action> _dGames;
    private Dictionary<string, Action> dGames
    {
        get
        {
            if (_dGames == null)
            {
                _dGames = new Dictionary<string, Action>();
                _dGames.Add("舒尔特方格", () =>
                {
                    LoadGame<GameSchulte>();
                });
                _dGames.Add("RPG游戏", () =>
                {
                    LoadAnotherUI<UMMORPG>();
                });
            }
            return _dGames;
        }
    }

    protected override void InitComponents()
    {
        var textUsername = FindWidget<Text>("textUsername");
        textUsername.text = UILogin.CachedUsername;

        var menuCell = FindWidget<Button>("menuCell");
        menuCell.gameObject.SetActive(false);

        var btnLogout = FindWidget<Button>("btnLogout");
        btnLogout.onClick.AddListener(() =>
        {
            UStaticWebRequests.DoLogout(UILogin.CachedUsername, UILogin.token
                , (jres) =>
                {
                    UnloadThis();
                    LoadAnotherUI<UILogin>();
                }, (err) =>
                {
                    UIAlert.Show("注销失败：" + Utils.ErrorFormat(err));
                }, (err) =>
                {
                    UIAlert.Show("web error:" + err);
                });

        });

        foreach (var g in dGames)
        {
            var menu = GameObject.Instantiate(menuCell, menuCell.transform.parent);
            menu.gameObject.SetActive(true);
            menu.GetComponentInChildren<Text>().text = g.Key;
            menu.onClick.AddListener(() =>
            {
                UnloadThis();
                g.Value();
            });
        }
    }
}

