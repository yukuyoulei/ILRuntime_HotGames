using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using LibNet;
using LibPacket;

public static class ServerUtils
{
	public static void PushMsg(ClientConnection client, PktBase message)
	{
		ByteStream buffer = new ByteStream(1024, (byte)eMessageType.PacketID_Common);
		buffer.WriteInt(message.pktDef);
		var ms = new MemoryStream();
		message.Serialize(ms);
		buffer.WriteByteArray(ms.ToArray());
		client.SendPacket(buffer);
	}
}
