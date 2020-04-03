using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MongoDB.Bson;
using LibPacket;
using LibCommon;
using LibServer.GameObj;
using LibNet.rpc;

public class APlayerManager : Singleton<APlayerManager>
{
	private Dictionary<string, Player> dConnectionIndexedPlayers = new Dictionary<string, Player>();
	public void ListAll()
	{
		AOutput.Log($"Total:{dConnectionIndexedPlayers.Count}");
	}

	internal void OnPlayerOffline(string connectionDesc)
	{
		if (!dConnectionIndexedPlayers.ContainsKey(connectionDesc)) return;
		dConnectionIndexedPlayers.Remove(connectionDesc);
	}

	private Dictionary<string, Player> dUIDIndexedPlayers = new Dictionary<string, Player>();
	public void OnAddPlayer(string uid, EPartnerID ePartnerID, IResponer client)
	{
		if (dUIDIndexedPlayers.ContainsKey(uid))
		{
			dUIDIndexedPlayers[uid].client.Response(new PktServerMessage() { message = "duplicatelogin" });
			dUIDIndexedPlayers[uid].OnDisconnect();
			dUIDIndexedPlayers.Remove(uid);
		}
		if (dConnectionIndexedPlayers.ContainsKey(client.playerConnDesc))
		{
			dConnectionIndexedPlayers[client.playerConnDesc].OnDisconnect();
			dConnectionIndexedPlayers.Remove(client.playerConnDesc);
		}
		var p = new Player(uid, ePartnerID, client);
		dConnectionIndexedPlayers.Add(client.playerConnDesc, p);
		dUIDIndexedPlayers.Add(uid, p);
	}
	public Player OnGetPlayerByID(string psid)
	{
		if (dUIDIndexedPlayers.ContainsKey(psid))
			return dUIDIndexedPlayers[psid];
		return null;
	}
	public Player OnGetPlayerByConn(string playerConnDesc)
	{
		if (dConnectionIndexedPlayers.ContainsKey(playerConnDesc))
			return dConnectionIndexedPlayers[playerConnDesc];
		return null;
	}
	Dictionary<string, Player> dPlayers = new Dictionary<string, Player>();
	public void OnAddAvatar(CakeAvatar avatar, Player player)
	{
		var avatarid = avatar.id;
		if (dPlayers.ContainsKey(avatarid))
		{
			dPlayers[avatarid].OnDisconnect();
			dPlayers.Remove(avatarid);
		}
		dPlayers.Add(avatarid, player);
		player.avatarCake = avatar;
	}
	public Player OnGetPlayerByPSID(string psid)
	{
		if (!dPlayers.ContainsKey(psid)) return null;
		return dPlayers[psid];
	}
	internal bool IsOnline(string avatarid)
	{
		return dPlayers.ContainsKey(avatarid);
	}

	internal void SendTo(string psid, PktBase pkt)
	{
		var p = OnGetPlayerByPSID(psid);
		if (p == null) return;
		p.client.Response(pkt);
	}
	public static void SendToClient(string psid, PktBase pkt)
	{
		Instance.SendTo(psid, pkt);
	}
}

public class Player
{
	public Player(string uid, EPartnerID ePartnerID, IResponer client)
	{
		this.client = client;
		this.unionid = uid;
		this.ePartnerID = ePartnerID;
	}
	public CakeAvatar avatarCake;
	public string psid { get { return avatarCake == null ? "" : avatarCake.id; } }
	public EPartnerID ePartnerID;
	public string unionid;
	public IResponer client { get; }
	public void OnDisconnect()
	{
		(client as CResponser).Client.CloseConnection();
	}
}