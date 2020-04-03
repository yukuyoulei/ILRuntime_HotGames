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
			RegistPacketResponers<PktEasy>(rcvEasy);
		}

		private static void rcvEasy(PktEasy obj)
		{
			clientComm.rcvEasy(obj.name, obj.ints, obj.strs);
		}

		private static void rcvContaData(PktContaData obj)
		{
			clientComm.rcvContaData(obj.id);
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

		private static string s_ip = "69.51.23.197";
		public static async System.Threading.Tasks.Task StartClient(string ip = "", int port = 999)
		{
			if (!string.IsNullOrEmpty(ip))
				s_ip = ip;
			connection = new AConnection(s_ip, port);
			await connection.Connect();
			if (connection.IsConnected)
				AOutput.Log($"Connected {s_ip}:{port}");
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
