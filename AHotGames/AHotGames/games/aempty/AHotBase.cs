
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public abstract class AHotBase
{
	public static System.Random random = new System.Random();
	public GameObject gameObj;
	public static AHotBase curGame;
	public static string strArg;
	protected bool bDestroying;
	protected virtual bool bCanBeAutoClosed { get; } = true;
	public void SetGameObj(GameObject gameObj, string arg = "")
	{
		this.gameObj = gameObj;

		if (arg == "Android" || arg == "IOS" || arg == "Windows")
		{
			Utils.TargetRuntimeInEditor = arg;
		}

		if (dGameObjects.ContainsKey(gameObj.name))
		{
			dGameObjects[gameObj.name] = gameObj;
		}
		else
		{
			dGameObjects.Add(gameObj.name, gameObj);
		}
		strArg = arg;
		var r = this.gameObj.AddComponent<UEmitMessager>();
		curGame = this;
		r.gameBase = this;
		r.OnRegistAction((msg) =>
		{
			onReceiveMsg?.Invoke(msg);
		});

		gameObj.AddComponent<UOnDestroy>().actionOnDestroy = () =>
		{
			OnDestroy();
		};

		InitComponents();

		if (bCanBeAutoClosed
			&& !loadedClasses.Contains(this))
			loadedClasses.Add(this);
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
		if (delay > 0)
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
		else
		{
			sth();
		}
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
	public static void SetUnityMessageReceiver(GameObject receiver)
	{
		msgReceiver = receiver;
		AOutput.Log($"MessageReceiver received:{receiver.name}");
	}
	public static void SendMessageToUnityReceiver(GameObject go, string message)
	{
		if (msgReceiver == null)
		{
			return;
		}
		msgReceiver.SendMessage("EmitMessageToObj", new object[] { go, message });
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
	static Dictionary<string, List<Action<GameObject>>> dPendingActions = new Dictionary<string, List<Action<GameObject>>>();
	protected static void LoadPrefab(string prefabPath, Action<GameObject> action)
	{
		UHotAssetBundleLoader.Instance.OnDownloadResources(() =>
		{
			action?.Invoke(UHotAssetBundleLoader.Instance.OnLoadAsset<GameObject>(prefabPath));
		}, prefabPath);
	}

	private Action<string> onReceiveMsg;
	public void OnRegistAction(Action<string> a)
	{
		onReceiveMsg += a;
	}
	protected AHotDrag RegistDragFunc(Graphic dragObj, Graphic dragDropObj, Action beginDrag, Action endDrag, Action dropDrag, Action draging = null)
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
		UHotAssetBundleLoader.Instance.OnDownloadResources(() =>
		{
			var go = UHotAssetBundleLoader.Instance.OnLoadAsset<GameObject>(prefab);
			SendMessageToUnityReceiver(go, classname);
		}, prefab);
	}
	private static List<AHotBase> loadedClasses = new List<AHotBase>();
	public static T LoadClass<T>(string prefabPath, Action<T> action = null) where T : AHotBase, new()
	{
		GameObject obj = UHotAssetBundleLoader.Instance.OnLoadAsset<GameObject>(prefabPath);
		if (obj == null)
		{
			UDebugHotLog.Log($"cannot find prefab {prefabPath}");
			return null;
		}
		var t = new T();
		t.SetGameObj(GameObject.Instantiate(obj), "");
		action?.Invoke(t);
		if (t.bCanBeAutoClosed)
			if (!loadedClasses.Contains(t))
				loadedClasses.Add(t);
		return t;
	}
	public static void LoadUI<T>(Action<T> action = null) where T : AHotBase, new()
	{
		UICommonWait.Show();
		var path = "UI/" + typeof(T).Name;
		UHotAssetBundleLoader.Instance.OnDownloadResources(() =>
		{
			UICommonWait.Hide();
			LoadClass<T>(path, action);
		}, path);
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
		foreach (var c in loadedClasses)
		{
			if (c.gameObj)
				GameObject.Destroy(c.gameObj);
		}
		loadedClasses.Clear();
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
	Dictionary<string, Action<UEventBase>> dRegisterredEvents;
	protected void RegisterEvent(string eventName, Action<UEventBase> action)
	{
		if (dRegisterredEvents == null)
			dRegisterredEvents = new Dictionary<string, Action<UEventBase>>();
		dRegisterredEvents.Add(eventName, action);
		UEventListener.Instance.OnRegisterEvent(eventName, action);
	}
	protected virtual void OnDestroy()
	{
		if (!UEventListener.hasInstance) return;
		if (dRegisterredEvents != null)
			foreach (var kv in dRegisterredEvents)
				UEventListener.Instance.OnUnregisterEvent(kv.Key, kv.Value);
	}

	protected void MoveTo(RectTransform transform, Vector3 to, float moveSeconds = 1, Action endAction = null)
	{
		var startTime = ApiDateTime.Now;
		var rawp = transform.anchoredPosition;
		addUpdateAction(() =>
		{
			transform.anchoredPosition = Vector3.Lerp(rawp, to, (float)(ApiDateTime.Now - startTime).TotalSeconds / moveSeconds);

			var bend = (ApiDateTime.Now - startTime).TotalSeconds > moveSeconds;
			if (bend)
			{
				transform.anchoredPosition = to;

				endAction?.Invoke();
			}
			return bend;
		});
	}
	protected void MoveTo(Transform transform, Vector3 to, float moveSeconds = 1, Space space = Space.World, Action endAction = null)
	{
		var startTime = ApiDateTime.Now;
		var rawp = space == Space.World ? transform.position : transform.localPosition;
		addUpdateAction(() =>
		{
			if (space == Space.World)
				transform.position = Vector3.Lerp(rawp, to, (float)(ApiDateTime.Now - startTime).TotalSeconds / moveSeconds);
			else
				transform.localPosition = Vector3.Lerp(rawp, to, (float)(ApiDateTime.Now - startTime).TotalSeconds / moveSeconds);

			var bend = (ApiDateTime.Now - startTime).TotalSeconds > moveSeconds;
			if (bend)
			{
				if (space == Space.World)
					transform.position = to;
				else
					transform.localPosition = to;

				endAction?.Invoke();
			}
			return bend;
		});
	}

	protected void OnSendSDKMessage(string v1, string v2, object arg = null)
	{
		var obj = GameObject.Find(v1);
		if (arg != null)
			obj.SendMessage(v2, arg, SendMessageOptions.RequireReceiver);
		else
			obj.SendMessage(v2, SendMessageOptions.RequireReceiver);
	}

	protected void RegisterReturnButton()
	{
		var btnReturn = FindWidget<Button>("btnReturn");
		if (btnReturn == null) return;
		btnReturn.onClick.AddListener(() =>
		{
			OnReturn();
		});
	}
	protected virtual void OnReturn()
	{
		OnUnloadThis();

		LoadUI<UIMain>();
	}
}
