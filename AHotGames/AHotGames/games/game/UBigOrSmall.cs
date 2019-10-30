using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine.UI;
using UnityEngine;

public class UBigOrSmall : AHotBase
{
	Button btnReturn;
	Text textGold;
	Text textAvatarname;
	protected override void InitComponents()
	{
		textAvatarname = FindWidget<Text>("textAvatarname");
		textAvatarname.text = URemoteData.AvatarName;

		textGold = FindWidget<Text>("textGold");
		ShowGold();

		btnReturn = FindWidget<Button>("btnReturn");
		btnReturn.onClick.AddListener(() =>
		{
			OnUnloadThis();

			LoadAnotherUI<UIMain>();
		});

		var bbig = false;
		var bsmall = false;
		var bigorsmall = FindWidget<Transform>("bigorsmall");
		var big = FindWidget<Toggle>(bigorsmall.transform, "big");
		big.onValueChanged.AddListener(value => { bbig = value; });
		var small = FindWidget<Toggle>(bigorsmall.transform, "small");
		small.onValueChanged.AddListener(value => { bsmall = value; });

		var multis = FindWidget<Transform>("multis");
		var multiSelected = new Dictionary<int, bool>();
		var multiWidgets = new Dictionary<int, Toggle>();
		foreach (var w in InitValueDefs.CaiDaXiaoMultis)
		{
			var t = FindWidget<Toggle>(multis, $"m{w}");
			multiWidgets.Add(w, t);
			t.onValueChanged.AddListener((value) =>
			{
				multiSelected[w] = value;
			});
		}

		var btnConfirm = FindWidget<Button>("btnConfirm");
		btnConfirm.onClick.AddListener(() =>
		{
			if (!bbig && !bsmall) return;
			int sel = 0;
			foreach (var kv in multiSelected)
			{
				if (kv.Value) sel = kv.Key;
			}
			if (sel == 0) return;
			UStaticWebRequests.OnWebRequest("Avatar/CaiDaXiao", $"{UILogin.CachedUsernameAndTokenArguments}&multi={sel}&isBig={(bbig ? "1" : "0")}", jobj =>
			{
				var res = jobj["res"].ToString();
				UIAlert.Show($"猜大小结果：{res}");
			});
		});

		URemoteData.ListeningParam(InfoNameDefs.AvatarGold, ShowGold);
	}
	protected override void OnDestroy()
	{
		URemoteData.CancelListeningParam(InfoNameDefs.AvatarGold, ShowGold);
	}

	private void ShowGold()
	{
		textGold.text = URemoteData.OnGetParam(InfoNameDefs.AvatarGold);
	}
}

