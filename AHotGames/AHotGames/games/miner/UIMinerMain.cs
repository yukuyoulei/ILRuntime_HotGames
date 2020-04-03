using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine.UI;
using UnityEngine;
using LibClient;
using LibCommon;
using LibClient.GameObj;

public class UIMinerMain : AHotBase
{
	Text textGold;
	Button btnDailyCheck;
	protected override void InitComponents()
	{
		btnDailyCheck = FindWidget<Button>("btnDailyCheck");
		btnDailyCheck.onClick.AddListener(() =>
		{
			AClientApis.OnDailyCheck();
		});

		var textUsername = FindWidget<Text>("textUsername");
		textUsername.text = CakeClient.GetCake("pinfo", CakeAvatar.myID).GetStringValue(ParamNameDefs.AvatarName);
		textGold = FindWidget<Text>("textGold");
		textGold.text = "0";
		OnParamUpdateCb(null);

		var btnLogout = FindWidget<Button>("btnLogout");
		btnLogout.onClick.AddListener(() =>
		{
			AClientApp.OnDisconnect();
			OnUnloadThis();
			LoadAnotherUI<UIMinerLogin>();
		});

		var btnPay = FindWidget<Button>("btnPay");
		btnPay.onClick.AddListener(() =>
		{
			AClientApis.OnPay(1);
		});
		var btnExchange = FindWidget<Button>("btnExchange");
		btnExchange.onClick.AddListener(() =>
		{
			AClientApis.OnExchange(1);
		});

		RegisterEvent(UEvents.ParamUpdate, OnParamUpdateCb);

		//var map01 = LoadClass<UMinerMap>("UI/MinerMap/Map01");
		AClientApis.OnGetSdata("");
	}

	private void OnParamUpdateCb(UEventBase eb)
	{
		textGold.text = CakeClient.GetCake("items", CakeAvatar.myID, LibCommon.InitValueDefs.gold.ToString()).GetIntValue(ParamNameDefs.Count).ToString();
		btnDailyCheck.gameObject.SetActive(!ApiDateTime.IsSameDay(CakeClient.GetCake("pinfo", CakeAvatar.myID).GetIntValue(ParamNameDefs.LastDailyCheckTime)));
	}
}

