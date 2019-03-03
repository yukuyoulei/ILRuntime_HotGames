using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.WebSockets;

namespace AWebServices
{
	public class UserWithToken
	{
		public UserWithToken()
		{
			cancellationTokenSource = new CancellationTokenSource();
			DoHeartBeat();
		}
		public WebSocket connect;
		public CancellationTokenSource cancellationTokenSource { get; private set; }
		public void DoHeartBeat()
		{
			HeartBeatTime = ApiDateTime.Now;
		}
		private DateTime HeartBeatTime { get; set; }
		public bool TestHeartBeatAlive
		{
			get
			{
				return (ApiDateTime.Now - HeartBeatTime).TotalSeconds < 30;
			}
		}
	}
	public class WSHandler : IHttpHandler
	{
		/// <summary>
		/// 您将需要在网站的 Web.config 文件中配置此处理程序 
		/// 并向 IIS 注册它，然后才能使用它。有关详细信息，
		/// 请参阅以下链接: https://go.microsoft.com/?linkid=8101007
		/// </summary>
		#region IHttpHandler Members
		public static Dictionary<string, UserWithToken> CONNECT_POOL = new Dictionary<string, UserWithToken>();//用户连接池
		public static void TickUsers()
		{
			var remove = new List<string>();
			foreach (var u in CONNECT_POOL)
			{
				if (!u.Value.TestHeartBeatAlive)
				{
					AOutput.Log(u.Key + " heart beat failed!");
					remove.Add(u.Key);
				}
			}
			foreach (var u in remove)
			{
				CONNECT_POOL[u].cancellationTokenSource.Cancel();
				CONNECT_POOL.Remove(u);
			}
		}

		public void ProcessRequest(HttpContext context)
		{
			if (context.IsWebSocketRequest)
			{
				context.AcceptWebSocketRequest(ProcessChat);
			}
		}

		private async Task ProcessChat(AspNetWebSocketContext context)
		{
			WebSocket socket = context.WebSocket;
			string user = "";
			try
			{
				var allkeys = context.QueryString.AllKeys.ToList();
				if (allkeys.Contains("username") && allkeys.Contains("token"))
				{
					user = context.QueryString["username"];
					var token = context.QueryString["token"];
					var avatar = AWebServices.Avatar.OnGetAvatar(user, token);
					if (avatar == null)
					{
						AOutput.Log("invalid token");
						return;
					}

					#region 用户添加连接池
					//第一次open时，添加到连接池中
					if (!CONNECT_POOL.ContainsKey(user))
					{
						CONNECT_POOL.Add(user, new UserWithToken() { connect = socket });//不存在，添加
					}
					else if (socket != CONNECT_POOL[user].connect)//当前对象不一致，更新
					{
						CONNECT_POOL[user].cancellationTokenSource.Cancel();
						CONNECT_POOL[user] = new UserWithToken() { connect = socket };
					}
					#endregion

					await DoSend(CONNECT_POOL[user], AWebServerUtils.OnGetJsonError("0"));
				}
				else
				{
					AOutput.Log("invalid argument");
					return;
				}

				if (string.IsNullOrEmpty(user))
				{
					return;
				}

				while (true)
				{
					if (socket.State == WebSocketState.Open)
					{
						ArraySegment<byte> buffer = new ArraySegment<byte>(new byte[1024]);
						WebSocketReceiveResult result = await socket.ReceiveAsync(buffer, CONNECT_POOL[user].cancellationTokenSource.Token);

						if (!CONNECT_POOL.ContainsKey(user)
							|| CONNECT_POOL[user].connect != socket)
						{
							//已经被踢下线。
							return;
						}

						#region 消息处理（字符截取、消息转发）
						try
						{
							#region 关闭Socket处理，删除连接池
							if (socket.State != WebSocketState.Open)//连接关闭
							{
								if (CONNECT_POOL.ContainsKey(user)) CONNECT_POOL.Remove(user);//删除连接池
								break;
							}
							#endregion

							string userMsg = Encoding.UTF8.GetString(buffer.Array, 0, result.Count);//发送过来的消息
							if (userMsg == "ping")
							{
								CONNECT_POOL[user].DoHeartBeat();
								await DoSend(CONNECT_POOL[user], userMsg);
							}
							else
							{
								var amsg = userMsg.Split('?');
								if (amsg[0] == "chat")
								{
									AOutput.Log(user + " msg:" + userMsg);
									await DoChat(CONNECT_POOL[user], user, amsg[1]);
								}
							}
						}
						catch (Exception exs)
						{
							//消息转发异常处理，本次消息忽略 继续监听接下来的消息
						}
						#endregion
					}
					else
					{
						break;
					}
				}//while end
			}
			catch (Exception ex)
			{
				try
				{
					AOutput.LogError("监听异常：" + ex.Message);
					AOutput.LogError("异常堆栈：" + ex.StackTrace);
				}
				catch { }

				//整体异常处理
				if (!string.IsNullOrEmpty(user))
					if (CONNECT_POOL.ContainsKey(user))
						CONNECT_POOL.Remove(user);
			}
		}

		static string[] chatArgs = new string[] { "u", "t", "c" };//target username, type, content
		private async Task DoChat(UserWithToken user, string username, string msg)
		{
			var aargs = msg.Split('&');
			Dictionary<string, string> dArgs = new Dictionary<string, string>();
			foreach (var args in aargs)
			{
				var aarg = args.Split(new char[] { '=' }, 2);
				if (aarg.Length != 2)
				{
					return;
				}
				if (dArgs.ContainsKey(aarg[0]))
				{
					return;
				}
				dArgs.Add(aarg[0], aarg[1]);
			}
			foreach (var a in chatArgs)
			{
				if (!dArgs.ContainsKey(a))
				{
					await DoSend(user, "cannot find argument " + a);
					return;
				}
			}

			var c = dArgs["c"];
			c = c.Replace("[", "").Replace("]", "").Replace("\"", "").Replace("\'", "");
			if (string.IsNullOrEmpty(c))
			{
				await DoSend(user, "empty chat content");
				return;
			}

			if (username == dArgs["u"])
			{
				await DoSend(user, "cannot send to yourself");
				return;
			}
			var avatar = AAvatarManager.Instance.OnGetAvatar(username);
			var head = 0;
			if (avatar != null)
			{
				head = avatar.AvatarHead;
			}
			var cm = AAvatar.OnAddChatHistory(username, dArgs["u"], dArgs["t"], c, head.ToString());

			await DoSend(user, cm.ToJson());

			var online = CONNECT_POOL.ContainsKey(dArgs["u"]);
			if (online)
			{
				await DoSend(CONNECT_POOL[dArgs["u"]], cm.ToJson());
			}
		}

		private async Task DoSend(UserWithToken user, string msg)
		{
			var buffer = new ArraySegment<byte>(Encoding.UTF8.GetBytes(msg));
			await user.connect.SendAsync(buffer, WebSocketMessageType.Text, true, user.cancellationTokenSource.Token);
		}

		public bool IsReusable
		{
			get
			{
				return false;
			}
		}
		#endregion
	}
}
