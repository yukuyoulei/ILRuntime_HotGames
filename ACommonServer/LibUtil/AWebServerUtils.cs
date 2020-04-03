#if !ENABLE_NETWORK
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Web;

public static class AWebServerUtils
{
    static Random rdm = new Random();
    static char[] startChars = new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'J', 'K', 'L', 'M', 'N', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z' };
    public static string GetEncryptCode(int length = 6, int ikeyCount = 2)
    {
        var result = "";
        int[] irdms = new int[ikeyCount];
        char[] astartChars = new char[ikeyCount];
        for (int i = 0; i < ikeyCount; i++)
        {
            irdms[i] = rdm.Next(startChars.Length);
            astartChars[i] = startChars[irdms[i]];
            result += astartChars[i];
        }
        for (int i = ikeyCount; i < length; i++)
        {
            irdms[i % ikeyCount] += (i + 1) * (i + 1);
            result += startChars[irdms[i % ikeyCount] % startChars.Length];
        }
        return result;
    }
    public static bool DetectEncryptCode(string scode, int ikeyCount = 2)
    {
        int[] irdms = new int[ikeyCount];
        char[] astartChars = new char[ikeyCount];
        for (int i = 0; i < ikeyCount; i++)
        {
            astartChars[i] = scode[i];
            irdms[i] = Array.IndexOf(startChars, astartChars[i]);
        }
        for (int i = ikeyCount; i < scode.Length; i += ikeyCount)
        {
            irdms[i % ikeyCount] += (i + 1) * (i + 1);
            if (startChars[irdms[i % ikeyCount] % startChars.Length] != scode[i])
            {
                return false;
            }
        }
        return true;
    }

    public static string OnWebRequestPost(string url, string body, bool useSSL = false, Dictionary<string, string> addHeaders = null)
    {
        HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
        request.Proxy = null;
        request.Credentials = CredentialCache.DefaultCredentials;
        if (addHeaders != null)
        {
            foreach (var h in addHeaders)
            {
                request.Headers.Add(h.Key, h.Value);
            }
        }

        if (useSSL)
        {
            ServicePointManager.ServerCertificateValidationCallback += new System.Net.Security.RemoteCertificateValidationCallback(ValidateServerCertificate);
            request.ProtocolVersion = HttpVersion.Version10;
        }
        request.ContentType = "application/x-www-form-urlencoded;charset=utf-8";
        byte[] bs = Encoding.UTF8.GetBytes(body);
        request.Method = "POST";
        using (Stream reqStream = request.GetRequestStream())
        {
            reqStream.Write(bs, 0, bs.Length);
        }
        HttpWebResponse response = (HttpWebResponse)request.GetResponse();
        Stream dataStream = response.GetResponseStream();
        StreamReader reader = new StreamReader(dataStream);
        return reader.ReadToEnd();
    }
    public static bool ValidateServerCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
    {
        return true;
    }
    public static string ToJsonArray(params List<string>[] aparam)
    {
        var l = new List<string>();
        foreach (var p in aparam)
        {
            l.Add(ToJson(p.ToArray()));
        }
        return ToJsonArray(l.ToArray());
    }
    public static string ToJsonArray(params string[] aparam)
    {
        return "[" + string.Join(",", aparam) + "]";
    }
    public static string ToJson(params string[] aparam)
    {
        if (aparam.Length == 0)
        {
            return "";
        }
        if (aparam.Length % 2 != 0)
        {
            return "invalid param count";
        }
        var result = "{";
        for (int i = 0; i < aparam.Length; i += 2)
        {
            if (i > 0)
            {
                result += ",";
            }
            result += "\"" + aparam[i] + "\":\"" + aparam[i + 1] + "\"";
        }
        result += "}";
        return result.Replace("\"{", "{").Replace("}\"", "}");
    }
    public static string OnGetJsonError(params string[] otherParams)
    {
        var result = "";
        if (otherParams.Length == 0)
        {
            result = ToJson("err", "0");
        }
        else if (otherParams.Length == 1)
        {
            result = ToJson("err", otherParams[0]);
        }
        else
        {
            var ps = new List<string>(new string[] { "err", "0" });
            ps.AddRange(otherParams);
            result = ToJson(ps.ToArray());
        }
        return result.Replace("\"[", "[").Replace("]\"", "]");
    }
}
#endif