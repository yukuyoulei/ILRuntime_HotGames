using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SDK_Orientation : MonoBehaviour
{
    public void ShowBar()
    {
        AndroidStatusBar.statusBarState = AndroidStatusBar.States.TranslucentOverContent;
    }
    public void HideBar()
    {
        AndroidStatusBar.statusBarState = AndroidStatusBar.States.Hidden;
    }
}
