using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine.UI;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class UMMORPG : AHotBase
{
	protected override void InitComponents()
	{
		UWebSender.Instance.OnRequest(Utils.BaseURL + "avatarlist", "username=" + UILogin.CachedUsername + "&token=" + UILogin.token, (res) =>
		{
			var jres = (JObject)JsonConvert.DeserializeObject(res);
			var err = jres["err"].ToString();
			if (err == "0")
			{
				UWebSender.Instance.OnRequest(Utils.BaseURL + "avatarselect", "username=" + UILogin.CachedUsername + "&token=" + UILogin.token, (ressel) =>
				{
					var jressel = (JObject)JsonConvert.DeserializeObject(ressel);
					var errsel = jressel["err"].ToString();
					if (errsel == "0")
					{
						Debug.Log("select avatar");
					}
				}, (errsel) => { });
			}
			else if (err == "3")
			{
				Debug.Log("create avatar");
                UnloadThis();
				LoadAnother<UMUICreateAvatar>();
			}
		}, (err) => { });
	}
}

