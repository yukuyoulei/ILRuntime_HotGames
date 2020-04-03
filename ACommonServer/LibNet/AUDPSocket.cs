using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Threading;
using System.Net;

public class AUDPSocket
{
	private static AUDPSocket s_instance;
	public static AUDPSocket Instance
	{
		get
		{
			if (s_instance == null)
			{
				s_instance = new AUDPSocket();
			}
			return s_instance;
		}
	}

	static Action<IPEndPoint, byte[]> receiveHandler;
	static UdpClient udpClient;
	static IPEndPoint ipendpoint;
	static IPEndPoint ipendpointv6;
	public void Start(int port, Action<IPEndPoint, byte[]> bytesReceiveHandler)
	{
		if (udpClient != null)
		{
			return;
		}
		udpClient = new UdpClient(port);
		receiveHandler = bytesReceiveHandler;
		ipendpoint = new IPEndPoint(IPAddress.Any, port);
		ipendpointv6 = new IPEndPoint(IPAddress.IPv6Any, port);

		var addresses = Dns.GetHostAddresses(Dns.GetHostName());
		AOutput.Log("Listening UDP " + addresses.Length + " addresses");
		foreach (var adr in addresses)
		{
			AOutput.Log("\t" + adr.ToString());
		}

		if (receiveHandler != null)
		{
			new Thread(new ThreadStart(ReceiveHand)).Start();
			new Thread(new ThreadStart(ReceiveHandV6)).Start();
		}
	}
	private void ReceiveHandV6()
	{
		while (true)
		{
			try
			{
				byte[] bytes = udpClient.Receive(ref ipendpointv6);
				if (receiveHandler != null)
				{
					receiveHandler(ipendpointv6, bytes);
				}
			}
			catch
			{
				break;
			}
		}
	}
	private void ReceiveHand()
	{
		while (true)
		{
			try
			{
				byte[] bytes = udpClient.Receive(ref ipendpoint);
				if (receiveHandler != null)
				{
					receiveHandler(ipendpoint, bytes);
				}
			}
			catch
			{
				break;
			}
		}
	}

	public void Dispose()
	{
		udpClient.Close();
	}

	public void SendTo(IPEndPoint iep, byte[] datas)
	{
		try
		{
			SendTo(iep.Address.ToString(), iep.Port, datas);
		}
		catch
		{

		}
	}
	public void SendTo(string ip, int port, byte[] datas)
	{
		try
		{
			udpClient.Send(datas, datas.Length, ip, port);
		}
		catch
		{

		}
	}
	public void SendTo(string ip, int port, string sContent)
	{
		try
		{
			byte[] datas = Encoding.UTF8.GetBytes(sContent);
			SendTo(ip, port, datas);
		}
		catch
		{

		}
	}
}
