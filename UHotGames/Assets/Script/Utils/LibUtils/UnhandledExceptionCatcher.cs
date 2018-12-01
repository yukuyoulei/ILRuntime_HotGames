using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace LibUtils
{
	public class UnhandledExceptionCatcher
	{
		public static void DoCatch()
		{
			AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(CurrentDomainUnhandledException);
		}

		private static void CurrentDomainUnhandledException(object sender, UnhandledExceptionEventArgs e)
		{
			CreateMiniDump(e.ExceptionObject.ToString());
		}
		private static void CreateMiniDump(string sExceptionArgs)
		{
			AOutput.Log("Crashed " + sExceptionArgs, ELogLevel.Error);
		}

	}
}
