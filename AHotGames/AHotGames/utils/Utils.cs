using System;
using System.Collections.Generic;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public static class Utils
{
	public static string BaseURL_Res { get { return PlayerPrefs.GetInt("USE_LOCAL_CDN") == 1 ? "http://127.0.0.1/hotgame/" : "https://yukuyoulei.github.io/ILRuntime_HotGames/ab1/"; } }
	public static string BaseURL_APIs { get { return PlayerPrefs.GetInt("USE_LOCAL_CDN") == 1 ? "http://127.0.0.1/hotgameapis/api/" : "http://www.fscoding.xyz/hotgameapis/api/"; } }
	public static string WebSocketURL { get { return PlayerPrefs.GetInt("USE_LOCAL_CDN") == 1 ? "ws://127.0.0.1/hotgameapis/ws/ws.enter?" : "ws://114.118.22.213/hotgameapis/ws/ws.enter?"; } }

	public static string MD5Hash(string sInput)
	{
		MD5 getmd5 = new MD5CryptoServiceProvider();
		byte[] targetStr = getmd5.ComputeHash(UnicodeEncoding.UTF8.GetBytes(sInput));
		var result = BitConverter.ToString(targetStr).Replace("-", "");
		return result;
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
			if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.OSXEditor)
				return new System.IO.DirectoryInfo(Application.dataPath).FullName + "/../AB/RemoteResources/";
			else if (Application.platform == RuntimePlatform.WindowsPlayer)
				return new System.IO.DirectoryInfo(Application.dataPath).FullName + "/../../AB/" + GetPlatformFolder() + "/";
			return Application.persistentDataPath + "/" + GetPlatformFolder() + "/";
		}
	}
	public static string TargetRuntimeInEditor = "";
	public static string GetPlatformFolder()
	{
		if (!string.IsNullOrEmpty(TargetRuntimeInEditor)) return TargetRuntimeInEditor;

		switch (Application.platform)
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

	public static void ToggleColor(Graphic g, bool bToggled)
	{
		g.color = bToggled ? Color.white : new Color(0.2f, 0.2f, 0.2f);
	}

	public static void SetPosition(Graphic g, float x)
	{
		var p = g.rectTransform.anchoredPosition;
		p.x = x;
		g.rectTransform.anchoredPosition = p;
	}
	public static void SetPosition(Graphic g, float x, float y)
	{
		var p = g.rectTransform.anchoredPosition;
		p.x = x;
		p.y = y;
		g.rectTransform.anchoredPosition = p;
	}

	private static AudioSource commonAudioSource;
	internal static void StopAudio(AudioClip recodingClip)
	{
		if (recodingClip == null)
		{
			return;
		}
		commonAudioSource.Stop();
	}
	internal static AudioSource PlayAudio(AudioClip recodingClip)
	{
		if (recodingClip == null)
		{
			return commonAudioSource;
		}
		if (commonAudioSource == null)
		{
			var obj = new GameObject("commonAudioSource");
			commonAudioSource = obj.AddComponent<AudioSource>();
		}
		else
		{
			commonAudioSource.Stop();
		}
		commonAudioSource.clip = recodingClip;
		commonAudioSource.Play();
		return commonAudioSource;
	}
	internal static void StopCurAudio()
	{
		if (commonAudioSource == null)
		{
			return;
		}
		commonAudioSource.Stop();
	}

	public static DateTime GetFormatedTime(string strTime)
	{
		if (string.IsNullOrEmpty(strTime))
		{
			return new DateTime();
		}
		var atime = strTime.Split(new char[] { '/', ' ', ':' }, StringSplitOptions.RemoveEmptyEntries);
		if (atime.Length == 6)
		{
			return new DateTime(typeParser.intParse(atime[0]), typeParser.intParse(atime[1]), typeParser.intParse(atime[2])
				, typeParser.intParse(atime[3]), typeParser.intParse(atime[4]), typeParser.intParse(atime[5]));
		}
		else if (atime.Length >= 3)
		{
			return new DateTime(typeParser.intParse(atime[0]), typeParser.intParse(atime[1]), typeParser.intParse(atime[2]));
		}
		return new DateTime();
	}

	public static string URLEncode(this string str)
	{
		return UnityWebRequest.EscapeURL(str);
	}
	public static string URLDecode(this string str)
	{
		return UnityWebRequest.UnEscapeURL(str);
	}

	public static string ToBase64String(this string str)
	{
		if (string.IsNullOrEmpty(str))
		{
			return str;
		}
		var bs = Encoding.UTF8.GetBytes(str);
		return Convert.ToBase64String(bs).Replace("+", ".").Replace("/", "_");
	}
	public static string FromBase64String(this string str)
	{
		if (string.IsNullOrEmpty(str))
		{
			return str;
		}
		try
		{
			var bs = Convert.FromBase64String(str.Replace(".", "+").Replace("_", "/"));
			return Encoding.UTF8.GetString(bs);
		}
		catch
		{
			return str;
		}
	}
	public static byte[] ConvertAudioClipToWav(AudioClip clip)
	{
		clip = TrimSilence(clip, 0);
		var samples = new float[clip.samples];

		clip.GetData(samples, 0);

		Int16[] intData = new Int16[samples.Length];
		//converting in 2 float[] steps to Int16[], //then Int16[] to Byte[]

		Byte[] bytesData = new Byte[samples.Length * 2];
		//bytesData array is twice the size of
		//dataSource array because a float converted in Int16 is 2 bytes.

		float rescaleFactor = 32767; //to convert float to Int16

		for (int i = 0; i < samples.Length; i++)
		{
			intData[i] = (short)(samples[i] * rescaleFactor);
			Byte[] byteArr = new Byte[2];
			byteArr = BitConverter.GetBytes(intData[i]);
			byteArr.CopyTo(bytesData, i * 2);
		}
		return bytesData;
	}
	public static AudioClip TrimSilence(AudioClip clip, float min = 0)
	{
		var samples = new float[clip.samples];

		clip.GetData(samples, 0);

		return TrimSilence(new List<float>(samples), min, clip.channels, clip.frequency);
	}

	public static AudioClip TrimSilence(List<float> samples, float min, int channels, int hz)
	{
		return TrimSilence(samples, min, channels, hz, false, false);
	}

	public static AudioClip TrimSilence(List<float> samples, float min, int channels, int hz, bool _3D, bool stream)
	{
		int i;

		for (i = 0; i < samples.Count; i++)
		{
			if (Mathf.Abs(samples[i]) > min)
			{
				break;
			}
		}

		if (samples.Count > i)
		{
			samples.RemoveRange(0, i);
		}

		for (i = samples.Count - 1; i > 0; i--)
		{
			if (Mathf.Abs(samples[i]) > min)
			{
				break;
			}
		}

		if (samples.Count > (samples.Count - i))
		{
			samples.RemoveRange(i, samples.Count - i);
		}

		var clip = AudioClip.Create("TempClip", samples.Count, channels, hz, stream);

		clip.SetData(samples.ToArray(), 0);

		return clip;
	}
	public static void Hide(this GameObject obj)
	{
		var gs = obj.GetComponentsInChildren<Graphic>();
		foreach (var g in gs)
		{
			g.enabled = false;
		}
	}
	public static void Show(this GameObject obj)
	{
		var gs = obj.GetComponentsInChildren<Graphic>();
		foreach (var g in gs)
		{
			g.enabled = true;
		}
	}
	public static string OnWebRequestPost(string url, string arguments, Dictionary<string, string> headers)
	{
		var req = (HttpWebRequest)HttpWebRequest.Create(url);
		req.Method = "POST";
		if (headers != null)
		{
			foreach (var kv in headers)
			{
				req.Headers.Add(kv.Key, kv.Value);
			}
		}
		byte[] bs = Encoding.UTF8.GetBytes(arguments);
		req.ContentLength = bs.Length;
		using (System.IO.Stream reqStream = req.GetRequestStream())
		{
			reqStream.Write(bs, 0, bs.Length);
		}
		using (WebResponse wr = req.GetResponse())
		{
			var result = new System.IO.StreamReader(wr.GetResponseStream(), Encoding.UTF8).ReadToEnd();
			return result;
		}
	}

	public static T LoadAsset<T>(string path) where T : UnityEngine.Object
	{
		if (!Environment.IsEditor)
			return null;
		var spath = Application.dataPath + "/RemoteResources/" + path;
		var dir = spath.Substring(0, spath.LastIndexOf("/"));
		var afiles = System.IO.Directory.EnumerateFiles(dir);
		foreach (var f in afiles)
		{
			if (f.Replace("\\", "/").Contains(path.Replace("\\", "/")))
			{
				spath = f.Replace(Application.dataPath, "Assets");
				break;
			}
		}
		return UnityEditor.AssetDatabase.LoadAssetAtPath<T>(spath);
	}
}

