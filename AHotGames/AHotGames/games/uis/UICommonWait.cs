using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine.UI;
using UnityEngine;

public class UICommonWait : AHotBase
{
	private static UICommonWait sinstance;
	protected override void InitComponents()
	{
		sinstance = this;

		var textProgress = FindWidget<Text>("textProgress");

		addUpdateAction(() =>
		{
			var euler = textProgress.transform.localEulerAngles;
			euler.z -= Time.deltaTime * 100;
			textProgress.transform.localEulerAngles = euler;
			return false;
		});
	}
	public static void Show()
	{
		if (sinstance == null)
		{
			LoadAnotherUI<UICommonWait>();
		}
	}
	public static void Hide()
	{
		if (sinstance != null)
		{
			GameObject.Destroy(sinstance.gameObj);
			sinstance = null;
		}
	}
}

