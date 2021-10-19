using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine.UI;
using UnityEngine;

public class WExpSlider : AHotBase
{
	Image expProgress;
	Text textExp;
	protected override void InitComponents()
	{
		expProgress = FindWidget<Image>("expProgress");
		textExp = FindWidget<Text>("textExp");
		textExp.text = "";

		URemoteData.ListeningParam(InfoNameDefs.CurExp, ShowCurExp);
		URemoteData.ListeningParam(InfoNameDefs.MaxExp, ShowCurExp);

		RefreshUI();

		gameObj.AddComponent<UOnDestroy>().actionOnDestroy = () =>
		{
			URemoteData.CancelListeningParam(InfoNameDefs.CurExp, ShowCurExp);
			URemoteData.CancelListeningParam(InfoNameDefs.MaxExp, ShowCurExp);
		};
	}

	protected override void OnDestroy()
	{
		URemoteData.CancelListeningParam(InfoNameDefs.CurExp, ShowCurExp);
		URemoteData.CancelListeningParam(InfoNameDefs.MaxExp, ShowCurExp);
	}

	private void RefreshUI()
	{
		ShowCurExp();
	}

	private void ShowCurExp()
	{
		if (URemoteData.MaxExp == 0)
		{
			return;
		}
		textExp.text = URemoteData.CurExp + "/" + URemoteData.MaxExp;
		expProgress.fillAmount = URemoteData.CurExp / (float)URemoteData.MaxExp;
	}
}

