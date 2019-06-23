using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine.UI;
using UnityEngine;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

public class GameSchulte : AHotBase
{
    protected int cellCount = 9;
    protected override void InitComponents()
    {
        var BestTime = FindWidget<Text>("BestTime");
        BestTime.text = "";
        var fBestTime = 0d;
        Action doReorder = null;
        Action actionRefreshBestTime = () =>
        {
            BestTime.text = "Best Time:" + fBestTime.ToString("f2");
        };

        var Time = FindWidget<Text>("Time");
        Time.text = "";
        var btnRestart = FindWidget<Button>("btnRestart");
        btnRestart.gameObject.SetActive(false);
        btnRestart.onClick.AddListener(() =>
        {
            doReorder();
        });
        var btnReturn = FindWidget<Button>("btnReturn");
        btnReturn.onClick.AddListener(() =>
        {
            UnloadThis();
            LoadAnotherUI<UIMain>();
        });

        var CurCount = 0;
        var Contents = FindWidget<GridLayoutGroup>("Contents");
        var cell = FindWidget<RawImage>(Contents.transform, "Schulte");
        cell.gameObject.SetActive(false);
        var lRawPos = new Vector3[cellCount];
        var lCells = new GameObject[cellCount];
        DateTime startTime;
        for (var i = 0; i < cellCount; i++)
        {
            GameObject c = null;
            if (lCells[i] == null)
            {
                c = GameObject.Instantiate(cell.gameObject, cell.transform.parent);
                c.gameObject.SetActive(true);
                var text = FindWidget<Text>(c.transform, "Name");
                text.raycastTarget = false;
                text.text = (i + 1).ToString();
                var idx = i;
                var image = c.GetComponent<RawImage>();
                var hotdrag = RegistDragFunc(image, null, () =>
                {
                    Debug.Log("drag");
                    if (idx != CurCount)
                    {
                        return;
                    }
                    CurCount++;
                    image.color = Color.grey;
                    text.color = Color.grey;
                    if (CurCount == 1)
                    {
                        startTime = DateTime.Now;
                        addUpdateAction(() =>
                        {
                            var t = (DateTime.Now - startTime).TotalSeconds;
                            Time.text = "Time:" + t.ToString("f2");
                            if (CurCount == cellCount)
                            {
                                if (t < fBestTime || fBestTime == 0)
                                {
                                    fBestTime = t;
                                    actionRefreshBestTime();
                                    UWebSender.Instance.OnRequest(Utils.BaseURL + "gameschulttime", "username="
                                        + UILogin.CachedUsername + "&token=" + UILogin.token + "&time=" + fBestTime
                                         , (res) =>
                                         {
                                             var jres = (JObject)JsonConvert.DeserializeObject(res);
                                             if (jres["err"].ToString() != "0")
                                             {
                                                 UIAlert.Show("同步属性出错：" + jres["err"]);
                                             }
                                         }, (err) =>
                                         {
                                             UIAlert.Show("同步属性失败：" + err);
                                         });

                                }
                                btnRestart.gameObject.SetActive(true);
                                return true;
                            }
                            return false;
                        });
                    }
                }, () => { }, () => { }, true, () => { });
                hotdrag.disableDragImage = true;
                lCells[i] = c;
            }
            else
            {
                c = lCells[i];
            }
            c.SetActive(true);
        }

        DelayDoSth(() =>
        {
            for (var i = 0; i < cellCount; i++)
            {
                lRawPos[i] = lCells[i].transform.position;
            }
            UWebSender.Instance.OnRequest(Utils.BaseURL + "avatarsingleinfo", "username="
                + UILogin.CachedUsername + "&token=" + UILogin.token + "&infoname=scTm"
                , (res) =>
                {
                    var jres = (JObject)JsonConvert.DeserializeObject(res);
                    if (jres["err"].ToString() == "0")
                    {
                        if (jres.ContainsKey("scTm"))
                        {
                            var tm = typeParser.doubleParse(jres["scTm"].ToString());
                            if (tm > 0.01)
                            {
                                fBestTime = tm;
                                actionRefreshBestTime();
                            }
                        }
                    }

                    doReorder();
                }
                , (err) =>
                {
                    UIAlert.Show("请求属性失败：" + err);
                });

        }, 0.1f);

        List<int> lPos = new List<int>();
        System.Random rdm = new System.Random();
        doReorder = () =>
        {
            lPos.Clear();
            CurCount = 0;
            Contents.enabled = false;
            var idx = 0;
            do
            {
                idx = rdm.Next(cellCount);
                if (lPos.Contains(idx))
                {
                    continue;
                }
                var image = lCells[lPos.Count].GetComponent<RawImage>();
                image.transform.position = lRawPos[idx];
                image.color = Color.white;
                FindWidget<Text>(image.transform, "Name").color = Color.green;
                lPos.Add(idx);
                if (lPos.Count == cellCount)
                {
                    break;
                }
            } while (true);
        };
    }
}

