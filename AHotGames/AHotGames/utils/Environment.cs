using UnityEngine;

public static class Environment
{
    public static bool IsEditor
    {
        get
        {
            return Application.platform == RuntimePlatform.WindowsEditor;
        }
    }
    private static bool _UseAB = false;
    public static bool UseAB
    {
        get
        {
            if (!IsEditor)
            {
                return true;
            }
            return _UseAB;
        }
        set
        {
            _UseAB = value;
        }
    }
}
