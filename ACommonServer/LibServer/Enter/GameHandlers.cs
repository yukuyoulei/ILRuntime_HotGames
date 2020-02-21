using LibCommon;
using LibNet;
using LibPacket;
using System;
using System.Collections.Generic;
using System.Text;

namespace LibServer.Enter
{
	public class GameHandlers : Singleton<GameHandlers>, ITickableSystem
	{
		public void Init()
		{
			AFactoryPacket.Instance.RegistPackets(new Caller<PktLoginRequest>(Handler_Login));
			AFactoryPacket.Instance.RegistPackets(new Caller<PktEnterGameRequest>(Handler_EnterGame));
			AFactoryPacket.Instance.RegistPackets(new Caller<PktCreateAvatarRequest>(Handler_CreateAvatar));
		}

		private void Handler_Login(IResponer responer, PktLoginRequest vo)
		{
			var res = new PktLoginResult();
			switch ((EPartnerID)vo.ePartnerID)
			{
				case EPartnerID.Test:
					if (vo.password == MD5String.Hash32(vo.username))
					{
						res.bSuccess = true;
						res.ePartnerID = vo.ePartnerID;
						res.uid = MD5String.Hash32(vo.password + vo.username);
					}
					break;
				default:
					break;
			}
			if (res.bSuccess)
			{
				AAvatarManager.Instance.OnAddPlayer(res.uid, (EPartnerID)vo.ePartnerID, responer);
			}
			responer.Response(res);
		}

		private void Handler_CreateAvatar(IResponer responer, PktCreateAvatarRequest vo)
		{
			
		}

		private void Handler_EnterGame(IResponer responer, PktEnterGameRequest vo)
		{
			var player = AAvatarManager.Instance.OnGetPlayer(responer.playerConnDesc);
			if (player == null) return;

			var res = new PktEnterGameResult();
			var dbr = ADBManager.Get(InitValueDefs.dbconnect, InitValueDefs.dbname).FindOneData(ParamNameDefs.TableAvatar
			   , ADBAccessor.filter_eq(ParamNameDefs.PartnerID, (int)player.ePartnerID) & ADBAccessor.filter_eq(ParamNameDefs.UID, player.uid));
			if (dbr != null)
			{
				var a = AAvatarManager.Instance.OnGetAvatar(player.uid);
				if (a == null)
					a = AAvatarManager.Instance.OnCreateAvatar(player.uid, dbr, player);
				res.info = a.ToPkt();
			}
			responer.Response(res);

		}

		public void Tick(double fDeltaSec)
		{

		}
	}
}
