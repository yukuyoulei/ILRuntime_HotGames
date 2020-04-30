using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine.UI;
using UnityEngine;
using Newtonsoft.Json.Linq;

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
		var curMulti = 0;
		foreach (var w in ClientValueDefs.CaiDaXiaoMultis)
		{
			var t = FindWidget<Toggle>(multis, $"m{w}");
			var tw = w;
			t.onValueChanged.AddListener((value) =>
			{
				if (value) curMulti = tw;
				else if (curMulti == tw) curMulti = 0;
			});
		}

		var btnConfirm = FindWidget<Button>("btnConfirm");
		btnConfirm.onClick.AddListener(() =>
		{
			if (!bbig && !bsmall) return;
			if (curMulti == 0) return;
			UStaticWebRequests.OnWebRequest("Avatar/CaiDaXiao", $"{UILogin.CachedUsernameAndTokenArguments}&multi={curMulti}&isBig={(bbig ? "1" : "0")}", jobj =>
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

