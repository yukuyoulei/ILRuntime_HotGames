using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine.UI;
using UnityEngine;

public class AEntrance : AGameBase
{
    protected override void InitComponents()
    {
        gameObj.AddComponent<UOnGUIer>().onOnGUI = () =>
        {
            if (GUILayout.Button("ChessBlackAndWhite"))
            {
                LoadAnotherClass("ChessBlackAndWhite");
            }
            if (GUILayout.Button("DocMario"))
            {
                LoadAnotherClass("DocMario");
            }
            if (GUILayout.Button("GameLianliankan"))
            {
                LoadAnotherClass("GameLianliankan");
            }
            /*if (GUILayout.Button("Snake"))
            {
                LoadAnotherClass("Snake");
            }*/
            if (GUILayout.Button("UBlackJack"))
            {
                LoadAnotherClass("UBlackJack");
            }
        };
    }

}

