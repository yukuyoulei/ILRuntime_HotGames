using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine.UI;
using UnityEngine;

public class AEntrance : AHotBase
{
    protected override void InitComponents()
    {
        LoadAnother<UILogin>();
    }

}

