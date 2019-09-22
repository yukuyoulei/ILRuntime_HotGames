using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using System.Web.Http;

namespace AWebServices
{
	public class Global : System.Web.HttpApplication
	{

		protected void Application_Start(object sender, EventArgs e)
		{
			GlobalConfiguration.Configure(WebApiConfig.Register);
			AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

			System.Net.HttpWebRequest.DefaultWebProxy = null;

			AWSEnter.Instance.Init();
			/*AFriendManager.Instance.Init();*/

			AOutput.Instance.RegistOutputDel(OnOutPut);

			new Thread(new ThreadStart(() =>
			{
				while (true)
				{
					Thread.Sleep(3000);
					WSHandler.TickUsers();
				}
			})).Start();

			new Thread(new ThreadStart(() =>
			{
				while (true)
				{
					Thread.Sleep(1000);
					AAvatarManager.Instance.OnTick();
				}
			})).Start();
		}
		private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
		{
			var ex = (e.ExceptionObject as Exception);
			AOutput.LogError("UnhandledException " + ex.Message);
			AOutput.LogError("UnhandledException " + ex.StackTrace);
		}

		private void OnOutPut(string slog, ELogLevel eLogLevel)
		{
			if (Directory.Exists(LogDir))
			{
				var logfile = LogDir + DateTime.Now.ToString("yyyy-MM-dd") + ".log";
				if (!File.Exists(logfile))
				{
					using (File.Create(logfile))
					{

					}
				}
				if (string.IsNullOrEmpty(slog))
				{
					slog = "\r\n";
				}
				else
				{
					slog = "[" + eLogLevel + "]" + "[" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "]" + slog + "\r\n";
				}
				File.AppendAllText(logfile, slog);
			}
		}

		protected void Session_Start(object sender, EventArgs e)
		{

		}

		private static string LogDir = ConfigurationManager.AppSettings["log"];
		protected void Application_BeginRequest(object sender, EventArgs e)
		{
			string hPath = Request.Url.LocalPath.ToString().ToLower();
			if (!hPath.Contains(".asmx"))
			{
				foreach (var p in ConfigurationManager.AppSettings.AllKeys)
				{
					if (!p.StartsWith("/"))
					{
						continue;
					}
					if (!hPath.Contains(p))
					{
						continue;
					}
					Context.RewritePath(hPath.Replace(p, ConfigurationManager.AppSettings[p]));
					break;
				}
				//Context.RewritePath(hPath.Replace("/validate", "/Enter.asmx/validate"));
			}
			AOutput.Log("");
			AOutput.Log("URL:" + Request.RawUrl);
			AOutput.Log("FORM:" + Request.Form.ToString());
			AOutput.Log("IP:" + Request.UserHostAddress);
			AOutput.Log("USER:" + Request.UserAgent);
			AOutput.Log("");
		}

		protected void Application_AuthenticateRequest(object sender, EventArgs e)
		{

		}

		protected void Application_Error(object sender, EventArgs e)
		{

		}

		protected void Session_End(object sender, EventArgs e)
		{

		}

		protected void Application_End(object sender, EventArgs e)
		{

		}
	}
}