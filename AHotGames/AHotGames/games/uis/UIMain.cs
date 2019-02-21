using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine.UI;
using UnityEngine;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

public class UIMain : AHotBase
{
	Button menuCell;
	protected override void InitComponents()
	{
		var textUsername = FindWidget<Text>("textUsername");
		textUsername.text = UILogin.CachedUsername;

		menuCell = FindWidget<Button>("menuCell");
		menuCell.gameObject.SetActive(false);

		var btnLogout = FindWidget<Button>("btnLogout");
		btnLogout.onClick.AddListener(() =>
		{
			UWebSender.Instance.OnRequest(Utils.BaseURL + "accountlogout"
				, "username=" + UILogin.CachedUsername + "&token=" + UILogin.token, (res) =>
			   {
				   var jres = (JObject)JsonConvert.DeserializeObject(res);
				   var err = jres["err"].ToString();
				   if (err == "0")
				   {
					   UnloadThis();
					   LoadAnother<UILogin>();
				   }
				   else
				   {
					   UIAlert.Show("注销失败：" + Utils.ErrorFormat(err));
				   }
			   }, (fail) =>
			   {
				   UIAlert.Show("web error:" + fail);
			   });

		});

		var menu = GameObject.Instantiate(menuCell, menuCell.transform.parent);
		menu.gameObject.SetActive(true);
		menu.GetComponentInChildren<Text>().text = "孤独的世界";
		menu.onClick.AddListener(() =>
		{
			UnloadThis();
			LoadAnother<GameLonelyWorld>();
		});

		menu = GameObject.Instantiate(menuCell, menuCell.transform.parent);
		menu.gameObject.SetActive(true);
		menu.GetComponentInChildren<Text>().text = "RPG游戏";
		menu.onClick.AddListener(() =>
		{
			UnloadThis();
			LoadAnother<UMUICreateAvatar>();
		});
	}
}

