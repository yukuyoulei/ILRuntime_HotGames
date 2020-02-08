using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MongoDB.Bson;
using LibPacket;
using LibCommon;

public class AAvatarManager : Singleton<AAvatarManager>
{
	private Dictionary<string, AAvatarServer> dUIDIndexedAvatars = new Dictionary<string, AAvatarServer>();
	private Dictionary<string, Player> dConnectionIndexedPlayers = new Dictionary<string, Player>();
	private void OnAddAvatar(AAvatarServer avatar, Player player)
	{
		string uid = avatar.uid;
		if (!dUIDIndexedAvatars.ContainsKey(uid))
			dUIDIndexedAvatars.Add(uid, avatar);

		player.uid = uid;
	}
	public AAvatarServer OnGetAvatar(string uid)
	{
		if (string.IsNullOrEmpty(uid)) return null;

		if (dUIDIndexedAvatars.ContainsKey(uid))
		{
			return dUIDIndexedAvatars[uid];
		}
		return null;
	}
	public AAvatarServer OnGetAvatarByConn(string playerConnDesc)
	{
		if (dConnectionIndexedPlayers.ContainsKey(playerConnDesc))
			return dConnectionIndexedPlayers[playerConnDesc].avatar;
		return null;
	}

	public void OnTick()
	{
		foreach (var a in dUIDIndexedAvatars.Values)
		{
			a.OnTick();
		}
	}

	public AAvatarServer OnCreateAvatar(EPartnerID ePartnerID, string uid, string avatarName, int sex, Player player)
	{
		var oid = ObjectId.GenerateNewId();
		var inst = ADBManager.Get(InitValueDefs.dbconnect, InitValueDefs.dbname).UpdateOneData(ParamNameDefs.TableAvatar
			, ADBAccessor.filter_eq(ParamNameDefs.UID, uid) & ADBAccessor.filter_eq(ParamNameDefs.PartnerID, (int)ePartnerID) & ADBAccessor.filter_eq(ParamNameDefs.AvatarName, avatarName)
			, ADBAccessor.updates_build(ADBAccessor.update(ParamNameDefs.AvatarSex, sex), ADBAccessor.update(ParamNameDefs.CollectionID, oid)), true);
		var a = new AAvatarServer(uid, null, oid, player);
		OnAddAvatar(a, player);
		return a;
	}
	internal AAvatarServer OnCreateAvatar(string uid, BsonDocument dbr, Player player)
	{
		var a = new AAvatarServer(uid, dbr, dbr[ParamNameDefs.CollectionID].AsObjectId, player);
		OnAddAvatar(a, player);
		return a;
	}

	public void OnAddPlayer(string uid, EPartnerID ePartnerID, IResponer client)
	{
		if (dConnectionIndexedPlayers.ContainsKey(client.playerConnDesc))
		{
			dConnectionIndexedPlayers[client.playerConnDesc].client.Response(new PktServerMessage() { message = "duplicatelogin" });
			if (dConnectionIndexedPlayers[client.playerConnDesc].avatar != null)
				dConnectionIndexedPlayers[client.playerConnDesc].avatar.OnDispose();
			dConnectionIndexedPlayers.Remove(client.playerConnDesc);
		}
		dConnectionIndexedPlayers.Add(client.playerConnDesc, new Player(uid, ePartnerID, client));
	}
	public Player OnGetPlayer(string playerConnDesc)
	{
		if (dConnectionIndexedPlayers.ContainsKey(playerConnDesc))
			return dConnectionIndexedPlayers[playerConnDesc];
		return null;
	}
}

public class Player
{
	public Player(string uid, EPartnerID ePartnerID, IResponer client)
	{
		this.client = client;
		this.uid = uid;
		this.ePartnerID = ePartnerID;
	}
	public EPartnerID ePartnerID;
	public string uid;
	public IResponer client { get; }
	public AAvatarServer avatar
	{
		get
		{
			return AAvatarManager.Instance.OnGetAvatar(uid);
		}
	}
}