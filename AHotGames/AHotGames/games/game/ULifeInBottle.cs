using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine.UI;
using UnityEngine;

public class ULifeInBottle : AHotBase
{
	WExpSlider expSlider;
	Button btnReturn;
	protected override void InitComponents()
	{
		var tr = FindWidget<Transform>("expBg");
		expSlider = new WExpSlider();
		expSlider.SetGameObj(tr.gameObject);

		btnReturn = FindWidget<Button>("btnReturn");
		btnReturn.onClick.AddListener(() =>
		{
			OnUnloadThis();

			LoadAnotherUI<UIMain>();
		});
	}
}

