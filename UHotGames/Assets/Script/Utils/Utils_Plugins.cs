using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.IO;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

public static class Utils_Plugins
{
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
	public static bool DetectApp(string packageName)
	{
#if UNITY_ANDROID
		if (IsEditor)
		{
			return false;
		}
		AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
		AndroidJavaObject obj = jc.GetStatic<AndroidJavaObject>("currentActivity");
		AndroidJavaClass cavyJc = new AndroidJavaClass("unityPlugin.utils");
		return cavyJc.CallStatic<int>("DetectApp", obj, packageName)>0;
#endif
		return false;
	}
	public static void StartApp(string packageName)
	{
#if UNITY_ANDROID
		if (IsEditor)
		{
			return;
		}
		AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
		AndroidJavaObject obj = jc.GetStatic<AndroidJavaObject>("currentActivity");
		AndroidJavaClass cavyJc = new AndroidJavaClass("unityPlugin.utils");
		cavyJc.CallStatic("StartApp", obj, packageName);
#endif
	}
#if !UNITY_EDITOR && UNITY_IOS
	[DllImport("__Internal")]
	private extern static string GetBundleVersion();
	[DllImport("__Internal")]
	private extern static string GetVersionNumber();
#endif
	public static string Util_GetBundleVersion()
	{
#if UNITY_EDITOR
#if UNITY_IOS
		return PlayerSettings.iOS.buildNumber;
#elif UNITY_STANDALONE
		if (!Directory.Exists(Application.streamingAssetsPath))
		{
			Directory.CreateDirectory(Application.streamingAssetsPath);
		}
		if (File.Exists(Application.streamingAssetsPath + "/Version"))
		{
			return File.ReadAllText(Application.streamingAssetsPath + "/Version");
		}
		return PlayerSettings.bundleVersion;
#else
		return PlayerSettings.bundleVersion;
#endif
#elif UNITY_IOS && !UNITY_EDITOR
		return GetBundleVersion();
#elif UNITY_ANDROID
		AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
		AndroidJavaObject jo = jc.GetStatic<AndroidJavaObject>("currentActivity");
		string pkName = jo.Call<string>("getPackageName");
		string sversion =
			jo.Call<AndroidJavaObject>("getPackageManager")
				.Call<AndroidJavaObject>("getPackageInfo", pkName, 0)
				.Get<string>("versionName");
        Debug.Log("- -!!! 当前版本号:" + sversion);
		return sversion;
#else
		if (!Directory.Exists(Application.streamingAssetsPath))
		{
			Directory.CreateDirectory(Application.streamingAssetsPath);
		}
		if (File.Exists(Application.streamingAssetsPath + "/Version"))
		{
			return File.ReadAllText(Application.streamingAssetsPath + "/Version");
		}
		return "win";
		return Application.platform.ToString();
#endif
	}
	public static string Util_GetVersionNumber()
	{
#if UNITY_IOS && !UNITY_EDITOR
		return GetVersionNumber();
#else
		return "0";
#endif
	}

#if UNITY_STANDALONE
	public static Rect screenPosition;
	[DllImport("user32.dll")]
	static extern IntPtr SetWindowLong(IntPtr hwnd, int _nIndex, int dwNewLong);
	[DllImport("user32.dll")]
	static extern bool SetWindowPos(IntPtr hWnd, int hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);
	[DllImport("user32.dll")]
	static extern IntPtr GetForegroundWindow();

	const uint SWP_SHOWWINDOW = 0x0040;
	const int GWL_STYLE = -16;
	const int WS_BORDER = 1;
	const int WS_POPUP = 0x800000;
#endif
	static public void Util_SetWindowLong()
	{
#if UNITY_STANDALONE && !UNITY_EDITOR
		Utils_Plugins.screenPosition = new Rect(0, 0, Screen.width, Screen.height);
		SetWindowLong(GetForegroundWindow (), GWL_STYLE, WS_POPUP);//将网上的WS_BORDER替换成WS_POPUP  
        bool result = SetWindowPos (GetForegroundWindow (), 0, (int)screenPosition.x,(int)screenPosition.y, (int)screenPosition.width,(int) screenPosition.height, SWP_SHOWWINDOW);  
#endif
	}
}
