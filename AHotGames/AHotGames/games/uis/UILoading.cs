using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine.UI;
using UnityEngine;

public class UILoading : AHotBase
{
	private static UILoading sinstance;
	public static UILoading Instance
	{
		get
		{
			return sinstance;
		}
	}
	Text TextContent;
	Image imageProgress;
	protected override void InitComponents()
	{
		sinstance = this;

		TextContent = FindWidget<Text>("TextContent");
		TextContent.text = "";
		imageProgress = FindWidget<Image>("SliderProgress");
		imageProgress.fillAmount = 0;
	}
	public override void OnUnloadThis()
	{
		if (imageProgress.fillAmount == 0)
		{
			base.OnUnloadThis();
			return;
		}
		addUpdateAction(() =>
		{
			if (imageProgress == null)
			{
				return true;
			}
			if (imageProgress.fillAmount < 1)
			{
				imageProgress.fillAmount += Time.deltaTime / 100;
				return false;
			}
			imageProgress.fillAmount = 1;
			base.OnUnloadThis();
			return true;
		});
	}
	public void OnSetJindu(string str)
	{
		UDebugHotLog.Log($"OnSetJindu [{str}]");
	}
	public void OnSetProgress(float p)
	{
		if (imageProgress != null)
			imageProgress.fillAmount = p;
		if (TextContent != null)
			TextContent.text = $"{(p * 100).ToString("f1")}%";
	}
}

