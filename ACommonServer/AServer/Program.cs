using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LibNet;

namespace ACommonServers
{
	class Program
	{
		static void Main(string[] args)
		{
			AOutput.Instance.RegistOutputDel((log, elv) => Console.WriteLine(log));

			EngineServer.Instance.EngineInit();
			EngineServer.Instance.ServerStartUp(999, EngineServer.EServerType.GatewayServer);

			Handlers.Instance.Init();
			EngineServer.Instance.RegistSubSystem(Handlers.Instance);
			StartConsoleServer();
		}

		private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
		{
			Console.WriteLine($"Exception {(e.ExceptionObject as Exception).Message}");
		}

		private static void StartConsoleServer()
		{
			while (true) ;
		}

	}
}
