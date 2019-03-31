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
        }

        private async Task OnPing(UserWithToken arg1, string arg2)
        {
            await WSHandler.DoSend(arg1, arg1.DoHeartBeat());
        }


        Dictionary<string, Func<UserWithToken, string, Task>> dActions = new Dictionary<string, Func<UserWithToken, string, Task>>();
        public void RegisterHandlers(string sCmd, Func<UserWithToken, string, Task> usernameArgAction)
        {
            dActions.Add(sCmd.ToLower(), usernameArgAction);
        }
        public async Task DoHandler(string sCmd, UserWithToken username, string arg)
        {
            try
            {
                sCmd = sCmd.ToLower();
                if (dActions.ContainsKey(sCmd))
                {
                    await dActions[sCmd](username, arg);
                }
                else
                {
                    AOutput.LogError("Invalid WSHandler " + sCmd);
                }
            }
            catch (Exception ex)
            {
                AOutput.LogError("WSHandler process error:" + ex.Message + "\r\n" + ex.StackTrace);
            }
        }
    }
}