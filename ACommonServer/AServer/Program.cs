using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LibNet;
using LibCommon;
using System.Threading;
using LibServer.Enter;
using System.IO;

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
			AOutput.Instance.RegistOutputDel(doLog);
			AOutput.LogTag = EngineServer.eServerType.ToString();

			EngineServer.Instance.EngineInit();
			EngineServer.Instance.ServerStartUp(typeParser.intParse(sport), EngineServer.EServerType.GatewayServer);

			GameHandlers_Enter.Instance.Init();
			EngineServer.Instance.RegistSubSystem(GameHandlers_Enter.Instance);
			StartConsoleServer();

			try
			{
				ConfigManager.Instance.Init();
				AOutput.Log($"Config loaded.");
			}
			catch (Exception ex)
			{
				AOutput.LogError($"Load config error:{ex.Message}");
			}
		}

		private static void doLog(string slog, ELogLevel eLogLevel)
		{
			AOutput.ColorfulOutput(slog);
			AOutput.SaveToFile(slog, eLogLevel);
		}

		private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
		{
			Console.WriteLine($"Exception {(e.ExceptionObject as Exception).Message}");
		}

		private static void StartConsoleServer()
		{
			(new Thread(new ThreadStart((new AConsoleServer()).run))).Start();
		}

	}
}
