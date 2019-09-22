
using System;
using System.Collections.Generic;
using UnityEngine;

class AndroidStatusBar
{
    // Enums
    public enum States
    {
        Unknown,
        Visible,
        VisibleOverContent,
        TranslucentOverContent,
        Hidden,
    }

    // Constants
    private const uint DEFAULT_BACKGROUND_COLOR = 0xff000000;



#if UNITY_ANDROID
    // Original Android flags
    private const int VIEW_SYSTEM_UI_FLAG_VISIBLE = 0;                                        // Added in API 14 (Android 4.0.x): Status bar visible (the default)
    private const int VIEW_SYSTEM_UI_FLAG_LOW_PROFILE = 1;                                // Added in API 14 (Android 4.0.x): Low profile for games, book readers, and video players; the status bar and/or navigation icons are dimmed out (if visible)
    private const int VIEW_SYSTEM_UI_FLAG_HIDE_NAVIGATION = 2;                        // Added in API 14 (Android 4.0.x): Hides all navigation. Cleared when theres any user interaction.
    private const int VIEW_SYSTEM_UI_FLAG_FULLSCREEN = 4;                                // Added in API 16 (Android 4.1.x): Hides status bar. Does nothing in Unity (already hidden if "status bar hidden" is checked)
    private const int VIEW_SYSTEM_UI_FLAG_LAYOUT_STABLE = 256;                        // Added in API 16 (Android 4.1.x): ?
    private const int VIEW_SYSTEM_UI_FLAG_LAYOUT_HIDE_NAVIGATION = 512;        // Added in API 16 (Android 4.1.x): like HIDE_NAVIGATION, but for layouts? it causes the layout to be drawn like that, even if the whole view isn't (to avoid artifacts in animation)
    private const int VIEW_SYSTEM_UI_FLAG_LAYOUT_FULLSCREEN = 1024;                // Added in API 16 (Android 4.1.x): like FULLSCREEN, but for layouts? it causes the layout to be drawn like that, even if the whole view isn't (to avoid artifacts in animation)
    private const int VIEW_SYSTEM_UI_FLAG_IMMERSIVE = 2048;                                // Added in API 19 (Android 4.4): like HIDE_NAVIGATION, but interactive (it's a modifier for HIDE_NAVIGATION, needs to be used with it)
    private const int VIEW_SYSTEM_UI_FLAG_IMMERSIVE_STICKY = 4096;                // Added in API 19 (Android 4.4): tells that HIDE_NAVIGATION and FULSCREEN are interactive (also just a modifier)

    private static int WINDOW_FLAG_FULLSCREEN = 0x00000400;
    private static int WINDOW_FLAG_FORCE_NOT_FULLSCREEN = 0x00000800;
    private static int WINDOW_FLAG_LAYOUT_IN_SCREEN = 0x00000100;
    private static int WINDOW_FLAG_TRANSLUCENT_STATUS = 0x04000000;
    private static int WINDOW_FLAG_TRANSLUCENT_NAVIGATION = 0x08000000;
    private static int WINDOW_FLAG_DRAWS_SYSTEM_BAR_BACKGROUNDS = -2147483648; // 0x80000000; // Added in API 21 (Android 5.0): tells the Window is responsible for drawing the background for the system bars. If set, the system bars are drawn with a transparent background and the corresponding areas in this window are filled with the colors specified in getStatusBarColor() and getNavigationBarColor()

    // Current values
    private static int systemUiVisibilityValue;
    private static int flagsValue;


#endif

    // Properties
    private static States _statusBarState;
    //        private static States _navigationBarState;

    private static uint _statusBarColor = DEFAULT_BACKGROUND_COLOR;
    //        private static uint _navigationBarColor = DEFAULT_BACKGROUND_COLOR;

    private static bool _isStatusBarTranslucent; // Just so we know whether its translucent when hidden or not
                                                 //        private static bool _isNavigationBarTranslucent;

    private static bool _dimmed;
    // ================================================================================================================
    // INTERNAL INTERFACE ---------------------------------------------------------------------------------------------

    static AndroidStatusBar()
    {
        applyUIStates();
        applyUIColors();
    }

    private static void applyUIStates()
    {

#if UNITY_ANDROID && !UNITY_EDITOR

                int newFlagsValue = 0;
                int newSystemUiVisibilityValue = 0;

                // Apply dim values
                if (_dimmed) newSystemUiVisibilityValue |= VIEW_SYSTEM_UI_FLAG_LOW_PROFILE;

                // Apply color values
//                if (_navigationBarColor != DEFAULT_BACKGROUND_COLOR || _statusBarColor != DEFAULT_BACKGROUND_COLOR) newFlagsValue |= WINDOW_FLAG_DRAWS_SYSTEM_BAR_BACKGROUNDS;
                if (_statusBarColor != DEFAULT_BACKGROUND_COLOR) newFlagsValue |= WINDOW_FLAG_DRAWS_SYSTEM_BAR_BACKGROUNDS;

                // Apply status bar values
                switch (_statusBarState) {
                case States.Visible:
                _isStatusBarTranslucent = false;
                newFlagsValue |= WINDOW_FLAG_FORCE_NOT_FULLSCREEN;
                break;
                case States.VisibleOverContent:
                _isStatusBarTranslucent = false;
                newFlagsValue |= WINDOW_FLAG_FORCE_NOT_FULLSCREEN | WINDOW_FLAG_LAYOUT_IN_SCREEN;
                newSystemUiVisibilityValue |= VIEW_SYSTEM_UI_FLAG_LAYOUT_FULLSCREEN;
                break;
                case States.TranslucentOverContent:
                _isStatusBarTranslucent = true;
                newFlagsValue |= WINDOW_FLAG_FORCE_NOT_FULLSCREEN | WINDOW_FLAG_LAYOUT_IN_SCREEN | WINDOW_FLAG_TRANSLUCENT_STATUS;
                newSystemUiVisibilityValue |= VIEW_SYSTEM_UI_FLAG_LAYOUT_FULLSCREEN;
                break;
                case States.Hidden:
                newFlagsValue |= WINDOW_FLAG_FULLSCREEN | WINDOW_FLAG_LAYOUT_IN_SCREEN;
                if (_isStatusBarTranslucent) newFlagsValue |= WINDOW_FLAG_TRANSLUCENT_STATUS;
                break;
                }

                // Applies navigation values
                /*
                switch (_navigationBarState) {
                case States.Visible:
                _isNavigationBarTranslucent = false;
                newSystemUiVisibilityValue |= VIEW_SYSTEM_UI_FLAG_LAYOUT_STABLE;
                break;
                case States.VisibleOverContent:
                // TODO: Side effect: forces status bar over content if set to VISIBLE
                _isNavigationBarTranslucent = false;
                newSystemUiVisibilityValue |= VIEW_SYSTEM_UI_FLAG_LAYOUT_STABLE | VIEW_SYSTEM_UI_FLAG_LAYOUT_HIDE_NAVIGATION;
                break;
                case States.TranslucentOverContent:
                // TODO: Side effect: forces status bar over content if set to VISIBLE
                _isNavigationBarTranslucent = true;
                newFlagsValue |= WINDOW_FLAG_TRANSLUCENT_NAVIGATION;
                newSystemUiVisibilityValue |= VIEW_SYSTEM_UI_FLAG_LAYOUT_STABLE | VIEW_SYSTEM_UI_FLAG_LAYOUT_HIDE_NAVIGATION;
                break;
                case States.Hidden:
                newSystemUiVisibilityValue |= VIEW_SYSTEM_UI_FLAG_FULLSCREEN | VIEW_SYSTEM_UI_FLAG_HIDE_NAVIGATION | VIEW_SYSTEM_UI_FLAG_IMMERSIVE_STICKY;
                if (_isNavigationBarTranslucent) newFlagsValue |= WINDOW_FLAG_TRANSLUCENT_NAVIGATION;
                break;
                }
                */
                if (Screen.fullScreen) Screen.fullScreen = false;

                // Applies everything natively
                setFlags(newFlagsValue);
                setSystemUiVisibility(newSystemUiVisibilityValue);

#endif
    }

    private static void applyUIColors()
    {





#if UNITY_ANDROID && !UNITY_EDITOR
                runOnAndroidUiThread(applyUIColorsAndroidInThread);

#endif
    }



#if UNITY_ANDROID
    private static void runOnAndroidUiThread(Action target)
    {
        using (var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        {
            using (var activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity"))
            {
                activity.Call("runOnUiThread", new AndroidJavaRunnable(target));
            }
        }
    }

    private static void setSystemUiVisibility(int value)
    {
        if (systemUiVisibilityValue != value)
        {
            systemUiVisibilityValue = value;
            runOnAndroidUiThread(setSystemUiVisibilityInThread);
        }
    }

    private static void setSystemUiVisibilityInThread()
    {
        //Debug.Log("SYSTEM FLAGS: " + systemUiVisibilityValue);
        using (var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        {
            using (var activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity"))
            {
                using (var window = activity.Call<AndroidJavaObject>("getWindow"))
                {
                    using (var view = window.Call<AndroidJavaObject>("getDecorView"))
                    {
                        view.Call("setSystemUiVisibility", systemUiVisibilityValue);
                    }
                }
            }
        }
    }

    private static void setFlags(int value)
    {
        if (flagsValue != value)
        {
            flagsValue = value;
            runOnAndroidUiThread(setFlagsInThread);
        }
    }

    private static void setFlagsInThread()
    {
        //Debug.Log("FLAGS: " + flagsValue);
        using (var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        {
            using (var activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity"))
            {
                using (var window = activity.Call<AndroidJavaObject>("getWindow"))
                {
                    window.Call("setFlags", flagsValue, -1); // (int)0x7FFFFFFF
                }
            }
        }
    }

    private static void applyUIColorsAndroidInThread()
    {
        using (var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        {
            using (var activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity"))
            {
                using (var window = activity.Call<AndroidJavaObject>("getWindow"))
                {
                    //Debug.Log("Colors SET: " + _statusBarColor);
                    window.Call("setStatusBarColor", unchecked((int)_statusBarColor));
                    //                                        window.Call("setNavigationBarColor", unchecked((int)_navigationBarColor));
                }
            }
        }
    }



#endif

    // ================================================================================================================
    // ACCESSOR INTERFACE ---------------------------------------------------------------------------------------------
    /*
        public static States navigationBarState {
                get { return _navigationBarState; }
                set {
                        if (_navigationBarState != value) {
                                _navigationBarState = value;
                                applyUIStates();
                        }
                }
        }
*/
    public static States statusBarState
    {
        get { return _statusBarState; }
        set
        {
            if (_statusBarState != value)
            {
                _statusBarState = value;
                applyUIStates();
            }
        }
    }

    public static bool dimmed
    {
        get { return _dimmed; }
        set
        {
            if (_dimmed != value)
            {
                _dimmed = value;
                applyUIStates();
            }
        }
    }

    public static uint statusBarColor
    {
        get { return _statusBarColor; }
        set
        {
            if (_statusBarColor != value)
            {
                _statusBarColor = value;
                applyUIColors();
                applyUIStates();
            }
        }
    }


    /*
        public static uint navigationBarColor {
                get { return _navigationBarColor; }
                set {
                        if (_navigationBarColor != value) {
                                _navigationBarColor = value;
                                applyUIColors();
                                applyUIStates();
                        }
                }
        }
        */
}
