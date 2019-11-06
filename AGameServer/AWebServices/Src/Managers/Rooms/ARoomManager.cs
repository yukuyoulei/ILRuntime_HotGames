using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

public class ARoomManager : Singleton<ARoomManager>
{
	Dictionary<string, Func<ARoomBase>> dCreations = new Dictionary<string, Func<ARoomBase>>();
	public ARoomManager()
	{
		dCreations.Add("老牛赶大车", () => { return new ARoomCard1(); });
	}
	public void OnTick()
	{
		foreach (var r in dRooms.Values)
		{
			r.OnTick();
		}
	}
	private Dictionary<string, ARoomBase> dRooms = new Dictionary<string, ARoomBase>();
	public ARoomBase OnCreate(string roomType)
	{
		var room = dCreations[roomType]();
		room.roomType = roomType;
		return room;
	}
	public void OnEnter(AAvatar avatar, ARoomBase room)
	{
		if (!avatar.bAI)
		{
			if (dRooms.ContainsKey(avatar.username))
			{
				if (dRooms[avatar.username].roomType != room.roomType)
				{
					dRooms[avatar.username].OnExit(avatar.username);
					dRooms.Remove(avatar.username);
				}
			}

			if (!dRooms.ContainsKey(avatar.username))
				dRooms.Add(avatar.username, room);
		}

		room.OnEnter(avatar);
	}

	public ARoomBase OnGetRoom(string username, string roomType = "")
	{
		if (string.IsNullOrEmpty(roomType))
		{
			if (dRooms.ContainsKey(username)) return dRooms[username];
			return null;
		}
		if (!dRooms.ContainsKey(username))
		{
			foreach (var m in dRooms.Values)
			{
				if (m.roomType == roomType && !m.IsFull)
					return m;
			}
			return null;
		}
		if (dRooms[username].roomType != roomType)
			OnExit(username);
		return dRooms[username];
	}

	internal void OnOffline(string username)
	{
		AOutput.Log($"OnOffline {username}");
		OnExit(username);
	}

	public void OnExit(string username)
	{
		if (!dRooms.ContainsKey(username))
		{
			return;
		}
		var room = dRooms[username];
		dRooms.Remove(username);
		room.OnExit(username);
	}

	internal void OnRemove(ARoomBase aRoomBase)
	{
		aRoomBase.OnDismiss();
	}
}
public abstract class ARoomBase
{
	public string roomType { get; set; }
	protected virtual int FullNumber { get { return 2; } }
	public bool IsFull { get { return FullNumber <= lUsernames.Count + dAIs.Count; } }
	protected List<string> lUsernames = new List<string>();
	protected Dictionary<string, AAvatar> dAIs = new Dictionary<string, AAvatar>();
	public void OnEnter(AAvatar avatar)
	{
		if (lUsernames.Contains(avatar.username)) return;
		if (avatar.bAI)
			dAIs.Add(avatar.username, avatar);

		lUsernames.Add(avatar.username);
		BroadcastUserEntrance(avatar);
		DoEnter(avatar);

		if (IsFull)
		{
			OnStart();
		}
	}

	private void BroadcastUserEntrance(AAvatar avatar)
	{
		var jinfo = avatar.GetSimpleInfo(this);
		var jentrance = new JObject();
		jentrance["enter"] = jinfo;
		var smsg = jentrance.ToString();
		var jothers = new JArray();
		foreach (var l in lUsernames)
		{
			if (dAIs.ContainsKey(l)) continue;
			BroadcastToAll("enter", smsg, avatar.username);
			jothers.Add(AAvatarManager.Instance.OnGetAvatar(l).GetSimpleInfo(this));
		}

		if (avatar.bAI) return;
		foreach (var l in dAIs.Values)
		{
			jothers.Add(l.GetSimpleInfo(this));
		}

		jentrance["infos"] = jothers;
		smsg = jentrance.ToString();
		avatar.OnSendToClient("enter", smsg);
	}

	protected abstract void DoEnter(AAvatar avatar);

	public void OnExit(string username)
	{
		if (!lUsernames.Contains(username)) return;
		lUsernames.Remove(username);
		ARoomManager.Instance.OnExit(username);

		DoExit(username);
	}
	protected abstract void DoExit(string username);

	protected bool bStarted = false;
	private void OnStart()
	{
		bStarted = true;
		DoStart();
	}
	protected abstract void DoStart();

	protected void BroadcastToAll(string method, string msg, string except = "")
	{
		foreach (var id in lUsernames)
		{
			if (id == except) continue;
			if (dAIs.ContainsKey(id)) continue;
			if (!AAvatarManager.Instance.HasAvatar(id)) continue;
			var avatar = AAvatarManager.Instance.OnGetAvatar(id);
			if (avatar == null) continue;
			if (avatar.client == null) continue;
			avatar.OnSendToClient(method, msg);
		}
	}
	public abstract void OnTick();

	protected void DoDismiss()
	{
		ARoomManager.Instance.OnRemove(this);
	}

	public abstract void OnOperation(string username, string arg3);

	internal void OnDismiss()
	{
		BroadcastToAll("dismissed", "");

		lUsernames.Clear();
		dAIs.Clear();
	}

	protected ESession eSession;
	public enum ESession
	{
		Waiting,
		Started,
		Ended,
	}
}