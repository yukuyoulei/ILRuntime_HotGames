using LibClient;
using System;
using System.Collections.Generic;
using UnityEngine;
public class UEventListener : MonoBehaviour
{
	private static UEventListener sinstance;
	public static bool hasInstance
	{
		get
		{
			return sinstance != null;
		}
	}
	public static UEventListener Instance
	{
		get
		{
			if (sinstance == null)
			{
				var obj = new GameObject("UEventListener");
				GameObject.DontDestroyOnLoad(obj);
				sinstance = obj.AddComponent<UEventListener>();
			}
			return sinstance;
		}
	}

	private static List<Action> callbacksProducing = new List<Action>();
	private static List<Action> callbacksConsuming = new List<Action>();
	public void AddProducingAction(Action a)
	{
		callbacksProducing.Add(a);
	}
	public static void OnAddProducingAction(Action a)
	{
		Instance.AddProducingAction(a);
	}

	Dictionary<string, List<Action<UEventBase>>> dListeningEvent;
	public void OnRegisterEvent(string eventName, Action<UEventBase> action)
	{
		if (dListeningEvent == null)
			dListeningEvent = new Dictionary<string, List<Action<UEventBase>>>();
		if (!dListeningEvent.ContainsKey(eventName))
			dListeningEvent.Add(eventName, new List<Action<UEventBase>>());
		dListeningEvent[eventName].Add(action);
	}
	public void OnUnregisterEvent(string eventName, Action<UEventBase> action)
	{
		if (dListeningEvent == null) return;
		if (!dListeningEvent.ContainsKey(eventName)) return;
		if (dListeningEvent[eventName].Contains(action))
			dListeningEvent[eventName].Remove(action);
	}
	public void OnDispatchEvent(string eventName, UEventBase eb)
	{
		if (!dListeningEvent.ContainsKey(eventName)) return;
		var l = dListeningEvent[eventName].ToArray();
		foreach (var e in l)
		{
			if (e == null)
			{
				dListeningEvent[eventName].Remove(e);
				continue;
			}
			AddProducingAction(()=> { e(eb); });
		}
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
		callbacksConsuming.AddRange(callbacksProducing);
		foreach (var c in callbacksConsuming)
		{
			callbacksProducing.Remove(c);
		}
		foreach (var cb in callbacksConsuming)
		{
			cb();
		}
		callbacksConsuming.Clear();

		UICommonWait.Hide();
	}

	private void OnDestroy()
	{
		AClientApp.OnDisconnect();
	}
}

