using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class UEmitMessager : MonoBehaviour
{
	public AHotBase gameBase;
	public Action<string> onReceiveMsg;
	public void OnRegistAction(Action<string> a)
	{
		onReceiveMsg += a;
	}
	public void ReceiveMessage(string message)
	{
		onReceiveMsg(message);
	}

	public void OnSendMessage(string message, float delay)
	{
		StartCoroutine(DelaySendMessage(message, delay));
	}

	private IEnumerator DelaySendMessage(string message, float delay)
	{
		yield return new WaitForSeconds(delay);

		AHotBase.SendMessageToUnityReceiver(message);
	}
}
