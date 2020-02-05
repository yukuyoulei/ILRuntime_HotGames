using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LibPacket;

public class AOnlineSubsystem : LibClient.AClientComm
{
	internal override void resultServerDisconnected()
	{
		AOutput.Log($"resultServerDisconnected");
	}
	internal override void rcvLoginCb(bool bSuccess, string uid, PktLoginRequest.EPartnerID ePartnerID)
	{
		if (bSuccess)
		{
			AClientApis.OnEnterGame(uid, ePartnerID);
		}
		AOutput.Log($"rcvLoginCb {bSuccess}, {uid}");
	}
	internal override void rcvItemNotify(PktItemNotify arg2)
	{
		AOutput.Log($"rcvItemNotify {arg2.lItems.Count}");
		foreach (var arg in arg2.lItems)
		{
			AOutput.Log($"rcv item {arg.contentID}:{arg.num}");
		}
	}
}
