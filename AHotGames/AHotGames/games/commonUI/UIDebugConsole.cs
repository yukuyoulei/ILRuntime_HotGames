using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine.UI;
using UnityEngine;

public class UIDebugConsole : AHotBase
{
	InputField outputLog;
	protected override void InitComponents()
	{
		var btnSwitch = FindWidget<Button>("btnSwitch");
		var btnShow = FindWidget<Button>("btnShow");
		var btnHide = FindWidget<Button>("btnHide");
		btnHide.onClick.AddListener(() =>
		{
			btnShow.gameObject.SetActive(false);
			btnSwitch.gameObject.SetActive(true);
			ShowWidget("bg", false);
		});

		btnShow.onClick.AddListener(() =>
		{
			btnShow.gameObject.SetActive(false);
			ShowWidget("bg", true);
		});
		btnSwitch.onClick.AddListener(() =>
		{
			btnShow.gameObject.SetActive(true);
			DelayDoSth(() => { btnShow.gameObject.SetActive(false); }, 3);
		});
		outputLog = FindWidget<InputField>("outputLog");

		Application.logMessageReceived += Application_logMessageReceived;
	}

	private void Application_logMessageReceived(string condition, string stackTrace, LogType type)
	{
		outputLog.text = $"[{type}]{condition}{((type == LogType.Error || type == LogType.Exception) ? $"\r\n{stackTrace}" : "")}\r\n{outputLog.text}";
		if (outputLog.text.Length > 1000) outputLog.text = outputLog.text.Substring(0,1000);
	}
}

