using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine.UI;
using UnityEngine;

public class RegisterUI : AHotBase
{
    protected override void InitComponents()
    {
        var btnReturn = FindWidget<Button>("btnReturn");
        btnReturn.onClick.AddListener(() =>
        {
            UnloadThisUI();

            LoadAnotherUI<LoginUI>();
        });


    }
}

