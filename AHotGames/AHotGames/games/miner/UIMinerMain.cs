using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine.UI;
using UnityEngine;
using LibClient;

public class UIMinerMain : AHotBase
{
	Text textGold;
	protected override void InitComponents()
	{
		var textUsername = FindWidget<Text>("textUsername");
		textUsername.text = AClientApp.myAvatar.AvatarName;
		textGold = FindWidget<Text>("textGold");
		OnParamUpdateCb(null);

		var btnLogout = FindWidget<Button>("btnLogout");
		btnLogout.onClick.AddListener(() =>
		{
			AClientApp.OnDisconnect();
			OnUnloadThis();
			LoadAnotherUI<UIMinerLogin>();
		});

		UEventListener.Instance.OnRegisterEvent(UEvents.ParamUpdate, OnParamUpdateCb);

		var map01 = LoadClass<UMinerMap>("UI/MinerMap/Map01");

	}

	private void OnParamUpdateCb(UEventBase eb)
	{
		textGold.text = AClientApp.myAvatar.AvatarGold.ToString();
	}
}

