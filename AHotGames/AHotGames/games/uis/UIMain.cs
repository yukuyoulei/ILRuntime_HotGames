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
				//AddGame<ULifeInBottle>("瓶中人生");
				AddGame<UCardGame>("老牛赶大车");
				AddGame<UBigOrSmall>("买大小");
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
		textUsername.text = URemoteData.AvatarName;
		textGold = FindWidget<Text>("textGold");
		RefreshGold();

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
			UStaticWebRequests.OnWebRequest("Avatar/DailyCheck", $"username={UILogin.CachedUsername}&token={UILogin.token}", jobj =>
			{
				URemoteData.OnReceiveAvatarData(jobj["avatar"]);
			});
		});

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

		URemoteData.ListeningParam(InfoNameDefs.AvatarGold, RefreshGold);
	}
	protected override void OnDestroy()
	{
		URemoteData.CancelListeningParam(InfoNameDefs.AvatarGold, RefreshGold);
	}

	private void RefreshGold()
	{
		textGold.text = URemoteData.OnGetParam(InfoNameDefs.AvatarGold);
	}
}

