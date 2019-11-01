using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public abstract class AHotBase
{
	public GameObject gameObj;
	public static AHotBase curGame;
	public static string strArg;
	protected bool bDestroying;
	private static AHotBase sinstance;
	public void SetGameObj(GameObject gameObj, string arg = "")
	{
		sinstance = this;

		if (arg.Equals("true", StringComparison.CurrentCultureIgnoreCase))
		{
			Environment.UseAB = true;
		}
		else if (arg.Equals("false", StringComparison.CurrentCultureIgnoreCase))
		{
			Environment.UseAB = false;
		}

		if (dGameObjects.ContainsKey(gameObj.name))
		{
			dGameObjects[gameObj.name] = gameObj;
		}
		else
		{
			dGameObjects.Add(gameObj.name, gameObj);
		}
		this.gameObj = gameObj;
		strArg = arg;
		var r = this.gameObj.AddComponent<UEmitMessager>();
		curGame = this;
		r.gameBase = this;
		r.OnRegistAction((msg) =>
		{
			onReceiveMsg?.Invoke(msg);

			var amsg = msg.Split(new char[] { ':' }, 2);
			switch (amsg[0])
			{
				case "op":
					receiveOpMsg(amsg[1] == "1");
					break;
				case "targetRuntime":
					Utils.TargetRuntimeInEditor = amsg[1];
					UDebugHotLog.Log($"Utils.TargetRuntimeInEditor {Utils.TargetRuntimeInEditor}");
					break;
			}
		});

		gameObj.AddComponent<UOnDestroy>().actionOnDestroy = () =>
		{
			OnDestroy();
		};

		InitComponents();
	}

	protected virtual void receiveOpMsg(bool bOp)
	{
		enableAllGraphicRaycasters(bOp);
	}

	protected void muteAllAudioSource(bool bMute)
	{
		var audios = gameObj.GetComponentsInChildren<AudioSource>(true);
		foreach (var a in audios)
		{
			a.enabled = !bMute;
		}
	}
	protected void enableAllGraphicRaycasters(bool enable)
	{
		var all = gameObj.GetComponentsInChildren<Graphic>(true);
		foreach (var a in all)
		{
			a.raycastTarget = enable;
		}
	}
	UUpdater updater;
	private List<Func<bool>> waitForAddFuncs = new List<Func<bool>>();
	private List<Func<bool>> updateActions = new List<Func<bool>>();
	protected void addUpdateAction(Func<bool> func)
	{
		waitForAddFuncs.Add(func);
		initUUpdater();
	}
	private void initUUpdater()
	{
		if (gameObj == null)
		{
			return;
		}
		if (updater == null)
		{
			updater = gameObj.AddComponent<UUpdater>();
			updater.onUpdate += () =>
			{
				if (waitForAddFuncs.Count > 0)
				{
					updateActions.AddRange(waitForAddFuncs);
					waitForAddFuncs.Clear();
				}
				if (updateActions.Count > 0)
				{
					var remove = new List<Func<bool>>();
					foreach (var u in updateActions)
					{
						/*try
						{*/
						if (u())
						{
							remove.Add(u);
						}
						/*}
						catch (Exception ex)
						{
							UDebugHotLog.Log($"update error:\r\n{this.GetType().FullName}\r\n{ex.Message}\r\n{ex.StackTrace}");
							remove.Add(u);
						}*/
					}
					foreach (var r in remove)
					{
						updateActions.Remove(r);
					}
				}
			};
		}
	}
	protected void DelayDoSth(float delay, Action sth)
	{
		DelayDoSth(sth, delay);
	}
	protected void DelayDoSth(Action sth, float delay)
	{
		var delta = 0f;
		addUpdateAction(() =>
		{
			delta += Time.deltaTime;
			if (delta > delay)
			{
				sth();
				return true;
			}
			return false;
		});
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
	public static T FindWidget<T>(Transform trans, string widgetName) where T : Component
	{
		return UStaticFuncs.FindChildComponent<T>(trans, widgetName);
	}

	protected void ShowWidget(string widgetName, bool bShow)
	{
		ShowWidget(gameObj.transform, widgetName, bShow);
	}
	protected void ShowWidget(Transform parent, string widgetName, bool bShow)
	{
		var w = UStaticFuncs.FindChildComponent<Component>(parent, widgetName);
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
	public static void SendInvokeToUnityReceiver(string gameObjName, string message, string argument = "")
	{
		SendMessageToUnityReceiver($"invoke|{gameObjName}:{message}{(string.IsNullOrEmpty(argument) ? "" : $":{argument}")}");
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
	public static void EmitMessage(string gameobjName, string message)
	{
		if (string.IsNullOrEmpty(gameobjName))
		{
			var allObjs = dGameObjects.Values.ToList();
			foreach (var g in allObjs)
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
	public static void EmitGameObject(string gameobjName, string prefabName, GameObject obj)
	{
		if (dPendingActions.ContainsKey(prefabName))
		{
			foreach (var a in dPendingActions[prefabName])
			{
				a?.Invoke(obj);
			}
			dPendingActions.Remove(prefabName);
		}
	}
	static Dictionary<string, List<Action<GameObject>>> dPendingActions = new Dictionary<string, List<Action<GameObject>>>();
	protected static void LoadPrefab(string prefabName, Action<GameObject> action)
	{
		if (!dPendingActions.ContainsKey(prefabName))
		{
			dPendingActions.Add(prefabName, new List<Action<GameObject>>());
		}
		dPendingActions[prefabName].Add(action);

		var smsg = "loadprefab|" + prefabName;
		Debug.Log("send message to unity:" + smsg);
		SendMessageToUnityReceiver(smsg);
	}

	private Action<string> onReceiveMsg;
	public void OnRegistAction(Action<string> a)
	{
		onReceiveMsg += a;
	}
	protected Dictionary<string, Button> dButtons = new Dictionary<string, Button>();
	protected void OnRegistButtonActions()
	{
		OnRegistAction((msg) =>
		{
			if (!dButtons.ContainsKey(msg))
			{
				return;
			}
			UStaticFuncs.EmitButtonClick(dButtons[msg]);
		});
	}
	protected AHotDrag RegistDragFunc(Graphic dragObj, Graphic dragDropObj, Action beginDrag, Action endDrag, Action dropDrag, bool DontSyncDrag = false, Action draging = null)
	{
		var drag = dragObj.gameObject.GetComponent<AHotDrag>();
		if (drag == null)
		{
			drag = dragObj.gameObject.AddComponent<AHotDrag>();
		}
		drag.onDragBegin += beginDrag;
		drag.onDragEnd += endDrag;
		if (draging != null)
		{
			drag.onDraging = draging;
		}

		if (dragDropObj == null)
		{
			return drag;
		}
		var drag1 = dragDropObj.gameObject.AddComponent<AHotDrag>();
		drag1.onDragDrop += dropDrag;
		return drag;
	}

	public static void LoadAnotherClass(string classname, string prefab = "", string arg = "")
	{
		var smsg = "load|" + classname + "|" + prefab + "|" + arg;
		Debug.Log("send message to unity:" + smsg);
		SendMessageToUnityReceiver(smsg);
	}
	public static T LoadClass<T>(string prefabPath) where T : AHotBase, new()
	{
		GameObject obj = UHotAssetBundleLoader.Instance.OnLoadAsset<GameObject>(prefabPath);
		if (obj == null)
		{
			UDebugHotLog.Log($"cannot find prefab {prefabPath}");
			return null;
		}
		var t = new T();
		t.SetGameObj(GameObject.Instantiate(obj), "");
		return t;
	}
	public static T LoadUI<T>() where T : AHotBase, new()
	{
		var path = "UI/" + typeof(T).Name;
		GameObject obj = UHotAssetBundleLoader.Instance.OnLoadAsset<GameObject>(path);
		if (obj == null)
		{
			UDebugHotLog.Log($"cannot find prefab {path}");
			return null;
		}
		var t = new T();
		t.SetGameObj(GameObject.Instantiate(obj), "");
		return t;
	}
	public static T LoadAnotherUI<T>() where T : AHotBase, new()
	{
		return LoadUI<T>();
	}
	public static void LoadAnotherUI(string uiname)
	{
		LoadAnotherClass(uiname, "UI/" + uiname + "");
	}
	public virtual void OnUnloadThis()
	{
		bDestroying = true;

		if (gameObj == null) return;
		if (dGameObjects.ContainsKey(gameObj.name))
		{
			dGameObjects.Remove(gameObj.name);
		}
		GameObject.Destroy(gameObj);
	}
	public static void UnloadAllClasses()
	{
		SendMessageToUnityReceiver("unloadall");
	}

	protected void OnInvokeFunc(string funcName)
	{
		SendMessageToUnityReceiver("sop:func:" + funcName);
	}
	List<OperationCell> operationList = new List<OperationCell>();
	protected void EnqueueOperation(float delay, Action action)
	{
		operationList.Add(new OperationCell() { action = action, delay = delay });
	}
	protected void DoStarOperations()
	{
		var n = 0f;
		addUpdateAction(() =>
		{
			if (operationList.Count == 0)
			{
				return true;
			}
			n += Time.deltaTime;
			if (n < operationList[0].delay)
			{
				return false;
			}
			n -= operationList[0].delay;
			operationList[0].action();
			operationList.RemoveAt(0);
			return false;
		});
	}
	struct OperationCell
	{
		public Action action;
		public float delay;
	}

	protected virtual void OnDestroy()
	{
	}
}
