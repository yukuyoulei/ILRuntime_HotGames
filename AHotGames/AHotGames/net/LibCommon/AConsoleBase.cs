using LibNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LibCommon
{
	public class AConsoleBase
	{
		public AConsoleBase()
		{
			RegistConsoleCmd("?", OnHelp, "列出所有控制台项");
			RegistConsoleCmd("Help", OnHelp, "列出所有控制台项");
			RegistConsoleCmd("clr", OnClear);
		}

		public void run()
		{
			while (true)
			{
				string scmd = Console.ReadLine();
				try
				{
					OnParseCmd(scmd);
				}
				catch (System.Exception ex)
				{
					AOutput.Log(">>>>>>>>\n" + ex.Message + "\n" + ex.StackTrace + "\n<<<<<<<<");
				}
			}
		}

		public void OnParseCmd(string scmd)
		{
			string[] cmds = scmd.Split(' ');
			string t1 = cmds[0];
			t1 = t1.ToLower();
			OnParseCmd(t1, cmds);
		}

		private Dictionary<string, CmdFuncion> dFunctions = new Dictionary<string, CmdFuncion>();
		private Dictionary<string, string> dFunctionsDes = new Dictionary<string, string>();

		public delegate void CmdFuncion(string[] sCmds);
		public void RegistConsoleCmd(string sCmd, CmdFuncion cf)
		{
			RegistConsoleCmd(sCmd, cf, "");
		}
		public void RegistConsoleCmd(string sCmd, CmdFuncion cf, string sDes)
		{
			dFunctionsDes.Add(sCmd, sDes);
			sCmd = sCmd.ToLower();
			dFunctions.Add(sCmd, cf);
		}
		private void OnParseCmd(string sCmd, string[] sCmds)
		{
			if (sCmd == "")
			{
				return;
			}
			if (!dFunctions.ContainsKey(sCmd))
			{
				AOutput.Log("Not registed cmd:" + sCmd);
				return;
			}
			dFunctions[sCmd](sCmds);
		}

		private void OnHelp(string[] cmds)
		{
			AOutput.Log(GetCmdHelp());
		}

		private void OnClear(string[] sCmds)
		{ 
#if !ENABLE_NETWORK
			Console.Clear();
#endif
		}

		public string GetCmdHelp(bool bServer = false)
		{
			string sCmdHelp = "";
			sCmdHelp += "控制台命令不区分大小写。";
			sCmdHelp += "\n";
			foreach (string sCmd in dFunctionsDes.Keys)
			{
				sCmdHelp += sCmd + "\t" + dFunctionsDes[sCmd];
				sCmdHelp += "\n";
			}
			return sCmdHelp;
		}
	}
}
