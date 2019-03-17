using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class AHotBase
{
    protected GameObject gameObj;
    public static AHotBase curGame;
    public static string strArg;
    protected bool bDestroying;
    public void SetGameObj(GameObject gameObj, string arg)
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
        strArg = arg;
        var r = this.gameObj.AddComponent<UEmitMessager>();
        curGame = this;
        r.gameBase = this;
        r.OnRegistAction((msg) =>
        {
            onReceiveMsg?.Invoke(msg);

            var amsg = msg.Split(':');
            switch (amsg[0])
            {
                case "op":
                    receiveOpMsg(amsg[1] == "1");
                    break;
            }
        });

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
                        if (u())
                        {
                            remove.Add(u);
                        }
                    }
                    foreach (var r in remove)
                    {
                        updateActions.Remove(r);
                    }
                }
            };
        }
    }
    protected void DelayDoSth(Action sth, float delay)
    {
        var n = DateTime.Now;
        addUpdateAction(() =>
        {
            if ((DateTime.Now - n).TotalSeconds > delay)
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
    public static void EmitMessage(string gameobjName, string message)
    {
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
    protected void LoadPrefab(string prefabName, Action<GameObject> action)
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
            drag.DontSyncDrag = DontSyncDrag;
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
        drag1.DontSyncDrag = DontSyncDrag;
        drag1.onDragDrop += dropDrag;
        return drag;
    }

    public static void LoadAnotherClass(string classname, string prefab = "", string arg = "")
    {
        var smsg = "load|" + classname + "|" + prefab + "|" + arg;
        Debug.Log("send message to unity:" + smsg);
        SendMessageToUnityReceiver(smsg);
    }
    public static void LoadAnother<T>() where T : AHotBase
    {
        LoadAnotherUI(typeof(T).Name);
    }
    public static void LoadAnotherUI(string uiname)
    {
        LoadAnotherClass(uiname, "UI/" + uiname + ".prefab");
    }
    protected void UnloadThis()
    {
        bDestroying = true;

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
        var n = DateTime.Now;
        addUpdateAction(() =>
        {
            if (operationList.Count == 0)
            {
                return true;
            }
            if ((DateTime.Now - n).TotalSeconds < operationList[0].delay)
            {
                return false;
            }
            operationList[0].action();
            operationList.RemoveAt(0);
            n = DateTime.Now;
            return false;
        });
    }
    struct OperationCell
    {
        public Action action;
        public float delay;
    }
}
