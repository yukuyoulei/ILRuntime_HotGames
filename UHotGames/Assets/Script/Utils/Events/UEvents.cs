using UnityEngine;
using System.Collections;
using System;

public static class UEvents
{
	public static string DownloadCompleteEvent = "DownloadCompleteEvent";
	public static string CreatureRemovedEvent = "CreatureRemovedEvent";
	public static string SMSGetCodeEvent = "SMSGetCodeEvent";
	public static string SMSCommitCodeEvent = "SMSCommitCodeEvent";

	public static string WebErrorEvent = "WebErrorEvent";

	public static string LevelFailedEvent = "LevelFailedEvent";
	public static string DetectLevelConditionEvent = "DetectLevelConditionEvent";

	public static string ServerDisconnectedEvent = "ServerDisconnectedEvent";
	public static string GatewayServerConnectedEvent = "GatewayServerConnectedEvent";
	public static string LoginEvent = "LoginEvent";
	public static string EnterGameEvent = "EnterGameEvent";
	public static string HeartBeatEvent = "HeartBeatEvent";

	public static string EnterRoomEvent = "EnterRoomEvent";
	public static string SendRoomMsgEvent = "SendRoomMsgEvent";
	public static string SendRoomOpsEvent = "SendRoomOpsEvent";
	public static string EnterLessonEvent = "EnterLessonEvent";
	public static string ChangeLessonEvent = "ChangeLessonEvent";

	public static string AgoraUserJoindEvent = "AgoraUserJoindEvent";
	public static string SyncOperationsEvent = "SyncOperationsEvent";
	public static string ClassmateStatusInformEvent = "ClassmateStatusInformEvent";
	public static string CurveSyncInformEvent = "CurveSyncInformEvent";
	public static string GiveStarEvent = "GiveStarEvent";
	public static string LessonOpEvent = "LessonOpEvent";
	public static string GameSyncEvent = "GameSyncEvent";
	public static string ImageSyncEvent = "ImageSyncEvent";
	public static string RemoteControllEvent = "RemoteControllEvent";
	public static string EncourageEvent = "EncourageEvent";

	public static string TeacherNextStepEvent = "TeacherNextStepEvent";
}
