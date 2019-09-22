using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(UICurve))]
public class UICurveTester : MonoBehaviour
{
    UICurve _curve;
    UICurve curve
    {
        get
        {
            if (_curve == null)
            {
                _curve = GetComponent<UICurve>();
            }
            return _curve;
        }
    }
    UICurveData curData;
    Vector2 lastPos;
    int _id;
    int id
    {
        get
        {
            return _id++;
        }
    }
    Canvas _rootCanvas;
    Canvas rootCanvas
    {
        get
        {
            if (_rootCanvas == null)
                _rootCanvas = gameObject.GetComponentInParent<Canvas>();
            return _rootCanvas;
        }
    }
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            curData = new UICurveData();
            curData.AddPos(Input.mousePosition / rootCanvas.scaleFactor);
            curData.color = Color.red;
            curve.AddCurveData(id, curData);
            lastPos = Input.mousePosition;
        }
        if (Input.GetMouseButton(0))
        {
            if (Vector2.Distance(Input.mousePosition, lastPos) > 0.1f)
            {
                curData.AddPos(Input.mousePosition / rootCanvas.scaleFactor);
                curve.SetAllDirty();
                lastPos = Input.mousePosition;
            }
        }
        if (Input.GetMouseButtonDown(1))
        {
            curve.DequeueData();
        }
    }
}
