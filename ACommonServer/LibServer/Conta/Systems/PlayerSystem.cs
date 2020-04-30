using LibPacket;
using System;
using System.Collections.Generic;
using System.Text;

public class PlayerSystem : SystemBase
{
	public PlayerSystem(ContaBase conta) : base(conta)
	{
	}

	public override void Tick(double fDeltaSec)
	{
	}

	public Entity Create(string psid)
	{
		var entity = new Entity(conta);
		entity.psid = psid;

		entity.dAttrs.Add("hp", 1);
		entity.dAttrs.Add("hpn", 1);
		
		var pkt = new LibPacket.PktCreatePlayer();
		pkt.psid = psid;
		pkt.contentID = 1;
		entity.toclient = psid =>
		{
			pkt.direction = (int)entity.eDirection;
			pkt.pos = new LibPacket.Int2() { int1 = entity.pos.Key, int2 = entity.pos.Value };
			pkt.lParams.Clear();
			foreach (var kv in entity.dAttrs)
			{
				pkt.lParams.Add(new LibPacket.ParamInfo() { paramName = kv.Key, paramValue = kv.Value.ToString() });
			}
			if (string.IsNullOrEmpty(psid))
				conta.Broadcast(pkt);
			else
				conta.NetSystem.SendTo(psid, pkt);
		};
        conta.AddPlayer(entity);
        return entity;
	}
}
