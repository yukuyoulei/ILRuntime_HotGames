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
    Dictionary<int, Dictionary<int, RawImage>> dCells = new Dictionary<int, Dictionary<int, RawImage>>();
    Transform MapCellRoot;
    const int col = 14;
    const int row = 10;
    protected override void InitComponents()
    {
        MapCell = FindWidget<RawImage>("MapCell");
        MapCell.gameObject.SetActive(false);
        MapCellRoot = FindWidget<Transform>("MapCellRoot");
        for (var y = 0; y < row; y++)
        {
            for (var x = 0; x < col; x++)
            {
                if (!dCells.ContainsKey(x))
                {
                    dCells.Add(x, new Dictionary<int, RawImage>());
                }
                AddCell(x, y);
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
                if (!CheckMove(0, -1))
                {
                    return false;
                }
                WebSocketConnector.Instance.OnRemoteCall("Move", "up", OnMoveCb);
            }
            if (Input.GetKeyDown(KeyCode.W))
            {
                if (!CheckMove(0, 1))
                {
                    return false;
                }
                WebSocketConnector.Instance.OnRemoteCall("Move", "down", OnMoveCb);
            }
            return false;
        });
    }
    private void AddCell(int x, int y)
    {
        var cell = GameObject.Instantiate(MapCell.gameObject, MapCell.transform.parent).GetComponent<RawImage>();
        cell.name = x + "," + y;
        dCells[x].Add(y, cell);
        cell.transform.localPosition = new Vector3((x - col / 2) * MapCell.rectTransform.sizeDelta.x
            , (y - row / 2) * MapCell.rectTransform.sizeDelta.y);
        cell.gameObject.SetActive(true);
    }
    bool bSetRawPos = false;
    int rawX = 0, rawY = 0;
    int deltaX, deltaY;
    private void OnMoveCb(string obj)
    {
        if (!bSetRawPos)
        {
            bSetRawPos = true;
            rawX = UMRemoteAvatarData.data.MapX;
            rawY = UMRemoteAvatarData.data.MapY;
        }
        var jres = (JObject)JsonConvert.DeserializeObject(obj);
        if (jres["err"] != null && jres["err"].ToString() == "0")
        {
            var oldPos = new Vector2(UMRemoteAvatarData.data.MapX, UMRemoteAvatarData.data.MapY);
            UMRemoteAvatarData.data.OnFormat(jres);
            var newPos = new Vector2(UMRemoteAvatarData.data.MapX, UMRemoteAvatarData.data.MapY);
            UMUIMain.OnParamUpdate();

            deltaX += (int)(newPos.x - oldPos.x);
            deltaY += (int)(newPos.y - oldPos.y);
            for (var x = 0; x < col; x++)
            {
                if (!dCells.ContainsKey(deltaX + x))
                {
                    dCells.Add(deltaX + x, new Dictionary<int, RawImage>());
                }
                for (var y = 0; y < row; y++)
                {
                    if (!dCells[deltaX + x].ContainsKey(deltaY + y))
                    {
                        AddCell(deltaX + x, deltaY + y);
                    }
                }
            }
            DoMove(oldPos, newPos);
        }
        else
        {
            Debug.Log("move error:" + obj);
        }
    }

    bool isMoving;
    Vector2 startPoint;
    Vector2 endPoint;
    Vector3 startPos;
    DateTime startTime;
    private void DoMove(Vector2 oldPos, Vector2 newPos)
    {
        if (oldPos == newPos)
        {
            return;
        }

        endPoint = newPos;
        if (!isMoving)
        {
            startPoint = oldPos;
            startPos = MapCellRoot.transform.localPosition;
        }
        var n = DateTime.Now;
        startTime = n;
        if (!isMoving)
        {
            isMoving = true;
            var v = -(endPoint - startPoint) * MapCell.rectTransform.sizeDelta.x;
            var target = startPos + new Vector3(v.x, v.y);
            addUpdateAction(() =>
              {
                  if ((DateTime.Now - startTime).TotalSeconds > 1)
                  {
                      MapCellRoot.localPosition = target;
                      isMoving = false;
                      return true;
                  }
                  var delta = (DateTime.Now - startTime).TotalSeconds;
                  MapCellRoot.localPosition = Vector3.Lerp(startPos, target, (float)delta);
                  return false;
              });
        }
    }

    private bool CheckMove(int deltax, int deltay)
    {
        return localLastMoveTime <= ApiDateTime.SecondsFromBegin();
    }
}

