using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
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
    HttpWebRequest req;
    public void OnRequest(string url, string scontent, Action<string> actionResult, Action<string> actionFailed)
    {
        try
        {
            byte[] bs = Encoding.ASCII.GetBytes(scontent);
            HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(url);
            req.Method = "POST";
            req.ContentType = "application/x-www-form-urlencoded";
            req.ContentLength = bs.Length;
            using (Stream reqStream = req.GetRequestStream())
            {
                reqStream.Write(bs, 0, bs.Length);
            }
            using (WebResponse wr = req.GetResponse())
            {
                actionResult(new StreamReader(wr.GetResponseStream(), Encoding.UTF8).ReadToEnd());
            }
        }
        catch (Exception ex)
        {
            actionFailed(ex.StackTrace);
        }
    }

    private IEnumerator Send(string url, string scontent, Action<string> actionResult, Action<string> actionFailed)
    {
        WWW www = null;
        if (string.IsNullOrEmpty(scontent))
        {
            www = new WWW(url);
        }
        else
        {
            www = new WWW(url, System.Text.Encoding.Default.GetBytes(scontent));
        }
        yield return www;
        if (string.IsNullOrEmpty(www.error))
        {
            var text = www.text;
            actionResult(text);
        }
        else
        {
            actionFailed(www.error);
        }
        www.Dispose();
    }
}
