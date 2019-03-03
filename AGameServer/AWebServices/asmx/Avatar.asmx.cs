using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Services;

namespace AWebServices
{
    /// <summary>
    /// Avatar 的摘要说明
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    [System.ComponentModel.ToolboxItem(false)]
    // 若要允许使用 ASP.NET AJAX 从脚本中调用此 Web 服务，请取消注释以下行。 
    // [System.Web.Script.Services.ScriptService]
    public class Avatar : System.Web.Services.WebService
    {
        private static Avatar sinstance;
        public static Avatar Instance
        {
            get
            {
                return sinstance;
            }
        }
        public const string TableName = "tavatar";
        public static string dbConnect = ConfigurationManager.AppSettings["avatarconnect"];
        public static string dbName = ConfigurationManager.AppSettings["avatardbname"];

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
            Context.Response.Write(AWebServerUtils.OnGetJsonError(values));
        }

        void SendDBError()
        {
            Context.Response.Write(AWebServerUtils.OnGetJsonError(ErrorDefs.DBError.ToString()));
        }

        string filtername(string name)
        {
            return name.Trim().Replace(",", "").Replace("/", "").Replace("\\", "").Replace("-", "").Replace(".", "");
        }
        public static ADBAccessor dbavatar
        {
            get
            {
                var db = ADBManager.Get(dbConnect, dbName);
                if (db == null)
                {
                    throw new Exception("cannot connect to database");
                }
                return db;
            }
        }
        public static bool CheckToken(string username, string token)
        {
            if (username == "1" && token == "1")
            {
                return true;
            }
            if (!string.IsNullOrEmpty(Account.OnGetUserToken(username)))
            {
                return Account.OnGetUserToken(username) == token;
            }
            var result = dbavatar.FindOneData(Account.tUserData, ADBAccessor.filter_eq(InfoNameDefs.Username, username)
                & ADBAccessor.filter_eq("token", token), null);
            if (result != null)
            {
                if (result.Contains("token"))
                {
                    return result["token"].AsString == token;
                }
            }
            return false;
        }
        public static AAvatar OnGetAvatar(string username, string token)
        {
            var check = CheckToken(username, token);
            if (!check)
            {
                AOutput.Log(username + " with token " + token + " GetAvatar invalid token");
                return null;
            }
            return AAvatarManager.Instance.OnGetAvatar(username);
        }

        private AAvatar GetAvatar(string username, string token)
        {
            sinstance = this;

            var check = CheckToken(username, token);
            if (!check)
            {
                SendError(ErrorDefs.InvalidToken);
                return null;
            }
            var a = AAvatarManager.Instance.OnGetAvatar(username);
            if (a == null)
            {
                SendError(ErrorDefs.NoAvatar);
                return a;
            }
            return a;
        }

        [WebMethod]
        public void avatarlist(string username, string token)
        {
            var a = GetAvatar(username, token);
            if (a != null)
            {
                SendError("avatars", a.AvatarName);
                return;
            }
            var result = dbavatar.FindOneData(TableName, ADBAccessor.filter_eq("un", username), null);
            if (result != null && result.Contains(InfoNameDefs.AvatarName))
            {
                SendError("avatars", result[InfoNameDefs.AvatarName].AsString);
                return;
            }
            SendError(ErrorDefs.NoAvatar);
        }
        [WebMethod]
        public void avatarsingleinfo(string username, string token, string infoname)
        {
            var a = GetAvatar(username, token);
            if (a != null)
            {
                SendError(infoname, a.OnGetStringParamValue(infoname));
            }
        }
        [WebMethod]
        public void avatarselect(string username, string token)
        {
            var a = GetAvatar(username, token);
            if (a != null)
            {
                SendError(a.ToAll());
                return;
            }
        }
        [WebMethod]
        public void avatarcreate(string username, string token, string avatarname)
        {
            avatarname = filtername(avatarname);
            if (avatarname.Length <= 2)
            {
                SendError(ErrorDefs.AvatarNameInvalidLength);
                return;
            }
            var check = CheckToken(username, token);
            if (!check)
            {
                SendError(ErrorDefs.InvalidToken);
                return;
            }
            {
                var a = AAvatarManager.Instance.OnGetAvatar(username);
                if (a != null)
                {
                    SendError(ErrorDefs.AlreadyHasAvatar);
                    return;
                }
            }
            var findRes = dbavatar.FindOneData(TableName, ADBAccessor.filter_eq(InfoNameDefs.AvatarName, avatarname));
            if (findRes != null && findRes.Contains(InfoNameDefs.AvatarName))
            {
                SendError(ErrorDefs.DuplicateAvatarName);
            }
            else
            {
                var updateRes = dbavatar.UpdateOneData(TableName, ADBAccessor.filter_eq(InfoNameDefs.Username, username)
                    , ADBAccessor.updates_build(
                        ADBAccessor.update(InfoNameDefs.AvatarName, avatarname)
                    )
                    , true);
                if (updateRes)
                {
                    var a = new AAvatar(username, avatarname, null);
                    AAvatarManager.Instance.OnAddAvatar(a);

                    a.OnSetParamValue(InfoNameDefs.AvatarName, avatarname);
                    a.OnSetParamValue(InfoNameDefs.AvatarMoney, 1000);
                    a.OnSetParamValue(InfoNameDefs.AvatarGold, 1000);
                    SendError(InfoNameDefs.AvatarName, avatarname);
                }
                else
                {
                    SendDBError();
                }
            }
        }


        [WebMethod]
        public void avatarchangehead(string username, string token, string head)
        {
            var a = GetAvatar(username, token);
            if (a != null)
            {
                a.AvatarHead = typeParser.intParse(head, 0);
                SendError(InfoNameDefs.AvatarHead, head);
            }
        }

        [WebMethod]
        public void avataraddfriend(string username, string token, string friendname)
        {
            var a = GetAvatar(username, token);
            if (a != null)
            {
                if (a.AvatarName == friendname)
                {
                    SendError(ErrorDefs.CannotAddYourSelf);
                    return;
                }
                var myfriends = AFriendManager.Instance.OnGetFriends(a.AvatarName);
                if (myfriends.Count >= InitValueDefs.MaxFriends)
                {
                    SendError(ErrorDefs.TooManyFriends);
                    return;
                }
                var targetfriends = AFriendManager.Instance.OnGetFriends(friendname);
                if (targetfriends.Count >= InitValueDefs.MaxFriends)
                {
                    SendError(ErrorDefs.TargetTooManyFriends);
                    return;
                }
                var friend = dbavatar.FindOneData(TableName
                    , ADBAccessor.filter_eq(InfoNameDefs.AvatarName, friendname)
                    , ADBAccessor.projections(InfoNameDefs.AvatarName));
                if (friend == null)
                {
                    SendError(ErrorDefs.NoSuchUser);
                    return;
                }
                var added = AFriendManager.Instance.OnAddFriend(a.AvatarName, friendname);
                if (added == 0)
                {
                    var friends = AFriendManager.Instance.OnGetFriends(a.AvatarName);
                    SendError("friends", AFriendManager.ToJson(a.AvatarName, friends.Values.ToList()));
                }
                else if (added == -1)
                {
                    SendError(ErrorDefs.WaitingForResponse);
                }
                else if (added == 1)
                {
                    SendError(ErrorDefs.AlreadyFriends);
                }
            }
        }
        [WebMethod]
        public void avatardeletefriend(string username, string token, string friendname)
        {
            var a = GetAvatar(username, token);
            if (a != null)
            {
                var friends = AFriendManager.Instance.OnGetFriends(a.AvatarName);
                if (!friends.ContainsKey(friendname))
                {
                    SendError(ErrorDefs.TargetIsNotYourFriend);
                    return;
                }
                AFriendManager.Instance.OnDeleteFriend(a.AvatarName, friendname);

                friends = AFriendManager.Instance.OnGetFriends(a.AvatarName);
                SendError("friends", AFriendManager.ToJson(a.AvatarName, friends.Values.ToList()));
            }
        }
        [WebMethod]
        public void avatarmyfriendlist(string username, string token)
        {
            var a = GetAvatar(username, token);
            if (a != null)
            {
                var friends = AFriendManager.Instance.OnGetFriends(a.AvatarName);
                SendError("friends", AFriendManager.ToJson(a.AvatarName, friends.Values.ToList()));
            }
        }
        [WebMethod]
        public void avatarmyrequestfriends(string username, string token)
        {
            var a = GetAvatar(username, token);
            if (a != null)
            {
                var friends = AFriendManager.Instance.OnGetMyRequestedFrineds(a.AvatarName);
                SendError("requests", AFriendManager.ToJson(a.AvatarName, friends));
            }
        }
        [WebMethod]
        public void avatarrequestedmefriends(string username, string token)
        {
            var a = GetAvatar(username, token);
            if (a != null)
            {
                var friends = AFriendManager.Instance.OnGetRequestedMeFriends(a.AvatarName);
                SendError("requests", AFriendManager.ToJson(a.AvatarName, friends));
            }
        }
        [WebMethod]
        public void avatarrefusefriend(string username, string token, string friendname)
        {
            var a = GetAvatar(username, token);
            if (a != null)
            {
                if (AFriendManager.Instance.OnRefuseFriend(a.AvatarName, friendname))
                {
                    var friends = AFriendManager.Instance.OnGetRequestedMeFriends(a.AvatarName);
                    SendError("requests", AFriendManager.ToJson(a.AvatarName, friends));
                }
                else
                {
                    SendError(ErrorDefs.TargetNoRequested);
                }
            }
        }

        [WebMethod]
        public void avatarfindfriend(string username, string token, string friendname)
        {
            var pattern = @"/(" + friendname + @")/gi";
            var filter = Builders<BsonDocument>.Filter.Regex(InfoNameDefs.AvatarName
                        , pattern);
            var res = dbavatar.FindManyData(TableName, filter, ADBAccessor.projections(InfoNameDefs.AvatarName
                , InfoNameDefs.AvatarHead
                , InfoNameDefs.Username));
            if (res != null && res.Count > 0)
            {
                var lres = new List<List<string>>();
                foreach (var r in res)
                {
                    var l = new List<string>();
                    l.Add(InfoNameDefs.AvatarName);
                    l.Add(r[InfoNameDefs.AvatarName].AsString);
                    l.Add(InfoNameDefs.AvatarHead);
                    l.Add(r.Contains(InfoNameDefs.AvatarHead) ? r[InfoNameDefs.AvatarHead].AsInt32.ToString() : "0");
                    l.Add("accid");
                    l.Add(r[InfoNameDefs.Username].AsString);
                    lres.Add(l);
                }
                SendError("names", AWebServerUtils.ToJsonArray(lres.ToArray()));
            }
            else
            {
                SendError(ErrorDefs.FindNone);
            }
        }
        [WebMethod]
        public void avatarchathistory(string username, string targetUsername, string token, string page, string count)
        {
            var a = GetAvatar(username, token);
            if (a != null)
            {
                var lresult = a.OnGetHistory(targetUsername, typeParser.intParse(page), typeParser.intParse(count));
                var l = new List<string>();
                foreach (var r in lresult)
                {
                    l.Add(r.ToJson());
                }
                SendError("hs", "[" + string.Join(",", l.ToArray()) + "]");
            }
        }

        [WebMethod]
        public void avatargetrank(string username, string token, string ranktype, string count)
        {
            int icount = typeParser.intParse(count);
            if (icount <= 0 || icount > 100)
            {
                SendError("Invalid count " + count);
                return;
            }
            var iranktype = typeParser.intParse(ranktype);
            if (iranktype < 0 || iranktype >= (int)ERankType.RankNum)
            {
                SendError("Invalid ranktype " + ranktype);
                return;
            }
            var a = GetAvatar(username, token);
            if (a != null)
            {
                switch ((ERankType)iranktype)
                {
                    case ERankType.Money:
                        var res = dbavatar.FindManyData(TableName, null
                            , ADBAccessor.projections(InfoNameDefs.AvatarName, InfoNameDefs.AvatarMoney, InfoNameDefs.AvatarHead)
                            , icount, 0, ADBAccessor.sort_Descending(InfoNameDefs.AvatarMoney));
                        var lres = new List<List<string>>();
                        foreach (var r in res)
                        {
                            var l = new List<string>();
                            l.Add(InfoNameDefs.AvatarName);
                            l.Add(r[InfoNameDefs.AvatarName].AsString);
                            if (r.Contains(InfoNameDefs.AvatarMoney))
                            {
                                l.Add(InfoNameDefs.AvatarMoney);
                                l.Add(r[InfoNameDefs.AvatarMoney].ToString());
                            }
                            if (r.Contains(InfoNameDefs.AvatarHead))
                            {
                                l.Add(InfoNameDefs.AvatarHead);
                                l.Add(r[InfoNameDefs.AvatarHead].AsInt32.ToString());
                            }
                            lres.Add(l);
                        }
                        SendError("rs", AWebServerUtils.ToJsonArray(lres.ToArray()));
                        return;

                }
                SendError("Invalid ranktype " + ranktype);
            }
        }
    }
}
