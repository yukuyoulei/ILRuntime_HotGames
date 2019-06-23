using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public static class Utils
{
    public static string BaseURL = "http://127.0.0.1/ab/";
    public static string WebURL = "http://127.0.0.1/w/";
    public static string WebSocketURL = "ws://127.0.0.1/ws/ws.enter?";

    public static String MD5Hash(string sInput)
    {
        MD5 md5Hasher = MD5.Create();
        byte[] data = md5Hasher.ComputeHash(Encoding.Default.GetBytes(sInput));
        StringBuilder sBuilder = new StringBuilder();
        for (int i = 0; i < data.Length; i++)
        {
            sBuilder.Append(data[i].ToString("x2"));
        }
        return sBuilder.ToString();
    }
    public static string GetMD5HashFromFile(string fileName)
    {
        try
        {
            var file = new System.IO.FileStream(fileName, System.IO.FileMode.Open);
            System.Security.Cryptography.MD5 md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
            byte[] retVal = md5.ComputeHash(file);
            file.Close();

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < retVal.Length; i++)
            {
                sb.Append(retVal[i].ToString("x2"));
            }
            return sb.ToString();
        }
        catch (Exception ex)
        {
            throw new Exception("GetMD5HashFromFile() fail,error:" + ex.Message);
        }
    }
    public static string ConfigSaveDir
    {
        get
        {
            if (Environment.IsEditor)
                return new DirectoryInfo(Application.dataPath).FullName + "/../RemoteResources/";
            else if (Application.platform == RuntimePlatform.WindowsPlayer)
                return new DirectoryInfo(Application.dataPath).FullName + "/../../AB/" + GetPlatformFolder(Application.platform) + "/";
            return Application.persistentDataPath + "/" + GetPlatformFolder(Application.platform) + "/";
        }
    }
    public static string GetPlatformFolder(RuntimePlatform target)
    {
        switch (target)
        {
            case RuntimePlatform.Android:
                return "Android";
            case RuntimePlatform.IPhonePlayer:
                return "IOS";
            case RuntimePlatform.WindowsPlayer:
                return "Windows";
            case RuntimePlatform.WebGLPlayer:
                return "WebGL";
            case RuntimePlatform.WindowsEditor:
                return "Windows";
            default:
                return "Windows";
        }
    }

    public static string GetAssetBundleName(string name)
    {
        var assetName = name;
        if (assetName.Contains("/"))
        {
            assetName = assetName.Substring(assetName.LastIndexOf("/") + 1);
        }
        if (assetName.Contains("\\"))
        {
            assetName = assetName.Substring(assetName.LastIndexOf("\\") + 1);
        }
        if (assetName.Contains("."))
        {
            assetName = assetName.Substring(0, assetName.LastIndexOf("."));
        }
        return assetName;
    }

    private static Dictionary<string, string> dErrors;
    public static string ErrorFormat(string error)
    {
        if (dErrors == null)
        {
            dErrors = new Dictionary<string, string>();
            dErrors.Add("2", "无效的token。");
            dErrors.Add("1", "账号密码错误。");
            dErrors.Add("-1", "系统错误。");
            dErrors.Add("-2", "重复的用户名。");
            dErrors.Add("-3", "无效的用户名长度。");
            dErrors.Add("-4", "无效的邮箱地址。");
        }
        if (dErrors.ContainsKey(error))
            return dErrors[error];
        return error;
    }
}

