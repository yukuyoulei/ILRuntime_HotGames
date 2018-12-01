using UnityEngine;
using System.Collections;
using System;

public class UEventUpdater : MonoBehaviour
{
	void Update()
	{
		if (CApplicationQuit.bQuited)
		{
			return;
		}
		UEventListener.Instance.OnTick();
	}
}
