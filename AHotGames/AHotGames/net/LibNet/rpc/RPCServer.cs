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
			Client = client;
			this.playerConnDesc = sPlayerConnDesc;
		}
		public ClientConnection Client { get; }
		public static bool bShowPktSize = false;
		public void Response(PktBase message)
		{
			ServerUtils.PushMsg(Client, message);
		}

		public string playerConnDesc { get; } = "";
	}
}