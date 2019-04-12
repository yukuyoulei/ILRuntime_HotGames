using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
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
    private bool CheckCanMoveTo(int deltax = 0, int deltay = 0)
    {
        return AMapManager.Instance.CheckCanMoveTo(deltax + MapX, deltay + MapY);
    }
}
