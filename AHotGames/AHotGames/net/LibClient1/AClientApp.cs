using LibNet;
using System;
using System.Collections.Generic;
using System.Text;
using LibPacket;

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
			Engine.AddClientDisconnectedEvent(Engine_OnClientDisconnected);

			RegisterProtocols();
		}
		private static void RegisterProtocols()
		{
			RegistPacketResponers<PktItemNotify>(rcvItemNotify);
		}
		private static void rcvItemNotify(PktItemNotify itemNotify)
		{
			clientComm.rcvItemNotify(itemNotify);
		}


		private static void Engine_OnClientDisconnected(ClientConnection client)
		{
			clientComm.resultServerDisconnected();
			connection = null;
		}

		public static async System.Threading.Tasks.Task StartClient(string ip = "127.0.0.1", int port = 999)
		{
			connection = new AConnection("127.0.0.1", 999);
			await connection.Connect();
		}
		public static bool bConnected
		{
			get
			{
				return connection != null ? connection.IsConnected : false;
			}
		}

		public static void RemoteCall<T>(PktBase pkt, Action<T> response)
			where T : PktBase, new()
		{
			if (!bConnected)
			{
				clientComm.resultServerDisconnected();
				return;
			}
			System.Threading.Tasks.Task.Run(async () => { await connection.Send(pkt); });
			RegistPacketResponers<T>(response);
		}
		private static void RegistPacketResponers<T>(Action<T> response)
			where T : PktBase, new()
		{
			if (AFactoryPacket.Instance.GetCaller(PktBase.GetPktDef(typeof(T))) == null)
				AFactoryPacket.Instance.RegistPackets(new Caller<T>((resp, t) => { response(t); }));
		}
	}
}
