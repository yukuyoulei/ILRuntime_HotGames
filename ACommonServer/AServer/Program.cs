using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LibNet;
using LibCommon;

namespace ACommonServers
{
	class Program
	{
		static void Main(string[] args)
		{
			var sport = args.Length > 0 ? args[0] : "999";
			InitValueDefs.dbconnect = args.Length > 1 ? args[1] : "mongodb://127.0.0.1:27018";
			InitValueDefs.dbname = args.Length > 2 ? args[2] : "common";

			AOutput.bIsConsole = true;
			AOutput.Instance.RegistOutputDel((log, elv) => Console.WriteLine(log));

			EngineServer.Instance.EngineInit();
			EngineServer.Instance.ServerStartUp(typeParser.intParse(sport), EngineServer.EServerType.GatewayServer);

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
