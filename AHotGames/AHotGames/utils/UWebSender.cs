using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using UnityEngine;
using UnityEngine.Networking;

public class UWebSender : MonoBehaviour
{
	private static UWebSender sinstance;
	public static UWebSender Instance
	{
		get
		{
			if (sinstance == null)
			{
				var obj = new GameObject("UWebSender");
				sinstance = obj.AddComponent<UWebSender>();
			}
			return sinstance;
		}
	}
	public static void OnAddProducingAction(Action a)
	{
		callbacksProducing.Add(a);
	}
	private static List<Action> callbacksProducing = new List<Action>();
	private static List<Action> callbacksConsuming = new List<Action>();
	public static void OnDownloadBytes(string url, string scontent, Action<byte[]> actionResult, Action<string> actionFailed = null)
	{
		UICommonWait.Show();
		new Thread(new ThreadStart(() =>
		{
			try
			{
				if (!string.IsNullOrEmpty(scontent))
				{
					url = $"{url}?{scontent}";
				}
				var req = (HttpWebRequest)HttpWebRequest.Create(url);
				req.Method = "GET";
				using (var wr = req.GetResponse() as HttpWebResponse)
				{
					var resStream = wr.GetResponseStream();          //获得Response的流

					int count = (int)resStream.Length;
					int offset = 0;
					var buf = new byte[count];
					while (count > 0)
					{
						int n = resStream.Read(buf, offset, count);
						if (n == 0) break;
						count -= n;
						offset += n;
					}

					callbacksProducing.Add(() =>
					{
						if (actionResult != null)
							actionResult(buf);
					});
				}
			}
			catch (Exception ex)
			{
				Debug.Log($"web sender error:{ex.Message}\r\n{ex.StackTrace}");
				//lock (lockObj)
				{
					callbacksProducing.Add(() =>
					{
						if (actionFailed != null)
							actionFailed(ex.Message);
					});
				}

			}
		}
		)).Start();
	}
	public static void OnRequest(string url, string scontent, Action<Newtonsoft.Json.Linq.JObject> actionResult, Action<string> actionFailed = null, bool bSortArguments = true, bool bShowCommonWait = true)
	{
		Instance.DoRequest(url, scontent, actionResult, actionFailed, bSortArguments, bShowCommonWait);
	}
	//private static object lockObj = new object();
	private void DoRequest(string url, string scontent, Action<Newtonsoft.Json.Linq.JObject> actionResult, Action<string> actionFailed, bool bSortArguments, bool bShowCommonWait)
	{
		if (bShowCommonWait)
			UICommonWait.Show();
		new Thread(new ThreadStart(() =>
		{
			try
			{
				var uri = $"{url}?{scontent}";
				Debug.Log($"request {uri}");
				var req = (HttpWebRequest)HttpWebRequest.Create(uri);
				req.Method = "GET";
				using (WebResponse wr = req.GetResponse())
				{
					var result = new StreamReader(wr.GetResponseStream(), Encoding.UTF8).ReadToEnd();
					if (string.IsNullOrEmpty(result))
					{
						UIAlert.Show($"{url}请求返回空。");
						return;
					}
					Debug.Log($"url {uri} result {result}");
					//lock (lockObj)
					{
						callbacksProducing.Add(() =>
						{
							actionResult(JsonConvert.DeserializeObject(result) as JObject);
						});
					}
				}
			}
			catch (WebException ex)
			{
				Debug.Log($"web sender WebException:{ex.Message}\r\n{ex.StackTrace}");
				//lock (lockObj)
				{
					callbacksProducing.Add(() =>
					{
						UIAlert.Show($"请求失败：{url} Web崩溃信息：{ex.Message}");
						if (actionFailed != null)
							actionFailed(ex.Message);
					});
				}

			}
			catch (Exception ex)
			{
				Debug.Log($"web sender error:{ex.Message}\r\n{ex.StackTrace}");
				//lock (lockObj)
				{
					callbacksProducing.Add(() =>
					{
						UIAlert.Show($"请求失败：{url} 崩溃信息：{ex.Message}");
						if (actionFailed != null)
							actionFailed(ex.Message);
					});
				}

			}
		}
		)).Start();
	}

	private void Start()
	{

	}
	private void Update()
	{
		if (callbacksProducing.Count == 0)
		{
			return;
		}
		//lock (lockObj)
		{
			callbacksConsuming.AddRange(callbacksProducing);
			foreach (var c in callbacksConsuming)
			{
				callbacksProducing.Remove(c);
			}
			foreach (var cb in callbacksConsuming)
			{
				//if (cb.Target != null)
				{
					cb();
				}
			}
			callbacksConsuming.Clear();

			UICommonWait.Hide();
		}
	}

	public static void OnGet(string url, Action<Newtonsoft.Json.Linq.JObject> actionResult, Action<string> actionFailed = null)
	{
		Instance.DoGet(url, actionResult, actionFailed);
	}
	//private static object lockObj = new object();
	private void DoGet(string url, Action<Newtonsoft.Json.Linq.JObject> actionResult, Action<string> actionFailed)
	{
		UICommonWait.Show();
		new Thread(new ThreadStart(() =>
		{
			try
			{
				var req = (HttpWebRequest)HttpWebRequest.Create(url);
				req.Method = "GET";
				using (WebResponse wr = req.GetResponse())
				{
					var result = new StreamReader(wr.GetResponseStream(), Encoding.UTF8).ReadToEnd();
					if (string.IsNullOrEmpty(result))
					{
						UIAlert.Show($"{url}请求返回空。");
						return;
					}
					Debug.Log($"url {url} result {result}");
					//lock (lockObj)
					{
						callbacksProducing.Add(() =>
						{
							//result = FilterResult(result);
							Debug.Log($"url {url} FilterResult {result.URLDecode()}");
							var jres = JsonConvert.DeserializeObject(result) as Newtonsoft.Json.Linq.JObject;
							actionResult?.Invoke(jres);
						});
					}
				}
			}
			catch (WebException ex)
			{
				Debug.Log($"web sender WebException:{ex.Message}\r\n{ex.StackTrace}");
				//lock (lockObj)
				{
					callbacksProducing.Add(() =>
					{
						UIAlert.Show($"请求失败：{url} Web崩溃信息：{ex.Message}");
						if (actionFailed != null)
							actionFailed(ex.Message);
					});
				}

			}
			catch (Exception ex)
			{
				Debug.Log($"web sender error:{ex.Message}\r\n{ex.StackTrace}");
				//lock (lockObj)
				{
					callbacksProducing.Add(() =>
					{
						UIAlert.Show($"请求失败：{url} 崩溃信息：{ex.Message}");
						if (actionFailed != null)
							actionFailed(ex.Message);
					});
				}

			}
		}
		)).Start();
	}

}
