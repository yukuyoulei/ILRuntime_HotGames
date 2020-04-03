using System;
using System.Collections.Generic;
using System.Text;
using LibPacket;
using LibServer.Managers;

public abstract class ContaBase : Context, LibNet.ITickableSystem
{
	protected CtSystem _CtSystem;
	public CtSystem CtSystem { get { if (_CtSystem == null) _CtSystem = GetSystem<CtSystem>(); return _CtSystem; } }
	protected NetSystem _NetSystem;
	public NetSystem NetSystem { get { if (_NetSystem == null) _NetSystem = GetSystem<NetSystem>(); return _NetSystem; } }
	protected PlayerSystem _PlayerSystem;
	public PlayerSystem PlayerSystem { get { if (_PlayerSystem == null) _PlayerSystem = GetSystem<PlayerSystem>(); return _PlayerSystem; } }
	protected virtual bool IsSingle { get { return true; } }
	public ContaBase()
	{
		AddSystem(new CtSystem(this));
		AddSystem(new NetSystem(this));
		AddSystem(new PlayerSystem(this));
	}

	internal void leave(string psid)
	{
		var entity = GetPlayer(psid);
		if (entity == null) return;
		remove(entity);

		Broadcast(new PktLeaveConta() { uid = entity.uid });

		entity.destroy();

		dAllPlayers.Remove(psid);

		if (IsSingle)
			destroy();
	}

	public void destroy()
	{
		foreach (var e in entities)
		{
			e.Value.destroy();
			AContaManager.Instance.OnLeaveConta(e.Value.id);
		}
		entities.Clear();

		dSystems.Clear();

		AContaManager.Instance.OnRemove(this);
	}

	internal void Broadcast(PktBase pkt)
	{
		foreach (var player in dAllPlayers)
		{
			var p = APlayerManager.Instance.OnGetPlayerByID(player.Key);
			if (p == null || p.client == null) continue;
			p.client.Response(pkt);
		}
	}

	public void Tick(double fDeltaSec)
	{
		CtSystem.Tick(fDeltaSec);
	}
	Dictionary<string, Entity> dAllPlayers = new Dictionary<string, Entity>();
	protected Entity GetPlayer(string psid)
	{
		return dAllPlayers.ContainsKey(psid) ? dAllPlayers[psid] : null;
	}
	protected void AddPlayer(Entity entity)
	{
		dAllPlayers.Add(entity.id, entity);
	}
	public virtual void EnterScene(string psid)
	{
		var entity = GetPlayer(psid);
		if (entity == null) return;

		NetSystem.all2me(entity);
	}
}
