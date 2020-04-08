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
    private static bool _UseAB = false;
    public static bool UseAB
    {
        get
        {
            if (!IsEditor) return true;
            return _UseAB;
        }
        set
        {
            _UseAB = value;
        }
    }
	public static string BundleVersion = "";
}
