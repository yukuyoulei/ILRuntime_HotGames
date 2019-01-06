using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading;
using UnityEngine;
using UnityEngine.Networking;

public class UWebSender : MonoBehaviour
{
    private static UWebSender sinstance;
    public static UWebSender Instance
    {
        get
        {
            if (sinstance == null)
            {
                var obj = new GameObject("UWebSender");
                sinstance = obj.AddComponent<UWebSender>();
            }
            return sinstance;
        }
    }
    private static List<Action> callbacks = new List<Action>();
    private static object lockObj = new object();
    public void OnRequest(string url, string scontent, Action<string> actionResult, Action<string> actionFailed)
    {
        Debug.Log("request " + url + "?" + scontent);
        new Thread(new ThreadStart(() =>
        {
            try
            {
                byte[] bs = Encoding.ASCII.GetBytes(scontent);
                var req = (HttpWebRequest)HttpWebRequest.Create(url);
                req.Method = "POST";
                req.ContentType = "application/x-www-form-urlencoded";
                req.ContentLength = bs.Length;
                using (Stream reqStream = req.GetRequestStream())
                {
                    reqStream.Write(bs, 0, bs.Length);
                }
                using (WebResponse wr = req.GetResponse())
                {
                    var result = new StreamReader(wr.GetResponseStream(), Encoding.UTF8).ReadToEnd();
                    Debug.Log("web result " + result);
                    lock (lockObj)
                    {
                        callbacks.Add(() =>
                        {
                            if (actionResult != null)
                                actionResult(result);
                        });
                    }

                }
            }
            catch (Exception ex)
            {
                lock (lockObj)
                {
                    callbacks.Add(() =>
                    {
                        if (actionFailed != null)
                            actionFailed(ex.Message);
                    });
                }

            }
        }
        )).Start();
    }
    private void Start()
    {

    }
    private void Update()
    {
        if (callbacks.Count == 0)
        {
            return;
        }
        lock (lockObj)
        {
            foreach (var cb in callbacks)
            {
                //if (cb.Target != null)
                {
                    cb();
                }
            }
            callbacks.Clear();
        }
    }
}
