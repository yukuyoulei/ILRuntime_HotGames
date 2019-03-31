using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WebSocketSharp;

public class WebSocketConnector
{
	private static WebSocketConnector sinstance;
	public static WebSocketConnector Instance
	{
		get
		{
			if (sinstance == null)
			{
				sinstance = new WebSocketConnector();
			}
			return sinstance;
		}
	}

	WebSocket ws;
	public bool bConnecting { get; private set; }
	private bool bConnected { get; set; }
	private static bool receive = false;
	Action<string> closeAction;
	private static bool bHeartbeat;
	public void OnInit(string wsurl, Action<EventArgs> openAction, Action<MessageEventArgs> msgAction
		, Action<ErrorEventArgs> errorAction, Action<string> closeAction)
	{
		if (bConnecting
			|| bConnected)
		{
			return;
		}
		this.closeAction = closeAction;

		if (!bHeartbeat)
		{
			bHeartbeat = true;
			new Thread(new ThreadStart(() =>
			{
				while (true)
				{
					Thread.Sleep(5000);

					if (!bConnected)
					{
						continue;
					}
					try
					{
						if (!receive)
						{
							ws.Close();
						}
						else
						{
							receive = false;
							ws.Send("ping");
						}
					}
					catch { }
				}
			})).Start();
		}

		ws = new WebSocket(wsurl);
		ws.OnOpen += (sender, e) =>
		{
			bConnecting = false;
			bConnected = true;
			try
			{
				openAction(e);
			}
			catch (Exception ex)
			{
				UnityEngine.Debug.Log("OnOpen:" + ex.Message);
				UnityEngine.Debug.Log("stacktrace:" + ex.StackTrace);
			}
			receive = true;
		};
		ws.OnMessage += (sender, e) =>
		{
			if (e.Data == "ping")
			{
				receive = true;
			}
			else
			{
				try
				{
					UnityEngine.Debug.Log("OnMessage:" + (e.IsText ? e.Data : e.RawData.Length.ToString()));
					msgAction(e);
				}
				catch (Exception ex)
				{
					UnityEngine.Debug.Log("OnMessage:" + ex.Message);
					UnityEngine.Debug.Log("stacktrace:" + ex.StackTrace);
				}
			}
		};
		ws.OnError += (sender, e) => { errorAction(e); };
		ws.OnClose += (sender, e) =>
		{
			bConnecting = false;
			bConnected = false;
			this.closeAction?.Invoke(e.Reason);
		};
		new Thread(new ThreadStart(() =>
		{
			bConnecting = true;
			try
			{
				ws.Connect();
			}
			catch
			{
				this.closeAction?.Invoke("connect failed");
			}

		})).Start();
	}

	public void Send(string msg)
	{
		new Task(() =>
		{
			ws.Send(msg);
		}).Start();
	}
	public void OnClose()
	{
		if (!bConnected)
		{
			return;
		}
		this.closeAction = null;
		UnityEngine.Debug.Log("ws OnClose()");
		if (ws != null)
		{
			ws.Close();
		}
	}
}
