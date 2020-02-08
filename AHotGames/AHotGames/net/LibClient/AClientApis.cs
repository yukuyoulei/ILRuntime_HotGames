using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LibClient;
using LibPacket;
using LibCommon;

public static class AClientApis
{
	public static void OnLogin(string username, string password, EPartnerID ePartnerID)
	{
		var req = new PktLoginRequest();
		req.username = username;
		req.password = password;
		req.ePartnerID = (int)ePartnerID;
		AClientApp.RemoteCall<PktLoginResult>(req, OnLoginCb);
	}

	private static void OnLoginCb(PktLoginResult res)
	{
		AClientApp.clientComm.rcvLoginCb(res.bSuccess, res.uid, (EPartnerID)res.ePartnerID);
	}

	internal static void OnEnterGame()
	{
		var req = new PktEnterGameRequest();
		AClientApp.RemoteCall<PktEnterGameResult>(req, OnEnterGameCb);
	}
	private static void OnEnterGameCb(PktEnterGameResult res)
	{
		if (res.info != null)
		{
			AClientApp.myAvatar = new LibClient.GameObj.AAvatarClient();
			AClientApp.myAvatar.FromPkt(res.info);
		}
		AClientApp.clientComm.rcvEnterGameCb(res.info);
	}

	public static void OnCreateAvatar(string avatarName, int sex)
	{
		var req = new PktCreateAvatarRequest();
		req.avatarName = avatarName;
		req.sex = sex;
		AClientApp.RemoteCall<PktCreateAvatarResult>(req, OnCreateAvatarCb);
	}

	private static void OnCreateAvatarCb(PktCreateAvatarResult res)
	{
		if (res.info != null)
		{
			AClientApp.myAvatar = new LibClient.GameObj.AAvatarClient();
			AClientApp.myAvatar.FromPkt(res.info);
		}
		AClientApp.clientComm.rcvCreateAvatarCb(res.eResult, res.info);
	}
}
