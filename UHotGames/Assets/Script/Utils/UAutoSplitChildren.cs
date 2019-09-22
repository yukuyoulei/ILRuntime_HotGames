using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[ExecuteInEditMode]
public class UAutoSplitChildren : MonoBehaviour
{
    public int ColumnCount;
    public Vector2 size;
    private Dictionary<int, List<RectTransform>> dChidren = new Dictionary<int, List<RectTransform>>();
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
        {
            if (bRefreshing)
            {
                return;
            }
            bRefreshing = true;
            StartCoroutine(DelayRefresh());
        }
    }
    IEnumerator DelayRefresh()
    {
        yield return new WaitForSeconds(0.5f);

        bRefreshing = false;
        Refresh();
    }
    bool bRefreshing;
    private void Start()
    {
        Refresh();
    }

    private void Refresh()
    {
        lastChildCount = transform.childCount;
        dChidren.Clear();

        for (var i = 0; i < ColumnCount; i++)
        {
            dChidren.Add(i, new List<RectTransform>());
        }
        for (var i = 0; i < transform.childCount; i++)
        {
            var tr = transform.GetChild(i) as RectTransform;
            dChidren[i % ColumnCount].Add(tr);
            var p = tr.anchoredPosition;
            p.x = (transform as RectTransform).sizeDelta.x / ColumnCount * (i % ColumnCount) + size.x;
            tr.anchoredPosition = p;
        }
        foreach (var kv in dChidren)
        {
            RectTransform lastTr = null;
            foreach (var v in kv.Value)
            {
                var p = v.anchoredPosition;
                if (lastTr == null)
                {
                    p.y = 0;
                }
                else
                {
                    p.y = lastTr.anchoredPosition.y - (lastTr as RectTransform).sizeDelta.y;
                }
                p.y -= size.y;
                lastTr = v;
                v.anchoredPosition = p;
            }

        }
    }
}
