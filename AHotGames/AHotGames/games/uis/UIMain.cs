using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine.UI;
using UnityEngine;

public class UIMain : AHotBase
{
	private T LoadGame<T>(string path) where T : AHotBase, new()
	{
		return LoadClass<T>(path);
	}
	private Dictionary<string, Action> _dGames;
	private Dictionary<string, Action> dGames
	{
		get
		{
			if (_dGames == null)
			{
				_dGames = new Dictionary<string, Action>();
				AddGame<UMMO>("MMO");
				AddGame<ULifeInBottle>("瓶中人生");
			}
			return _dGames;
		}
	}
	private void AddGame<T>(string gameLabel) where T : AHotBase, new()
	{
		_dGames.Add(gameLabel, () =>
		{
			LoadGame<T>("Game/" + typeof(T).Name);
		});
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
					OnUnloadThis();
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
				OnUnloadThis();
				g.Value();
			});
		}
	}
}

