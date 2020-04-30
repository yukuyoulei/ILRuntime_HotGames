using UnityEngine;

public static class Environment
{
	public static bool IsEditor
	{
		get
		{
			return Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.OSXEditor;
		}
	}
	public static bool UseAB
	{
		get
		{
			if (!PlayerPrefs.HasKey("UseAB"))
				return true;
			return PlayerPrefs.GetInt("UseAB") != 0;
		}
		set
		{
			PlayerPrefs.SetInt("UseAB", value ? 1 : 0);
		}
	}
	public static string BundleVersion = "";
}
