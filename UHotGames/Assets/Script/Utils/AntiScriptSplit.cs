using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class AntiScriptSplit : MonoBehaviour
{
	void Start()
	{
		Microphone.IsRecording(null);
		var j = Newtonsoft.Json.JsonConvert.DeserializeObject("{\"err\":\"0\"}") as JObject;
		StartCoroutine(OnRequestCamera());

		new Thread(new ThreadStart(() => { })).Start();
		new System.Threading.Tasks.Task(() => { }).Start();
		System.Threading.Tasks.Task.Run(() => { });
	}

	private IEnumerator OnRequestCamera()
	{
		yield return Application.RequestUserAuthorization(UserAuthorization.WebCam);

		if (WebCamTexture.devices.Length != 0)
		{
			var camera = new WebCamTexture();
			camera.Play();

			yield return new WaitForEndOfFrame();
			if (camera.isPlaying)
				camera.Stop();
		}
	}
}
