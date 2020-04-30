using LibPacket;
using System;
using System.Collections.Generic;
using System.Text;

public class ContaSingle : ContaBase
{
	public string wx;
	public ContaSingle(int confid, string wx) : base(confid)
	{
		lastSeconds = 10;
		this.wx = wx;
	}
	public override void Tick(double fDeltaSec)
	{
		base.Tick(fDeltaSec);


	}
	public override void GameOverSuccess()
	{
		var pkt = new PktSettlement();
		pkt.ret = true;
		Broadcast(pkt);
	}
	public override void GameOverFailed(string reason)
	{
		var pkt = new PktSettlement();
		pkt.ret = true;
		pkt.pData.strArg = reason;
		Broadcast(pkt);
	}
}
