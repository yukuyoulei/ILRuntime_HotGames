using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LibPacket;
using LibCommon;

public class AOnlineSubsystem : LibClient.AClientComm
{
	public override void resultServerDisconnected()
	{
		AOutput.Log($"resultServerDisconnected");
		UEventListener.Instance.OnDispatchEvent(UEvents.ServerDisconnected, null);
	}
	public override void rcvLoginCb(bool bSuccess, string uid, EPartnerID ePartnerID)
	{
		if (bSuccess)
		{
			AClientApis.OnEnterGame();
		}
		UEventListener.Instance.OnDispatchEvent(UEvents.Login, new EventLogin() { bSuccess = bSuccess });
	}
	public override void rcvEnterGameCb(AvatarInfo info)
	{
		AOutput.Log($"rcvEnterGameCb");
		UEventListener.Instance.OnDispatchEvent(UEvents.EnterGame, new EventEnterGame() { info = info });
	}
	public override void rcvCreateAvatarCb(PktCreateAvatarResult.EResult eResult, AvatarInfo info)
	{
		UEventListener.Instance.OnDispatchEvent(UEvents.CreateAvatar, new EventCreateAvatar() { eResult = eResult, info = info });
	}

	public override void rcvParamUpdate()
	{
		UEventListener.Instance.OnDispatchEvent(UEvents.ParamUpdate, null);
	}
	public override void rcvContaData(int id)
	{
		UEventListener.Instance.OnDispatchEvent(UEvents.EventContaData, new EventContaData() { id = id });
	}
}
