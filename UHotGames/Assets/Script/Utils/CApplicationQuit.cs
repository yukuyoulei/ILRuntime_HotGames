using UnityEngine;
using System.Collections;
using System;
using System.Threading;

public class CApplicationQuit : MonoBehaviour
{
	public static bool bQuited;
	void OnDestroy()
	{
		if (bQuited)
		{
			return;
		}
		bQuited = true;
	}
}
