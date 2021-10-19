using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using LibPacket;

namespace LibNet.rpc
{
	public interface IRPCClient
	{
		void RemoteCall(PktBase pkt);

		ClientConnection GetClient();
	}

	class CRPCClient : IRPCClient
	{
		ClientConnection m_client;
		public CRPCClient(ClientConnection client)
		{
			m_client = client;
		}
		public void RemoteCall(PktBase message)
		{
			ByteStream buffer = new ByteStream(1024, (byte)eMessageType.PacketID_Common);
			buffer.WriteInt(message.pktDef);
			var ms = new MemoryStream();
			message.Serialize(ms);
			buffer.WriteByteArray(ms.ToArray());
			m_client.SendPacket(buffer);
		}

		public ClientConnection GetClient()
		{
			return m_client;
		}
	}

	public class RPCClientManager
	{
		private static RPCClientManager s_Instance = null;
		private SortedList<string, CRPCClient> m_vRPCClients = new SortedList<string, CRPCClient>();

		static public RPCClientManager Instance
		{
			get
			{
				if (null == s_Instance)
				{
					s_Instance = new RPCClientManager();
				}
				return s_Instance;
			}
		}
		public IRPCClient CreateRPCClient(String sIP, Int32 iPort)
		{
			try
			{
				IPAddress ipaddress;
				var isip = IPAddress.TryParse(sIP, out ipaddress);
				if (!isip)
				{
					IPHostEntry ipHost = Dns.GetHostEntry(sIP);
					ipaddress = ipHost.AddressList[0];
					foreach (var l in ipHost.AddressList)
					{
						if (l.AddressFamily == AddressFamily.InterNetwork)
							ipaddress = l;
					}
				}
				IPEndPoint serverAddress = new IPEndPoint(ipaddress, iPort);
				TcpClient client = new TcpClient(serverAddress.AddressFamily);
				IAsyncResult result = client.BeginConnect(ipaddress, iPort, null, null);

				TimeSpan timeSpan = TimeSpan.FromMilliseconds(2000);
				bool success = result.AsyncWaitHandle.WaitOne(timeSpan, true);
				bool connected = client.Connected;
				if (!connected)
				{
					return null;
				}
				ClientConnection clientConnection = new ClientConnection(client);
				EngineControler.Instance.QueForTick(clientConnection);
				CRPCClient rpcClient = new CRPCClient(clientConnection);
				m_vRPCClients[clientConnection.GetHashCode().ToString()] = rpcClient;
				return rpcClient;
			}
			catch (System.Exception ex)
			{
				AOutput.Log($"ex.Message   {ex.Message}");
				return null;
			}

		}
		public IRPCClient LookupRPCClient(ClientConnection client)
		{
			if (m_vRPCClients.ContainsKey(client.GetHashCode().ToString()))
			{
				return m_vRPCClients[client.GetHashCode().ToString()];
			}
			return null;
		}
	}
}