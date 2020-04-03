using System;
using System.Collections.Generic;
using System.Text;
using LibPacket;

public class NetSystem : SystemBase
{
	public NetSystem(ContaBase conta) : base(conta)
	{
	}

	public override void Tick(double fDeltaSec)
	{
	}

	public void all2me(Entity entity)
	{
		foreach (var e in this.conta.entities.Values)
		{
			e.toclient?.Invoke(entity.id);
		}
	}

	internal void SendTo(string id, PktBase pkt)
	{
		var p = APlayerManager.Instance.OnGetPlayerByID(id);
		if (p == null) return;
		p.client.Response(pkt);
	}
}
