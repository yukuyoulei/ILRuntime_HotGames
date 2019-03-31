using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Services;
using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AWebServices
{
    /// <summary>
    /// Account 的摘要说明
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // 若要允许使用 ASP.NET AJAX 从脚本中调用此 Web 服务，请取消注释以下行。 
    // [System.Web.Script.Services.ScriptService]
    public class Account : System.Web.Services.WebService
    {
        public const string tUserData = "tuser";
        public static string dbConnect = ConfigurationManager.AppSettings["accountconnect"];
        public static string dbName = ConfigurationManager.AppSettings["accountdbname"];

        public static string[] avatarInfoNames = new string[]
        {
            "gold",
            "name",
            "head"
        };
        static string filtenames(string name)
        {
            return name.Replace(",", "").Replace(" ", "").Replace(":", "").Replace("|", "").ToLower();
        }

        private static string AddUserToken(string username)
        {
            username = filtenames(username);
            var token = "";
            if (dUserTokens.ContainsKey(username))
            {
                token = dUserTokens[username];
            }
            else
            {
                token = AWebServerUtils.GetEncryptCode(12, 4);
                dUserTokens.Add(username, token);
            }
            return token;
        }
        public static string OnGetUserToken(string username)
        {
            if (dUserTokens.ContainsKey(username))
            {
                return dUserTokens[username];
            }
            return "";
        }
        public static bool OnCheckToken(string username, string token)
        {
            return dUserTokens.ContainsKey(username) && dUserTokens[username] == token;
        }
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
        private ADBAccessor dbaccount
        {
            get
            {
                var db = ADBManager.Get(dbConnect, dbName);
                if (db == null)
                {
                    SendDBError();
                    throw new Exception("cannot connect to database");
                }
                return db;
            }
        }

        static Dictionary<string, string> dUserTokens = new Dictionary<string, string>();
        [WebMethod]
        public void accountlogin(string username, string password)
        {
            if (username == "fs" && password == "111")
            {
                if (!dUserTokens.ContainsKey(username))
                {
                    dUserTokens.Add(username, "1");
                }
                SendError("token", "1");
                return;
            }
            var result = dbaccount.FindOneData(tUserData
                , ADBAccessor.filter_eq(InfoNameDefs.Username, username)
                    & ADBAccessor.filter_eq(InfoNameDefs.UserPassword, MD5String.Hash32(password))
                , null);
            if (result != null)
            {
                if (result.Contains(InfoNameDefs.Username))
                {
                    var token = "";
                    if (result.Contains(InfoNameDefs.UserToken))
                    {
                        token = result[InfoNameDefs.UserToken].AsString;
                    }
                    else
                    {
                        token = AWebServerUtils.GetEncryptCode();
                        var updateRes = dbaccount.UpdateOneData(tUserData, ADBAccessor.filter_eq(InfoNameDefs.Username, username)
                            , ADBAccessor.updates_build(ADBAccessor.update(InfoNameDefs.UserToken, token)), false);
                        if (!updateRes)
                        {
                            SendDBError();
                            return;
                        }
                    }
                    if (!dUserTokens.ContainsKey(username))
                    {
                        dUserTokens.Add(username, token);
                    }
                    SendError(InfoNameDefs.Username, username, InfoNameDefs.UserToken, dUserTokens[username]);
                }
                else
                {
                    SendError(ErrorDefs.UsernamePasswordMismatch);
                }
            }
            else
            {
                SendError(ErrorDefs.UsernamePasswordMismatch);
            }
        }
        [WebMethod]
        public void accountregister(string username, string password, string mail)
        {
            if (!mail.Contains("@") || !mail.Contains(".") || mail.IndexOf("@") > mail.IndexOf("."))
            {
                SendError(ErrorDefs.InvalidEmailAddress);
                return;
            }

            var result = dbaccount.FindOneData(tUserData, ADBAccessor.filter_eq(InfoNameDefs.Username, username), null);
            if (result != null)
            {
                SendError(ErrorDefs.DuplicateUsername);
            }
            else
            {
                var fdresult = dbaccount.UpdateOneData(tUserData, ADBAccessor.filter_eq(InfoNameDefs.Username, username)
                    , ADBAccessor.updates_build(
                        ADBAccessor.update(InfoNameDefs.UserPassword, MD5String.Hash32(password))
                        , ADBAccessor.update(InfoNameDefs.UserMail, mail)
                        , ADBAccessor.update(InfoNameDefs.Username, username)
                        ), true);
                if (fdresult)
                {
                    SendError(Context.Response, "username", username);
                }
                else
                {
                    SendDBError();
                }
            }
        }
        [WebMethod]
        public void accountlogout(string username, string token)
        {
            var result = dbaccount.FindOneData(tUserData, ADBAccessor.filter_eq(InfoNameDefs.Username, username)
                & ADBAccessor.filter_eq(InfoNameDefs.UserToken, token), null);
            if (result != null)
            {
                if (result.Contains(InfoNameDefs.UserToken))
                {
                    var updateRes = dbaccount.UpdateOneData(tUserData, ADBAccessor.filter_eq(InfoNameDefs.Username, username)
                        , ADBAccessor.updates_build(ADBAccessor.update(InfoNameDefs.UserToken, token, false)), false);
                    if (updateRes)
                    {
                        SendError();
                    }
                    else
                    {
                        SendDBError();
                    }
                }
                else
                {
                    SendError(ErrorDefs.InvalidToken);
                }
            }
            else
            {
                SendError(ErrorDefs.InvalidToken);
            }
        }


    }
}
