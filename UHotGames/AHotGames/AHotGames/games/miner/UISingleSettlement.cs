using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine.UI;
using UnityEngine;

public class UISingleSettlement : AHotBase
{
	Text textRet;
	protected override void InitComponents()
	{
		textRet = FindWidget<Text>("textRet");
		textRet.text = "";
		var btnReturnCity = FindWidget<Button>("btnReturnCity");
		btnReturnCity.onClick.AddListener(() =>
		{
			AClientApis.OnEnterCity();
		});
	}

	internal void SetData(EventSettlement eb)
	{
		textRet.text = eb.pdata.strArg;
	}
}

