using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LibPacket;
using LibCommon;

public class AOnlineSubsystem : LibClient.AClientComm
{
	internal override void resultServerDisconnected()
	{
		AOutput.Log($"resultServerDisconnected");
		UEventListener.Instance.OnDispatchEvent(UEvents.ServerDisconnected, null);
	}
	internal override void rcvLoginCb(bool bSuccess, string uid, EPartnerID ePartnerID)
	{
		if (bSuccess)
		{
			AClientApis.OnEnterGame();
		}
		else
		{
			UEventListener.Instance.OnDispatchEvent(UEvents.LoginFailed, new EventLoginFailed());
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
	internal override void rcvEnterGameCb(AvatarInfo info)
	{
		UEventListener.Instance.OnDispatchEvent(UEvents.EnterGame, new EventEnterGame() { info = info });
	}
	internal override void rcvCreateAvatarCb(PktCreateAvatarResult.EResult eResult, AvatarInfo info)
	{
		UEventListener.Instance.OnDispatchEvent(UEvents.CreateAvatar, new EventCreateAvatar() { eResult = eResult, info = info });
	}
}
