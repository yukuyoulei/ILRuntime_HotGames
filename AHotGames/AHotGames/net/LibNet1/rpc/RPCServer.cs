using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using System.Reflection;
using System.Net.Sockets;
using System.IO;
using LibPacket;

namespace LibNet.rpc
{
	public class CResponser : IResponer
	{
		public CResponser(string sPlayerConnDesc, ClientConnection client)
		{
			m_client = client;
			this._playerConnDesc = sPlayerConnDesc;
			eMsgType = eMessageType.PacketID_Common;
		}
		public ClientConnection Client
		{
			get
			{
				return m_client;
			}
		}
		public static bool bShowPktSize = false;
		public void Response(PktBase message)
		{
			ByteStream buffer = new ByteStream(1024, (byte)eMsgType);
			buffer.WriteInt(message.pktDef);
			var ms = new MemoryStream();
			message.Serialize(ms);
			buffer.WriteByteArray(ms.ToArray());
			m_client.SendPacket(buffer);
		}
		private ClientConnection m_client;
		public string playerConnDesc
		{
			get
			{
				return _playerConnDesc;
			}
		}
		private string _playerConnDesc = "";
		protected eMessageType eMsgType = eMessageType.PacketID_Common;
	}
}