using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public enum EUser
{
	EGuest,
	ECommonUser,
	ESchoolUser,
}
public static class UStaticFuncs
{
	public static EUser eUser { get; set; }

	public static bool IsStudent { get; set; }
	public static bool IsTeacher { get; set; }
	public static bool IsInitializing = false;
	public static bool IsPlaying = false;
	public static bool IsGameEditor { get; set; }
	public static bool IsOnline { get; set; }
	public static string operationingUser { get; set; }
	public static List<string> lStudents { get; set; }
	public static bool IsEditor
	{
		get
		{
#if UNITY_EDITOR
			return true;
#else
			return false;
#endif
		}
	}
	public static string finalCodes = "";

	private static Dictionary<string, string> dAllOps = new Dictionary<string, string>();
	public static void SetOp(string username, string ops)
	{
		if (dAllOps.ContainsKey(username))
		{
			dAllOps[username] = ops;
		}
		else
		{
			dAllOps.Add(username, ops);
		}
	}
	public static string CurOpUsername { get; set; }

	public static Transform FindChild(Transform tr, string childName)
	{
		for (int i = 0; i < tr.childCount; i++)
		{
			if (tr.GetChild(i).name == childName)
			{
				return tr.GetChild(i);
			}
			else
			{
				Transform t = FindChild(tr.GetChild(i), childName);
				if (t != null)
				{
					return t;
				}
			}
		}
		return null;
	}

	internal static object GetMachineInfo()
	{
		return SystemInfo.deviceType + "/" + SystemInfo.deviceName + "/" + SystemInfo.operatingSystem;
	}

	public static List<Transform> FindAllChild(Transform tr, string childName)
	{
		List<Transform> lresult = new List<Transform>();
		for (int i = 0; i < tr.childCount; i++)
		{
			if (tr.GetChild(i).name == childName)
			{
				Transform t = tr.GetChild(i);
				if (t != null)
				{
					lresult.Add(t);
				}
			}
			lresult.AddRange(FindAllChild(tr.GetChild(i), childName));
		}
		return lresult;
	}
	public static T FindChildComponent<T>(Transform tr, string childName) where T : Component
	{
		Transform t = FindChild(tr, childName);
		if (t == null)
		{
			return null;
		}
		var cn = t.GetComponent<T>();
		if (cn == null)
		{
			//AOutput.LogError("FindChildComponent cannot find " + childName);
		}
		return cn;
	}
	public static string GetStreamAssetPath()
	{
#if UNITY_EDITOR
		{
			return "file:///" + Application.dataPath + "/StreamingAssets/";
		}
#elif UNITY_ANDROID
		{
			return "jar:file://" + Application.dataPath + "!/assets/";
		}
#elif UNITY_IOS
		{
			return "file://" + Application.streamingAssetsPath + "/";
		}
#else
		{
			return "file:///" + Application.dataPath + "/StreamingAssets/";
		}
#endif
	}
	public static string GetPersistentDataPath()
	{
#if UNITY_IOS
		return Application.persistentDataPath;
#else
		return Application.dataPath;
#endif
	}
	public static byte[] GetBytesFromServer(byte[] data)
	{
		if (data == null)
		{
			return null;
		}
		SymmetricAlgorithm mCSP = new DESCryptoServiceProvider();
		byte[] byt2 = Convert.FromBase64String("tkGGRmBxrvz=");
		mCSP.Key = byt2;
		byte[] byt3 = Convert.FromBase64String("Kl7ZitM1dvm=");
		mCSP.IV = byt3;

		ICryptoTransform ct = mCSP.CreateDecryptor(mCSP.Key, mCSP.IV);

		MemoryStream ms = new MemoryStream();
		CryptoStream cs = new CryptoStream(ms, ct, CryptoStreamMode.Write);
		cs.Write(data, 0, data.Length);
		cs.FlushFinalBlock();
		cs.Close();
		mCSP.Clear();
		return ms.ToArray();
	}

	internal static string TimeFormat(double totalSeconds)
	{
		if (totalSeconds < 10)
		{
			return "00:00:0" + (int)totalSeconds;
		}
		if (totalSeconds < 60)
		{
			return "00:00:" + (int)totalSeconds;
		}
		int minute = (int)totalSeconds / 60;
		int second = (int)totalSeconds % 60;
		if (minute < 10)
		{
			return "00:0" + minute + ":" + (second >= 10 ? "" + second : "0" + second);
		}
		else if (minute < 60)
		{
			return "00:" + minute + ":" + (second >= 10 ? "" + second : "0" + second);
		}
		else
		{
			int hour = minute / 60;
			minute = minute % 60;
			if (hour < 10)
			{
				return "0" + hour + ":" + (minute >= 10 ? "" + minute : "0" + minute) + ":" + (second >= 10 ? "" + second : "0" + second);
			}
			else
			{
				return hour + ":" + (minute >= 10 ? "" + minute : "0" + minute) + ":" + (second >= 10 ? "" + second : "0" + second);
			}
		}
	}

	public static float GetEffectLifeTime(Transform effect)
	{
		float lifeTime = 0;
		ParticleSystem[] ps = effect.GetComponentsInChildren<ParticleSystem>();
		for (int i = 0; i < ps.Length; i++)
		{
			if (lifeTime < ps[i].main.duration)
			{
				lifeTime = ps[i].main.duration;
			}
		}
		ParticleSystem p = effect.GetComponent<ParticleSystem>();
		if (p != null)
		{
			if (lifeTime < p.main.duration)
			{
				lifeTime = p.main.duration;
			}
		}
		return lifeTime;
	}

	public static string GetPathFileName(string filePath)
	{
		filePath = filePath.Replace("\\", "/");
		if (filePath.Contains("/"))
		{
			string[] ap = filePath.Split('/');
			return ap[ap.Length - 1];
		}
		return filePath;
	}
	public static string GetPathFilePath(string filePath)
	{
		filePath = filePath.Replace("\\", "/");
		if (filePath.Contains("/"))
		{
			string[] ap = filePath.Split('/');
			return string.Join("/", ap, 0, ap.Length - 1);
		}
		return "";
	}
	public static string NumberToStringWithCount(int i, int count)
	{
		string s = i.ToString();
		for (int j = 0; j < count; j++)
		{
			if (s.Length > j)
			{
				continue;
			}
			s = "0" + s;
		}
		return s;
	}

	private static Texture2D _scaled;
	public static Texture2D ScaleTexture(Texture2D source, int targetWidth, int targetHeight)
	{
		if (_scaled == null || _scaled.width != targetWidth || _scaled.height != targetHeight)
		{
			if (_scaled != null)
			{
				GameObject.Destroy(_scaled);
			}
			_scaled = new Texture2D(targetWidth, targetHeight, TextureFormat.RGB24, false);
		}
		for (int i = 0; i < _scaled.height; ++i)
		{
			for (int j = 0; j < _scaled.width; ++j)
			{
				Color newColor = source.GetPixelBilinear((float)j / (float)_scaled.width, (float)i / (float)_scaled.height);
				_scaled.SetPixel(j, i, newColor);
			}
		}
		_scaled.Apply();
		return _scaled;
	}
	public static int JPGQuality = 85;
	public static int thumbSize = 64;

	static public int ColorToInt(Color c)
	{
		int retVal = 0;
		retVal |= (int)(c.r * 255f) << 16;
		retVal |= (int)(c.g * 255f) << 8;
		retVal |= (int)(c.b * 255f);
		return retVal;
	}
	static public Color IntToColor(int retVal)
	{
		Color c = new Color();
		c.r = (retVal >> 16) / 255f;
		c.g = (retVal >> 8 & 0xff) / 255f;
		c.b = (retVal & 0xff) / 255f;
		c.a = 1;
		return c;
	}

	public static T[] FromJson<T>(string json)
	{
		if (!json.StartsWith("{"))
		{
			json = "{\"Items\":" + json + "}";
		}
		Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(json);
		return wrapper.Items;
	}

	public static string ToJson<T>(T[] array)
	{
		Wrapper<T> wrapper = new Wrapper<T>();
		wrapper.Items = array;
		return JsonUtility.ToJson(wrapper);
	}

	public static string ToJson<T>(T[] array, bool prettyPrint)
	{
		Wrapper<T> wrapper = new Wrapper<T>();
		wrapper.Items = array;
		return JsonUtility.ToJson(wrapper, prettyPrint);
	}

	[Serializable]
	private class Wrapper<T>
	{
		public T[] Items;
	}

	private static Dictionary<string, List<GameObject>> dCaches = new Dictionary<string, List<GameObject>>();
	public static void ReturnToCachePool(GameObject obj, string cacheName = "")
	{
		var name = cacheName;
		if (string.IsNullOrEmpty(name))
		{
			name = obj.name;
			if (name.Contains("(Clone)"))
			{
				name = name.Replace("(Clone)", "");
			}
		}
		if (!dCaches.ContainsKey(name))
		{
			dCaches.Add(name, new List<GameObject>());
		}
		obj.SetActive(false);
		dCaches[name].Add(obj);
	}
	public static GameObject FetchFromCachePool(string name)
	{
		if (!dCaches.ContainsKey(name) || dCaches[name].Count == 0)
		{
			return null;
		}
		var go = dCaches[name][0];
		dCaches[name].RemoveAt(0);
		return go;
	}
	static System.Random rdm = new System.Random();
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
	public static bool bDirectEnter
	{
		get
		{
			return PlayerPrefs.GetInt("de") == 1;
		}
		set
		{
			PlayerPrefs.SetInt("de", value ? 1 : 0);
		}
	}
	public static DateTime FormatFromString(string str)
	{
		DateTime dt = new DateTime();
		try
		{
			var astr = str.Split(new char[] { ':', ' ', '/', '-' }, StringSplitOptions.RemoveEmptyEntries);
			if (astr.Length == 2)
			{
				dt = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, int.Parse(astr[0]), int.Parse(astr[1]), 0);
			}
			else if (astr.Length == 5)
			{
				dt = new DateTime(int.Parse(astr[0]), int.Parse(astr[1]), int.Parse(astr[2]), int.Parse(astr[3]), int.Parse(astr[4]), 0);
			}
			else if (astr.Length == 3)
			{
				dt = new DateTime(int.Parse(astr[0]), int.Parse(astr[1]), int.Parse(astr[2]));
			}
		}
		catch
		{
			throw new Exception("只支持输入\"yyyy/mm/dd hh:mm\"和\"hh:mm\"两种格式的时间。");
		}
		return dt;
	}

	public static string ConfigSaveDir
	{
		get
		{
			if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.OSXEditor)
				return new System.IO.DirectoryInfo(Application.dataPath).FullName + "/../AB/RemoteResources/";
			else if (Application.platform == RuntimePlatform.WindowsPlayer)
				return new System.IO.DirectoryInfo(Application.dataPath).FullName + "/../../AB/" + GetPlatformFolder(Application.platform) + "/";
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

	public static void ScalePoint(Texture2D tex, int newWidth, int newHeight)
	{
		ThreadedScale(tex, newWidth, newHeight, false);
	}

	public static void ScaleBilinear(Texture2D tex, int newWidth, int newHeight)
	{
		ThreadedScale(tex, newWidth, newHeight, true);
	}

	private static void ThreadedScale(Texture2D tex, int newWidth, int newHeight, bool useBilinear)
	{
		texColors = tex.GetPixels();
		newColors = new Color[newWidth * newHeight];
		if (useBilinear)
		{
			ratioX = 1.0f / ((float)newWidth / (tex.width - 1));
			ratioY = 1.0f / ((float)newHeight / (tex.height - 1));
		}
		else
		{
			ratioX = ((float)tex.width) / newWidth;
			ratioY = ((float)tex.height) / newHeight;
		}
		w = tex.width;
		w2 = newWidth;
		var cores = Mathf.Min(SystemInfo.processorCount, newHeight);
		var slice = newHeight / cores;

		finishCount = 0;
		if (mutex == null)
		{
			mutex = new Mutex(false);
		}
		if (cores > 1)
		{
			int i = 0;
			ThreadData threadData;
			for (i = 0; i < cores - 1; i++)
			{
				threadData = new ThreadData(slice * i, slice * (i + 1));
				ParameterizedThreadStart ts = useBilinear ? new ParameterizedThreadStart(BilinearScale) : new ParameterizedThreadStart(PointScale);
				Thread thread = new Thread(ts);
				thread.Start(threadData);
			}
			threadData = new ThreadData(slice * i, newHeight);
			if (useBilinear)
			{
				BilinearScale(threadData);
			}
			else
			{
				PointScale(threadData);
			}
			while (finishCount < cores)
			{
				Thread.Sleep(1);
			}
		}
		else
		{
			ThreadData threadData = new ThreadData(0, newHeight);
			if (useBilinear)
			{
				BilinearScale(threadData);
			}
			else
			{
				PointScale(threadData);
			}
		}

		tex.Resize(newWidth, newHeight);
		tex.SetPixels(newColors);
		tex.Apply();
	}
	public class ThreadData
	{
		public int start;
		public int end;
		public ThreadData(int s, int e)
		{
			start = s;
			end = e;
		}
	}
	private static Color[] texColors;
	private static Color[] newColors;
	private static int w;
	private static float ratioX;
	private static float ratioY;
	private static int w2;
	private static int finishCount;
	private static System.Threading.Mutex mutex;
	public static void BilinearScale(System.Object obj)
	{
		ThreadData threadData = (ThreadData)obj;
		for (var y = threadData.start; y < threadData.end; y++)
		{
			int yFloor = (int)Mathf.Floor(y * ratioY);
			var y1 = yFloor * w;
			var y2 = (yFloor + 1) * w;
			var yw = y * w2;

			for (var x = 0; x < w2; x++)
			{
				int xFloor = (int)Mathf.Floor(x * ratioX);
				var xLerp = x * ratioX - xFloor;
				newColors[yw + x] = ColorLerpUnclamped(ColorLerpUnclamped(texColors[y1 + xFloor], texColors[y1 + xFloor + 1], xLerp),
													   ColorLerpUnclamped(texColors[y2 + xFloor], texColors[y2 + xFloor + 1], xLerp),
													   y * ratioY - yFloor);
			}
		}

		mutex.WaitOne();
		finishCount++;
		mutex.ReleaseMutex();
	}

	public static void PointScale(System.Object obj)
	{
		ThreadData threadData = (ThreadData)obj;
		for (var y = threadData.start; y < threadData.end; y++)
		{
			var thisY = (int)(ratioY * y) * w;
			var yw = y * w2;
			for (var x = 0; x < w2; x++)
			{
				newColors[yw + x] = texColors[(int)(thisY + ratioX * x)];
			}
		}

		mutex.WaitOne();
		finishCount++;
		mutex.ReleaseMutex();
	}

	private static Color ColorLerpUnclamped(Color c1, Color c2, float value)
	{
		return new Color(c1.r + (c2.r - c1.r) * value,
						  c1.g + (c2.g - c1.g) * value,
						  c1.b + (c2.b - c1.b) * value,
						  c1.a + (c2.a - c1.a) * value);
	}

	public static void EmitButtonClick(Button button)
	{
		ExecuteEvents.Execute(button.gameObject, new PointerEventData(EventSystem.current), ExecuteEvents.submitHandler);
	}

	public static byte[] GZipCompress(byte[] ts)
	{
		byte[] compressedbuffer = null;
		MemoryStream ms = new MemoryStream();
		using (System.IO.Compression.GZipStream zs = new System.IO.Compression.GZipStream(ms, System.IO.Compression.CompressionMode.Compress, true))
		{
			zs.Write(ts, 0, ts.Length);
		}
		compressedbuffer = ms.ToArray();
		return compressedbuffer;
	}
	public static byte[] GZipDecompress(byte[] ts)
	{
		MemoryStream srcMs = new MemoryStream(ts);
		System.IO.Compression.GZipStream zipStream = new System.IO.Compression.GZipStream(srcMs, System.IO.Compression.CompressionMode.Decompress);
		MemoryStream ms = new MemoryStream();
		byte[] bytes = new byte[40960];
		int n;
		while ((n = zipStream.Read(bytes, 0, bytes.Length)) > 0)
		{
			ms.Write(bytes, 0, n);
		}
		zipStream.Close();
		return ms.ToArray();
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
}

