using System;
using System.Linq;
using System.Text;
using LibCommon;
using LibServer.GameObj;
using MongoDB.Bson;

namespace ACommonServers
{
	public class AConsoleServer : LibCommon.AConsoleBase
	{
		public AConsoleServer()
		{
			RegistConsoleCmd("lr", OnListAvatars, "List Avatars");
		}

		private void OnListAvatars(string[] sCmds)
		{
			APlayerManager.Instance.ListAll();
		}
	}
}
