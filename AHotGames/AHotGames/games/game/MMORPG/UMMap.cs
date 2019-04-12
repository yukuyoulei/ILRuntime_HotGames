using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine.UI;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class UMMap : AHotBase
{
    long localLastMoveTime;
    RawImage MapCell;
    Dictionary<string, RawImage> dCells = new Dictionary<string, RawImage>();
    Transform AvatarPos;
    protected override void InitComponents()
    {
        var col = 14;
        var row = 10;
        MapCell = FindWidget<RawImage>("MapCell");
        MapCell.gameObject.SetActive(false);
        AvatarPos = FindWidget<Transform>("AvatarPos");
        for (var i = 0; i < row; i++)
        {
            for (var j = 0; j < col; j++)
            {
                var cell = GameObject.Instantiate(MapCell.gameObject, MapCell.transform.parent).GetComponent<RawImage>();
                cell.name = i + "," + j;
                dCells.Add(cell.name, cell);
                cell.transform.localPosition = new Vector3((j - col / 2) * MapCell.rectTransform.sizeDelta.x
                    , (i - row / 2) * MapCell.rectTransform.sizeDelta.y);
                cell.gameObject.SetActive(true);
            }
        }

        localLastMoveTime = UMRemoteAvatarData.data.LastMoveTime;
        addUpdateAction(() =>
        {
            if (Input.GetKeyDown(KeyCode.A))
            {
                if (!CheckMove(-1, 0))
                {
                    return false;
                }
                WebSocketConnector.Instance.OnRemoteCall("Move", "left", OnMoveCb);
            }
            if (Input.GetKeyDown(KeyCode.D))
            {
                if (!CheckMove(1, 0))
                {
                    return false;
                }
                WebSocketConnector.Instance.OnRemoteCall("Move", "right", OnMoveCb);
            }
            if (Input.GetKeyDown(KeyCode.S))
            {
                if (!CheckMove(0, 1))
                {
                    return false;
                }
                WebSocketConnector.Instance.OnRemoteCall("Move", "up", OnMoveCb);
            }
            if (Input.GetKeyDown(KeyCode.W))
            {
                if (!CheckMove(0, -1))
                {
                    return false;
                }
                WebSocketConnector.Instance.OnRemoteCall("Move", "down", OnMoveCb);
            }
            return false;
        });
    }
    private void OnMoveCb(string obj)
    {
        var jres = (JObject)JsonConvert.DeserializeObject(obj);
        if (jres["err"] != null && jres["err"].ToString() == "0")
        {
            var oldPos = new Vector2(UMRemoteAvatarData.data.MapX, UMRemoteAvatarData.data.MapY);
            UMRemoteAvatarData.data.OnFormat(jres);
            var newPos = new Vector2(UMRemoteAvatarData.data.MapX, UMRemoteAvatarData.data.MapY);
            UMUIMain.OnParamUpdate();

            DoMove(oldPos, newPos);
        }
        else
        {
            Debug.Log("move error:" + obj);
        }
    }

    private void DoMove(Vector2 oldPos, Vector2 newPos)
    {
        if (oldPos == newPos)
        {
            return;
        }
        Debug.Log("move from " + oldPos + " to " + newPos);
        var n = DateTime.Now;
        var start = n;
        addUpdateAction(() =>
        {
            if ((DateTime.Now - start).TotalSeconds > 1)
            {
                return true;
            }
            var delta = (DateTime.Now - n).TotalSeconds;
            n = DateTime.Now;
            AvatarPos.Translate((newPos - oldPos) * (float)delta * MapCell.rectTransform.sizeDelta.x);
            return false;
        });
    }

    private bool CheckMove(int deltax, int deltay)
    {
        if (deltax == 0)
        {
            var y = UMRemoteAvatarData.data.MapY + deltay;
            if (y > 5 || y < -5)
            {
                return false;
            }
        }
        else if (deltay == 0)
        {
            var x = UMRemoteAvatarData.data.MapX + deltax;
            if (x > 7 || x < -7)
            {
                return false;
            }
        }
        return localLastMoveTime <= ApiDateTime.SecondsFromBegin();
    }
}

