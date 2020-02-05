using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace LibNet
{
	public class EngineServer : Engine, IDisposable
	{
		private Thread m_ListenThread;
		private Thread m_ListenThreadV6;
		private TcpListener m_Listener;
		private TcpListener m_ListenerV6;
		private ClientList m_ClientConnections = new ClientList();
		private List<ITickableSystem> m_vSubSystems = new List<ITickableSystem>();

		private static EngineServer s_Instance = null;
		internal EngineServer() { }
		static public EngineServer Instance
		{
			get
			{
				if (null == s_Instance)
				{
					s_Instance = new EngineServer();
				}
				return s_Instance;
			}
		}

		internal class ServerTickTask : IProducerConsumerTask
		{
			public ServerTickTask(EngineServer server)
			{
				m_server = server;
			}

			#region IProducerConsumerTask Members
			public void DoWork(object o)
			{
				m_server.ServerTick();
				EngineControler.Instance.QueForProcessing(new ServerTickTask(m_server));
			}
			EngineServer m_server;
			#endregion
		}

		public void EngineInit()
		{
			RegistSubSystem(DataAccess.CLongRunningTaskMgr.Instance);
			DataAccess.CLongRunningTaskMgr.Instance.Initialize();

			EngineControler.Instance.EngineInit();
			EngineControler.Instance.QueForProcessing(new ServerTickTask(this));
		}

		public enum EServerType
		{
			GatewayServer,
			GameServer,
		}
		public static EServerType eServerType { get; set; }
		public static int i_port;
		public void ServerStartUp(Int32 iPort, EServerType eType)
		{
			i_port = iPort;

			eServerType = eType;

			IPAddress lisenIP = IPAddress.Any;

			m_Listener = new TcpListener(lisenIP, iPort);
			m_ListenerV6 = new TcpListener(IPAddress.IPv6Any, iPort);

			m_ListenThread = new Thread(new ThreadStart(ListenForClients));
			m_ListenThread.Start();
			m_ListenThreadV6 = new Thread(new ThreadStart(ListenForClientsV6));
			m_ListenThreadV6.Start();

			var addresses = Dns.GetHostAddresses(Dns.GetHostName());
			foreach (var adr in addresses)
			{
				AOutput.Log("\t" + adr.ToString());
			}
		}

		private void ListenForClientsV6()
		{
			try
			{
				m_ListenerV6.Start();
			}
			catch (System.Exception e)
			{
				AOutput.Log("ListenForClientsV6 Message:" + e.Message);
				AOutput.Log("ListenForClientsV6 StackTrace:" + e.StackTrace);

				System.Environment.Exit(0);
			}

			asyncBeginAcceptV6();
		}

		private void asyncBeginAcceptV6()
		{
			try
			{
				m_ListenerV6.BeginAcceptTcpClient(OnAcceptSocketV6, m_ListenerV6);
			}
			catch (Exception ex)
			{
				AOutput.Log("Listen for Clients has exited V6:" + ex.Message);
			}
		}

		private void OnAcceptSocketV6(IAsyncResult ar)
		{
			try
			{
				TcpClient skClient = m_ListenerV6.EndAcceptTcpClient(ar);

				ClientConnection clientSocket = new ClientConnection(skClient);
				m_ClientConnections.Add(clientSocket);
			}
			catch (Exception)
			{
			}
			finally
			{
				asyncBeginAcceptV6();
			}
		}

		public void ShutDown()
		{
			this.Dispose();
		}

		private void ListenForClients()
		{
			try
			{
				m_Listener.Start();
			}
			catch (System.Exception e)
			{
				AOutput.Log("ListenForClients Message:" + e.Message);
				AOutput.Log("ListenForClients StackTrace:" + e.StackTrace);

				System.Environment.Exit(0);
			}

			asyncBeginAccept();
		}
		void asyncBeginAccept()
		{
			try
			{
				m_Listener.BeginAcceptTcpClient(OnAcceptSocket, m_Listener);
			}
			catch (Exception ex)
			{
				AOutput.Log("Listen for Clients has exited:" + ex.Message);
			}
		}

		private void OnAcceptSocket(IAsyncResult ar)
		{
			try
			{
				TcpClient skClient = m_Listener.EndAcceptTcpClient(ar);

				ClientConnection clientSocket = new ClientConnection(skClient);
				m_ClientConnections.Add(clientSocket);
			}
			catch (Exception)
			{
			}
			finally
			{
				asyncBeginAccept();
			}
		}
		internal void RemoveClient(ClientConnection client)
		{
			m_ClientConnections.Remove(client);
		}

		public ClientList ClientConnections
		{
			get
			{
				return m_ClientConnections;
			}
		}
		public void RegistSubSystem(ITickableSystem subSystem)
		{
			m_vSubSystems.Add(subSystem);
		}
		public bool bAllSubSystemRegisted = false;
		private static DateTime m_OldTime = DateTime.Now;
		public void ServerTick()
		{
			if (!bAllSubSystemRegisted)
			{
				Thread.Sleep(1);
				return;
			}
			DateTime nowTime = DateTime.Now;
			TimeSpan span = nowTime - m_OldTime;
			foreach (ITickableSystem subSystem in m_vSubSystems)
			{
				m_OldTime = nowTime;

				DateTime ot = DateTime.Now;
				try
				{
					subSystem.Tick(span.TotalSeconds);
				}
				catch (Exception ex)
				{
					AOutput.Log("Tick exception " + ex.Message);
					AOutput.Log(ex.StackTrace);
				}
				DateTime nt = DateTime.Now;
				double t = (nt - ot).TotalMilliseconds;
				if (t > 300)
				{
					AOutput.Log(subSystem.GetType().FullName + " spans (ms)" + t);
				}
			}
			Thread.Sleep(1);
		}
		#region IDisposable Members

		public void Dispose()
		{

			m_ClientConnections.Dispose();
			m_Listener.Stop();
			m_ListenerV6.Stop();
			if (!m_ListenThreadV6.Join(1000))
			{
				m_ListenThreadV6.Abort();
			}
			if (!m_ListenThreadV6.Join(1000))
			{
				m_ListenThreadV6.Abort();
			}
		}

		#endregion
	}
	public class ClientList : IDisposable
	{
		private List<ClientConnection> m_ConnectedClients = new List<ClientConnection>();
		private object m_ListLockObj = new object();

		public ClientList()
		{
		}
		public List<ClientConnection> Clients
		{
			get
			{
				return m_ConnectedClients;
			}
		}
		public void Add(ClientConnection client)
		{
			lock (m_ListLockObj)
			{
				m_ConnectedClients.Add(client);
			}
		}

		public void Remove(ClientConnection client)
		{
			lock (m_ListLockObj)
			{
				m_ConnectedClients.Remove(client);
			}
			client.Dispose();
		}
		#region IDisposable Members

		public void Dispose()
		{
			foreach (ClientConnection clientSocket in m_ConnectedClients)
			{
				clientSocket.Dispose();
			}
			m_ConnectedClients.Clear();
		}

		#endregion

		public static int ClientConnectionOutTimeSecond = 15;
		public static bool ConnectionsCanTimeOut = false;
		public List<ClientConnection> OnGetInvalidClientConnections()
		{
			List<ClientConnection> result = new List<ClientConnection>();
			lock (m_ListLockObj)
			{
				foreach (ClientConnection clientSocket in m_ConnectedClients)
				{
					if (!clientSocket.IsServerConnection
						&& (ApiDateTime.Now - clientSocket.lastReceiveDataTime).TotalSeconds > ClientConnectionOutTimeSecond)
					{
						result.Add(clientSocket);
					}
				}
			}
			foreach (ClientConnection cc in result)
			{
				Remove(cc);
			}
			return result;
		}
	}
}
