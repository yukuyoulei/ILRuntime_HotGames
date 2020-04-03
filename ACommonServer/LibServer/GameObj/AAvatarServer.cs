using LibPacket;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using LibCommon;

public partial class AAvatarServer : AAvatarCommon
{
	public ObjectId objectId;
	public AAvatarServer(string uid, BsonDocument dbdocument, ObjectId objectId, Player player)
	{
		this.player = player;
		this.uid = uid;
		objID = this.uid;
		this.objectId = objectId;

		RegisterAllComponents();
		RegisterAllParams();

		if (dbdocument != null)
		{
			(componentParam as AComponentParamServer).OnRead(dbdocument);
		}

		InitAllComponents();
	}

	public override void InitAllComponents()
	{
		foreach (var c in allComponents)
		{
			c.InitComponent();
		}

		OnInitComplete();
	}

	internal AvatarInfo ToPkt()
	{
		var info = new AvatarInfo();
		info.avatarID = objectId.ToString();
		info.avatarName = AvatarName;
		return info;
	}

	public string playerConnDesc
	{
		get
		{
			return client?.playerConnDesc;
		}
	}
	public Player player { get; set; }
	public IResponer client
	{
		get { return player?.client; }
	}
	public void OnSendToClient(PktBase pkt)
	{
		if (client == null) return;
		client.Response(pkt);
	}

	#region components
	AComponentBagServer _componentBag;
	public AComponentBagServer componentBag
	{
		get
		{
			if (_componentBag == null)
			{
				_componentBag = OnGetComponent<AComponentBagServer>();
			}
			return _componentBag;
		}
	}

	#endregion

	internal void OnTick()
	{
		if (componentParam == null) return;
		if (bAI) return;

		componentParam.OnSave();
	}
	public override void RegisterAllComponents()
	{
		RegisterComponent(new AComponentBagServer(this));
		RegisterComponent(new AComponentParamServer(this));
	}

	void OnInitComplete()
	{
		if (AvatarLevel == 0)
		{
			AvatarLevel = 1;

			OnSetParamValue(ParamNameDefs.AvatarMoney, 1000);
			OnSetParamValue(ParamNameDefs.AvatarGold, 1000);
		}

		MaxEXP = MaxEXP;
	}

	public void OnAddExp(int exp)
	{
		var curExp = CurExp;
		var ilast = MaxEXP - curExp;
		if (ilast > exp)
		{
			CurExp += exp;
		}
		else
		{
			CurExp = exp - ilast;
			DoLevelUp();
			while (CurExp >= MaxEXP)
			{
				CurExp -= MaxEXP;
				DoLevelUp();
			}
		}

	}

	private void DoLevelUp()
	{
		AvatarLevel++;
		MaxEXP = MaxEXP;
	}
}
