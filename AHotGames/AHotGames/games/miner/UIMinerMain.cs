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
	Text textDiamond;
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
		textDiamond = FindWidget<Text>("textDiamond");
		textDiamond.text = "0";

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
			//AClientApis.OnPay(1);
			AClientApis.OnCreateOrder(1);
		});
		var btnExchange = FindWidget<Button>("btnExchange");
		btnExchange.onClick.AddListener(() =>
		{
			AClientApis.OnExchange(1);
		});

		OnParamUpdateCb(null);
		RegisterEvent(UEvents.ParamUpdate, OnParamUpdateCb);
		RegisterEvent(UEvents.CreateOrder, OnCreateOrderCb);

		OnRegistAction(str =>
		{
			UICommonWait.Hide();
			var astrs = str.Split(' ');
			switch (astrs[0])
			{
				case "ProvideContent":
					UIAlert.Show("ProvideContent " + str);
					break;
				case "FailedTransactions":
					UIAlert.Show("FailedTransactions " + str);
					break;
				case "UpdateTransactions":
					UIAlert.Show("UpdateTransactionss " + str);
					break;
			}
		});
	}

	private void OnCreateOrderCb(UEventBase obj)
	{
		var eb = obj as EventCreateOrder;
		AOutput.Log($"obj {eb.eResult} {eb.orderID} {eb.extraInfo}");
		//AClientApis.OnPay(1);

		UICommonWait.Show();
		OnSendSDKMessage("SDK_AppleInApp", "OnBuyProduct", "0");
	}

	private void OnParamUpdateCb(UEventBase eb)
	{
		textGold.text = CakeClient.GetCake("items", CakeAvatar.myID, LibCommon.InitValueDefs.gold.ToString()).GetIntValue(ParamNameDefs.Count).ToString();
		textDiamond.text = CakeClient.GetCake("items", CakeAvatar.myID, LibCommon.InitValueDefs.money.ToString()).GetIntValue(ParamNameDefs.Count).ToString();
		btnDailyCheck.gameObject.SetActive(!ApiDateTime.IsSameDay(CakeClient.GetCake("pinfo", CakeAvatar.myID).GetIntValue(ParamNameDefs.LastDailyCheckTime)));
	}
}

