using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine.UI;
using UnityEngine;

public class UCardGame : AHotBase
{
	Button btnReturn;
	Text textMyLevel;
	Text textMyAvatarname;
	Text textMyCardCount;
	Text textOtherLevel;
	Text textOtherAvatarname;
	Text textOtherCardCount;

	Transform cardcell;
	protected override void InitComponents()
	{
		textMyCardCount = FindWidget<Text>("textMyCardCount");
		textMyCardCount.text = "0";

		textMyAvatarname = FindWidget<Text>("textMyAvatarname");
		textMyAvatarname.text = URemoteData.AvatarName;

		textMyLevel = FindWidget<Text>("textMyLevel");
		ShowLevel();

		textOtherCardCount = FindWidget<Text>("textOtherCardCount");
		textOtherCardCount.text = "0";

		textOtherLevel = FindWidget<Text>("textOtherLevel");
		textOtherAvatarname = FindWidget<Text>("textOtherAvatarname");

		btnReturn = FindWidget<Button>("btnReturn");
		btnReturn.onClick.AddListener(() =>
		{
			OnUnloadThis();

			LoadAnotherUI<UIMain>();
		});

		cardcell = FindWidget<Transform>("cardcell");
		cardcell.gameObject.SetActive(false);

		for (var i = 0; i < 10; i++)
		{
			var obj = GameObject.Instantiate(cardcell, cardcell.parent);
			obj.gameObject.SetActive(true);
			UHotAssetBundleLoader.Instance.OnDownloadResources(() =>
			{
				UDebugHotLog.Log($"UHotAssetBundleLoader.Instance.OnDownloadResources");
			}, $"Images/Pai/b{i + 1}");
		}

		ShowWidget("otherinfo", false);
		URemoteData.ListeningParam(InfoNameDefs.AvatarLevel, ShowLevel);

		UICommonWait.Show();
		WebSocketConnector.Instance.OnInit(Utils.WebSocketURL + UILogin.CachedUsernameAndTokenArguments, evt =>
		{
			UICommonWait.Hide();

		}, msgEvt =>
		{
		}, errEvt =>
		{
			UICommonWait.Hide();
		}, closeEvt =>
		{
			UICommonWait.Hide();
		});
	}
	protected override void OnDestroy()
	{
		URemoteData.CancelListeningParam(InfoNameDefs.AvatarLevel, ShowLevel);
	}

	private void ShowLevel()
	{
		textMyLevel.text = URemoteData.AvatarLevel;
	}
}

