using System;
using System.Collections.Generic;
using System.Text;

namespace LibNet
{
	public class Engine
	{
		private static object lockObj = new object();
		private static List<Action<ClientConnection>> OnClientDisconnected = new List<Action<ClientConnection>>();
		public static void AddClientDisconnectedEvent(Action<ClientConnection> evt)
		{
			lock (lockObj)
			{
				OnClientDisconnected.Add(evt);
			}
		}
		public static void RaseClientDisconnectedEvent(ClientConnection client)
		{
			lock (lockObj)
			{
				foreach (var ceh in OnClientDisconnected)
				{
					ceh(client);
				}
			}
		}
		public static byte[][] b1 = new byte[][] {
		 new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0 },
		 new byte[] { 0, 0, 1, 1, 1, 1, 0, 0, 0 },
		 new byte[] { 0, 1, 0, 0, 0, 0, 1, 0, 0 },
		 new byte[] { 0, 1, 0, 0, 0, 0, 0, 0, 0 },
		 new byte[] { 0, 1, 0, 0, 0, 0, 0, 0, 0 },
		 new byte[] { 0, 1, 0, 0, 0, 0, 0, 0, 0 },
		 new byte[] { 0, 1, 1, 1, 1, 1, 0, 0, 0 },
		 new byte[] { 0, 0, 1, 1, 1, 1, 1, 0, 0 },
		 new byte[] { 0, 0, 0, 0, 0, 0, 1, 0, 0 },
		 new byte[] { 0, 0, 0, 0, 0, 0, 1, 0, 0 },
		 new byte[] { 0, 0, 0, 0, 0, 0, 1, 0, 0 },
		 new byte[] { 0, 0, 0, 0, 0, 0, 1, 0, 0 },
		 new byte[] { 0, 1, 0, 0, 0, 0, 1, 0, 0 },
		 new byte[] { 0, 0, 1, 1, 1, 1, 0, 0, 0 },
		 new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0 }};
	}
}
