using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine.UI;
using UnityEngine;
using Newtonsoft.Json.Linq;

public class UIRank : AHotBase
{
	Transform rankcell;
	Button btnNext;
	Text textTitle;
	private static string[] tops = new string[] { "GetTopLevel", "GetTopGold" };
	private static string[] topsTitle = new string[] { "等级排行榜", "金币排行榜" };
	private static string[] topsRankTag = new string[] { "等级：", "金币：" };
	private int curTopIndex = 0;
	protected override void InitComponents()
	{
		var btnReturn = FindWidget<Button>("btnReturn");
		btnReturn.onClick.AddListener(() =>
		{
			OnUnloadThis();
		});
		btnNext = FindWidget<Button>("btnNext");
		btnNext.onClick.AddListener(() =>
		{
			curTopIndex++;
			curTopIndex = curTopIndex % tops.Length;
			OnGetTop(curTopIndex);
		});
		textTitle = FindWidget<Text>("textTitle");

		rankcell = FindWidget<Transform>("rankcell");
		rankcell.gameObject.SetActive(false);

		OnGetTop(curTopIndex);
	}

	List<Transform> lCells = new List<Transform>();
	private void OnGetTop(int curTopIndex)
	{
		textTitle.text = topsTitle[curTopIndex];
		foreach (var lc in lCells)
		{
			GameObject.Destroy(lc.gameObject);
		}
		lCells.Clear();

		UStaticWebRequests.OnWebRequest("Rank/" + tops[curTopIndex], "count=10", res =>
		{
			var jarray = res["r"] as JArray;
			foreach (var ja in jarray)
			{
				var obj = GameObject.Instantiate(rankcell, rankcell.parent);
				lCells.Add(obj);
				obj.gameObject.SetActive(true);

				FindWidget<Text>(obj, "avatarName").text = ja["name"].ToString();
				FindWidget<Text>(obj, "avatarValue").text = ja["value"].ToString();
				FindWidget<Text>(obj, "textRankTag").text = topsRankTag[curTopIndex];
			}
		});
	}
}

