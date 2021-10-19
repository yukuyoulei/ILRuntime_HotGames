using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.Diagnostics;
using System.IO;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using LibNet.DataAccess;

namespace LibNet
{
	public enum eConnectionStatus
	{
		Connected = 0,
		DisConnected,
	}


	public class ClientConnection : IDisposable
	{
		class SocketData
		{
			private byte[] m_vData = null;
			private int m_iPosition;
			private int m_iLength;

			public int BytesUnread
			{
				get
				{
					return (m_iLength - m_iPosition);
				}
			}

			public SocketData(byte[] vData, int iBytesAvailable)
			{
				m_vData = vData;
				m_iPosition = 0;
				m_iLength = iBytesAvailable;
			}

			public SocketData(TcpClient tcpClient, NetworkStream stream)
			{
				int iAvailable = tcpClient.Available;
				m_vData = new byte[iAvailable];
				m_iPosition = 0;
				m_iLength = stream.Read(m_vData, 0, iAvailable);
			}

			public void Read(MemoryStream packetStream, int iNumBytes)
			{
				packetStream.Write(m_vData, m_iPosition, iNumBytes);
				m_iPosition += iNumBytes;
			}

			public byte ReadByte()
			{
				byte bRet = m_vData[m_iPosition];
				++m_iPosition;
				return bRet;
			}
			public void Reset()
			{
				m_iPosition = 0;
			}
		}

		public class SinglePacketProcessor
		{
			private Packet m_Packet;

			public SinglePacketProcessor(Packet packet)
			{
				m_Packet = packet;
			}

			public bool Process(ClientConnection client)
			{
				if (m_Packet == null)
				{
					return false;
				}
				m_Packet.PreProcess();
				m_Packet.Process();
				return true;
			}
		}

		public class SinglePacketSender
		{
			private byte[] m_bytes;

			public SinglePacketSender(byte[] bytes)
			{
				m_bytes = bytes;
			}

			public async Task<bool> SendPackets(ClientConnection client)
			{
				return await client.RawSendData(m_bytes);
			}
		}

		private static readonly byte[] m_ByteReqeust = Encoding.GetEncoding("UTF-8").GetBytes("<policy-file-request/>");
		private int m_iMatchIndex = 0;

		public object lockTickObj = new object();

		public bool IsServerConnection { get; set; }

		private TcpClient m_TcpClient;
		private NetworkStream m_ClientStream;
		private String m_sConnectionDesc;

		private byte m_bMagicA;
		private byte m_bMagicB;
		private byte m_bOpCode;
		private byte[] m_vPacketSize = new byte[4];
		private Int32 m_uiPacketSize;
		private bool m_bHas1stByteSize = false;
		private bool m_bHas2ndByteSize = false;
		private bool m_bHas3rdByteSize = false;
		private bool m_bHas4thByteSize = false;
		private bool m_bValidPacketHeader = false;

		private bool m_bSecurityRequestTryProcessed = false; //for flash SecurityRequest

		private ConcurrentQueue<SocketData> m_vSocketData = new ConcurrentQueue<SocketData>();
		private SocketData m_SocketData = null;
		private MemoryStream m_PacketDataStream = null;

		private ConcurrentQueue<SinglePacketProcessor> m_vPacketsToProcess = new ConcurrentQueue<SinglePacketProcessor>();
		private ConcurrentQueue<SinglePacketSender> m_vPacketsToSend = new ConcurrentQueue<SinglePacketSender>();

		public DateTime lastReceiveDataTime { get; set; }
		public string ConnectionDesc
		{
			get { return m_sConnectionDesc; }
		}
		public TcpClient TcpClient
		{
			get { return m_TcpClient; }
		}



		internal bool NeedsToSend
		{
			get { return (0 < m_vPacketsToSend.Count); }
		}
		internal bool NeedsProcessing
		{
			get { return (0 < m_vPacketsToProcess.Count); }
		}

		public bool IsConnected
		{
			get
			{
				return Status == eConnectionStatus.Connected;
			}
		}
		public eConnectionStatus Status
		{
			get;
			set;
		}

		internal ClientConnection(TcpClient client)
		{
			m_TcpClient = client;
			m_sConnectionDesc = m_TcpClient.Client.RemoteEndPoint.ToString();

			m_ClientStream = m_TcpClient.GetStream();

			Status = eConnectionStatus.Connected;

			lastReceiveDataTime = ApiDateTime.Now;

			Task.Run(async () =>
			{
				try
				{
					await BeginRead();
				}
				catch (Exception ex)
				{
					AOutput.Log($"ClientConnection {ex.Message}");
				}
			});
		}

		public void SendPacket(ByteStream Stream)
		{
			// Write packet using the following protocol:
			// [Magic1|Magic2|Length|Packet Stream]
			MemoryStream packetStream = new MemoryStream();
			BinaryWriter writer = new BinaryWriter(packetStream);
			writer.Write((byte)0xBE);
			writer.Write((byte)0xEF);
			writer.Write((UInt32)Stream.Position);
			writer.Write(Stream.Data, 0, Stream.Position);
			byte[] bytes = new byte[packetStream.Position];
			Array.ConstrainedCopy(packetStream.GetBuffer(), 0, bytes, 0, (int)packetStream.Position);

			AddForSendProcessing(new SinglePacketSender(bytes));
		}
		public void SendByteData(byte[] bytes)
		{
			AddForSendProcessing(new SinglePacketSender(bytes));
		}
		public async Task<bool> RawSendData(Byte[] byteArray)
		{
			if (Status == eConnectionStatus.DisConnected) return false;
			try
			{
				await m_ClientStream.WriteAsync(byteArray, 0, byteArray.Length);
				if (NeedsToSend)
				{
					EngineControler.Instance.QueForSend(this);
				}
			}
			catch (System.Exception ex)
			{
				AOutput.Log($"RawSendData ex {ex.Message}");
				AddDisconectPacket();
			}
			return (Status == eConnectionStatus.Connected);
		}

		public delegate void delegateAllOutput(string logName, string log);
		public static delegateAllOutput delOutput;
		private void EndWriteCb(IAsyncResult ar)
		{
			try
			{
				m_ClientStream.EndWrite(ar);
			}
			catch (Exception ex)
			{
				AOutput.Log($"EndWriteCb {ex.Message}");
				AddDisconectPacket();
			}
		}

		private void AddDisconectPacket()
		{
			if (!IsConnected) return;

			Status = eConnectionStatus.DisConnected;

			Packet packet = new ClientDisConnectedPacket();
			packet.FactoryInit(null, this);

			m_vPacketsToProcess.Enqueue(new SinglePacketProcessor(packet));
			EngineControler.Instance.QueForProcessing(this);
		}

		private const int readbuffsize = 1024;
		private async Task BeginRead()
		{
			if (!IsConnected)
			{
				return;
			}
			try
			{
				byte[] buffer = new byte[readbuffsize];

				var iBytesAvailable = await m_ClientStream.ReadAsync(buffer, 0, readbuffsize);

				if (0 >= iBytesAvailable)
				{
					AddDisconectPacket();
					return;
				}
				else
				{
					SocketData data = new SocketData(buffer, iBytesAvailable);
					m_vSocketData.Enqueue(data);

					EngineControler.Instance.QueForTick(this);
				}

				lastReceiveDataTime = ApiDateTime.Now;

				await BeginRead();
			}
			catch (System.Exception ex)
			{
				//AOutput.Log($"ReadComplete {ex.Message}");
				AddDisconectPacket();
			}
		}

		private bool RecieveData()
		{
			if (m_vSocketData.Count == 0)
			{
				return false;
			}

			bool bRet = false;

			while (true)
			{
				m_SocketData = null;
				if (0 < m_vSocketData.Count)
				{
					m_vSocketData.TryDequeue(out m_SocketData);
				}

				if (null == m_SocketData)
				{
					break;
				}
				ProcessStreamData();
				bRet = true;
			}

			return bRet;
		}
		private bool TryPraseProcessSecurityRequest()
		{
			if (m_bSecurityRequestTryProcessed) return false;
			while (0 < m_SocketData.BytesUnread)
			{
				byte c = m_SocketData.ReadByte();
				if (c != m_ByteReqeust[m_iMatchIndex])
				{
					m_bSecurityRequestTryProcessed = true;
					m_SocketData.Reset();
					return false;
				}
				if (m_iMatchIndex == m_ByteReqeust.Length - 1)
				{
					m_bSecurityRequestTryProcessed = true;
					CreateSecurityReqeustPacket();
					return true;
				}
				m_iMatchIndex++;
			}
			return true;

		}
		public static bool showInvalidSocketData = true;
		private void ProcessStreamData()
		{
			if (TryPraseProcessSecurityRequest())
			{
				return;
			}

			while (0 < m_SocketData.BytesUnread)
			{

				if (!m_bValidPacketHeader)
				{
					if (!ReadPacketHeader())
					{
						if (showInvalidSocketData)
						{
							AOutput.Log("Invalid socket data from " + ConnectionDesc + " count:" + m_SocketData.BytesUnread);
						}
						break;
					}
				}
				else
				{
					ReadPacketData();
				}
			}
		}

		private void ResetPacketHeader()
		{
			m_bMagicA = 0;
			m_bMagicB = 0;
			m_bOpCode = 0;
			m_bHas1stByteSize = false;
			m_bHas2ndByteSize = false;
			m_bHas3rdByteSize = false;
			m_bHas4thByteSize = false;
			m_bValidPacketHeader = false;
		}

		private bool ReadPacketHeader()
		{
			if (0xBE != m_bMagicA)
			{
				if (0 < m_SocketData.BytesUnread)
				{
					m_bMagicA = m_SocketData.ReadByte();
					if (0xBE != m_bMagicA)
					{
						return false;
					}
				}
				return true;
			}

			if (0xEF != m_bMagicB)
			{
				if (0 < m_SocketData.BytesUnread)
				{
					m_bMagicB = m_SocketData.ReadByte();
					if (0xEF != m_bMagicB)
					{
						ResetPacketHeader();
						return false;
					}
				}
				return true;
			}

			if (!m_bHas1stByteSize)
			{
				if (0 < m_SocketData.BytesUnread)
				{
					m_vPacketSize[0] = m_SocketData.ReadByte();
					m_bHas1stByteSize = true;
					return true;
				}
				return false;
			}

			if (!m_bHas2ndByteSize)
			{
				if (0 < m_SocketData.BytesUnread)
				{
					m_vPacketSize[1] = m_SocketData.ReadByte();
					m_bHas2ndByteSize = true;
					return true;
				}
				return false;
			}

			if (!m_bHas3rdByteSize)
			{
				if (0 < m_SocketData.BytesUnread)
				{
					m_vPacketSize[2] = m_SocketData.ReadByte();
					m_bHas3rdByteSize = true;
					return true;
				}
				return false;
			}

			if (!m_bHas4thByteSize)
			{
				if (0 < m_SocketData.BytesUnread)
				{
					m_vPacketSize[3] = m_SocketData.ReadByte();
					m_bHas4thByteSize = true;
					return true;
				}
				return false;
			}

			if (0 == m_bOpCode)
			{
				if (0 < m_SocketData.BytesUnread)
				{
					m_bOpCode = m_SocketData.ReadByte();
					m_bValidPacketHeader = true;

					m_uiPacketSize = System.BitConverter.ToInt32(m_vPacketSize, 0);
					m_uiPacketSize -= 1; // Remove 1 bytes for the opcode

					eMessageType eOpType = (eMessageType)m_bOpCode;

					if ((Int32)m_uiPacketSize < 0)//prevent m_uiPacketSize from being valued -1
					{
						ResetPacketHeader();
						AOutput.Log("ClientConnection ReadPacketHeader caught m_uiPacketSize being valued -1");
						return true;
					}
					if (m_uiPacketSize > 1024000)
					{
						ResetPacketHeader();
						AOutput.Log("ClientConnection ReadPacketHeader caught m_uiPacketSize being valued " + m_uiPacketSize);
						return true;
					}
					m_PacketDataStream = new MemoryStream((Int32)m_uiPacketSize);
					if (0 == m_uiPacketSize)
					{
						/// - Ping packet we need to process this bad boy :)
						CreatePacket();
					}
				}
				else
				{
					AOutput.Log("ReadPacketHeader failed 2!");
				}
				return true;
			}
			else
			{
				AOutput.Log("ReadPacketHeader failed 1!");
				return false;
			}
		}

		private void ReadPacketData()
		{
			if (m_bValidPacketHeader)
			{
				if (0 == m_uiPacketSize)
				{
					CreatePacket();
				}
				else
				{
					int iNumBytesNeeded = (int)m_uiPacketSize - (int)m_PacketDataStream.Position;

					if (iNumBytesNeeded <= m_SocketData.BytesUnread)
					{
						m_SocketData.Read(m_PacketDataStream, iNumBytesNeeded);
						/// - She's ready make this packet and ship it to processing :)
						CreatePacket();
					}
					else
					{
						//SystemLogger.Log.Debug( string.Format( "Partial Packet Data Read, need more {0}", iNumBytesNeeded ) );
						/// - Boys we have a partial here so lets just read what we can
						m_SocketData.Read(m_PacketDataStream, m_SocketData.BytesUnread);
					}
				}
			}
		}
		public delegate void delegateTooManyPacketsPerSec(string ip, int num);
		public static delegateTooManyPacketsPerSec deleTooManyPacketsPerSec;
		public static void RegistTooManyPacketPerSecDelegate(delegateTooManyPacketsPerSec del)
		{
			deleTooManyPacketsPerSec = del;
		}
		public static int maxPacketNumPerSecond = 100;
		private DateTime oldTime = DateTime.Now;
		private void CreatePacket()
		{
			eMessageType eOpCode = (eMessageType)m_bOpCode;

			ResetPacketHeader();

			m_PacketDataStream.Position = 0; // reset position so it is ready for reading

			Packet packet = ClientConnectionUtil.ProcessClientPacket(eOpCode, m_PacketDataStream, this);
			if (packet != null)
			{
				m_vPacketsToProcess.Enqueue(new SinglePacketProcessor(packet));
				EngineControler.Instance.QueForProcessing(this);
			}
			else
			{
				AOutput.Log("Create packet failed!");
			}

			if (m_vSocketData.Count > 0)
			{
				EngineControler.Instance.QueForTick(this);
			}
		}
		private void CreateSecurityReqeustPacket()
		{
			Packet packet = new SecurityReqeustPacket();
			packet.FactoryInit(null, this);
			m_vPacketsToProcess.Enqueue(new SinglePacketProcessor(packet));
			EngineControler.Instance.QueForProcessing(this);
		}

		public static int processPktNum = 0;
		public void AddForSendProcessing(SinglePacketSender sendProcessor)
		{
			m_vPacketsToSend.Enqueue(sendProcessor);
			EngineControler.Instance.QueForSend(this);

			processPktNum++;
		}
		public void OnClose()
		{
			if (!IsConnected)
			{
				return;
			}
			AddDisconectPacket();
		}
		public void CloseConnection()
		{
			AddDisconectPacket();
			m_TcpClient.Close();
		}
		internal bool SendPackets()
		{
			SinglePacketSender processor = null;

			if (0 < m_vPacketsToSend.Count)
			{
				m_vPacketsToSend.TryDequeue(out processor);
			}

			if (null != processor)
			{
				processor.SendPackets(this);
			}

			if (NeedsToSend)
			{
				EngineControler.Instance.QueForSend(this);
			}
			return m_vPacketsToSend.Count != 0;
		}

		public static int processedPktsNum { get; set; }
		internal bool ProcessPackets()
		{
			SinglePacketProcessor processor = null;
			if (0 < m_vPacketsToProcess.Count)
			{
				m_vPacketsToProcess.TryDequeue(out processor);
			}

			if (processor != null)
			{
				/*try
				{*/
					processor.Process(this);
				/*}
				catch (Exception ex)
				{
					AOutput.Log($"SinglePacketProcessor ex {ex.Message} \r\n {ex.StackTrace}");
				}*/
				processedPktsNum++;
			}

			if (NeedsProcessing)
			{
				EngineControler.Instance.QueForProcessing(this);
			}

			return m_vPacketsToProcess.Count != 0;
		}

		internal void ClientTick()
		{
			RecieveData();
		}

		public static int MinClientCount = 3;

		#region IDisposable Members

		public void Dispose()
		{
			m_TcpClient.Close();
		}

		#endregion
	}

	public interface IProducerConsumerTask
	{
		void DoWork(object o = null);
	}

	internal class ExitThreadTask : IProducerConsumerTask
	{
		#region IProducerConsumerTask Members

		public void DoWork(object o)
		{
			throw new Exception("The method should never be called as we are for exiting the thread.");
		}

		#endregion
	}

	internal class BaseClientTask
	{
		protected ClientConnection m_Client;

		public BaseClientTask(ClientConnection client)
		{
			m_Client = client;
		}
	}

	internal class ClientPacketSendTask : BaseClientTask, IProducerConsumerTask
	{
		public ClientPacketSendTask(ClientConnection client)
			: base(client)
		{
		}

		#region IProducerConsumerTask Members

		public void DoWork(object o)
		{
			m_Client.SendPackets();
		}

		#endregion
	}

	internal class ClientPacketProcessingTask : BaseClientTask, IProducerConsumerTask
	{
		public ClientPacketProcessingTask(ClientConnection client)
			: base(client)
		{
		}

		#region IProducerConsumerTask Members

		public void DoWork(object o)
		{
			m_Client.ProcessPackets();
		}

		#endregion
	}

	internal class ClientTickTask : BaseClientTask, IProducerConsumerTask
	{
		public ClientTickTask(ClientConnection client)
			: base(client)
		{
		}

		#region IProducerConsumerTask Members

		public void DoWork(object o)
		{
			lock (m_Client.lockTickObj)
			{
				m_Client.ClientTick();
			}
		}

		#endregion
	}

	internal class LongRunTaskFinishTask : IProducerConsumerTask
	{
		public LongRunTaskFinishTask(CLongRunningTask finished)
		{
			m_finished = finished;
		}

		#region IProducerConsumerTask Members

		public void DoWork(object o)
		{
			try
			{
				m_finished.Finished();
			}
			catch (Exception ex)
			{
				AOutput.Log(ex.StackTrace);
			}
		}
		CLongRunningTask m_finished;
		#endregion
	}

	internal class ProducerConsumerQueueProcess : ProducerConsumerQueue
	{
		public ProducerConsumerQueueProcess(Int32 iWorkerCount)
			: base(iWorkerCount, ThreadPriority.Normal)
		{

		}
		protected override void Consume()
		{
			while (true)
			{
				IProducerConsumerTask task = null;
				while (true)
				{
					if (m_queClientTickTask.Count == 0)
					{
						break;
					}
					task = null;
					m_queClientTickTask.TryDequeue(out task);
					if (task == null)
					{
						break;
					}
					task.DoWork();
				}
				task = null;
				if (m_queTask.Count > 0)
				{
					m_queTask.TryDequeue(out task);
				}
				if (task != null)
				{
					if (task is ExitThreadTask)
					{
						return;         // This signals our exit
					}
					task.DoWork();
				}
				Thread.Sleep(1);
			}
		}
	}

	public abstract class ProducerConsumerQueue : IDisposable
	{
		public static int[] sleptTimes = new int[3];
		protected object m_objLock = new object();
		protected List<Thread> m_vWorkerThreads;
		protected ConcurrentQueue<IProducerConsumerTask> m_queTask = new ConcurrentQueue<IProducerConsumerTask>();
		protected ConcurrentQueue<IProducerConsumerTask> m_queClientTickTask = new ConcurrentQueue<IProducerConsumerTask>();

		public ProducerConsumerQueue(Int32 iWorkerCount, ThreadPriority priority)
		{
			m_vWorkerThreads = new List<Thread>();

			// Create and start a separate thread for each worker
			for (int i = 0; i < iWorkerCount; i++)
			{
				AddNewWorkerThread(priority);
			}
		}

		public int ThreadCount
		{
			get
			{
				return m_vWorkerThreads.Count;
			}
		}

		protected void AddNewWorkerThread(ThreadPriority priority)
		{
			Thread t = new Thread(Consume);
			t.Priority = priority;

			m_vWorkerThreads.Add(t);
			t.Start();
		}
		public void Dispose()
		{
			// Enqueue one null task per worker to make each exit.
			foreach (Thread worker in m_vWorkerThreads)
			{
				EnqueueTask(new ExitThreadTask());
			}
			foreach (Thread worker in m_vWorkerThreads)
			{
				worker.Join();
			}
		}

		public void EnqueueTask(IProducerConsumerTask task, bool bClientTickTask = false)
		{
			EngineControler.Instance.QueForProcessing(task);
		}

		protected int consumed = 0;
		protected virtual void Consume()
		{
		}
	}
}
