using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine.UI;
using UnityEngine;

public class UICommonTips : AHotBase
{
	protected override bool bCanBeAutoClosed => false;
	static UICommonTips sinstance;
	Text tipcell;
	protected override void InitComponents()
	{
		sinstance = this;

		tipcell = FindWidget<Text>("tipcell");
		tipcell.gameObject.SetActive(false);
	}

	public static void AddTip(string content)
	{
		if (sinstance == null) LoadAnotherUI<UICommonTips>(instance => { sinstance = instance; sinstance.DoAddTip(content); });
		else sinstance.DoAddTip(content);
	}

	private void DoAddTip(string content)
	{
		var cell = GameObject.Instantiate(tipcell.gameObject, tipcell.transform.parent).GetComponent<Text>();
		cell.gameObject.SetActive(true);
		cell.text = content;
		var start = ApiDateTime.Now;
		addUpdateAction(() =>
		{
			if ((ApiDateTime.Now - start).TotalSeconds > 1)
			{
				GameObject.Destroy(cell.gameObject);
				return true;
			}
			var p = cell.transform.localPosition;
			p.y += Time.deltaTime * ((ApiDateTime.Now - start).TotalSeconds > 0.1 ? 20 : 100);
			cell.transform.localPosition = p;
			return false;
		});
	}
}

