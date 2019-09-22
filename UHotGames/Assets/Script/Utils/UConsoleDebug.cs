using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UConsoleDebug : MonoBehaviour
{
	private void Awake()
	{
		Application.logMessageReceived += Application_logMessageReceived;
	}

	private void Application_logMessageReceived(string condition, string stackTrace, LogType type)
	{
		slog = $"{slog} \r\n [{ApiDateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")}] {condition}";
		if (slog.Length > 3000)
		{
			slog = slog.Substring(0, 3000);
		}
	}

	string slog = "";
	string sinput = "";
	Vector2 pos;
	private void OnGUI()
	{
		if (!bDisplay)
		{
			return;
		}
		GUILayout.BeginHorizontal();
		sinput = GUILayout.TextField(sinput, GUILayout.Width(Screen.width - 160));
		if (GUILayout.Button("Close", GUILayout.Width(160)))
		{
			bDisplay = false;
		}
		GUILayout.EndHorizontal();

		pos = GUILayout.BeginScrollView(pos);
		GUILayout.TextField(slog, GUILayout.Width(Screen.width - 20), GUILayout.Height(Screen.height));
		GUILayout.EndScrollView();
	}
	bool bDisplay;
	void Update()
	{
		if (Input.GetKeyDown(KeyCode.BackQuote))
		{
			bDisplay = !bDisplay;
		}
	}
}
