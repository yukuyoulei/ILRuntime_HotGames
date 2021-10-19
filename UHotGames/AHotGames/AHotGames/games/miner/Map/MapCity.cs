using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine.UI;
using UnityEngine;
using LibCommon;
using LibCommon.GameObj;
using LibClient.GameObj;

public class MapCity : AHotBase
{
	protected override void InitComponents()
	{
		LoadUI<UIMinerMain>();

		foreach (var wx in InitValueDefs.wuxing.Keys)
		{
			var swx = wx;
			var btn = FindWidget<Button>(swx);
			var str = $"{InitValueDefs.wuxing[swx]} {CakeClient.GetCake("pinfo", CakeAvatar.myID).GetIntValue(swx)}级";
			FindWidget<Text>(btn.transform, "Text").text = str;
			btn.onClick.AddListener(() =>
			{
				OnEnterYuansuConta(swx);
			});
		}
		AClientApis.OnEnterScene();
	}

	private void OnEnterYuansuConta(string swx)
	{
		AClientApis.OnEnterConta(swx);
	}
}

