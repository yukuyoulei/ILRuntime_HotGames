using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine.UI;
using UnityEngine;

public class UIFirst : AHotBase
{
	protected override void InitComponents()
	{
        var Text = FindWidget<Text>("Text");
        Text.text = "Hello ILRuntime and FS";
	}
}

