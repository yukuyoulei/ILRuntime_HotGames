using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LibClient;
using LibPacket;

public static class AClientApis
{
	public static void OnLogin(string username, string password, PktLoginRequest.EPartnerID ePartnerID)
	{
		var req = new PktLoginRequest();
		req.username = username;
		req.password = password;
		req.ePartnerID = ePartnerID;
		AClientApp.RemoteCall<PktLoginResult>(req, OnLoginCb);
	}

	private static void OnLoginCb(PktLoginResult res)
	{
		AClientApp.clientComm.rcvLoginCb(res.bSuccess, res.uid, res.ePartnerID);
	}

	internal static void OnEnterGame(string uid, PktLoginRequest.EPartnerID ePartnerID)
	{
		var req = new PktEnterGameRequest();
		req.uid = uid;
		req.ePartnerID = ePartnerID;
		AClientApp.RemoteCall<PktEnterGameResult>(req, OnEnterGameCb);
	}
	private static void OnEnterGameCb(PktEnterGameResult res)
	{
		AClientApp.clientComm.rcvEnterGameCb(res.info);
	}
}
