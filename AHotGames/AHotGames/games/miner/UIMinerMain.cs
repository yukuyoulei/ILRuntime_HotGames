using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine.UI;
using UnityEngine;
using LibClient;

public class UIMinerMain : AHotBase
{
	private void LoadGame<T>(string path) where T : AHotBase, new()
	{
		LoadClass<T>(path);
	}
	private Dictionary<string, Action> _dGames;
	private Dictionary<string, Action> dGames
	{
		get
		{
			if (_dGames == null)
			{
				_dGames = new Dictionary<string, Action>();
			}
			return _dGames;
		}
	}
	private void AddGame<T>(string gameLabel) where T : AHotBase, new()
	{
		_dGames.Add(gameLabel, () =>
		{
			var gamename = typeof(T).Name;
			UICommonWait.Show();
			UHotAssetBundleLoader.Instance.OnDownloadResources(() =>
			{
				UICommonWait.Hide();
				LoadGame<T>("Game/" + gamename);
			}, "game/" + gamename);
		});
	}

	Text textGold;
	protected override void InitComponents()
	{
		var textUsername = FindWidget<Text>("textUsername");
		textUsername.text = AClientApp.myAvatar.AvatarName;
		textGold = FindWidget<Text>("textGold");
		OnParamUpdateCb(null);

		var menuCell = FindWidget<Button>("menuCell");
		menuCell.gameObject.SetActive(false);

		var btnRank = FindWidget<Button>("btnRank");
		btnRank.onClick.AddListener(() =>
		{
			LoadAnotherUI<UIRank>();
		});

		var btnCheck = FindWidget<Button>("btnCheck");
		btnCheck.onClick.AddListener(() =>
		{

		});

		var btnLogout = FindWidget<Button>("btnLogout");
		btnLogout.onClick.AddListener(() =>
		{
			AClientApp.OnDisconnect();
			OnUnloadThis();
			LoadAnotherUI<UIMinerLogin>();
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

		UEventListener.Instance.OnRegisterEvent(UEvents.ParamUpdate, OnParamUpdateCb);
	}

	private void OnParamUpdateCb(UEventBase eb)
	{
		textGold.text = AClientApp.myAvatar.AvatarGold.ToString();
	}
}

