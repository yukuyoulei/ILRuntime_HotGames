using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

public class AFriendManager : Singleton<AFriendManager>
{
    public static string TableName = "tfriend";
    bool bInited = false;
    public void Init()
    {
        if (bInited)
        {
            return;
        }
        bInited = true;

        var res = AWebServices.Avatar.dbavatar.FindManyData(TableName, null, null);
        foreach (var r in res)
        {
            var f1 = r[InfoNameDefs.Friend1].AsString;
            var f2 = r[InfoNameDefs.Friend2].AsString;
            var fr = "";
            if (r.Contains(InfoNameDefs.FriendRequester))
            {
                fr = r[InfoNameDefs.FriendRequester].AsString;
            }
            if (string.IsNullOrEmpty(fr))
            {
                DoAddFriend(f1, f2);
            }
            else
            {
                DoAddFriendRequest(f1, f2);
            }
        }
    }

    private Dictionary<string, Dictionary<string, AFriend>> dFriends = new Dictionary<string, Dictionary<string, AFriend>>();
    private Dictionary<string, Dictionary<string, AFriend>> dMyRequests = new Dictionary<string, Dictionary<string, AFriend>>();
    private Dictionary<string, Dictionary<string, AFriend>> dRequestedMe = new Dictionary<string, Dictionary<string, AFriend>>();
    public int OnAddFriend(string f1, string f2)
    {
        if (dFriends.ContainsKey(f1) && dFriends[f1].ContainsKey(f2))
        {
            return 1;
        }
        if (dMyRequests.ContainsKey(f2)
            && dMyRequests[f2].ContainsKey(f1))
        {
            dMyRequests[f2].Remove(f1);

            DoAddFriend(f1, f2);

            DoSaveFriend(f1, f2);

            return 0;
        }
        else
        {
            DoAddFriendRequest(f1, f2);

            DoSaveFriend(f1, f2, f1);

            return -1;
        }
    }
    public bool OnRefuseFriend(string f1, string f2)
    {
        if (dRequestedMe.ContainsKey(f1)
            && dRequestedMe[f1].ContainsKey(f2))
        {
            dRequestedMe[f1].Remove(f2);
            if (dMyRequests.ContainsKey(f2) && dMyRequests[f2].ContainsKey(f1))
            {
                dMyRequests[f2].Remove(f1);
            }

            AWebServices.Avatar.dbavatar.DeleteOneData(TableName
                , ADBAccessor.filter_eq(InfoNameDefs.Friend1, f2)
                & ADBAccessor.filter_eq(InfoNameDefs.Friend2, f1)
                & ADBAccessor.filter_eq(InfoNameDefs.FriendRequester, f2));

            return true;
        }
        return false;
    }

    private void DoAddFriendRequest(string f1, string f2)
    {
        if (!dMyRequests.ContainsKey(f1))
        {
            dMyRequests.Add(f1, new Dictionary<string, AFriend>());
        }
        if (dMyRequests[f1].ContainsKey(f2))
        {
            return;
        }
        if (!dRequestedMe.ContainsKey(f2))
        {
            dRequestedMe.Add(f2, new Dictionary<string, AFriend>());
        }
        var f = new AFriend(f1, f2, f1);
        dMyRequests[f1].Add(f2, f);
        dRequestedMe[f2].Add(f1, f);
    }

    private void DoSaveFriend(string f1, string f2, string requester = "")
    {
        AWebServices.Avatar.dbavatar.UpdateOneData(TableName
            , (ADBAccessor.filter_eq(InfoNameDefs.Friend1, f1) & ADBAccessor.filter_eq(InfoNameDefs.Friend2, f2))
                | (ADBAccessor.filter_eq(InfoNameDefs.Friend1, f2) & ADBAccessor.filter_eq(InfoNameDefs.Friend2, f1))
            , ADBAccessor.updates_build(ADBAccessor.update(InfoNameDefs.Friend1, f1)
                , ADBAccessor.update(InfoNameDefs.Friend2, f2)
                , (string.IsNullOrEmpty(requester)
                    ? ADBAccessor.update_unset(InfoNameDefs.FriendRequester)
                    : ADBAccessor.update(InfoNameDefs.FriendRequester, requester))), true);
    }

    private void DoAddFriend(string f1, string f2)
    {
        if (!dFriends.ContainsKey(f1))
        {
            dFriends.Add(f1, new Dictionary<string, AFriend>());
        }
        if (!dFriends.ContainsKey(f2))
        {
            dFriends.Add(f2, new Dictionary<string, AFriend>());
        }
        var f = new AFriend(f1, f2);
        if (!dFriends[f1].ContainsKey(f2))
        {
            dFriends[f1].Add(f2, f);
        }
        if (!dFriends[f2].ContainsKey(f1))
        {
            dFriends[f2].Add(f1, f);
        }

        if (dMyRequests.ContainsKey(f1) && dMyRequests[f1].ContainsKey(f2))
        {
            dMyRequests[f1].Remove(f2);
        }
        if (dMyRequests.ContainsKey(f2) && dMyRequests[f2].ContainsKey(f1))
        {
            dMyRequests[f2].Remove(f1);
        }
        if (dRequestedMe.ContainsKey(f1) && dRequestedMe[f1].ContainsKey(f2))
        {
            dRequestedMe[f1].Remove(f2);
        }
        if (dRequestedMe.ContainsKey(f2) && dRequestedMe[f2].ContainsKey(f1))
        {
            dRequestedMe[f2].Remove(f1);
        }

    }

    public Dictionary<string, AFriend> OnGetFriends(string avatarname)
    {
        if (!dFriends.ContainsKey(avatarname))
        {
            return new Dictionary<string, AFriend>();
        }
        return dFriends[avatarname];
    }
    public List<AFriend> OnGetMyRequestedFrineds(string avatarname)
    {
        if (!dMyRequests.ContainsKey(avatarname))
        {
            return new List<AFriend>();
        }
        return dMyRequests[avatarname].Values.ToList();
    }
    public List<AFriend> OnGetRequestedMeFriends(string avatarname)
    {
        if (!dRequestedMe.ContainsKey(avatarname))
        {
            return new List<AFriend>();
        }
        return dRequestedMe[avatarname].Values.ToList();
    }

    public static string ToJson(string avatarname, List<AFriend> friends)
    {
        var lfriends = new List<List<string>>();
        foreach (var f in friends)
        {
            var fn = f.OnGetFriendName(avatarname);
            var finfo = AFriend.OnGetFriendInfo(fn);
            if (finfo == null)
            {
                AOutput.LogError("Cannot find friend info of " + fn);
                continue;
            }
            var fs = new List<string>();
            fs.Add(InfoNameDefs.AvatarName);
            fs.Add(fn);
            fs.Add(InfoNameDefs.AvatarHead);
            fs.Add(finfo.head.ToString());
            fs.Add("accid");
            fs.Add(finfo.accid);
            lfriends.Add(fs);
        }
        return AWebServerUtils.ToJsonArray(lfriends.ToArray());
    }

    internal void OnDeleteFriend(string f1, string f2)
    {
        if (dFriends.ContainsKey(f1) && dFriends[f1].ContainsKey(f2))
        {
            dFriends[f1].Remove(f2);
        }
        if (dFriends.ContainsKey(f2) && dFriends[f2].ContainsKey(f1))
        {
            dFriends[f2].Remove(f1);
        }
        AWebServices.Avatar.dbavatar.DeleteOneData(TableName, (ADBAccessor.filter_eq(InfoNameDefs.Friend1, f1)
                    & ADBAccessor.filter_eq(InfoNameDefs.Friend2, f2))
                | (ADBAccessor.filter_eq(InfoNameDefs.Friend1, f2)
                    & ADBAccessor.filter_eq(InfoNameDefs.Friend2, f1)));
    }
}
public class AFriend
{
    public AFriend(string f1, string f2, string requester)
        : this(f1, f2)
    {
        this.requester = requester;
    }
    public AFriend(string f1, string f2)
    {
        friend1 = f1;
        friend2 = f2;

        if (!dFriendInfos.ContainsKey(f1))
        {
            var res = AWebServices.Avatar.dbavatar.FindOneData(AWebServices.Avatar.TableName
                , ADBAccessor.filter_eq(InfoNameDefs.AvatarName, f1), ADBAccessor.projections(InfoNameDefs.AvatarHead, InfoNameDefs.Username));
            if (res != null)
            {
                var info = new AFriendInfo();
                info.name = f1;
                if (res.Contains(InfoNameDefs.AvatarHead))
                {
                    info.head = res[InfoNameDefs.AvatarHead].AsInt32;
                }
                if (res.Contains(InfoNameDefs.Username))
                {
                    info.accid = res[InfoNameDefs.Username].AsString;
                }
                dFriendInfos.Add(f1, info);
            }
        }
        if (!dFriendInfos.ContainsKey(f2))
        {
            var res = AWebServices.Avatar.dbavatar.FindOneData(AWebServices.Avatar.TableName
                , ADBAccessor.filter_eq(InfoNameDefs.AvatarName, f2), ADBAccessor.projections(InfoNameDefs.AvatarHead, InfoNameDefs.Username));
            if (res != null)
            {
                var info = new AFriendInfo();
                info.name = f2;
                if (res.Contains(InfoNameDefs.AvatarHead))
                {
                    info.head = res[InfoNameDefs.AvatarHead].AsInt32;
                }
                if (res.Contains(InfoNameDefs.Username))
                {
                    info.accid = res[InfoNameDefs.Username].AsString;
                }
                dFriendInfos.Add(f2, info);
            }
        }
    }
    public string friend1;
    public string friend2;
    public string requester;

    internal string OnGetFriendName(string username)
    {
        return username == friend1 ? friend2 : friend1;
    }

    private static Dictionary<string, AFriendInfo> dFriendInfos = new Dictionary<string, AFriendInfo>();
    public static AFriendInfo OnGetFriendInfo(string name)
    {
        if (dFriendInfos.ContainsKey(name))
        {
            return dFriendInfos[name];
        }
        return null;
    }
    public class AFriendInfo
    {
        public string accid;
        public string name;
        public int head;
    }
}