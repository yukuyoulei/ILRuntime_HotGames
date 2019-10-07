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
	Text textNovel;
	Button btnLeft;
	Button btnRight;

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

		textNovel = FindWidget<Text>("textNovel");
		textNovel.text = GetCurPage();

		btnLeft = FindWidget<Button>("btnLeft");
		btnLeft.gameObject.SetActive(iCurPage > 0);
		btnLeft.onClick.AddListener(() =>
		{
			var c = Novels.GetContent(iCurSection, iCurPage - 1);
			if (c == null)
			{
				UIAlert.Show("已到达本章第一页。");
				btnLeft.gameObject.SetActive(true);
				return;
			}
			textNovel.text = c;
			iCurPage--;
			btnRight.gameObject.SetActive(true);
		});
		btnRight = FindWidget<Button>("btnRight");
		btnRight.gameObject.SetActive(!Novels.IsLastPage(iCurSection, iCurPage));
		btnRight.onClick.AddListener(() =>
		{
			var c = Novels.GetContent(iCurSection, iCurPage + 1);
			if (c == null)
			{
				UIAlert.Show("已到达本章最后一页。");
				btnRight.gameObject.SetActive(false);
				return;
			}
			textNovel.text = c;
			iCurPage++;
			btnLeft.gameObject.SetActive(true);
		});
	}

	private int iCurPage
	{
		get
		{
			return PlayerPrefs.GetInt("cp");
		}
		set
		{
			PlayerPrefs.SetInt("cp", value);
		}
	}
	private int iCurSection
	{
		get
		{
			return PlayerPrefs.GetInt("cs");
		}
		set
		{
			PlayerPrefs.SetInt("cs", value);
		}
	}
	private string GetCurPage()
	{
		return Novels.GetContent(iCurSection, iCurPage);
	}
}

