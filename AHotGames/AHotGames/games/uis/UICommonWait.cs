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
		textProgress.text = "==========";

		var n = DateTime.Now;
		addUpdateAction(() =>
		{
			var istart = (int)((DateTime.Now - n).TotalSeconds * 9) % (textProgress.text.Length + 5) - 5;
			var str = "";
			for (var i = 0; i < textProgress.text.Length; i++)
			{
				if (i >= istart && i < istart + 5)
				{
					str += ">";
				}
				else
				{
					str += "=";
				}
			}
			textProgress.text = str;
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

