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
		public WebSocket connect { get; set; }
		public string username { get; set; }
		private AAvatar _gameAvatar;
		public AAvatar gameAvatar
		{
			get
			{
				if (_gameAvatar == null)
				{
					_gameAvatar = AAvatarManager.Instance.OnGetAvatar(username);
				}
				return _gameAvatar;
			}
		}
		public CancellationTokenSource cancellationTokenSource { get; private set; }
		public string DoHeartBeat()
		{
			HeartBeatTime = ApiDateTime.Now;
			return "ping";
		}
		private DateTime HeartBeatTime { get; set; }
		public bool TestHeartBeatAlive
		{
			get
			{
				return (ApiDateTime.Now - HeartBeatTime).TotalSeconds < 30;
			}
		}
		public void DoCancelConnect()
		{
			if (cancellationTokenSource != null
				&& !cancellationTokenSource.IsCancellationRequested)
			{
				cancellationTokenSource.Cancel();
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
				CONNECT_POOL[u].DoCancelConnect();
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

		async Task StartReceive(WebSocket socket, string user)
		{
			while (true)
			{
				try
				{
					if (socket.State == WebSocketState.Open)
					{
						ArraySegment<byte> buffer = new ArraySegment<byte>(new byte[1024]);
						WebSocketReceiveResult result = await socket.ReceiveAsync(buffer, CONNECT_POOL[user].cancellationTokenSource.Token);

						if (!CONNECT_POOL.ContainsKey(user)
							|| CONNECT_POOL[user].connect != socket)
						{
							if (CONNECT_POOL.ContainsKey(user))
							{
								CONNECT_POOL[user].DoCancelConnect();
								CONNECT_POOL.Remove(user);//删除连接池
							}
							//已经被踢下线。
							return;
						}

						#region 消息处理（字符截取、消息转发）
						try
						{
							#region 关闭Socket处理，删除连接池
							if (socket.State != WebSocketState.Open)//连接关闭
							{
								if (CONNECT_POOL.ContainsKey(user))
								{
									CONNECT_POOL[user].DoCancelConnect();
									CONNECT_POOL.Remove(user);//删除连接池
								}
								break;
							}
							#endregion

							CONNECT_POOL[user].DoHeartBeat();

							string userMsg = Encoding.UTF8.GetString(buffer.Array, 0, result.Count);//发送过来的消息
							AOutput.Log("ws receive " + userMsg);
							if (!userMsg.Contains("?"))
							{
								await AWSEnter.Instance.DoHandler(userMsg, CONNECT_POOL[user], "");
							}
							else
							{
								AOutput.Log("ws received:" + userMsg);
								var amsg = userMsg.Split(new char[] { '?' }, 2);
								await AWSEnter.Instance.DoHandler(amsg[0], CONNECT_POOL[user], amsg[1]);
							}
						}
						catch (Exception exs)
						{
							AOutput.LogError("消息转发异常处理," + exs.Message + "\r\n" + exs.StackTrace);
							//消息转发异常处理，本次消息忽略 继续监听接下来的消息
						}
						#endregion
					}
					else
					{
						break;
					}
				}
				catch
				{
					if (CONNECT_POOL.ContainsKey(user))
					{
						CONNECT_POOL[user].DoCancelConnect();
						CONNECT_POOL.Remove(user);
					}
					break;
				}
			}//while end
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
					var check = ATokenManager.Instance.OnCheckToken(user, token);
					if (!check)
					{
						AOutput.Log("invalid token " + user + "/" + token);
						return;
					}

					#region 用户添加连接池
					//第一次open时，添加到连接池中
					if (!CONNECT_POOL.ContainsKey(user))
					{
						CONNECT_POOL.Add(user, new UserWithToken() { connect = socket, username = user });//不存在，添加
					}
					else if (socket != CONNECT_POOL[user].connect)//当前对象不一致，更新
					{
						CONNECT_POOL[user].DoCancelConnect();
						CONNECT_POOL[user] = new UserWithToken() { connect = socket, username = user };
					}
					#endregion

					await DoSend(CONNECT_POOL[user], AWebServerUtils.OnGetJsonError("0"));
				}
				else
				{
					AOutput.Log("invalid argument");
					return;
				}

				await StartReceive(socket, user);
			}
			catch (Exception ex)
			{
				try
				{
					AOutput.LogError("监听异常：" + ex.Message);
					AOutput.LogError("异常堆栈：" + ex.StackTrace);
				}
				catch { }
			}
		}

		public static async Task DoSend(string user, string msg)
		{
			if (!CONNECT_POOL.ContainsKey(user))
			{
				return;
			}
			await DoSend(CONNECT_POOL[user], msg);
		}
		public static async Task DoSend(UserWithToken user, string msg)
		{
			if (user == null || user.connect == null)
			{
				return;
			}
			if (!string.IsNullOrEmpty(msg))
			{
				var buffer = new ArraySegment<byte>(Encoding.UTF8.GetBytes(msg));
				await user.connect.SendAsync(buffer, WebSocketMessageType.Text, true, user.cancellationTokenSource.Token);
			}
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
