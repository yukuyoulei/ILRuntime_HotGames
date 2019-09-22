using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class UAutoStrechHeight : MonoBehaviour
{
    RectTransform recttransform;
    void Awake()
    {
        recttransform = transform as RectTransform;
    }
    private void Start()
    {
        Refresh();
        lastChildCount = 0;
    }
    private int lastChildCount;
    private int count;
    void LateUpdate()
    {
        //#if !UNITY_EDITOR
        count++;
        if (count < 10)
        {
            return;
        }
        count = 0;

        //#endif
        Refresh();
    }
    void Refresh()
    {
        lastChildCount = transform.childCount;
        var iheight = 0f;
        for (var i = 0; i < transform.childCount; i++)
        {
            var tr = transform.GetChild(i);
            var y = (tr as RectTransform).sizeDelta.y + Mathf.Abs((tr as RectTransform).anchoredPosition.y);
            if (y > iheight)
            {
                iheight = y;
            }
        }
        var s = recttransform.sizeDelta;
        s.y = iheight;
        recttransform.sizeDelta = s;
    }
}
