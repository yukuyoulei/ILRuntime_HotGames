using LibCommon;
using LibCommon.GameObj;
using LibNet;
using LibPacket;
using LibServer.GameObj;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Text;

namespace LibServer.Enter
{
	public class GameHandlers_Enter : Singleton<GameHandlers_Enter>, ITickableSystem
	{
		public void Init()
		{
			Engine.AddClientDisconnectedEvent(Engine_DisconnectEvent);
			AFactoryPacket.Instance.RegistPackets(new Caller<PktLoginRequest>(GameHandler_Login));
			AFactoryPacket.Instance.RegistPackets(new Caller<PktEnterGameRequest>(GameHandler_EnterGame));
			AFactoryPacket.Instance.RegistPackets(new Caller<PktCreateAvatarRequest>(GameHandler_CreateAvatar));
			AFactoryPacket.Instance.RegistPackets(new Caller<PktGetSdata>(GameHandler_GetSdata));
			AFactoryPacket.Instance.RegistPackets(new Caller<PktDailyCheckRequest>(GameHandler_DailyCheckRequest));
			AFactoryPacket.Instance.RegistPackets(new Caller<PktPayRequest>(GameHandler_PayRequest));
			AFactoryPacket.Instance.RegistPackets(new Caller<PktExchangeRequest>(GameHandler_ExchangeRequest));
		}

		private void GameHandler_ExchangeRequest(IResponer responer, PktExchangeRequest vo)
		{
			var player = APlayerManager.Instance.OnGetPlayerByConn(responer.playerConnDesc);
			if (player == null) return;
			if (SCommonds.IsEnough(player.psid, InitValueDefs.money, vo.count))
			{
				SCommonds.Use("Exchange", player.psid, InitValueDefs.money, -vo.count);
				SCommonds.AddItem("Exchange", player.psid, InitValueDefs.gold, vo.count * 10);
			}
		}

		private void GameHandler_PayRequest(IResponer responer, PktPayRequest vo)
		{
			var player = APlayerManager.Instance.OnGetPlayerByConn(responer.playerConnDesc);
			if (player == null) return;
			SCommonds.AddItems("Exchange", player.psid, new Dictionary<int, int>() { { InitValueDefs.money, vo.productID * 10 } });
		}

		private void GameHandler_DailyCheckRequest(IResponer responer, PktDailyCheckRequest vo)
		{
			var player = APlayerManager.Instance.OnGetPlayerByConn(responer.playerConnDesc);
			if (player == null) return;
			var pinfo = new CakeAvatar(player.psid);
			if (ApiDateTime.IsSameDay(pinfo.GetIntValue(ParamNameDefs.LastDailyCheckTime))) return;
			var count = pinfo.GetIntValue(ParamNameDefs.DailyCheckCount);
			var data = DailyCheckLoader.Instance.OnGetData(count + 1);
			if (data == null)
			{
				count = 1;
				data = DailyCheckLoader.Instance.OnGetData(count);
			}
			SCommonds.AddItem("dailycheck", player.psid, data.itemID, data.itemCount);
			pinfo.SetValue(ParamNameDefs.LastDailyCheckTime, ApiDateTime.SecondsFromBegin());
			var pkt = new PktDailyCheckResult();
			pkt.eResult = PktDailyCheckResult.EResult.Success;
			pkt.lItems.Add(new Int2() { int1 = data.itemID, int2 = data.itemCount });
			responer.Response(pkt);
		}

		private string[] loadList = new string[] { "items", "pinfo" };
		private void GameHandler_GetSdata(IResponer responer, PktGetSdata vo)
		{
			var player = APlayerManager.Instance.OnGetPlayerByConn(responer.playerConnDesc);
			if (player == null) return;

			if (string.IsNullOrEmpty(vo.name))
				foreach (var l in loadList)
				{
					var cake = new CakeServer(l, player.psid);
					cake.Sync();
				}
			else
			{
				var cake = new CakeServer(vo.name, player.psid);
				cake.Sync();
			}
		}

		private void GameHandler_Login(IResponer responer, PktLoginRequest vo)
		{
			var res = new PktLoginResult();
			switch ((EPartnerID)vo.ePartnerID)
			{
				case EPartnerID.Test:
					if (vo.password == MD5String.Hash32(vo.username))
					{
						res.bSuccess = true;
						res.ePartnerID = vo.ePartnerID;
						res.unionid = MD5String.Hash32(vo.password + vo.username);
					}
					break;
				default:
					break;
			}
			if (res.bSuccess)
			{
				APlayerManager.Instance.OnAddPlayer(res.unionid, (EPartnerID)vo.ePartnerID, responer);
			}
			responer.Response(res);
		}

		private void GameHandler_CreateAvatar(IResponer responer, PktCreateAvatarRequest vo)
		{
			var player = APlayerManager.Instance.OnGetPlayerByConn(responer.playerConnDesc);
			if (player == null) return;
			var res = new PktCreateAvatarResult();
			{
				var dbr = ADBManager.Get(InitValueDefs.dbconnect, InitValueDefs.dbname).FindOneData(ParamNameDefs.TableAvatar
								, ADBAccessor.filter_eq(ParamNameDefs.AvatarName, vo.avatarName)
									| ADBAccessor.filter_eq(ParamNameDefs.UnionID, player.unionid)
								, ADBAccessor.projections(ParamNameDefs.AvatarLevel));
				if (dbr != null)
				{
					res.eResult = PktCreateAvatarResult.EResult.SameName;
					responer.Response(res);
					return;
				}
			}
			{
				var dbr = ADBManager.Get(InitValueDefs.dbconnect, InitValueDefs.dbname).FindOneData(ParamNameDefs.TableAvatar
								, ADBAccessor.filter_eq(ParamNameDefs.UnionID, player.unionid), ADBAccessor.projections(ParamNameDefs.AvatarLevel));
				if (dbr != null)
				{
					res.eResult = PktCreateAvatarResult.EResult.MaxCount;
					responer.Response(res);
					return;
				}
			}
			var psid = ObjectId.GenerateNewId().ToString();
			var cake = new CakeAvatar(psid);
			cake.Create(player.unionid, player.ePartnerID, vo.avatarName, vo.sex);
			APlayerManager.Instance.OnAddAvatar(cake, player);
			res.info = cake.ToPkt();
			res.eResult = PktCreateAvatarResult.EResult.Success;
			responer.Response(res);
		}

		private void GameHandler_EnterGame(IResponer responer, PktEnterGameRequest vo)
		{
			var player = APlayerManager.Instance.OnGetPlayerByConn(responer.playerConnDesc);
			if (player == null) return;

			var res = new PktEnterGameResult();
			var dbr = ADBManager.Get(InitValueDefs.dbconnect, InitValueDefs.dbname).FindOneData(ParamNameDefs.TableAvatar
			   , ADBAccessor.filter_eq(ParamNameDefs.PartnerID, (int)player.ePartnerID) & ADBAccessor.filter_eq(ParamNameDefs.UnionID, player.unionid));
			if (dbr != null)
			{
				var c = new CakeAvatar(dbr[ParamNameDefs.CollectionID].ToString());
				APlayerManager.Instance.OnAddAvatar(c, player);
				res.info = c.ToPkt();
			}
			responer.Response(res);
		}

		public void Tick(double fDeltaSec)
		{

			CakeServer.Tick();
		}

		private void Engine_DisconnectEvent(ClientConnection obj)
		{
			APlayerManager.Instance.OnPlayerOffline(obj.ConnectionDesc);
		}
	}
}
