using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public static class UHotStaticFuncs
{
    public static List<Transform> FindAllChild(Transform tr, string childName)
    {
        List<Transform> lresult = new List<Transform>();
        for (int i = 0; i < tr.childCount; i++)
        {
            if (tr.GetChild(i).name == childName)
            {
                Transform t = tr.GetChild(i);
                if (t != null)
                {
                    lresult.Add(t);
                }
            }
            lresult.AddRange(FindAllChild(tr.GetChild(i), childName));
        }
        return lresult;
    }
    public static Transform FindChild(Transform tr, string childName)
    {
        for (int i = 0; i < tr.childCount; i++)
        {
            if (tr.GetChild(i).name == childName)
            {
                return tr.GetChild(i);
            }
        }
        for (int i = 0; i < tr.childCount; i++)
        {
            Transform t = FindChild(tr.GetChild(i), childName);
            if (t != null)
            {
                return t;
            }
        }
        return null;
    }

    public static T FindChildComponent<T>(Transform tr, string childName) where T : Component
    {
        Transform t = FindChild(tr, childName);
        if (t == null)
        {
            return null;
        }
        return t.GetComponent<T>();
    }

    public static void EmitButtonClick(Button button)
    {
        ExecuteEvents.Execute(button.gameObject, new PointerEventData(EventSystem.current), ExecuteEvents.submitHandler);
    }
}
