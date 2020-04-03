using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

public static class AOutput
{
	private static Action<string> del;
	public static void Register(Action<string> a)
	{
		del = a;
	}
	public static void Log(string slog)
	{
		del?.Invoke(slog);
	}
	public static void LogError(string slog)
	{
		del?.Invoke($"[ERROR]{slog}");
	}
}
