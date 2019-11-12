using System;
using System.Collections.Generic;
using MongoDB.Bson;
using Newtonsoft.Json.Linq;

public class ARoomCard1 : ARoomBase
{
	ARoomCard1Avatar ai;
	Dictionary<DateTime, List<Action>> dActions = new Dictionary<DateTime, List<Action>>();
	List<DateTime> removes = new List<DateTime>();
	public override void OnTick()
	{
		TickActions();
	}

	private void TickActions()
	{
		if (dActions.Count == 0)
			return;
		{
			foreach (var kv in dActions)
			{
				if (kv.Key < ApiDateTime.Now)
				{
					removes.Add(kv.Key);
				}
			}
			if (removes.Count > 0)
			{
				foreach (var k in removes)
				{
					foreach (var a in dActions[k])
						a();
					dActions.Remove(k);
				}
				removes.Clear();

			}
		}
	}


	private void DelayDo(float seconds, Action doAction)
	{
		var t = ApiDateTime.Now.AddSeconds(seconds);
		if (!dActions.ContainsKey(t))
			dActions.Add(t, new List<Action>());
		dActions[t].Add(doAction);
	}

	protected override void DoEnter(AAvatar avatar)
	{
		if (eSession == ESession.Waiting)
		{
			if (IsFull)
			{
				eSession = ESession.Started;
				DelayDo(1, StartToFaPai);
			}
			else if (!IsFull && ai == null)
			{
				ai = new ARoomCard1Avatar("AI1", "AI陪练", null);
				DelayDo(1, () =>
				{
					OnEnter(ai);
				});
			}
		}
		else if (eSession == ESession.Started)
		{
			SyncCards();
		}
	}

	Dictionary<string, List<int>> dCards = new Dictionary<string, List<int>>();
	private void StartToFaPai()
	{
		List<int> allCards = new List<int>();
		for (var i = 0; i < 54; i++)
		{
			allCards.Add(i);
		}

		foreach (var u in lUsernames)
		{
			if (allCards.Count == 0) break;

			if (!dCards.ContainsKey(u))
			{
				dCards.Add(u, new List<int>());
			}

			for (var i = 0; i < 27; i++)
			{
				if (allCards.Count == 0) break;
				var idx = ApiRandom.Instance.Next(allCards.Count);
				dCards[u].Add(allCards[idx]);
				allCards.RemoveAt(idx);
			}
		}

		curTurn = ApiRandom.Instance.Next(FullNumber);
		SyncCards();

		DelayDo(1, CheckProcess);
	}

	int curTurn = 0;
	private void CheckProcess()
	{
		foreach (var kv in dCards)
		{
			if (kv.Value.Count == 0)
			{
				OnResult(kv.Key);
				return;
			}
		}
		var a = GetCurTurn();
		if (a != null && a.bAI)
		{
			DelayDo(1, DoAI);
		}
	}

	private void DoAI()
	{
		var ai = GetCurTurn();
		if (ai == null) return;
		ChuPai(ai.username);
	}

	private void ChuPai(string username)
	{
		lastOperateUser = username;

		var c = dCards[username][0];
		dCards[username].RemoveAt(0);
		lOuts.Add(new ACards(c));
		SyncCards(null, username, c);

		DelayDo(1, () => { CheckCards(username); });
	}

	List<int> lCardsToFetch = new List<int>();
	private void CheckCards(string username)
	{
		var fetchedTurn = -1;
		if (lOuts.Count > 1)
		{
			var c = lOuts[lOuts.Count - 1];
			lCardsToFetch.Add(c.content);

			var ifetch = -1;
			for (var i = lOuts.Count - 2; i >= 0; i--)
			{
				lCardsToFetch.Add(lOuts[i].content);
				if (lOuts[i].number == c.number
					|| c.color == ACards.EColor.Joker && lOuts[i].color == ACards.EColor.Joker)
				{
					ifetch = i;
					fetchedTurn = curTurn;
					break;
				}
			}
			if (ifetch >= 0)
			{
				lOuts.RemoveRange(ifetch, lCardsToFetch.Count);
				dCards[username].AddRange(lCardsToFetch);
			}
		}

		lastOperateUser = "";
		if (fetchedTurn == -1)
		{
			curTurn = (curTurn + 1) % FullNumber;
			SyncCards();
			DelayDo(1, CheckProcess);
		}
		else
		{
			curTurn = fetchedTurn;
			DelayDo(1, () =>
			{
				SyncCards(lCardsToFetch);
				DelayDo(1, CheckProcess);
			});
		}
		lCardsToFetch.Clear();
	}

	private void OnResult(string loser)
	{
		var jobj = new JObject();
		jobj["loser"] = loser;
		BroadcastToAll("result", jobj.ToString());

		eSession = ESession.Ended;
		DelayDo(2, DoDismiss);
	}

	private AAvatar GetCurTurn()
	{
		if (curTurn == -1) return null;
		if (curTurn >= lUsernames.Count) return null;
		var l = lUsernames[curTurn];
		if (dAIs.ContainsKey(l)) return dAIs[l];
		return AAvatarManager.Instance.OnGetAvatar(l);
	}

	List<ACards> lOuts = new List<ACards>();
	private void SyncCards(List<int> lfetched = null, string outUser = "", int outc = -1)
	{
		var jobj = new JObject();

		var loutcards = new List<int>();
		foreach (var o in lOuts)
			loutcards.Add(o.content);
		jobj["out"] = string.Join(",", loutcards);
		if (!string.IsNullOrEmpty(outUser))
		{
			jobj["outu"] = outUser;
			jobj["outc"] = outc;
		}
		if (lfetched != null)
		{
			jobj["fu"] = GetCurTurn().username;
			jobj["fs"] = string.Join(",", lfetched);
		}

		var jarray = new JArray();
		foreach (var u in lUsernames)
		{
			var ju = new JObject();
			ju["u"] = u;
			ju["c"] = dCards[u].Count;
			jarray.Add(ju);
		}
		foreach (var u in dAIs.Keys)
		{
			var ju = new JObject();
			ju["u"] = u;
			ju["c"] = dCards[u].Count;
			jarray.Add(ju);
		}
		jobj["counts"] = jarray;
		var curTurnUser = "";
		var a = GetCurTurn();
		if (a.username != lastOperateUser)
			curTurnUser = a.username;
		jobj["curTurn"] = curTurnUser;
		BroadcastToAll("cardsync", jobj.ToString());
	}

	protected override void DoExit(string username)
	{
		DoDismiss();
	}

	protected override void DoStart()
	{

	}

	private string lastOperateUser;
	public override void OnOperation(string username, string arg3)
	{
		if (GetCurTurn()?.username != username) return;
		if (lastOperateUser == username) return;
		ChuPai(username);
	}

	private class ARoomCard1Avatar : AAvatar
	{
		public ARoomCard1Avatar(string username, string nickname, BsonDocument dbdocument) : base(username, nickname, dbdocument)
		{
			bAI = true;
		}

		public override void RegisterAllComponents()
		{
			base.RegisterAllComponents();
		}

		public override void RegisterAllParams()
		{
			base.RegisterAllParams();
		}
	}
}
