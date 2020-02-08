using LibPacket;

public static class UEvents
{
	public const string ServerDisconnected = "ServerDisconnected";
	public const string LoginFailed = "LoginFailed";
	public const string EnterGame = "EnterGame";
	public const string CreateAvatar = "CreateAvatar";
	public const string ParamUpdate = "ParamUpdate";
}
public abstract class UEventBase { }

public class EventLoginFailed : UEventBase { }
public class EventEnterGame : UEventBase
{
	public AvatarInfo info;
}
public class EventCreateAvatar : UEventBase
{
	public PktCreateAvatarResult.EResult eResult;
	public AvatarInfo info;
}
