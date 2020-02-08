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
			RegistConsoleCmd("login", OnLogin, "Send Login Request");
		}

		private void OnLogin(string[] sCmds)
		{
			AClientApis.OnLogin(sCmds[1], sCmds[2], EPartnerID.Test);
		}
	}
}
