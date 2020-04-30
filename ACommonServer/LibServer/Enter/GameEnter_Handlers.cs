using LibCommon;
using LibCommon.GameObj;
using LibNet;
using LibPacket;
using LibServer.GameObj;
using LibServer.Managers;
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
			InitCommonRequests();
			Engine.AddClientDisconnectedEvent(Engine_DisconnectEvent);
			AFactoryPacket.Instance.RegistPackets(new Caller<PktLoginRequest>(GameHandler_Login));
			AFactoryPacket.Instance.RegistPackets(new Caller<PktEnterGameRequest>(GameHandler_EnterGame));
			AFactoryPacket.Instance.RegistPackets(new Caller<PktCreateAvatarRequest>(GameHandler_CreateAvatar));
			AFactoryPacket.Instance.RegistPackets(new Caller<PktGetSdata>(GameHandler_GetSdata));
			AFactoryPacket.Instance.RegistPackets(new Caller<PktDailyCheckRequest>(GameHandler_DailyCheckRequest));
			AFactoryPacket.Instance.RegistPackets(new Caller<PktPayRequest>(GameHandler_PayRequest));
			AFactoryPacket.Instance.RegistPackets(new Caller<PktExchangeRequest>(GameHandler_ExchangeRequest));
			AFactoryPacket.Instance.RegistPackets(new Caller<PktCreateOrderRequest>(GameHandler_CreateOrderRequest));
			AFactoryPacket.Instance.RegistPackets(new Caller<PktCommonRequest>(GameHandler_CommonRequest));
		}

		private Dictionary<ECommonMethod, Action<IResponer, Player, PData>> dCommonRequests;
		private void InitCommonRequests()
		{
			dCommonRequests = new Dictionary<ECommonMethod, Action<IResponer, Player, PData>>();
			dCommonRequests.Add(ECommonMethod.EnterConta, OnEnterConta);
			dCommonRequests.Add(ECommonMethod.EnterScene, OnEnterScene);
			dCommonRequests.Add(ECommonMethod.BeginFight, OnBeginFight);
		}

		private void OnBeginFight(IResponer responer, Player player, PData pData)
		{
			var conta = AContaManager.Instance.OnGetConta(player.psid);
			conta.BeginFight(player.psid);
		}

		private void OnEnterConta(IResponer responer, Player player, PData pData)
		{
			var pkt = new PktContaData();
			if (string.IsNullOrEmpty(pData.strArg))
			{
				pkt.id = pData.intArg;
				responer.Response(pkt);
			}
			else
			{
				var cake = new CakeServer("pinfo", player.psid);
				var data = AConfigManager.Instance.OnGetMapData(pData.strArg, cake.GetIntValue(pData.strArg));
				pkt.id = data.id;
				responer.Response(pkt);
			}
			AContaManager.Instance.OnEnterConta(player.psid, pkt.id);
		}

		private void OnEnterScene(IResponer responer, Player player, PData pData)
		{
			var conta = AContaManager.Instance.OnGetConta(player.psid);
			conta.EnterScene(player.psid);
		}
		private void GameHandler_CommonRequest(IResponer responer, PktCommonRequest vo)
		{
			var player = APlayerManager.Instance.OnGetPlayerByConn(responer.playerConnDesc);
			if (player == null) return;
			//AOutput.Log($"GameHandler_CommonRequest:[{player.psid}]{(ECommonMethod)vo.method}, {vo.pData.intArg}, {vo.pData.strArg}");
			dCommonRequests[(ECommonMethod)vo.method](responer, player, vo.pData);
		}

		private void GameHandler_CreateOrderRequest(IResponer responer, PktCreateOrderRequest vo)
		{
			var player = APlayerManager.Instance.OnGetPlayerByConn(responer.playerConnDesc);
			if (player == null) return;
			var data = PaymentLoader.Instance.OnGetData(vo.productID);
			var result = new PktCreateOrderResult();
			if (data == null) return;
			result.eResult = PktCreateOrderResult.EResult.Success;
			var orders = new CakeServer("order", player.psid);
			var order = orders.GetSingleItem(LibUtils.StringUtils.GetGUIDString());
			order.SetValue(ParamNameDefs.ProductID, vo.productID);
			order.SetValue(ParamNameDefs.Price, data.Price);
			result.orderID = order.iid;
			//result.extraInfo = APIWechatPay.Pay(data.Price, order.iid, $"描述：{data.Desc}");
			responer.Response(result);
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
			{
				foreach (var l in loadList)
				{
					var cake = new CakeServer(l, player.psid);
					cake.Sync();
				}

				var pdata = new PktContaData();
				pdata.id = InitValueDefs.CityID;
				responer.Response(pdata);

				AContaManager.Instance.OnEnterConta(player.psid, InitValueDefs.CityID);
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
			var player = APlayerManager.Instance.OnGetPlayerByConn(obj.ConnectionDesc);
			if (player == null) return;
			AContaManager.Instance.OnOffline(player.psid);
			APlayerManager.Instance.OnPlayerOffline(obj.ConnectionDesc);
		}
	}
}
