using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

public class UCardGame : AHotBase
{
	Text textMyLevel;
	Text textMyAvatarname;
	Text textMyCardCount;
	RawImage mycard;

	Transform otherinfo;
	Text textOtherLevel;
	Text textOtherAvatarname;
	Text textOtherCardCount;
	RawImage othercard;

	Button btnJoinRoom;

	Transform cardcell;
	protected override void InitComponents()
	{
		mycard = FindWidget<RawImage>("mycard");
		mycard.gameObject.SetActive(false);
		var btn = mycard.gameObject.AddComponent<Button>();
		btn.onClick.AddListener(() =>
		{
			if (whosTurn != UILogin.CachedUsername)
			{
				UICommonTips.AddTip("还没轮到你出牌。");
				return;
			}
			mycard.color = Color.grey;
			WebSocketConnector.Instance.OnRemoteCall("roomOp", "");
		});
		othercard = FindWidget<RawImage>("othercard");
		othercard.gameObject.SetActive(false);
		var bgpath = new string[] { "Images/Pai/bg1", "Images/Pai/bg2" };
		UHotAssetBundleLoader.Instance.OnDownloadResources(() =>
		{
			mycard.texture = UHotAssetBundleLoader.Instance.OnLoadAsset<Texture2D>(bgpath[random.Next(bgpath.Length)]);
			othercard.texture = UHotAssetBundleLoader.Instance.OnLoadAsset<Texture2D>(bgpath[random.Next(bgpath.Length)]);
		}, bgpath);

		textMyCardCount = FindWidget<Text>("textMyCardCount");
		textMyCardCount.text = "0";

		textMyAvatarname = FindWidget<Text>("textMyAvatarname");
		textMyAvatarname.text = URemoteData.AvatarName;

		textMyLevel = FindWidget<Text>("textMyLevel");
		ShowLevel();

		textOtherCardCount = FindWidget<Text>("textOtherCardCount");
		textOtherCardCount.text = "0";

		textOtherLevel = FindWidget<Text>("textOtherLevel");
		textOtherAvatarname = FindWidget<Text>("textOtherAvatarname");

		RegisterReturnButton();

		btnJoinRoom = FindWidget<Button>("btnJoinRoom");
		btnJoinRoom.onClick.AddListener(() =>
		{
			WebSocketConnector.Instance.OnRemoteCall("joinRoom", "老牛赶大车");
		});

		cardcell = FindWidget<Transform>("cardcell");
		cardcell.gameObject.SetActive(false);

		otherinfo = FindWidget<Transform>("otherinfo");
		otherinfo.gameObject.SetActive(false);
		URemoteData.ListeningParam(InfoNameDefs.AvatarLevel, ShowLevel);

		UICommonWait.Show();
		WebSocketConnector.Instance.OnInit(Utils.WebSocketURL + UILogin.CachedUsernameAndTokenArguments, evt =>
		{
			UICommonWait.Hide();

		}, msgEvt =>
		{
		}, errEvt =>
		{
		}, closeEvt =>
		{
		});

		WebSocketConnector.Instance.OnRegisterResponse("enter", OnJoinRoomCB);
		WebSocketConnector.Instance.OnRegisterResponse("cardsync", OnCardsSync);
		WebSocketConnector.Instance.OnRegisterResponse("result", OnResult);
		WebSocketConnector.Instance.OnRegisterResponse("dismissed", OnDismissed);
	}
	protected override void OnReturn()
	{
		WebSocketConnector.Instance.OnClose();

		base.OnReturn();
	}
	private void OnDismissed(string obj)
	{
		UIAlert.Show("房间已解散。", OnClearRoom, null, true);
	}

	private void OnResult(string obj)
	{
		var jobj = JsonConvert.DeserializeObject(obj) as JObject;
		var loser = jobj["loser"].ToString();
		if (loser != UILogin.CachedUsername)
			UIAlert.Show("你赢了。", OnClearRoom, null, true);
		else
			UIAlert.Show("你输了。", OnClearRoom, null, true);
	}
	private void OnClearRoom()
	{
		otherinfo.gameObject.SetActive(false);
		othercard.gameObject.SetActive(false);
		foreach (var c in lOutModels)
		{
			ReturnCardToPool(c);
		}
		lOutModels.Clear();
		textMyCardCount.text = "0";
	}

	private List<Transform> lCardsPool = new List<Transform>();
	private Transform GetCardFromPool(int cardContent)
	{
		Transform tr = null;
		if (lCardsPool.Count > 0)
		{
			tr = lCardsPool[0];
			lCardsPool.RemoveAt(0);
		}
		else
		{
			tr = GameObject.Instantiate(cardcell, cardcell.parent);
		}
		tr.gameObject.SetActive(true);
		tr.SetAsLastSibling();
		var image = FindWidget<RawImage>(tr, "image");
		image.texture = null;
		var card = new ACards(cardContent);
		UHotAssetBundleLoader.Instance.OnDownloadResources(() =>
		{
			if (image != null)
				image.texture = UHotAssetBundleLoader.Instance.OnLoadAsset<Texture2D>(card.resourcePath);
		}, card.resourcePath);
		return tr;
	}
	private void ReturnCardToPool(Transform tr)
	{
		lCardsPool.Add(tr);
		tr.gameObject.SetActive(false);
	}
	List<int> lCardContents = new List<int>();
	List<Transform> lOutModels = new List<Transform>();
	private void OnCardsSync(string obj)
	{
		var jobj = JsonConvert.DeserializeObject(obj) as JObject;
		if (jobj.ContainsKey("outu"))
		{
			var outu = jobj["outu"].ToString();
			var outc = typeParser.intParse(jobj["outc"].ToString());
			var from = outu == UILogin.CachedUsername ? mycard : othercard;

			var tc = GetCardFromPool(outc);
			lCardContents.Add(outc);
			lOutModels.Add(tc);

			var c = tc.GetChild(0);
			c.position = from.transform.position;
			MoveTo(c, Vector3.zero, 0.2f, Space.Self);
		}

		var fu = jobj.ContainsKey("fu") ? jobj["fu"].ToString() : "";

		if (jobj.ContainsKey("out"))
		{
			var sout = jobj["out"].ToString();
			if (!string.IsNullOrEmpty(sout.Trim()))
			{
				var acards = sout.Split(',');
				for (var i = 0; i < acards.Length; i++)
				{
					var icard = typeParser.intParse(acards[i]);
					if (lCardContents.Count <= i)
					{
						lCardContents.Add(icard);
						lOutModels.Add(GetCardFromPool(icard));
					}
					else if (lCardContents[i] != icard)
					{
						ReturnCardToPool(lOutModels[i]);
						var tr = GetCardFromPool(icard);
						lCardContents[i] = icard;
						lOutModels[i] = tr;
					}
				}
				if (acards.Length < lOutModels.Count)
				{
					ReturnCards(acards.Length, fu);
					lOutModels.RemoveRange(acards.Length, lOutModels.Count - acards.Length);
					lCardContents.RemoveRange(acards.Length, lCardContents.Count - acards.Length);
				}
			}
			else
			{
				ReturnCards(0, fu);
				lOutModels.Clear();
				lCardContents.Clear();
			}
		}

		if (jobj.ContainsKey("counts"))
		{
			var acounts = jobj["counts"] as JArray;
			foreach (var count in acounts)
			{
				if (count["u"].ToString() == UILogin.CachedUsername)
				{
					textMyCardCount.text = count["c"].ToString();
					mycard.gameObject.SetActive(true);
				}
				else
				{
					otherUsername = count["u"].ToString();
					textOtherCardCount.text = count["c"].ToString();
					othercard.gameObject.SetActive(true);
				}
			}
		}

		whosTurn = jobj["curTurn"].ToString();
		mycard.color = whosTurn == UILogin.CachedUsername ? Color.white : Color.grey;
		othercard.color = whosTurn == otherUsername ? Color.white : Color.grey;
	}

	private void ReturnCards(int length, string fu)
	{
		for (var i = length; i < lOutModels.Count; i++)
		{
			if (string.IsNullOrEmpty(fu))
				ReturnCardToPool(lOutModels[i]);
			else
			{
				var from = fu == UILogin.CachedUsername ? mycard : othercard;
				var c = lOutModels[i].GetChild(0);
				MoveTo(c, from.transform.position, 0.2f, Space.World, () =>
				{
					ReturnCardToPool(c.parent);
				});
			}
		}
	}

	private string whosTurn;
	private string otherUsername;

	private void OnJoinRoomCB(string obj)
	{
		btnJoinRoom.gameObject.SetActive(false);
		var jobj = JsonConvert.DeserializeObject(obj) as JObject;
		if (jobj.ContainsKey("infos"))
		{
			var jenter = jobj["infos"] as JArray;
			foreach (var jo in jenter)
			{
				if (jo["name"].ToString() == URemoteData.AvatarName)
					continue;
				SetOtherInfo(jo);
			}
		}
		if (jobj.ContainsKey("enter"))
		{
			var jo = jobj["enter"];
			if (jo["name"].ToString() != URemoteData.AvatarName)
			{
				SetOtherInfo(jo);
			}
		}
	}
	private void SetOtherInfo(JToken jo)
	{
		otherinfo.gameObject.SetActive(true);
		textOtherAvatarname.text = jo["name"].ToString();
		textOtherLevel.text = jo["level"].ToString();
	}

	protected override void OnDestroy()
	{
		URemoteData.CancelListeningParam(InfoNameDefs.AvatarLevel, ShowLevel);
		WebSocketConnector.Instance.OnCloseImmediately();
	}

	private void ShowLevel()
	{
		textMyLevel.text = URemoteData.AvatarLevel;
	}
}

