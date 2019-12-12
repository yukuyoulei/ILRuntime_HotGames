using System;
using System.Collections.Generic;
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
}
