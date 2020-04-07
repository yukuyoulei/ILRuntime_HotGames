using LibPacket;
using System.Collections.Generic;

public static class UEvents
{
	public const string ServerDisconnected = "ServerDisconnected";
	public const string Login = "Login";
	public const string EnterGame = "EnterGame";
	public const string CreateAvatar = "CreateAvatar";
	public const string ParamUpdate = "ParamUpdate";
	public const string EventContaData = "EventContaData";
	public const string EventCreateOrder = "EventCreateOrder";
}
public abstract class UEventBase { }

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
}
public class EventCreateOrder : UEventBase
{
	public PktCreateOrderResult.EResult eResult;
	public string orderID;
	public string extraInfo;
}