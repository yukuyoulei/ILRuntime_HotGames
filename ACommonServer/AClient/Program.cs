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

			AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
			Task.Run(async () =>
			{
				await AClientApp.StartClient();
				if (AClientApp.bConnected)
					StartConsoleClient();
				else
					AOutput.LogError($"Connect server failed.");
			});

			while (true) ;
		}

		private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
		{
			Console.WriteLine($"Exception {(e.ExceptionObject as Exception).Message}");
		}

		private static void StartConsoleClient()
		{
			Task.Run(() => { new AConsoleClient().run(); });
		}
	}
}
