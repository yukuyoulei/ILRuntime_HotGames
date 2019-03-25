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
            LoadAnother<UIMain>();
        });

        var CurCount = 0;
        var Contents = FindWidget<GridLayoutGroup>("Contents");
        var cell = FindWidget<Button>(Contents.transform, "Schulte");
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
                var text = FindWidget<Text>(c.transform, "Name");
                text.text = (i + 1).ToString();
                var idx = i;
                var btn = c.GetComponent<Button>();
                btn.onClick.AddListener(() =>
                {
                    if (idx != CurCount)
                    {
                        return;
                    }
                    CurCount++;
                    btn.enabled = false;
                    btn.targetGraphic.color = Color.grey;
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
                });
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
                var btn = lCells[lPos.Count].GetComponent<Button>();
                btn.transform.position = lRawPos[idx];
                btn.targetGraphic.color = Color.white;
                btn.enabled = true;
                FindWidget<Text>(btn.transform, "Name").color = Color.green;
                lPos.Add(idx);
                if (lPos.Count == cellCount)
                {
                    break;
                }
            } while (true);
        };
    }
}

