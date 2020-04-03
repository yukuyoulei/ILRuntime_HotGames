using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

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
	public static bool bIsConsole = false;
	public static void ColorfulOutput(string slog, ELogLevel elog = ELogLevel.Info)
	{
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
		Console.WriteLine(slog);
		Console.ForegroundColor = ConsoleColor.White;
		Console.BackgroundColor = ConsoleColor.Black;
	}

	private static List<string> logs = new List<string>();
	private static Action saveTo;
	public static string LogTag = "Log";
	public static void SaveToFile(string slog, ELogLevel eLogLevel)
	{
		logs.Add(slog);
		if (saveTo == null)
		{
			saveTo = () =>
			{
				Task.Run(() =>
				{
					Thread.Sleep(2000);
					var ls = new List<string>();
					ls.AddRange(logs);
					foreach (var s in ls)
					{
						logs.Remove(s);
					}
					var dir = LogTag + "/";
					if (!Directory.Exists(dir))
						Directory.CreateDirectory(dir);
					File.AppendAllText(dir  + DateTime.Now.ToString("yyyy-MM-dd") + ".log", string.Join("\r\n", ls));
					saveTo = null;
				});
			};
			saveTo();
		}
	}
	private static string FormatLog(string slog, ELogLevel elog)
	{
		return "[" + elog + "] " + "[" + DateTime.Now.ToShortDateString() + " " + DateTime.Now.ToLongTimeString() + "] " + slog;
	}
	public string log(string slog, ELogLevel elog = ELogLevel.Info, bool bIgnoreDel = false)
	{
		if (!bShowOutput)
		{
			return "";
		}
		string result = FormatLog(slog, elog);
		if (delOutput != null && !bIgnoreDel)
		{
			delOutput(bDelWithTime ? result : slog, elog);
		}
		else
		{
			if (bIsConsole)
			{
				ColorfulOutput(result, elog);
			}
			else
			{
#if ENABLE_NETWORK
				if (elog == ELogLevel.Info)
					UnityEngine.Debug.Log(slog);
				else if (elog == ELogLevel.Warning)
					UnityEngine.Debug.LogWarning(slog);
				else if (elog == ELogLevel.Error)
					UnityEngine.Debug.LogError(slog);
#endif
			}
		}
		if (delAfterOutput != null)
		{
			delAfterOutput();
		}
		return result;
	}
}
