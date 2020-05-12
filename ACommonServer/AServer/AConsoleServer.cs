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
			RegistConsoleCmd("co", CreateOrder, "Create Order");
		}

		private void CreateOrder(string[] sCmds)
		{
			AOutput.Log(APIWechatPay.Pay(1, Guid.NewGuid().ToString().Replace("-", ""), $"描述：1"));
		}

		private void OnListAvatars(string[] sCmds)
		{
			APlayerManager.Instance.ListAll();
		}

	}
}
