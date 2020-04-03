using System;
using System.Text;

namespace LibNet
{

	public enum eMessageType
	{
		PacketID_Common = 1,
		
		MaxPacketID = 255
	}


	public class Packet
	{
		protected ClientConnection m_Client;
		protected bool m_bValid;

		public bool IsValid
		{
			get { return m_bValid; }
		}

		public static byte[][] b1 = new byte[][] {
		 new byte[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
		 new byte[] {0, 0, 0, 0, 0, 1, 1, 0, 0, 0 },
		 new byte[] {0, 0, 0, 0, 1, 0, 0, 1, 0, 0 },
		 new byte[] {0, 0, 0, 0, 1, 0, 0, 0, 0, 0 },
		 new byte[] {0, 0, 0, 0, 1, 0, 0, 0, 0, 0 },
		 new byte[] {0, 0, 0, 0, 1, 0, 0, 1, 0, 0 },
		 new byte[] {0, 0, 0, 1, 1, 1, 1, 0, 0, 0 },
		 new byte[] {0, 0, 1, 0, 1, 0, 0, 0, 0, 0 },
		 new byte[] {0, 1, 0, 0, 1, 0, 0, 0, 0, 0 },
		 new byte[] {0, 0, 0, 0, 1, 0, 0, 0, 0, 0 },
		 new byte[] {0, 0, 0, 0, 1, 0, 0, 0, 0, 0 },
		 new byte[] {0, 0, 0, 0, 1, 0, 0, 0, 0, 0 },
		 new byte[] {0, 0, 1, 0, 1, 0, 0, 0, 0, 0 },
		 new byte[] {0, 0, 0, 1, 0, 0, 0, 0, 0, 0 },
		 new byte[] {0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }};
		public Packet()
		{
			m_bValid = true;
		}

		public void FactoryInit(ByteStream buffer, ClientConnection client)
		{
			m_Client = client;

			if (null == buffer) return;
			try
			{
				int iPacketSize = buffer.Length;
				Initialize(buffer);

				if (buffer.BytesRead != iPacketSize)
				{
					string error = string.Format(@"Packet size mismatch of type {0}", this.GetType().ToString());
					AOutput.Log(error);
					throw new System.Exception(error);
				}
			}
			catch
			{
				AOutput.Log(string.Format(@"Failed Initializing Packet of type {0}", this.GetType().ToString()));
				m_bValid = false;
			}
		}

		virtual public void Initialize(ByteStream buffer)
		{
		}

		virtual public void PreProcess()
		{
		}

		virtual public void Process()
		{
			AOutput.Log("Packet.Process: Unknown Packet");
		}
	}

	internal class RPCCommonPacket : Packet
	{
		private int m_iPacket;
		private byte[] m_bytes;
		override public void Initialize(ByteStream buffer)
		{
			m_iPacket = buffer.ReadInt();
			m_bytes = buffer.ReadBytesToEnd();
		}

		override public void Process()
		{
			CallBase caller = AFactoryPacket.Instance.GetCaller(m_iPacket);
			if (null != caller)
			{
				caller.Call(new rpc.CResponser(m_Client.ConnectionDesc, m_Client), m_bytes);
			}
			else
			{
				AOutput.Log("RPCCommonPacket.Process invalid caller " + m_iPacket + " !");
				m_Client.CloseConnection();
			}
		}

	}

	internal class SecurityReqeustPacket : Packet
	{

		override public void Process()
		{
			String sXml = "<?xml version=\"1.0\"?><cross-domain-policy><site-control permitted-cross-domain-policies=\"all\"/>"
				+ "<allow-access-from domain=\"*\" to-ports=\"*\"/>"
				+ "</cross-domain-policy>\0";
			byte[] arrData = Encoding.GetEncoding("UTF-8").GetBytes(sXml);
			m_Client.SendByteData(arrData);
		}
	}
	internal class ClientDisConnectedPacket : Packet
	{
		override public void Process()
		{
			Engine.RaseClientDisconnectedEvent(this.m_Client);

			this.m_Client.CloseConnection();
		}
	}

}
