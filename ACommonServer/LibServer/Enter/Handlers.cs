using LibNet;
using LibNet.rpc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LibPacket;

namespace ACommonServers
{
	public class Handlers : Singleton<Handlers>, ITickableSystem
	{
		public void Init()
		{
			AFactoryPacket.Instance.RegistPackets(new Caller<PktLoginRequest>(Handler_Login));
			AFactoryPacket.Instance.RegistPackets(new Caller<PktEnterGameRequest>(Handler_EnterGame));
		}

		private void Handler_EnterGame(IResponer arg1, PktEnterGameRequest arg2)
		{
			
		}

		private void Handler_Login(IResponer arg1, PktLoginRequest arg2)
		{
			var res = new PktLoginResult();
			switch (arg2.ePartnerID)
			{
				case PktLoginRequest.EPartnerID.Test:
					if (arg2.password == MD5String.Hash32(arg2.username))
					{
						res.bSuccess = true;
						res.ePartnerID = arg2.ePartnerID;
						res.uid = MD5String.Hash32(arg2.password + arg2.username);
					}
					break;
				default:
					break;
			}
			if (res.bSuccess)
			{
				res.token = LibServer.Managers.ATokenManager.Instance.AddToken(res.ePartnerID, res.uid);
			}
			arg1.Response(res);
		}

		public void Tick(double fDeltaSec)
		{

		}
	}
}
