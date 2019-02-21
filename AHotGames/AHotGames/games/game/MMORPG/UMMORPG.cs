using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine.UI;
using UnityEngine;

public class UMMORPG : AHotBase
{
	protected override void InitComponents()
	{
		UWebSender.Instance.OnRequest(Utils.BaseURL + "", "", (res) => { }, (err) => { });
		LoadAnother<UMUICreateAvatar>();
	}
}

