using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
namespace AWebServices
{
	public class AWSEnter : Singleton<AWSEnter>
	{
		private bool bInited = false;
		public void Init()
		{
			if (bInited)
			{
				return;
			}
			bInited = true;

			RegisterHandlers("ping", OnPing);
			RegisterHandlers("move", OnMove);
			RegisterHandlers("joinRoom", OnJoinRoom);
			RegisterHandlers("roomOp", OnRoomOperation);
		}

		private async Task OnRoomOperation(string method, UserWithToken arg2, string arg3)
		{
			arg2.gameAvatar?.GameHandler_RoomOperation(arg3);
			await WSHandler.DoSend(arg2);
		}

		private async Task OnJoinRoom(string method, UserWithToken arg2, string roomType)
		{
			arg2.gameAvatar?.GameHandler_JoinRoom(roomType);
			await WSHandler.DoSend(arg2);
		}

		private async Task OnMove(string method, UserWithToken arg2, string arg3)
		{
			await WSHandler.DoSend(arg2, method + "?" + arg2.gameAvatar?.GameHandler_Move(arg3));
		}
		private async Task OnPing(string method, UserWithToken arg2, string arg3)
		{
			await WSHandler.DoSend(arg2, arg2.DoHeartBeat());
		}


		Dictionary<string, Func<string, UserWithToken, string, Task>> dActions = new Dictionary<string, Func<string, UserWithToken, string, Task>>();
		public void RegisterHandlers(string method, Func<string, UserWithToken, string, Task> usernameArgAction)
		{
			dActions.Add(method.ToLower(), usernameArgAction);
		}
		public async Task DoHandler(string method, UserWithToken username, string arg)
		{
			try
			{
				method = method.ToLower();
				if (dActions.ContainsKey(method))
				{
					await dActions[method](method, username, arg);
				}
				else
				{
					AOutput.LogError("Invalid WSHandler " + method);
				}
			}
			catch (Exception ex)
			{
				AOutput.LogError("WSHandler process error:" + ex.Message + "\r\n" + ex.StackTrace);
			}
		}
	}
}