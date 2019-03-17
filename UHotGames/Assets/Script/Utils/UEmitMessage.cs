using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class UEmitMessage : MonoBehaviour
{
    public void EmitMessage(string msg)
    {
        Debug.Log("Receive message " + msg);
        var amsg = msg.Split('|');
        if (amsg[0] == "load")
        {
            var obj = UAssetBundleDownloader.Instance.OnLoadAsset<GameObject>(amsg[2]);
            if (!obj)
            {
                obj = new GameObject(amsg[1]);
            }
            else
            {
                obj = Instantiate(obj);
            }
            ILRuntimeHandler.Instance.OnLoadClass(amsg[1], obj);
        }
        else if (amsg[0] == "loadprefab")
        {
            var obj = UAssetBundleDownloader.Instance.OnLoadAsset<GameObject>(amsg[1]);
            if (!obj)
            {
                obj = new GameObject(amsg[1]);
            }
            else
            {
                obj = Instantiate(obj);
            }
            ILRuntimeHandler.Instance.EmitGameObject(amsg[1], obj, amsg.Length > 2 ? amsg[2] : "");
        }
        else if (amsg[0] == "unloadall")
        {
            ILRuntimeHandler.Instance.OnUnloadAllClasses();
        }
    }
}
