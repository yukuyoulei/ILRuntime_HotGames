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
	Image Progress;
	protected override void InitComponents()
	{
		sinstance = this;

		TextContent = FindWidget<Text>("TextContent");
		TextContent.text = "";
		Progress = FindWidget<Image>("SliderProgress");
		Progress.fillAmount = 0;
	}
	public override void OnUnloadThis()
	{
		if (Progress.fillAmount == 0)
		{
			base.OnUnloadThis();
			return;
		}
		addUpdateAction(() =>
		{
			if (Progress == null)
			{
				return true;
			}
			if (Progress.fillAmount < 1)
			{
				Progress.fillAmount += Time.deltaTime / 100;
				return false;
			}
			Progress.fillAmount = 1;
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
		Progress.fillAmount = p;
		if (TextContent != null)
			TextContent.text = $"{(p * 100).ToString("f1")}%";
	}
}

