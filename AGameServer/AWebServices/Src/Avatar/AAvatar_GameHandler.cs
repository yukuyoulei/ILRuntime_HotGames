using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;

public partial class AAvatar
{
	internal string GameHandler_Move(string arg)
	{
		if (LastMoveTime > 0
				&& LastMoveTime >= ApiDateTime.SecondsFromBegin())
		{
			return AWebServerUtils.OnGetJsonError(ErrorDefs.MoveInCold);
		}
		switch (arg)
		{
			case "up":
				if (!CheckCanMoveTo(0, -1))
				{
					return AWebServerUtils.OnGetJsonError(ErrorDefs.InvalidMove);
				}
				MapY--;
				break;
			case "down":
				if (!CheckCanMoveTo(0, 1))
				{
					return AWebServerUtils.OnGetJsonError(ErrorDefs.InvalidMove);
				}
				MapY++;
				break;
			case "left":
				if (!CheckCanMoveTo(-1))
				{
					return AWebServerUtils.OnGetJsonError(ErrorDefs.InvalidMove);
				}
				MapX--;
				break;
			case "right":
				if (!CheckCanMoveTo(1))
				{
					return AWebServerUtils.OnGetJsonError(ErrorDefs.InvalidMove);
				}
				MapX++;
				break;
		}
		LastMoveTime = ApiDateTime.SecondsFromBegin();

		var l = new List<string>();
		l.AddRange(new string[] { InfoNameDefs.MapX, MapX.ToString()
			, InfoNameDefs.MapY, MapY.ToString()
			, InfoNameDefs.LastMoveTime, LastMoveTime.ToString() });
		var monster = AMapManager.Instance.OnGetMonster(MapX, MapY);
		if (monster != null)
		{
			l.Add("m");
			l.Add(JsonConvert.SerializeObject(monster));
		}
		return AWebServerUtils.OnGetJsonError(l.ToArray());
	}

	internal void GameHandler_RoomOperation(string arg3)
	{
		var room = ARoomManager.Instance.OnGetRoom(username);
		if (room == null) return;
		room.OnOperation(username, arg3);
	}

	internal void GameHandler_JoinRoom(string roomType)
	{
		ARoomManager.Instance.OnExit(username);
		var room = ARoomManager.Instance.OnCreate(roomType);
		ARoomManager.Instance.OnEnter(this, room);
	}

	internal bool OnAnswer(string answer)
	{
		var res = ADatabaseConfigsManager.avatarDB.FindOneData(ADatabaseConfigsManager.tAvatarData
			, ADBAccessor.filter_eq(InfoNameDefs.Username, username), ADBAccessor.projections("qa"));
		if (res != null && res.Contains("qa"))
		{
			var a = res["qa"].ToString();
			if (a == answer)
			{
				var ia = typeParser.intParse(a);
				OnAddExp(ia > AvatarLevel ? ia : AvatarLevel);
				return true;
			}
		}
		return false;
	}

	internal string OnGetOneQuestion()
	{
		if (AvatarLevel < 10)
		{
			return DoGetQuestion_JiaFa();
		}
		else
		{
			if (ApiRandom.Instance.Next(4) == 0)
			{
				return DoGetQuestion_JiaFa();
			}
			return DoGetQuestion_ChengFa();
		}
	}
	private string DoGetQuestion_ChengFa()
	{
		var ileft = ApiRandom.Instance.Next(AvatarLevel) + 1;
		var iright = ApiRandom.Instance.Next(AvatarLevel) + 1;
		SetOneParam("qa", ileft * iright);
		return $"{ileft}×{iright}";
	}
	private string DoGetQuestion_JiaFa()
	{
		var ileft = ApiRandom.Instance.Next(AvatarLevel * 10) + 1;
		var iright = ApiRandom.Instance.Next(AvatarLevel * 10) + 1;
		SetOneParam("qa", ileft + iright);
		return $"{ileft}+{iright}";
	}
	internal HttpResponseMessage OnDailyCheck()
	{
		if (ApiDateTime.IsSameDay(LastDailyCheckTime))
		{
			return ResultToJson.GetErrorJsonResponse(ErrorDefs.DailyChecked);
		}
		LastDailyCheckTime = ApiDateTime.SecondsFromBegin();
		AvatarGold += 1000;
		return GetDiryParamResponse();
	}

	internal HttpResponseMessage OnCaiDaXiao(int multi, int isBig)
	{
		if (!InitValueDefs.CaiDaXiaoMultis.Contains(multi)) return ResultToJson.GetErrorJsonResponse($"multi {multi}");
		if (AvatarGold < multi * InitValueDefs.CaiDaXiaoCost) return ResultToJson.GetErrorJsonResponse(ErrorDefs.NotEnoughGold);
		int rdm = ApiRandom.Instance.Next(6);
		if ((isBig == 1 && rdm >= 3) || (isBig == 0 && rdm < 3))
		{
			AvatarGold += multi * InitValueDefs.CaiDaXiaoCost;
		}
		else
		{
			AvatarGold -= multi * InitValueDefs.CaiDaXiaoCost;
		}
		return GetDiryParamResponse("res", rdm.ToString());
	}

	private void SetOneParam(string paramname, int value)
	{
		SetOneParam(paramname, value.ToString());
	}
	private void SetOneParam(string paramname, string value)
	{
		ADatabaseConfigsManager.avatarDB.UpdateOneData(ADatabaseConfigsManager.tAvatarData
			, ADBAccessor.filter_eq(InfoNameDefs.Username, username), ADBAccessor.update(paramname, value));
	}

	private bool CheckCanMoveTo(int deltax = 0, int deltay = 0)
	{
		return AMapManager.Instance.CheckCanMoveTo(deltax + MapX, deltay + MapY);
	}
}
