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

	NoError = 0,//
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
	InvalidSex,//无效的性别 = _ =
	MoveInCold,//移动冷却中
	InvalidMove,//无效的移动
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
	public const string Username = "username";
	public const string UserPassword = "pw";
	public const string UserMail = "mail";
	public const string UserToken = "token";

	public const string AvatarName = "name";
	public const string AvatarGold = "gold";
	public const string AvatarSex = "sex";
	public const string AvatarMoney = "Money";
	public const string AvatarHead = "head";
	public const string AvatarLevel = "level";
	public const string DailyCheckCount = "dcc";
	public const string LastDailyCheckTime = "dcct";

	public const string CurExp = "curExp";
	public const string MaxExp = "maxExp";

	public const string BagSlotPre = "b_";

	public const string RewardSlotPre = "r_";

	public const string Friend1 = "f1";
	public const string Friend2 = "f2";
	public const string FriendRequester = "fr";

	public const string SenderName = "sn";
	public const string TargetName = "tn";
	public const string SendTime = "t";
	public const string SendContent = "c";
	public const string ChatType = "tp";

	public const string SchulteTime = "scTm";

	public const string MapX = "x";
	public const string MapY = "y";
	public const string LastMoveTime = "lmt";
}