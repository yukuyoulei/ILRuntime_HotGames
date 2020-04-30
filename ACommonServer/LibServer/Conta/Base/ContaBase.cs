using System;
using System.Collections.Generic;
using System.Text;
using LibPacket;
using LibServer.Managers;

public abstract class ContaBase : Context, LibNet.ITickableSystem
{
	public virtual bool Immortal { get { return false; } }
	protected CtSystem _CtSystem;
	public CtSystem CtSystem { get { if (_CtSystem == null) _CtSystem = GetSystem<CtSystem>(); return _CtSystem; } }
	protected NetSystem _NetSystem;
	public NetSystem NetSystem { get { if (_NetSystem == null) _NetSystem = GetSystem<NetSystem>(); return _NetSystem; } }
	protected PlayerSystem _PlayerSystem;
	public PlayerSystem PlayerSystem { get { if (_PlayerSystem == null) _PlayerSystem = GetSystem<PlayerSystem>(); return _PlayerSystem; } }
	protected GameoverSystem _GameoverSystem;
	public GameoverSystem GameoverSystem { get { if (_GameoverSystem == null) _GameoverSystem = GetSystem<GameoverSystem>(); return _GameoverSystem; } }
	protected virtual bool IsSingle { get { return true; } }
	public bool bRemove;
	public ContaBase(int confid) : base(confid)
	{
		AddSystem(new CtSystem(this));
		AddSystem(new NetSystem(this));
		AddSystem(new PlayerSystem(this));
		AddSystem(new GameoverSystem(this));
		//赋值持续时间
	}

	internal void leave(string psid)
	{
		var entity = GetPlayer(psid);
		if (entity == null) return;
		remove(entity);

		Broadcast(new PktRemoveEntity() { uid = entity.uid });

		entity.destroy();

		dAllPlayers.Remove(psid);

		if (!Immortal && dAllPlayers.Count == 0)
			destroy();
	}

	public void destroy()
	{
		AContaManager.Instance.OnRemove(this);
	}
	public void Dispose()
	{
		foreach (var e in entities)
		{
			e.Value.destroy();
		}
		entities.Clear();
		dAllPlayers.Clear();

		dSystems.Clear();
	}

	internal void Broadcast(PktBase pkt)
	{
		foreach (var player in dAllPlayers)
		{
			if (!player.Value.enabled) continue;
			var p = APlayerManager.Instance.OnGetPlayerByPSID(player.Key);
			if (p == null || p.client == null) continue;
			p.client.Response(pkt);
		}
	}

	public virtual void Tick(double fDeltaSec)
	{
		CtSystem.Tick(fDeltaSec);
		if (!this.enabled) return;

		GameoverSystem.Tick(fDeltaSec);
	}
	protected Entity GetPlayer(string psid)
	{
		return dAllPlayers.ContainsKey(psid) ? dAllPlayers[psid] : null;
	}
	public virtual void EnterScene(string psid)
	{
		var entity = GetPlayer(psid);
		if (entity == null) return;

		entity.enabled = true;
		NetSystem.all2me(entity);
		NetSystem.me2all(entity);
		enabled = true;
		OnStart();//重置开始时间
	}

	internal void BeginFight(string psid)
	{
		var entity = GetPlayer(psid);
		if (entity == null) return;
		entity.enabled = true;
	}

	public virtual void GameOverSuccess() { }
	public virtual void GameOverFailed(string reason) { }
}
