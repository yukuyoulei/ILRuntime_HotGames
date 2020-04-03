using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LibClient;
using LibNet;

namespace AClient
{
	class Program
	{
		static void Main(string[] args)
		{
			AOutput.Instance.RegistOutputDel((log, elv) => Console.WriteLine(log));

			AClientApp.Init(new AOnlineSubsystem());
			AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

			Task.Run(() => { new AConsoleClient().run(); });

			while (true) ;
		}

		private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
		{
			Console.WriteLine($"Exception {(e.ExceptionObject as Exception).Message}");
		}

	}
}
