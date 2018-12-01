using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class AGameBase
{
	protected GameObject gameObj;
	protected bool bTeacher;
	public void SetGameObj(GameObject gameObj, bool bTeacher)
	{
		if (dGameObjects.ContainsKey(gameObj.name))
		{
			dGameObjects[gameObj.name] = gameObj;
		}
		else
		{
			dGameObjects.Add(gameObj.name, gameObj);
		}
		this.gameObj = gameObj;
		this.bTeacher = bTeacher;
		var r = this.gameObj.AddComponent<UEmitMessager>();
		r.gameBase = this;
		r.onReceiveMsg = (msg) =>
		{
			onReceiveMsg?.Invoke(msg);
		};

		InitComponents();
	}
	protected void OnDelaySendMessage(string message, float delay)
	{
		gameObj.GetComponent<UEmitMessager>().OnSendMessage(message, delay);
	}
	protected abstract void InitComponents();
	public T FindWidget<T>(string widgetName) where T : Component
	{
		return UStaticFuncs.FindChildComponent<T>(gameObj.transform, widgetName);
	}
	public T FindWidget<T>(Transform trans, string widgetName) where T : Component
	{
		return UStaticFuncs.FindChildComponent<T>(trans, widgetName);
	}

	protected void ShowWidget(string widgetName, bool bShow)
	{
		var w = UStaticFuncs.FindChildComponent<Component>(gameObj.transform, widgetName);
		if (w != null)
		{
			w.gameObject.SetActive(bShow);
		}
	}

	private static GameObject msgReceiver;
	static void SetUnityMessageReceiver(GameObject receiver)
	{
		msgReceiver = receiver;
	}
	public static void SendMessageToUnityReceiver(string message)
	{
		if (msgReceiver == null)
		{
			return;
		}
		msgReceiver.SendMessage("EmitMessage", message);
	}

	static Dictionary<string, GameObject> dGameObjects = new Dictionary<string, GameObject>();
	static void EmitMessage(string gameobjName, string message)
	{
		Debug.Log("EmitMessage");
		if (string.IsNullOrEmpty(gameobjName))
		{
			foreach (var g in dGameObjects.Values)
			{
				if (g == null)
				{
					continue;
				}
				var rec = g.GetComponent<UEmitMessager>();
				if (rec != null)
				{
					rec.ReceiveMessage(message);
				}
			}
			return;
		}
		if (!dGameObjects.ContainsKey(gameobjName) || dGameObjects[gameobjName] == null)
		{
			return;
		}
		dGameObjects[gameobjName].GetComponent<UEmitMessager>().ReceiveMessage(message);
	}

	private Action<string> onReceiveMsg;
	protected void RegistReceiveMsg(Action<string> a)
	{
		onReceiveMsg = a;
	}

}
