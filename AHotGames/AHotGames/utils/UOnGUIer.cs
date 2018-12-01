using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class UOnGUIer : MonoBehaviour
{
    public void Start()
    {
    }
    private GUISkin _skin;
    protected GUISkin skin
    {
        get
        {
            if (_skin == null)
            {
                _skin = new GUISkin();
                _skin.button = new GUIStyle();
                _skin.box = new GUIStyle();
                _skin.customStyles = new GUIStyle[0];
            }
            return _skin;
        }
    }
    public Action onOnGUI;
    public void OnGUI()
    {
        if (onOnGUI != null)
        {
            onOnGUI();
        }
    }
}

