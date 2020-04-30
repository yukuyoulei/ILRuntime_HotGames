using LibNet;
using System;
using System.Collections.Generic;
using System.Text;
using LibPacket;
using LibClient.GameObj;

namespace LibClient
{
	public static class AClientApp
	{
		public static AClientComm clientComm;
		static AConnection connection;
		public static void Init(AClientComm comm)
		{
			if (clientComm != null)
				throw new Exception("AClientApp cannot only be inited once!");
			clientComm = comm;

			EngineClient.EngineInit();
			Engine.AddClientDisconnectedEvent(Engine_OnClientDisconnected);

			RegisterNotifies();
		}
		private static void RegisterNotifies()
		{
			RegistPacketResponers<PktParamUpdate>(rcvParamUpdate);
			RegistPacketResponers<PktContaData>(rcvContaData);
			RegistPacketResponers<PktSettlement>(rcvSettlement);
			RegistPacketResponers<PktCreatePlayer>(rcvCreatePlayer);
		}

		private static void rcvCreatePlayer(PktCreatePlayer obj)
		{
			clientComm.rcvCreatePlayer(obj);
		}

		private static void rcvSettlement(PktSettlement obj)
		{
			clientComm.rcvSettlement(obj.ret, obj.pData);
		}

		private static void rcvContaData(PktContaData obj)
		{
			clientComm.rcvContaData(obj.id, obj.lDatas);
		}

		private static void rcvParamUpdate(PktParamUpdate obj)
		{
			var cake = CakeClient.GetCake(obj.cakeType, obj.id, obj.iid);
			if (cake == null)
			{
				cake = new CakeClient(obj.cakeType, obj.id, obj.iid);
				CakeClient.AddCake(cake);
			}
			cake.UpdateFromPkt(obj);
			clientComm.rcvParamUpdate();
		}

		private static void Engine_OnClientDisconnected(ClientConnection client)
		{
			if (!bCloseOnPurpose)
				clientComm.resultServerDisconnected();
			bCloseOnPurpose = false;
			connection = null;
		}

		private static string ip = "127.0.0.1";//"69.51.23.197";
		private static int port = 999;
		public static async System.Threading.Tasks.Task StartClient()
		{
			AOutput.Log($"Connecting {ip}:{port}");
			connection = new AConnection(ip, port);
			await connection.Connect();
			if (connection.IsConnected)
				AOutput.Log($"Connected {ip}:{port}");
		}
		public static void SetEndpoint(string sip, int iport)
		{
			ip = sip;
			port = iport;
		}
		public static bool bConnected
		{
			get
			{
				return connection != null ? connection.IsConnected : false;
			}
		}

		public static void RemoteCall(PktBase pkt)
		{
			if (!bConnected)
			{
				clientComm.resultServerDisconnected();
				return;
			}
			System.Threading.Tasks.Task.Run(async () => { await connection.Send(pkt); });
		}
		public static void RemoteCall<T>(PktBase pkt, Action<T> response)
			where T : PktBase, new()
		{
			RemoteCall(pkt);
			RegistPacketResponers(response);
		}
		private static void RegistPacketResponers<T>(Action<T> response)
			where T : PktBase, new()
		{
			if (AFactoryPacket.Instance.GetCaller(PktBase.GetPktDef(typeof(T))) == null)
				AFactoryPacket.Instance.RegistPackets(new Caller<T>((resp, t) => { response(t); }));
		}

		private static bool bCloseOnPurpose;
		internal static void OnDisconnect()
		{
			bCloseOnPurpose = true;
			System.Threading.Tasks.Task.Run(async () => { await connection.Close(); });
		}
	}
}
