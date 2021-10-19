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
	public override void rcvContaData(int id, List<PData> lDatas)
	{
		UEventListener.Instance.OnDispatchEvent(UEvents.ContaData, new EventContaData() { id = id, lDatas = lDatas });
	}

	public override void rcvCreateOrderCb(PktCreateOrderResult.EResult eResult, string orderID, string extraInfo)
	{
		UEventListener.Instance.OnDispatchEvent(UEvents.CreateOrder, new EventCreateOrder() { eResult = eResult, orderID = orderID, extraInfo = extraInfo });
	}

	public override void rcvSettlement(bool ret, PData pData)
	{
		UEventListener.Instance.OnDispatchEvent(UEvents.Settlement, new EventSettlement() { bRet = ret, pdata = pData });
	}

	public override void rcvCreatePlayer(PktCreatePlayer info)
	{
		UEventListener.Instance.OnDispatchEvent(UEvents.CreatePlayer, new EventCreatePlayer() { pkt =info });
	}
}
