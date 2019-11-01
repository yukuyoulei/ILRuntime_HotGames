using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using WebSocketSharp;

public class WebSocketConnector : MonoBehaviour
{
	private static WebSocketConnector sinstance;
	public static WebSocketConnector Instance
	{
		get
		{
			if (sinstance == null)
			{
				var obj = new GameObject("WebSocketConnector");
				sinstance = obj.AddComponent<WebSocketConnector>();
			}
			return sinstance;
		}
	}

	WebSocket ws;
	public bool bConnecting { get; private set; }
	private bool bConnected { get; set; }
	private static bool receive = false;
	Action<string> closeAction;
	private static List<Action> callbacks = new List<Action>();
	private static object lockObj = new object();
	public void OnInit(string wsurl, Action<EventArgs> openAction, Action<MessageEventArgs> msgAction
		, Action<ErrorEventArgs> errorAction, Action<string> closeAction)
	{
		if (bConnecting
			|| bConnected)
		{
			return;
		}
		this.closeAction = closeAction;

		ws = new WebSocket(wsurl);
		ws.OnOpen += (sender, e) =>
		{
			UnityEngine.Debug.Log("OnOpen:" + wsurl);
			bConnecting = false;
			bConnected = true;
			lock (lockObj)
			{
				callbacks.Add(() =>
				{
					UICommonWait.Hide();
					openAction(e);
				});
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
				UnityEngine.Debug.Log("OnMessage:" + (e.IsText ? e.Data : e.RawData.Length.ToString()));
				lock (lockObj)
				{
					callbacks.Add(() =>
					{
						msgAction(e);

						var amsg = e.Data.Split('?');
						if (amsg.Length > 1)
						{
							OnResponse(amsg[0], amsg[1]);
						}
					});
				}
			}
		};
		ws.OnError += (sender, e) =>
		{
			UnityEngine.Debug.Log("OnError:" + e.Message);
			lock (lockObj)
			{
				callbacks.Add(() =>
				{
					errorAction(e);
				});
			}
		};
		ws.OnClose += (sender, e) =>
		{
			bConnecting = false;
			bConnected = false;

			closeAction?.Invoke(e.Reason);
			UnityEngine.Debug.Log("OnClose:" + e.Reason);
		};
		bConnecting = true;
		UnityEngine.Debug.Log("ws.Connect() " + wsurl);
		UICommonWait.Show();
		new Task(() =>
		{
			try
			{
				ws.Connect();
			}
			catch
			{
				UnityEngine.Debug.Log("connect failed:" + wsurl);
			}
		}).Start();
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
		UnityEngine.Debug.Log("ws close on purpose.");
		if (ws != null)
		{
			ws.Close();
		}
	}
	private void Start()
	{

	}
	float fdelta = 0;
	private void Update()
	{
		fdelta += Time.deltaTime;
		if (fdelta > 5)
		{
			fdelta -= 5;
			if (!bConnected)
			{
				return;
			}
			try
			{
				if (!receive)
				{
					Debug.Log("not receive ping");
					//ws.Close();
				}
				else
				{
					receive = false;
					ws.Send("ping");
				}
			}
			catch { }
		}

		if (callbacks.Count == 0)
		{
			return;
		}
		lock (lockObj)
		{
			foreach (var cb in callbacks)
			{
				try
				{
					cb();
				}
				catch (Exception ex)
				{
					UnityEngine.Debug.Log("callback invoke error:" + ex.Message);
					UnityEngine.Debug.Log(ex.StackTrace);
				}
			}
			callbacks.Clear();
		}
	}
	static Dictionary<string, Action<string>> dResponses = new Dictionary<string, Action<string>>();
	private void OnResponse(string method, string argument)
	{
		if (dResponses.ContainsKey(method.ToLower()))
		{
			dResponses[method.ToLower()](argument);
		}
	}
	public void OnRemoteCall(string method, string arguments, Action<string> response)
	{
		if (dResponses.ContainsKey(method.ToLower()))
		{
			dResponses.Remove(method.ToLower());
		}
		dResponses.Add(method.ToLower(), response);

		Send(method + "?" + arguments);
	}
}
