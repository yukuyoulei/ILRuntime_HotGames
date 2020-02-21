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
			RegistConsoleCmd("cp", OnCreatePInfo, "Create pinfo");
		}

		private void OnCreatePInfo(string[] sCmds)
		{
			var cake = new CakeServer("pinfo", sCmds.Length > 1 ? sCmds[1] : ObjectId.GenerateNewId().ToString());
			AOutput.Log($"Create at {cake.GetValue(ParamNameDefs.CreateTime)}");
		}

		private void OnListAvatars(string[] sCmds)
		{
			AAvatarManager.Instance.ListAll();
		}
	}
}
