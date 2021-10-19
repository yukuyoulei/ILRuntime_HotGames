using LibPacket;
using System.Collections.Generic;

public static class UEvents
{
	public const string ServerDisconnected = "ServerDisconnected";
	public const string Login = "Login";
	public const string EnterGame = "EnterGame";
	public const string CreateAvatar = "CreateAvatar";
	public const string ParamUpdate = "ParamUpdate";
	public const string ContaData = "ContaData";
	public const string CreateOrder = "CreateOrder";
	public const string CreatePlayer = "CreatePlayer";
	public const string Settlement = "Settlement";
}
public abstract class UEventBase { }

public class EventCreatePlayer : UEventBase
{
	public PktCreatePlayer pkt;
}
public class EventCommon : UEventBase
{
	public List<string> lstr = new List<string>();
}
public class EventLogin : UEventBase
{
	public bool bSuccess;
}
public class EventEnterGame : UEventBase
{
	public AvatarInfo info;
}
public class EventCreateAvatar : UEventBase
{
	public PktCreateAvatarResult.EResult eResult;
	public AvatarInfo info;
}
public class EventContaData : UEventBase
{
	public int id;
	public List<PData> lDatas;
}
public class EventCreateOrder : UEventBase
{
	public PktCreateOrderResult.EResult eResult;
	public string orderID;
	public string extraInfo;
}
public class EventSettlement : UEventBase
{
	public bool bRet;
	public PData pdata;
}