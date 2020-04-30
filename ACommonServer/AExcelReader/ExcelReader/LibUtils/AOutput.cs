using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
#if ENABLE_NETWORK
using UnityEngine;
#endif

public enum ELogLevel
{
	Info,
	Error,
	Warning,
}
public class AOutput : Singleton<AOutput>
{
	public delegate void delegateOutput(string slog, ELogLevel eLogLevel);
	public delegateOutput delOutput = null;
	public delegate void delegateAfterOutput();
	public delegateAfterOutput delAfterOutput = null;
	public void RegistOutputDel(delegateOutput del)
	{
		delOutput = del;
	}
	public void RegistAfterOutputDel(delegateAfterOutput del)
	{
		delAfterOutput = del;
	}
	public static void LogWarn(string slog)
	{
		Instance.log(slog, ELogLevel.Warning);
	}
	public static void LogError(string slog)
	{
		Instance.log(slog, ELogLevel.Error);
	}
	public static void Log(string slog, ELogLevel elog = ELogLevel.Info)
	{
		Instance.log(slog, elog);
	}
	public bool bDelWithTime = false;
	public static bool bShowOutput = true;
	public string log(string slog, ELogLevel elog = ELogLevel.Info, bool bIgnoreDel = false)
	{
		if (!bShowOutput)
		{
			return "";
		}
		string result = "[" + elog + "] " + "[" + DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToLongTimeString() + "] " + slog;
		if (delOutput != null && !bIgnoreDel)
		{
			delOutput(bDelWithTime ? result : slog, elog);
		}
		else
		{
#if !ENABLE_NETWORK
			Console.ForegroundColor = ConsoleColor.White;
			if (elog == ELogLevel.Info)
			{
				Console.ForegroundColor = ConsoleColor.Black;
				Console.BackgroundColor = ConsoleColor.DarkGreen;
			}
			else if (elog == ELogLevel.Warning)
			{
				Console.BackgroundColor = ConsoleColor.Blue;
			}
			else if (elog == ELogLevel.Error)
			{
				Console.BackgroundColor = ConsoleColor.Red;
			}
			Console.SetCursorPosition(0, Console.CursorTop);
			Console.WriteLine(result);
			Console.ForegroundColor = ConsoleColor.White;
			Console.BackgroundColor = ConsoleColor.Black;
#else
			if (elog == ELogLevel.Info)
				Debug.Log(slog);
			else if (elog == ELogLevel.Warning)
				Debug.LogWarning(slog);
			else if (elog == ELogLevel.Error)
				Debug.LogError(slog);
#endif
		}
		if (delAfterOutput != null)
		{
			delAfterOutput();
		}
		return result;
	}
}
