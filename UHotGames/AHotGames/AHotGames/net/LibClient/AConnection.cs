using LibNet;
using LibPacket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class AConnection
{
	public LibNet.rpc.IRPCClient client { get; private set; }
	public string ip { get; private set; }
	public int port { get; private set; }
	public AConnection(string sip, int port)
	{
		this.ip = sip;
		this.port = port;
	}

	public bool IsConnected { get { return client != null && client.GetClient() != null; } }

	public async Task Close()
	{
		await Task.Run(() => { client?.GetClient().CloseConnection(); });
	}
	public async Task Connect()
	{
		await Task.Run(() =>
		{
			client = EngineClient.Connect(ip, port);
		});
	}
	public async Task Send(PktBase pkt)
	{
		await Task.Run(() => { client?.RemoteCall(pkt); });
	}
}
