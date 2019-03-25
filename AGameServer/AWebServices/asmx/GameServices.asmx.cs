using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;

namespace AWebServices.asmx
{
    /// <summary>
    /// GameServices 的摘要说明
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // 若要允许使用 ASP.NET AJAX 从脚本中调用此 Web 服务，请取消注释以下行。 
    // [System.Web.Script.Services.ScriptService]
    public class GameServices : System.Web.Services.WebService
    {
        void SendError(ErrorDefs error)
        {
            SendError((int)error);
        }
        void SendError(int error)
        {
            SendError(error.ToString());
        }
        void SendError(params string[] values)
        {
            SendError(Context.Response, values);
        }
        void SendError(HttpResponse response, params string[] values)
        {
            response.Write(AWebServerUtils.OnGetJsonError(values));
        }
        void SendDBError()
        {
            Context.Response.Write(AWebServerUtils.OnGetJsonError(ErrorDefs.DBError.ToString()));
        }

        [WebMethod]
        public void gameschulttime(string username, string token, string time)
        {
            if (!Avatar.CheckToken(username, token))
            {
                SendError(ErrorDefs.InvalidToken);
                return;
            }
            var avatar = AAvatarManager.Instance.OnGetAvatar(username);
            if (avatar == null)
            {
                SendError(ErrorDefs.NoAvatar);
                return;
            }
            var ftime = typeParser.doubleParse(time);
            if (avatar.SchulteTime > ftime)
            {
                avatar.SchulteTime = ftime;
            }
            SendError(InfoNameDefs.SchulteTime, avatar.SchulteTime.ToString("f2"));
        }
    }
}
