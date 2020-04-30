using System;
using System.Net.NetworkInformation;
using System.Net.Sockets;
#if UNITY_ANDROID
using UnityEngine;
#endif

public class NetStates
{
	public static bool _TryPing(string strIpAddress, int intPort, int nTimeoutMsec)
	{
		Socket socket = null;
		try
		{
			socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
			socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.DontLinger, false);

			IAsyncResult result = socket.BeginConnect(strIpAddress, intPort, null, null);
			TimeSpan timeSpan = TimeSpan.FromMilliseconds(nTimeoutMsec);
			bool success = result.AsyncWaitHandle.WaitOne(timeSpan, true);
			bool connected = socket.Connected;
			if (connected)
			{
				socket.Disconnect(false);
			}
			return connected;
		}
		catch (Exception ex)
		{
			return false;
		}
		finally
		{
			if (null != socket)
				socket.Close();
		}
	}

	public static string Util_MacAddress()
	{
#if UNITY_ANDROID
		return "";//SystemInfo.deviceUniqueIdentifier;
#else
		return "";
		string physicalAddress = "";

		NetworkInterface[] nice = NetworkInterface.GetAllNetworkInterfaces();

		foreach (NetworkInterface adaper in nice)
		{
			if (adaper.Description == "en0")
			{
				physicalAddress = adaper.GetPhysicalAddress().ToString();
				break;
			}
			else
			{
				physicalAddress = adaper.GetPhysicalAddress().ToString();

				if (physicalAddress != "")
				{
					break;
				};
			}
		}

		return physicalAddress; 
#endif
	}

}
