using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


public enum ErrorDefs
{
    WebSocketInvalidArguments = -101,//WebSocket参数错误
    InvalidEmailPhoneNum = -5,//无效的手机号
    InvalidEmailAddress = -4,//无效的邮件
    UsernameInvalidLength = -3,//无效的用户名长度
    DuplicateUsername = -2,//重复的用户名
    DBError = -1,

    UsernamePasswordMismatch = 1,//账号密码错误
    InvalidToken,//无效的token
    NoAvatar,//没有角色
    DuplicateAvatarName,//重复的昵称
    AlreadyHasAvatar,//已经有角色
    AvatarNameInvalidLength,//无效的昵称长度
    NoSuchUser,//没有这个角色
    WaitingForResponse,//等待对方回应
    TargetNoRequested,//对方没有请求好友
    CannotAddYourSelf,//你不能加自己为好友
    AlreadyFriends,//已经是好友了
    TooManyFriends,//好友数量已满，目前是可以加20个好友，随时可以调整
    TargetTooManyFriends,//对方好友数量已满，目前是可以加20个好友，随时可以调整
    TargetIsNotYourFriend,//对方还不是你的好友
    FindNone,//没有搜索到
}

public static class InitValueDefs
{
    public const int MaxFriends = 20;

}
public enum EParamType
{
    String,
    Int,
    Long,
    Double,
}

public enum ERankType
{
    Money,

    RankNum,
}

public static class InfoNameDefs
{
    public static string Username = "username";
    public static string UserPassword = "pw";
    public static string UserMail = "mail";
    public static string UserToken = "token";

    public static string AvatarName = "name";
    public static string AvatarGold = "gold";
    public static string AvatarMoney = "Money";
    public static string AvatarHead = "head";
    public static string DailyCheckCount = "dcc";
    public static string LastDailyCheckTime = "dcct";

    public static string BagSlotPre = "b_";

    public static string RewardSlotPre = "r_";

    public static string Friend1 = "f1";
    public static string Friend2 = "f2";
    public static string FriendRequester = "fr";

    public static string SenderName = "sn";
    public static string TargetName = "tn";
    public static string SendTime = "t";
    public static string SendContent = "c";
    public static string ChatType = "tp";

}