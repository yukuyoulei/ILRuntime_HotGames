using LibClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LibCommon;

namespace AClient
{
	public class AConsoleClient : LibCommon.AConsoleBase
	{
		public AConsoleClient()
		{
			RegistConsoleCmd("connect", OnConnect, "Connect server");
			RegistConsoleCmd("login", OnLogin, "Send Login Request");
		}

		private void OnConnect(string[] sCmds)
		{
			AClientApp.StartClient("127.0.0.1");
		}

		private void OnLogin(string[] sCmds)
		{
			AClientApis.OnLogin(sCmds[1], MD5String.Hash32(sCmds[2]), EPartnerID.Test);
		}
	}
}
