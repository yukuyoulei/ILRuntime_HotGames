using System;
using System.Collections.Generic;
using System.Text;
using LibNet;
using LibPacket;
using LibServer.Managers;

public class ContaEnter_Handlers : Singleton<ContaEnter_Handlers>
{
	public void Init()
	{
		Engine.AddClientDisconnectedEvent(Engine_DisconnectEvent);
		AFactoryPacket.Instance.RegistPackets(new Caller<PktEnterCity>(ContaHandler_EnterCity));
		AFactoryPacket.Instance.RegistPackets(new Caller<PktEnterConta>(ContaHandler_EnterConta));
		AFactoryPacket.Instance.RegistPackets(new Caller<PktLeaveConta>(ContaHandler_LeaveConta));

	}

	private void ContaHandler_LeaveConta(IResponer responer, PktLeaveConta vo)
	{
		var easy = new PktEasy();
		easy.name = "LeaveConta_Success";
		responer.Response(easy);
	}

	public void ContaHandler_EnterConta(IResponer responer, PktEnterConta vo)
	{
		var easy = new PktEasy();
		easy.name = "EnterConta_Success";
		responer.Response(easy);
	}

	private ContaCity city = new ContaCity();
	private void ContaHandler_EnterCity(IResponer responer, PktEnterCity arg2)
	{
		var player = APlayerManager.Instance.OnGetPlayerByConn(responer.playerConnDesc);
		if (player == null || player.avatarCake == null) return;

		AContaManager.Instance.OnEnterConta(player.psid, city);
	}

	private void Engine_DisconnectEvent(ClientConnection conn)
	{
		var player = APlayerManager.Instance.OnGetPlayerByConn(conn.ConnectionDesc);
		if (player == null || player.avatarCake == null) return;

		AContaManager.Instance.OnOffline(player.psid);
	}
}
